Imports System.Runtime.CompilerServices
Imports System.Data
Public Module ORMapper
    Private _Entities As Dictionary(Of Type, _Entity) = New Dictionary(Of Type, _Entity)()

    Public Property Connection As IDbConnection
    Public Property Cache As ICache

    Public Function [Get](Of T)(pk As Object) As T
        If pk Is Nothing Then
            Throw New ArgumentNullException(NameOf(pk))
        End If

        Return CreateObj(GetType(T), pk, Nothing)
    End Function

    Public Sub SaveObj(ByVal obj As Object)
        'Create entity out of object
        Dim entity As _Entity = obj.GetType().GetEntity
        Dim query As String = "INSERT INTO " & entity.TableName & " ("
        Dim _insertval = ""
        Dim conflict As String = "ON CONFLICT (" & entity.PrimaryKey.ColumnName & ") DO UPDATE SET "
        Dim dbc As IDbCommand = Connection.CreateCommand()
        'dbc.CommandText = insertsql;
        Dim p As IDbDataParameter
        Dim i = 0

        While i < entity.Internals.Length

            If i = 0 Then
                query += entity.Internals(i).ColumnName & ", "
                _insertval += " VALUES ( :v0, "
                p = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = entity.Internals(i).ToColumnType(entity.Internals(i).GetVal(obj))
                dbc.Parameters.Add(p)
            ElseIf i > 0 AndAlso i < entity.Internals.Length - 1 Then
                query += entity.Internals(i).ColumnName & ", "
                _insertval += ":v" & i.ToString() & ", "
                p = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = entity.Internals(i).ToColumnType(entity.Internals(i).GetVal(obj))
                dbc.Parameters.Add(p)
            Else
                query += entity.Internals(i).ColumnName
                _insertval += ":v" & i.ToString()
                p = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = entity.Internals(i).ToColumnType(entity.Internals(i).GetVal(obj))
                dbc.Parameters.Add(p)
            End If

            If Not entity.Internals(i).IsPrimaryKey Then
                If i <> 0 Then
                    conflict += ", "
                End If

                conflict += (entity.Internals(i).ColumnName & " = " & (":c" & i.ToString()))
                p = dbc.CreateParameter()
                p.ParameterName = (":c" & i.ToString())
                p.Value = entity.Internals(i).ToColumnType(entity.Internals(i).GetVal(obj))
                dbc.Parameters.Add(p)
            End If

            i += 1
        End While

        dbc.CommandText = query & ")" & _insertval & ") " & conflict
        dbc.ExecuteNonQuery()
        dbc.Dispose()

        For i = 0 To entity.Externals.Length - 1
            Dim ii As _Field = entity.Externals(i)
            ii.UpdateRef(obj)
        Next

        If Cache IsNot Nothing Then
            Cache.Put(obj)
        End If
    End Sub



    <Extension()>
    Friend Function GetEntity(o As Object) As _Entity
        Dim t As Type = If(Not (TypeOf o Is Type), o.GetType(), CType(o, Type))

        Dim hv As Boolean = Not _Entities.ContainsKey(t)

        If hv Then
            Call _Entities.Add(t, New _Entity(t))
        End If

        Return _Entities(t)
    End Function


    Friend Function CreateObj(t As Type, pk As Object, localCache As ICollection(Of Object)) As Object
        Dim rval As Object = Nothing
        Dim cmd As IDbCommand = Connection.CreateCommand()
        cmd.CommandText = t.GetEntity().GetSQL() & " WHERE " & t.GetEntity().PrimaryKey.ColumnName & " = :pk"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":pk"
        p.Value = pk
        cmd.Parameters.Add(p)
        Dim re As IDataReader = cmd.ExecuteReader()

        If re.Read() Then
            rval = CreateObj(t, re, localCache)
        End If

        re.Close()
        cmd.Dispose()

        If Cache Is Nothing Then
            Return rval
        End If
        Cache.Put(rval)

        Return rval
    End Function

    Friend Function CreateObj(t As Type, re As IDataReader, localCache As ICollection(Of Object)) As Object
        Dim ent As _Entity = t.GetEntity()
        Dim rval = _SearchCache(t, ent.PrimaryKey.ToFieldType(re.GetValue(re.GetOrdinal(ent.PrimaryKey.ColumnName)), localCache), localCache)


        If rval Is Nothing Then
            If localCache Is Nothing Then
                localCache = New List(Of Object)()
            End If

            localCache.Add((rval, Activator.CreateInstance(t)))
            rval = _SearchCache(t, ent.PrimaryKey.ToFieldType(re.GetValue(re.GetOrdinal(ent.PrimaryKey.ColumnName)), localCache), localCache)
        End If

        For i1 = 0 To ent.Internals.Length - 1
            Dim i = ent.Internals(i1)
            i.SetVal(rval, i.ToFieldType(re.GetValue(re.GetOrdinal(i.ColumnName)), localCache))
        Next

        For i1 = 0 To ent.Externals.Length - 1
            Dim i = ent.Externals(i1)
            i.SetVal(rval, i.Fill(Activator.CreateInstance(i.Type), rval, localCache))

        Next

        Return rval
    End Function

    Friend Sub _FillList(ByVal t As Type, ByVal list As Object, ByVal sql As String, ByVal parameters As IEnumerable(Of Tuple(Of String, Object)), ByVal Optional localCache As ICollection(Of Object) = Nothing)
        Dim cmd As IDbCommand = Connection.CreateCommand()
        cmd.CommandText = sql

        For Each i In parameters
            Dim p As IDataParameter = cmd.CreateParameter()
            p.ParameterName = i.Item1
            p.Value = i.Item2
            cmd.Parameters.Add(p)
        Next

        Dim re As IDataReader = cmd.ExecuteReader()
        _FillList(t, list, re, localCache)
        re.Close()
        re.Dispose()
        cmd.Dispose()
    End Sub

    Friend Sub _FillList(ByVal t As Type, ByVal list As Object, ByVal re As IDataReader, ByVal Optional localCache As ICollection(Of Object) = Nothing)
        While re.Read()
            list.GetType().GetMethod("Add").Invoke(list, New Object() {CreateObj(t, re, localCache)})
        End While
    End Sub

    Friend Function _SearchCache(ByVal t As Type, ByVal pk As Object, ByVal localCache As ICollection(Of Object)) As Object

        If Cache Is Nothing OrElse Not Cache.Contains(t, pk) Then
            If localCache Is Nothing Then

                Return Nothing
            End If
            For Each i In localCache
                If i.GetType() _
                   IsNot t Then Continue For

                If Not t.GetEntity().PrimaryKey.GetVal(i).Equals(pk) Then
                    Continue For
                End If
                Return i
            Next

            Return Nothing
        End If
        Return Cache.Get(t, pk)
    End Function



End Module
