
Imports System.Data
Imports System.Reflection
Friend Class _Field

    Private _Entity As _Entity
    Private _Member As MemberInfo
    Private _ColumnName As String
    Private _ColumnType As Type
    Private _IsPrimaryKey As Boolean
    Private _IsForeignKey As Boolean
    Private _AssignmentTable As String
    Private _RemoteColumnName As String
    Private _IsManyToMany As Boolean
    Private _IsNullable As Boolean
    Private _IsExternal As Boolean

    Public Sub New(entity As _Entity)
        Me.SetEntity(entity)
    End Sub

    Public Function GetEntity() As _Entity
        Return _Entity
    End Function

    Private Sub SetEntity(value As _Entity)
        _Entity = value
    End Sub

    Public Function GetMember() As MemberInfo
        Return _Member
    End Function

    Friend Sub SetMember(value As MemberInfo)
        _Member = value
    End Sub

    Public ReadOnly Property Type As Type
        Get

            If TypeOf GetMember() Is PropertyInfo Then
                Return CType(GetMember(), PropertyInfo).PropertyType
            End If

            Throw New NotSupportedException("Member type not supported.")
        End Get
    End Property

    Public Function GetColumnName() As String
        Return _ColumnName
    End Function

    Friend Sub SetColumnName(value As String)
        _ColumnName = value
    End Sub

    Public Function GetColumnType() As Type
        Return _ColumnType
    End Function

    Friend Sub SetColumnType(value As Type)
        _ColumnType = value
    End Sub

    Public Function GetIsPrimaryKey() As Boolean
        Return _IsPrimaryKey
    End Function

    Friend Sub SetIsPrimaryKey(value As Boolean)
        _IsPrimaryKey = value
    End Sub

    Public Function GetIsForeignKey() As Boolean
        Return _IsForeignKey
    End Function

    Friend Sub SetIsForeignKey(value As Boolean)
        _IsForeignKey = value
    End Sub

    Public Function GetAssignmentTable() As String
        Return _AssignmentTable
    End Function

    Friend Sub SetAssignmentTable(value As String)
        _AssignmentTable = value
    End Sub

    Public Function GetRemoteColumnName() As String
        Return _RemoteColumnName
    End Function

    Friend Sub SetRemoteColumnName(value As String)
        _RemoteColumnName = value
    End Sub

    Public Function GetIsManyToMany() As Boolean
        Return _IsManyToMany
    End Function

    Friend Sub SetIsManyToMany(value As Boolean)
        _IsManyToMany = value
    End Sub

    Public Function GetIsNullable() As Boolean
        Return _IsNullable
    End Function

    Friend Sub SetIsNullable(value As Boolean)
        _IsNullable = value
    End Sub

    Public Function GetIsExternal() As Boolean
        Return _IsExternal
    End Function

    Friend Sub SetIsExternal(value As Boolean)
        _IsExternal = value
    End Sub

    Public Function ToColumnType(ByRef value As Object) As Object
        If Not GetIsForeignKey() Then

            If Type Is GetColumnType() Then
                Return value
            End If

            If TypeOf value Is Boolean Then
                If GetColumnType() Is GetType(Integer) Then
                    Return If(value, 1, 0)
                End If

                If GetColumnType() Is GetType(Short) Then
                    Return CShort(If(value, 1, 0))
                End If

                If GetColumnType() Is GetType(Long) Then
                    Return CLng(If(value, 1, 0))
                End If
            End If

            Return value
        End If
        If value IsNot Nothing Then

            Dim t = If(GetType(ILazyLoading).IsAssignableFrom(Type), Type.GenericTypeArguments(0), Type)
            Return t.GetEntity.GetPrimaryKey().ToColumnType(t.GetEntity.GetPrimaryKey().GetVal(value))
        End If
        Return Nothing
    End Function

    Public Sub SetVal(ByRef obj As Object, ByRef value As Object)
        If TypeOf GetMember() Is PropertyInfo Then
            Dim propertyInfo As PropertyInfo = CType(GetMember(), PropertyInfo)
            'Dim a = value.GetType()

            If GetMember().ToString() _
                          .Contains("Int") Then
                Dim objAsConvertible As Int32 = CType(value, Int32)
                propertyInfo.SetValue(obj, objAsConvertible)
            ElseIf GetMember().ToString() _
                              .Contains("String") Then
                Dim objAsConvertible As String = CType(value, String)
                propertyInfo.SetValue(obj, objAsConvertible)
            ElseIf GetMember().ToString() _
                              .Contains("Date") Then
                Dim objAsConvertible As DateTime = CType(value, DateTime)
                propertyInfo.SetValue(obj, objAsConvertible)
            Else
                propertyInfo.SetValue(obj, value)
            End If


            Return
        End If

        Throw New NotSupportedException("Type of Member is not supported.")
    End Sub

    Public Function GetVal(ByRef obj As Object) As Object
        Dim hv As Boolean = TypeOf GetMember() Is PropertyInfo

        If hv Then
            Dim rval = CType(GetMember(), PropertyInfo).GetValue(obj)
            If TypeOf rval Is ILazyLoading Then
                If Not (TypeOf rval Is IEnumerable) Then
                    Return rval.GetType() _
                               .GetProperty("Value") _
                               .GetValue(rval)
                End If
            End If
            Return rval
        End If

        Throw New NotSupportedException("Type of Member is not supported.")
    End Function

    Public Sub UpdateRef(ByRef obj As Object)
        If GetIsExternal() Then If GetVal(obj) Is Nothing Then Return
        Dim innerType As Type = Type.GetGenericArguments()(0)
        Dim ent As _Entity = GetEntity()
        Dim pk = ent.GetPrimaryKey().ToColumnType(ent.GetPrimaryKey().GetVal(obj))

        If Not GetIsManyToMany() Then

            If innerType.GetEntity() _
                        .GetFieldForColumn(GetColumnName()) _
                        .GetIsNullable() Then
                Try
                    Using cmd As IDbCommand = GetConnection().CreateCommand()
                        cmd.CommandText = "UPDATE " & innerType.GetEntity().GetTableName() & " SET " & GetColumnName() & " = NULL WHERE " & GetColumnName() & " = :fk"
                        Dim p As IDataParameter = cmd.CreateParameter()
                        p.ParameterName = ":fk"
                        p.Value = pk
                        cmd.Parameters.Add(p)
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                    End Using
                Catch ex As Exception
                End Try
            End If

            For Each i In CType(GetVal(obj), IEnumerable)
                innerType.GetEntity() _
                         .GetFieldForColumn(GetColumnName()) _
                         .SetVal(i, obj)
                Using cmd As IDbCommand = GetConnection().CreateCommand()
                    cmd.CommandText = "UPDATE " & innerType.GetEntity().GetTableName() & " SET " & GetColumnName() & " = :fk WHERE " & innerType.GetEntity().GetPrimaryKey().GetColumnName() & " = :pk"
                    Dim p As IDataParameter = cmd.CreateParameter()
                    p.ParameterName = ":fk"
                    p.Value = pk
                    cmd.Parameters.Add(p)
                    p = cmd.CreateParameter()
                    p.ParameterName = ":pk"
                    p.Value = innerType.GetEntity().GetPrimaryKey().ToColumnType(innerType.GetEntity().GetPrimaryKey().GetVal(i))
                    cmd.Parameters.Add(p)
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                End Using
            Next
        Else
            Using cmd As IDbCommand = GetConnection().CreateCommand()
                cmd.CommandText = "DELETE FROM " & GetAssignmentTable() & " WHERE " & GetColumnName() & " = :pk"
                Dim p As IDataParameter = cmd.CreateParameter()
                p.ParameterName = ":pk"
                p.Value = pk
                cmd.Parameters.Add(p)
                cmd.ExecuteNonQuery()
                cmd.Dispose()

                For Each i In CType(GetVal(obj), IEnumerable)
                    cmd.CommandText = "INSERT INTO " & GetAssignmentTable() & "(" & GetColumnName() & ", " & GetRemoteColumnName() & ") VALUES (:pk, :fk)"
                    p = cmd.CreateParameter()
                    p.ParameterName = ":pk"
                    p.Value = pk
                    cmd.Parameters.Add(p)
                    p = cmd.CreateParameter()
                    p.ParameterName = ":fk"
                    p.Value = innerType.GetEntity().GetPrimaryKey().ToColumnType(innerType.GetEntity().GetPrimaryKey().GetVal(i))
                    cmd.Parameters.Add(p)
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                Next
            End Using
        End If
    End Sub

    Public Function ToFieldType(ByRef value As Object) As Object
        If Not GetIsForeignKey() Then

            If Type Is GetType(Boolean) Then
                If TypeOf value Is Integer Then
                    Return CInt(value) <> 0
                End If

                If TypeOf value Is Long Then
                    Return CLng(value) <> 0
                End If

                If TypeOf value Is Short Then
                    Return CShort(value) <> 0
                End If


            End If

            If Type Is GetType(Short) Then
                Return Convert.ToInt16(value)
            End If

            If Type Is GetType(Integer) Then
                Return Convert.ToInt32(value)
            End If

            If Type Is GetType(Long) Then
                Return Convert.ToInt64(value)
            End If

            If Type.IsEnum Then
                Return [Enum].ToObject(Type, value)
            End If

            Return value
        End If
        Return If(GetType(ILazyLoading).IsAssignableFrom(Type), Activator.CreateInstance(Type, value), NewObj(Type, value))
    End Function

    Public Function Fill(ByRef list As Object, ByRef obj As Object) As Object
        Call _FillList(Type.GenericTypeArguments(0), list, Get_FkSql(),
                       New Tuple(Of String, Object)() {New Tuple(Of String, Object)(":fk", GetEntity().GetPrimaryKey().GetVal(obj))})
        Return list
    End Function

    Friend Function Get_FkSql() As String
        If GetIsManyToMany() Then
            Return Type.GenericTypeArguments(0).GetEntity().GetSQL() & " WHERE ID IN (SELECT " & GetRemoteColumnName() & " FROM " & GetAssignmentTable() & " WHERE " & GetColumnName() & " = :fk)"
        End If

        Return Type.GenericTypeArguments(0).GetEntity().GetSQL() & " WHERE " & GetColumnName() & " = :fk"
    End Function
End Class
