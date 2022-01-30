
Imports System.Reflection
Friend Class _Entity

    Private _Member As Type
    Private _TableName As String
    Private _Fields As _Field()
    Private _Externals As _Field()
    Private _Internals As _Field()
    Private _PrimaryKey As _Field

    Public Sub New(t As Type)
        If t IsNot Nothing Then


            If CType(t.GetCustomAttribute(GetType(EntityAttr)), EntityAttr) Is Nothing OrElse String.IsNullOrWhiteSpace(CType(t.GetCustomAttribute(GetType(EntityAttr)), EntityAttr).TableName) Then
                SetTableName(t.Name.ToUpper())
            Else
                SetTableName(CType(t.GetCustomAttribute(GetType(EntityAttr)), EntityAttr).TableName)
            End If

            SetMember(t)
            Dim fields As List(Of _Field) = New List(Of _Field)()

            For i1 = 0 To t.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance).Length - 1
                Dim i = t.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)(i1)
                If CType(i.GetCustomAttribute(GetType(IgnoreAttr)), IgnoreAttr) IsNot Nothing Then Continue For
                Dim field As _Field = New _Field(Me)
                Dim fattr = CType(i.GetCustomAttribute(GetType(FieldAttr)), FieldAttr)

                If fattr Is Nothing Then
                    If (i.GetGetMethod() Is Nothing) OrElse (Not i.GetGetMethod().IsPublic) Then Continue For
                    field.SetColumnName(value:=i.Name)
                    field.SetColumnName(value:=i.Name)
                    field.SetColumnType(value:=i.PropertyType)
                Else

                    If TypeOf fattr Is PKAttr Then
                        SetPrimaryKey(field)
                        field.SetIsPrimaryKey(True)
                    End If

                    field.SetColumnName(If(fattr?.ColumnName, i.Name))
                    field.SetColumnType(If(fattr?.ColumnType, i.PropertyType))
                    field.SetIsNullable(fattr.IsNullable)
                    field.SetIsForeignKey((TypeOf fattr Is FKAttr))
                    If field.GetIsForeignKey() Then
                        field.SetIsExternal(GetType(IEnumerable).IsAssignableFrom(i.PropertyType))
                        field.SetAssignmentTable(CType(fattr, FKAttr).AssignmentTable)
                        field.SetRemoteColumnName(CType(fattr, FKAttr).RemoteColumnName)
                        field.SetIsManyToMany(Not String.IsNullOrWhiteSpace(field.GetAssignmentTable()))
                    End If
                End If

                field.SetMember(i)
                fields.Add(field)
            Next

            Me.SetFields(fields.ToArray())
            SetInternals(fields.Where(Function(m) Not m.GetIsExternal()).ToArray())
            SetExternals(fields.Where(Function(m) m.GetIsExternal()).ToArray())
        Else
            Throw New ArgumentNullException(NameOf(t))
        End If
    End Sub

    Public Function GetMember() As Type
        Return _Member
    End Function

    Private Sub SetMember(value As Type)
        _Member = value
    End Sub

    Public Function GetTableName() As String
        Return _TableName
    End Function

    Private Sub SetTableName(value As String)
        _TableName = value
    End Sub

    Public Function GetFields() As _Field()
        Return _Fields
    End Function

    Private Sub SetFields(value As _Field())
        _Fields = value
    End Sub

    Public Function GetExternals() As _Field()
        Return _Externals
    End Function

    Private Sub SetExternals(value As _Field())
        _Externals = value
    End Sub

    Public Function GetInternals() As _Field()
        Return _Internals
    End Function

    Private Sub SetInternals(value As _Field())
        _Internals = value
    End Sub

    Public Function GetPrimaryKey() As _Field
        Return _PrimaryKey
    End Function

    Private Sub SetPrimaryKey(value As _Field)
        _PrimaryKey = value
    End Sub

    Public Function GetSQL(Optional prefix As String = Nothing) As String
        If Equals(prefix, Nothing) Then
            prefix = ""
        End If

        Dim query = "SELECT "

        For i = 0 To GetInternals().Length - 1

            If i > 0 Then
                query += ", "
            End If

            query += prefix.Trim() & GetInternals()(i).GetColumnName()
        Next

        If query Is "SELECT " Then
            query += "*"
        End If
        query += " FROM " & GetTableName()
        Return query
    End Function

    Public Function GetFieldForColumn(ByVal columnName As String) As _Field
        columnName = columnName.ToUpper()

        For Each i In GetInternals()

            If Equals(i.GetColumnName().ToUpper(), columnName) Then
                Return i
            End If
        Next

        Return Nothing
    End Function

    Public Function GetFieldByName(ByVal fieldName As String) As _Field
        For Each i In GetFields()

            If Equals(i.GetMember().Name, fieldName) Then
                Return i
            End If
        Next

        Return Nothing
    End Function


End Class
