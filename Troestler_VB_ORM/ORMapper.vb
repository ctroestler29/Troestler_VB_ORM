Imports System.Runtime.CompilerServices
Imports System.Data
Public Module ORMapper

    Private _DBConnection As IDbConnection

    Public Function GetConnection() As IDbConnection
        Return _DBConnection
    End Function

    Public Sub SetConnection(value As IDbConnection)
        _DBConnection = value
    End Sub

    Private _LocalCache As ICache

    Public Function GetLocalCache() As ICache
        Return _LocalCache
    End Function

    Public Sub SetLocalCache(value As ICache)
        _LocalCache = value
    End Sub



    Private ReadOnly Tables As Dictionary(Of Type, Table) = New Dictionary(Of Type, Table)()

    Friend Function GetTables() As Dictionary(Of Type, Table)
        Return Tables
    End Function

    Private _ORLocking As ILockDB

    Public Function GetORLocking() As ILockDB
        Return _ORLocking
    End Function

    Public Sub SetORLocking(value As ILockDB)
        _ORLocking = value
    End Sub

    Private _TableManagement As ITableManagement

    Public Function GetTableManagement() As ITableManagement
        Return _TableManagement
    End Function

    Public Sub SetTableManagement(value As ITableManagement)
        _TableManagement = value
    End Sub


    Public Function [GetObjectType](Of Type)(pk As Object) As Type
        If pk Is Nothing Then
            Throw New ArgumentNullException(NameOf(pk))
        End If
        Dim o = NewObj(pk, GetType(Type))
        Return o
    End Function

    Public Sub SaveObject(ByRef obj As Object)
        If obj Is Nothing Then
            Throw New ArgumentNullException(NameOf(obj))
        End If

        Dim cache As ICache = GetLocalCache()

        If cache IsNot Nothing Then
            If Not cache.ObjectChanged(obj) Then Return
        End If

        'Create table out of object
        Dim tab As Table = obj.GetType().GetTableOf
        Using dbc As IDbCommand = GetConnection().CreateCommand()
            dbc.CommandText = "INSERT INTO " & tab.GetTableName() & " ("
            Dim update = "ON CONFLICT (" _
                         & tab.GetPrimaryKey().GetColumnName() _
                         & ") DO UPDATE SET "
            Dim insert = ""
            Dim first = True

            For i = 0 To tab.GetInternals().Length - 1

                If i > 0 Then
                    dbc.CommandText += ", "
                    insert += ", "
                End If

                dbc.CommandText += tab.GetInternals()(i).GetColumnName()
                insert += (":v" & i.ToString())
                Dim p As IDataParameter = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = tab.GetInternals()(i).ToColumnType(tab.GetInternals()(i).GetVal(obj))
                If p.Value Is Nothing Then
                    p.Value = DBNull.Value
                End If
                dbc.Parameters.Add(p)

                If tab.GetInternals()(i).GetIsPrimaryKey Then
                    Continue For
                End If
                If first Then
                    first = False
                Else
                    update += ", "
                End If

                update += (tab.GetInternals()(i).GetColumnName() & " = " & (":w" & i.ToString()))
                p = dbc.CreateParameter()
                p.ParameterName = (":w" & i.ToString())
                p.Value = tab.GetInternals()(i) _
                             .ToColumnType(tab.GetInternals()(i).GetVal(obj))
                If p.Value Is Nothing Then
                    p.Value = DBNull.Value
                End If
                dbc.Parameters.Add(p)

            Next

            dbc.CommandText += ") VALUES (" & insert & ") " & update
            dbc.ExecuteNonQuery()
            dbc.Dispose()
        End Using

        Dim arr = tab.GetExternals()
        For i1 = 0 To arr.Length - 1
            Dim i = arr(i1)
            i.UpdateRef(obj)
        Next

        If cache IsNot Nothing Then
            cache.StoreObject(obj)
        End If
    End Sub



    <Extension()>
    Friend Function GetTableOf(ByRef obj As Object) As Table
        If obj Is Nothing Then
            Throw New ArgumentNullException(NameOf(obj))
        End If

        Dim t As Type = If(Not (TypeOf obj Is Type), obj.GetType(), CType(obj, Type))

        Dim hv As Boolean = Not GetTables().ContainsKey(t)

        If hv Then
            Call GetTables().Add(t, New Table(t))
        End If

        Return GetTables()(t)
    End Function


    Friend Function NewObj(pk As Object, t As Type) As Object

        If pk Is Nothing Then
            Throw New ArgumentNullException(NameOf(pk))
        End If

        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        Using dbc As IDbCommand = GetConnection().CreateCommand()
            dbc.CommandText = t.GetTableOf().GetSQL() & " WHERE " & t.GetTableOf().GetPrimaryKey().GetColumnName() & " = :pk"
            Dim p As IDataParameter = dbc.CreateParameter()
            p.ParameterName = ":pk"
            p.Value = pk
            dbc.Parameters.Add(p)
            Using dbr As IDataReader = dbc.ExecuteReader()

                Dim obj As Object = Nothing
                If dbr.Read() Then
                    obj = NewObj(dbr, t)
                End If

                dbr.Close()
                dbc.Dispose()

                If GetLocalCache() Is Nothing Then
                    Return obj
                End If
                GetLocalCache().StoreObject(obj)

                Return obj
            End Using
        End Using
    End Function

    Friend Function NewObj(re As IDataReader, t As Type) As Object
        If re Is Nothing Then
            Throw New ArgumentNullException(NameOf(re))
        End If

        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        Dim obj = LocalCacheFind(t.GetTableOf().GetPrimaryKey().ToFieldType(re.GetValue(re.GetOrdinal(t.GetTableOf().GetPrimaryKey().GetColumnName()))), t)


        If obj Is Nothing Then
            obj = LocalCacheFind(t.GetTableOf().GetPrimaryKey().ToFieldType(re.GetValue(re.GetOrdinal(t.GetTableOf().GetPrimaryKey().GetColumnName()))), t)
        End If

        For i1 = 0 To t.GetTableOf().GetInternals().Length - 1
            Dim i = t.GetTableOf().GetInternals()(i1)
            i.SetVal(i.ToFieldType(re.GetValue(re.GetOrdinal(i.GetColumnName()))), obj)
        Next


        For i1 = 0 To t.GetTableOf().GetExternals().Length - 1
            Dim i = t.GetTableOf().GetExternals()(i1)
            If GetType(ILazyLoading).IsAssignableFrom(i.Type) Then
                i.SetVal(Activator.CreateInstance(i.Type, obj, i.GetMember().Name), obj)
            Else
                i.SetVal(i.Fill(obj, Activator.CreateInstance(i.Type)), obj)
            End If

        Next

        Return obj
    End Function

    Friend Sub FList(ByRef list As Object, ByVal sql As String, ByVal t As Type, ByVal parameters As IEnumerable(Of Tuple(Of String, Object)))
        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        If list Is Nothing Then
            Throw New ArgumentNullException(NameOf(list))
        End If

        If String.IsNullOrEmpty(sql) Then
            Throw New ArgumentException($"" + NameOf(sql) + " kann nicht NULL oder leer sein.", NameOf(sql))
        End If

        If parameters Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameters))
        End If

        Dim dbc As IDbCommand = GetConnection().CreateCommand()
        dbc.CommandText = sql

        For Each i In parameters
            Dim p As IDataParameter = dbc.CreateParameter()
            p.ParameterName = i.Item1
            p.Value = i.Item2
            dbc.Parameters.Add(p)
        Next

        Dim re As IDataReader = dbc.ExecuteReader()
        FList(list, t, re)
        re.Close()
        re.Dispose()
        dbc.Dispose()
    End Sub

    Friend Sub FList(ByRef list As Object, ByVal t As Type, ByVal re As IDataReader)
        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        If list Is Nothing Then
            Throw New ArgumentNullException(NameOf(list))
        End If

        If re Is Nothing Then
            Throw New ArgumentNullException(NameOf(re))
        End If

        While re.Read()
            list.GetType().GetMethod("Add").Invoke(list, New Object() {NewObj(re, t)})
        End While
    End Sub

    Friend Function LocalCacheFind(ByRef pk As Object, ByVal t As Type) As Object
        If pk Is Nothing Then
            Throw New ArgumentNullException(NameOf(pk))
        End If

        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        Return If(GetLocalCache() IsNot Nothing AndAlso GetLocalCache().ContainsObjectWithPK(t, pk), GetLocalCache().GetObjectByPK(t, pk), Nothing)
    End Function

    Public Sub LockDBObject(obj As Object)
        If obj Is Nothing Then
            Throw New ArgumentNullException(NameOf(obj))
        End If

        If GetORLocking() Is Nothing Then
            Return
        End If

        GetORLocking().LockObj(obj)
    End Sub

    Public Sub ReleaseDBObject(obj As Object)
        If obj Is Nothing Then
            Throw New ArgumentNullException(NameOf(obj))
        End If

        If GetORLocking() Is Nothing Then
            Return
        End If

        GetORLocking().ReleaseObj(obj)
    End Sub

    Public Sub CreateTable(Of Type)(Optional tablename As String = Nothing)
        If GetTableManagement() Is Nothing Then
            Return
        End If

        GetTableManagement().CreateTable(Of Type)(tablename)
    End Sub

    Public Sub DropTable(Of Type)(Optional tablename As String = Nothing)
        If GetTableManagement() Is Nothing Then
            Return
        End If

        GetTableManagement().DropTable(Of Type)(tablename)
    End Sub

    Public Sub CreateIndex(indexname As String, tablename As String)
        If GetTableManagement() Is Nothing Then
            Return
        End If

        GetTableManagement().CreateIndex(indexname, tablename)
    End Sub

    Public Sub ResetSchema()
        If GetTableManagement() Is Nothing Then
            Return
        End If

        GetTableManagement().ResetDBSchema()
    End Sub


    Public Sub RemoveObj(obj As Object)

        If obj Is Nothing Then
            Throw New ArgumentNullException(NameOf(obj))
        End If

        Dim tab As Table = obj.GetType().GetTableOf
        Dim cmd As IDbCommand = GetConnection.CreateCommand()
        cmd.CommandText = "DELETE FROM " & tab.GetTableName & " WHERE " & tab.GetPrimaryKey.GetColumnName & " = :pk"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":pk"
        p.Value = tab.GetPrimaryKey.GetVal(obj)
        cmd.Parameters.Add(p)
        cmd.ExecuteNonQuery()
        cmd.Dispose()

        Dim cache As ICache = GetLocalCache()

        If cache IsNot Nothing Then
            cache.RemoveObject(obj)
        End If
    End Sub

    <Extension()>
    Friend Function GetChildrenOf(ByVal t As Type) As Type()
        Dim obj As List(Of Type) = New List(Of Type)()

        For Each i In Tables.Keys

            If t.IsAssignableFrom(i) AndAlso Not i.IsAbstract Then
                obj.Add(i)
            End If
        Next

        Return obj.ToArray()
    End Function

    Public Function [Select](Of _type)() As QueryBuilder(Of _type)
        Return New QueryBuilder(Of _type)(Nothing)
    End Function



End Module
