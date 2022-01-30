Imports System.Runtime.CompilerServices
Imports System.Data
Public Module ORMapper

    Private _Connection As IDbConnection

    Public Function GetConnection() As IDbConnection
        Return _Connection
    End Function

    Public Sub SetConnection(value As IDbConnection)
        _Connection = value
    End Sub

    Private _Cache As ICache

    Public Function GetCache() As ICache
        Return _Cache
    End Function

    Public Sub SetCache(value As ICache)
        _Cache = value
    End Sub



    Private ReadOnly _Entities As Dictionary(Of Type, _Entity) = New Dictionary(Of Type, _Entity)()

    Friend Function GetEntities() As Dictionary(Of Type, _Entity)
        Return _Entities
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
        Dim o = NewObj(GetType(Type), pk)
        Return o
    End Function

    Public Sub Save(ByRef obj As Object)
        If obj Is Nothing Then
            Throw New ArgumentNullException(NameOf(obj))
        End If

        Dim cache As ICache = GetCache()

        If cache IsNot Nothing Then
            If Not cache.ObjectChanged(obj) Then Return
        End If

        'Create entity out of object
        Dim ent As _Entity = obj.GetType().GetEntity
        Using dbc As IDbCommand = GetConnection().CreateCommand()
            dbc.CommandText = "INSERT INTO " & ent.GetTableName() & " ("
            Dim update = "ON CONFLICT (" _
                         & ent.GetPrimaryKey().GetColumnName() _
                         & ") DO UPDATE SET "
            Dim insert = ""
            Dim first = True

            For i = 0 To ent.GetInternals().Length - 1

                If i > 0 Then
                    dbc.CommandText += ", "
                    insert += ", "
                End If

                dbc.CommandText += ent.GetInternals()(i).GetColumnName()
                insert += (":v" & i.ToString())
                Dim p As IDataParameter = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = ent.GetInternals()(i).ToColumnType(ent.GetInternals()(i).GetVal(obj))
                dbc.Parameters.Add(p)

                If ent.GetInternals()(i).GetIsPrimaryKey Then
                    Continue For
                End If
                If first Then
                    first = False
                Else
                    update += ", "
                End If

                update += (ent.GetInternals()(i).GetColumnName() & " = " & (":w" & i.ToString()))
                p = dbc.CreateParameter()
                p.ParameterName = (":w" & i.ToString())
                p.Value = ent.GetInternals()(i) _
                             .ToColumnType(ent.GetInternals()(i).GetVal(obj))
                dbc.Parameters.Add(p)
            Next

            dbc.CommandText += ") VALUES (" & insert & ") " & update
            dbc.ExecuteNonQuery()
            dbc.Dispose()
        End Using

        Dim arr = ent.GetExternals()
        For i1 = 0 To arr.Length - 1
            Dim i = arr(i1)
            i.UpdateRef(obj)
        Next

        If cache IsNot Nothing Then
            cache.StoreObject(obj)
        End If
    End Sub



    <Extension()>
    Friend Function GetEntity(ByRef o As Object) As _Entity
        If o Is Nothing Then
            Throw New ArgumentNullException(NameOf(o))
        End If

        Dim t As Type = If(Not (TypeOf o Is Type), o.GetType(), CType(o, Type))

        Dim hv As Boolean = Not GetEntities().ContainsKey(t)

        If hv Then
            Call GetEntities().Add(t, New _Entity(t))
        End If

        Return GetEntities()(t)
    End Function


    Friend Function NewObj(t As Type, pk As Object) As Object

        If pk Is Nothing Then
            Throw New ArgumentNullException(NameOf(pk))
        End If

        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        Using dbc As IDbCommand = GetConnection().CreateCommand()
            dbc.CommandText = t.GetEntity().GetSQL() & " WHERE " & t.GetEntity().GetPrimaryKey().GetColumnName() & " = :pk"
            Dim p As IDataParameter = dbc.CreateParameter()
            p.ParameterName = ":pk"
            p.Value = pk
            dbc.Parameters.Add(p)
            Using dbr As IDataReader = dbc.ExecuteReader()

                Dim obj As Object = Nothing
                If dbr.Read() Then
                    obj = NewObj(t, dbr)
                End If

                dbr.Close()
                dbc.Dispose()

                If GetCache() Is Nothing Then
                    Return obj
                End If
                GetCache().StoreObject(obj)

                Return obj
            End Using
        End Using
    End Function

    Friend Function NewObj(t As Type, re As IDataReader) As Object
        If re Is Nothing Then
            Throw New ArgumentNullException(NameOf(re))
        End If

        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        Dim obj = _SearchCache(t, t.GetEntity().GetPrimaryKey().ToFieldType(re.GetValue(re.GetOrdinal(t.GetEntity().GetPrimaryKey().GetColumnName()))))


        If obj Is Nothing Then
            obj = _SearchCache(t, t.GetEntity().GetPrimaryKey().ToFieldType(re.GetValue(re.GetOrdinal(t.GetEntity().GetPrimaryKey().GetColumnName()))))
        End If

        For i1 = 0 To t.GetEntity().GetInternals().Length - 1
            Dim i = t.GetEntity().GetInternals()(i1)
            i.SetVal(obj, i.ToFieldType(re.GetValue(re.GetOrdinal(i.GetColumnName()))))
        Next

        For i1 = 0 To t.GetEntity().GetExternals().Length - 1
            Dim i = t.GetEntity().GetExternals()(i1)
            If GetType(ILazyLoading).IsAssignableFrom(i.Type) Then
                i.SetVal(obj, Activator.CreateInstance(i.Type, obj, i.GetMember().Name))
            Else
                i.SetVal(obj, i.Fill(Activator.CreateInstance(i.Type), obj))
            End If

        Next

        Return obj
    End Function

    Friend Sub _FillList(ByVal t As Type, ByRef list As Object, ByVal sql As String, ByVal parameters As IEnumerable(Of Tuple(Of String, Object)))
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
        _FillList(t, list, re)
        re.Close()
        re.Dispose()
        dbc.Dispose()
    End Sub

    Friend Sub _FillList(ByVal t As Type, ByRef list As Object, ByVal re As IDataReader)
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
            list.GetType().GetMethod("Add").Invoke(list, New Object() {NewObj(t, re)})
        End While
    End Sub

    Friend Function _SearchCache(ByVal t As Type, ByRef pk As Object) As Object
        If pk Is Nothing Then
            Throw New ArgumentNullException(NameOf(pk))
        End If

        If t Is Nothing Then
            Throw New ArgumentNullException(NameOf(t))
        End If

        Return If(GetCache() IsNot Nothing AndAlso GetCache().ContainsObjectWithPK(t, pk), GetCache().GetObjectByPK(t, pk), Nothing)
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

        Dim ent As _Entity = obj.GetType().GetEntity
        Dim cmd As IDbCommand = GetConnection.CreateCommand()
        cmd.CommandText = "DELETE FROM " & ent.GetTableName & " WHERE " & ent.GetPrimaryKey.GetColumnName & " = :pk"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":pk"
        p.Value = ent.GetPrimaryKey.GetVal(obj)
        cmd.Parameters.Add(p)
        cmd.ExecuteNonQuery()
        cmd.Dispose()

        Dim cache As ICache = GetCache()

        If cache IsNot Nothing Then
            cache.RemoveObject(obj)
        End If
    End Sub

    <Extension()>
    Friend Function GetChildrenOf(ByVal t As Type) As Type()
        Dim rval As List(Of Type) = New List(Of Type)()

        For Each i In _Entities.Keys

            If t.IsAssignableFrom(i) AndAlso Not i.IsAbstract Then
                rval.Add(i)
            End If
        Next

        Return rval.ToArray()
    End Function

    Public Function [Select](Of _type)() As QueryBuilder(Of _type)
        Return New QueryBuilder(Of _type)(Nothing)
    End Function



End Module
