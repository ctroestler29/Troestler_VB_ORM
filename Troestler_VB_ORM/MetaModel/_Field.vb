
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
        Me.Entity = entity
    End Sub

    Public Property Entity As _Entity
        Get
            Return _Entity
        End Get
        Private Set(value As _Entity)
            _Entity = value
        End Set
    End Property

    Public Property Member As MemberInfo
        Get
            Return _Member
        End Get
        Friend Set(value As MemberInfo)
            _Member = value
        End Set
    End Property


    Public ReadOnly Property Type As Type
        Get

            If TypeOf Member Is PropertyInfo Then
                Return CType(Member, PropertyInfo).PropertyType
            End If

            Throw New NotSupportedException("Member type not supported.")
        End Get
    End Property

    Public Property ColumnName As String
        Get
            Return _ColumnName
        End Get
        Friend Set(value As String)
            _ColumnName = value
        End Set
    End Property

    Public Property ColumnType As Type
        Get
            Return _ColumnType
        End Get
        Friend Set(value As Type)
            _ColumnType = value
        End Set
    End Property

    Public Property IsPrimaryKey As Boolean
        Get
            Return _IsPrimaryKey
        End Get
        Friend Set(value As Boolean)
            _IsPrimaryKey = value
        End Set
    End Property

    Public Property IsForeignKey As Boolean
        Get
            Return _IsForeignKey
        End Get
        Friend Set(value As Boolean)
            _IsForeignKey = value
        End Set
    End Property

    Public Property AssignmentTable As String
        Get
            Return _AssignmentTable
        End Get
        Friend Set(value As String)
            _AssignmentTable = value
        End Set
    End Property

    Public Property RemoteColumnName As String
        Get
            Return _RemoteColumnName
        End Get
        Friend Set(value As String)
            _RemoteColumnName = value
        End Set
    End Property

    Public Property IsManyToMany As Boolean
        Get
            Return _IsManyToMany
        End Get
        Friend Set(value As Boolean)
            _IsManyToMany = value
        End Set
    End Property

    Public Property IsNullable As Boolean
        Get
            Return _IsNullable
        End Get
        Friend Set(value As Boolean)
            _IsNullable = value
        End Set
    End Property

    Public Property IsExternal As Boolean
        Get
            Return _IsExternal
        End Get
        Friend Set(value As Boolean)
            _IsExternal = value
        End Set
    End Property

    Public Function ToColumnType(ByVal value As Object) As Object
        If IsForeignKey Then
            If value Is Nothing Then
                Return Nothing
            End If

            Dim t = If(GetType(ILazyLoading).IsAssignableFrom(Type), Type.GenericTypeArguments(0), Type)
            Return t.GetEntity.PrimaryKey.ToColumnType(t.GetEntity.PrimaryKey.GetVal(value))
        End If

        If Type Is ColumnType Then
            Return value
        End If

        If TypeOf value Is Boolean Then
            If ColumnType Is GetType(Integer) Then
                Return If(value, 1, 0)
            End If

            If ColumnType Is GetType(Short) Then
                Return CShort(If(value, 1, 0))
            End If

            If ColumnType Is GetType(Long) Then
                Return CLng(If(value, 1, 0))
            End If
        End If

        Return value
    End Function

    Public Sub SetVal(ByVal obj As Object, ByVal value As Object)
        If TypeOf Member Is PropertyInfo Then
            'Dim a = value.GetType()

            If Member.ToString().Contains("Int") Then
                Dim objAsConvertible As Int32 = CType(value, Int32)
                CType(Member, PropertyInfo).SetValue(obj, objAsConvertible)
            ElseIf Member.ToString().Contains("String") Then
                Dim objAsConvertible As String = CType(value, String)
                CType(Member, PropertyInfo).SetValue(obj, objAsConvertible)
            ElseIf Member.ToString().Contains("Date") Then
                Dim objAsConvertible As DateTime = CType(value, DateTime)
                CType(Member, PropertyInfo).SetValue(obj, objAsConvertible)
            Else
                CType(Member, PropertyInfo).SetValue(obj, value)
            End If


            Return
        End If

        Throw New NotSupportedException("Type of Member is not supported.")
    End Sub

    Public Function GetVal(ByVal obj As Object) As Object
        If TypeOf Member Is PropertyInfo Then
            Dim rval = CType(Member, PropertyInfo).GetValue(obj)
            If TypeOf rval Is ILazyLoading Then
                If Not (TypeOf rval Is IEnumerable) Then
                    Return rval.GetType().GetProperty("Value").GetValue(rval)
                End If
            End If
            Return rval
        End If

        Throw New NotSupportedException("Type of Member is not supported.")
    End Function

    Public Sub UpdateRef(ByVal obj As Object)
        If Not IsExternal Then Return
        If GetVal(obj) Is Nothing Then Return
        Dim innerType As Type = Type.GetGenericArguments()(0)
        Dim innerEntity As _Entity = innerType.GetEntity()
        Dim pk = Entity.PrimaryKey.ToColumnType(Entity.PrimaryKey.GetVal(obj))

        If IsManyToMany Then
            Dim cmd As IDbCommand = Connection.CreateCommand()
            cmd.CommandText = "DELETE FROM " & AssignmentTable & " WHERE " & ColumnName & " = :pk"
            Dim p As IDataParameter = cmd.CreateParameter()
            p.ParameterName = ":pk"
            p.Value = pk
            cmd.Parameters.Add(p)
            cmd.ExecuteNonQuery()
            cmd.Dispose()

            For Each i In CType(GetVal(obj), IEnumerable)
                cmd = Connection.CreateCommand()
                cmd.CommandText = "INSERT INTO " & AssignmentTable & "(" & ColumnName & ", " & RemoteColumnName & ") VALUES (:pk, :fk)"
                p = cmd.CreateParameter()
                p.ParameterName = ":pk"
                p.Value = pk
                cmd.Parameters.Add(p)
                p = cmd.CreateParameter()
                p.ParameterName = ":fk"
                p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetVal(i))
                cmd.Parameters.Add(p)
                cmd.ExecuteNonQuery()
                cmd.Dispose()
            Next
        Else
            Dim remoteField = innerEntity.GetFieldForColumn(ColumnName)

            If remoteField.IsNullable Then
                Try
                    Dim cmd As IDbCommand = Connection.CreateCommand()
                    cmd.CommandText = "UPDATE " & innerEntity.TableName & " SET " & ColumnName & " = NULL WHERE " & ColumnName & " = :fk"
                    Dim p As IDataParameter = cmd.CreateParameter()
                    p.ParameterName = ":fk"
                    p.Value = pk
                    cmd.Parameters.Add(p)
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                Catch __unusedException1__ As Exception
                End Try
            End If

            For Each i In CType(GetVal(obj), IEnumerable)
                remoteField.SetVal(i, obj)
                Dim cmd As IDbCommand = Connection.CreateCommand()
                cmd.CommandText = "UPDATE " & innerEntity.TableName & " SET " & ColumnName & " = :fk WHERE " & innerEntity.PrimaryKey.ColumnName & " = :pk"
                Dim p As IDataParameter = cmd.CreateParameter()
                p.ParameterName = ":fk"
                p.Value = pk
                cmd.Parameters.Add(p)
                p = cmd.CreateParameter()
                p.ParameterName = ":pk"
                p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetVal(i))
                cmd.Parameters.Add(p)
                cmd.ExecuteNonQuery()
                cmd.Dispose()
            Next
        End If
    End Sub

    Public Function ToFieldType(ByVal value As Object, ByVal localCache As ICollection(Of Object)) As Object
        If IsForeignKey Then
            If GetType(ILazyLoading).IsAssignableFrom(Type) Then
                Return Activator.CreateInstance(Type, value)
            End If
            Return CreateObj(Type, value, localCache)
        End If

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
    End Function

    Public Function Fill(ByVal list As Object, ByVal obj As Object, ByVal localCache As ICollection(Of Object)) As Object
        Call _FillList(Type.GenericTypeArguments(0), list, _FkSql, New Tuple(Of String, Object)() {New Tuple(Of String, Object)(":fk", Entity.PrimaryKey.GetVal(obj))}, localCache)
        Return list
    End Function

    Friend ReadOnly Property _FkSql As String
        Get
            If IsManyToMany Then
                Return Type.GenericTypeArguments(0).GetEntity().GetSQL() & " WHERE ID IN (SELECT " & RemoteColumnName & " FROM " & AssignmentTable & " WHERE " & ColumnName & " = :fk)"
            End If

            Return Type.GenericTypeArguments(0).GetEntity().GetSQL() & " WHERE " & ColumnName & " = :fk"
        End Get
    End Property

End Class
