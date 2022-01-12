
Imports System.Reflection
Friend Class _Entity

    Private _Member As System.Type
    Private _TableName As String
    Private _Fields As _Field()
    Private _Externals As _Field()
    Private _Internals As _Field()
    Private _PrimaryKey As _Field

    Public Sub New(t As Type)
        Dim tattr = CType(t.GetCustomAttribute(GetType(EntityAttr)), EntityAttr)

        If tattr Is Nothing OrElse String.IsNullOrWhiteSpace(tattr.TableName) Then
            TableName = t.Name.ToUpper()
        Else
            TableName = tattr.TableName
        End If

        Member = t
        Dim fields As List(Of _Field) = New List(Of _Field)()

        Dim array = t.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
        For i1 = 0 To array.Length - 1
            Dim i = array(i1)
            If CType(i.GetCustomAttribute(GetType(IgnoreAttr)), IgnoreAttr) IsNot Nothing Then Continue For
            Dim field As _Field = New _Field(Me)
            Dim fattr = CType(i.GetCustomAttribute(GetType(FieldAttr)), FieldAttr)

            If fattr IsNot Nothing Then
                If TypeOf fattr Is PKAttr Then
                    PrimaryKey = field
                    field.IsPrimaryKey = True
                End If

                field.ColumnName = If(fattr?.ColumnName, i.Name)
                field.ColumnType = If(fattr?.ColumnType, i.PropertyType)
                field.IsNullable = fattr.IsNullable
                field.IsForeignKey = (TypeOf fattr Is FKAttr)
                If field.IsForeignKey Then
                    field.IsExternal = GetType(IEnumerable).IsAssignableFrom(i.PropertyType)
                    field.AssignmentTable = CType(fattr, FKAttr).AssignmentTable
                    field.RemoteColumnName = CType(fattr, FKAttr).RemoteColumnName
                    field.IsManyToMany = Not String.IsNullOrWhiteSpace(field.AssignmentTable)
                End If
            Else
                If (i.GetGetMethod() Is Nothing) OrElse (Not i.GetGetMethod().IsPublic) Then Continue For
                field.ColumnName = i.Name
                field.ColumnName = i.Name
                field.ColumnType = i.PropertyType
            End If

            field.Member = i
            fields.Add(field)
        Next

        Me.Fields = fields.ToArray()
        Internals = fields.Where(Function(m) Not m.IsExternal).ToArray()
        Externals = fields.Where(Function(m) m.IsExternal).ToArray()
    End Sub

    Public Property Member As Type
        Get
            Return _Member
        End Get
        Private Set(value As Type)
            _Member = value
        End Set
    End Property

    Public Property TableName As String
        Get
            Return _TableName
        End Get
        Private Set(value As String)
            _TableName = value
        End Set
    End Property

    Public Property Fields As _Field()
        Get
            Return _Fields
        End Get
        Private Set(value As _Field())
            _Fields = value
        End Set
    End Property

    Public Property Externals As _Field()
        Get
            Return _Externals
        End Get
        Private Set(value As _Field())
            _Externals = value
        End Set
    End Property

    Public Property Internals As _Field()
        Get
            Return _Internals
        End Get
        Private Set(value As _Field())
            _Internals = value
        End Set
    End Property

    Public Property PrimaryKey As _Field
        Get
            Return _PrimaryKey
        End Get
        Private Set(value As _Field)
            _PrimaryKey = value
        End Set
    End Property

    Public Function GetSQL(Optional prefix As String = Nothing) As String
        If Equals(prefix, Nothing) Then
            prefix = ""
        End If

        Dim query = "SELECT "

        For i = 0 To Internals.Length - 1

            If i > 0 Then
                query += ", "
            End If

            query += prefix.Trim() & Internals(i).ColumnName
        Next

        query += " FROM " & TableName
        Return query
    End Function

    Public Function GetFieldForColumn(ByVal columnName As String) As _Field
        columnName = columnName.ToUpper()

        For Each i In Internals

            If Equals(i.ColumnName.ToUpper(), columnName) Then
                Return i
            End If
        Next

        Return Nothing
    End Function

    Public Function GetFieldByName(ByVal fieldName As String) As _Field
        For Each i In Fields

            If Equals(i.Member.Name, fieldName) Then
                Return i
            End If
        Next

        Return Nothing
    End Function


End Class
