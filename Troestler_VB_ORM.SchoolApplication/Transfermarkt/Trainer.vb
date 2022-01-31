<TableAttr(TableName:="Trainer")>
Public Class Trainer
    Inherits Persons

    <FKAttr(ColumnName:="KVerein", IsNullable:=True)>
    Public Property Verein As Verein

    Public Property Gehalt As Integer

    Public Property HireDate As Date

End Class
