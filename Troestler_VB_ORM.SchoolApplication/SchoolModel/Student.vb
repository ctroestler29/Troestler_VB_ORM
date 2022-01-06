<EntityAttr(TableName:="STUDENTS")>
Public Class Student
    Inherits Person

    Public Property Grade As Integer

    <FKAttr(ColumnName:="KCLASS")>
    Public Property [Class] As _Class
End Class
