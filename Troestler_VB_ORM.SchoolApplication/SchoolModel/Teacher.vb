<EntityAttr(TableName:="TEACHERS")>
Public Class Teacher
    Inherits Person

    <FieldAttr(ColumnName:="HDATE")>
    Public Property HireDate As Date
    Public Property Salary As Integer

    <FKAttr(ColumnName:="KTEACHER")>
    Public Property _CList As List(Of _Class) = New List(Of _Class)
End Class
