Imports System.Runtime.CompilerServices
Imports System.Data
Public Module ORMapper
    Private _Entities As Dictionary(Of Type, _Entity) = New Dictionary(Of Type, _Entity)()

    Public Property Connection As IDbConnection

    Public Function [Get](Of T)(ByVal pk As Object) As T
        Return CreateObj(GetType(T), pk, Nothing)
    End Function

    Public Sub SaveObj(ByVal obj As Object)
        'Create entity out of object
        Dim entity As _Entity = obj.GetType().GetEntity
        Dim insertsql As String = "INSERT INTO " & entity.TableName & " ("
        Dim _insertval = ""
        Dim conflict As String = "ON CONFLICT (" & entity.PrimaryKey.ColumnName & ") DO UPDATE SET "
        Dim dbc As IDbCommand = Connection.CreateCommand()
        'dbc.CommandText = insertsql;
        Dim p As IDbDataParameter
        Dim i = 0

        While i < entity.Internals.Length

            If i = 0 Then
                insertsql += entity.Internals(i).ColumnName & ", "
                _insertval += " VALUES ( :v0, "
                p = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = entity.Internals(i).ToColumnType(entity.Internals(i).GetVal(obj))
                dbc.Parameters.Add(p)
            ElseIf i > 0 AndAlso i < entity.Internals.Length - 1 Then
                insertsql += entity.Internals(i).ColumnName & ", "
                _insertval += ":v" & i.ToString() & ", "
                p = dbc.CreateParameter()
                p.ParameterName = (":v" & i.ToString())
                p.Value = entity.Internals(i).ToColumnType(entity.Internals(i).GetVal(obj))
                dbc.Parameters.Add(p)
            Else
                insertsql += entity.Internals(i).ColumnName
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

        dbc.CommandText = insertsql & ")" & _insertval & ") " & conflict
        dbc.ExecuteNonQuery()
        dbc.Dispose()

        For Each ii As _Field In entity.Externals
            ii.UpdateRef(obj)
        Next
    End Sub



    <Extension()>
    Friend Function GetEntity(ByVal o As Object) As _Entity
        Dim t As Type = (If((TypeOf o Is Type), CType(o, Type), o.GetType()))

        If Not _Entities.ContainsKey(t) Then
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


        Return rval
    End Function



End Module
