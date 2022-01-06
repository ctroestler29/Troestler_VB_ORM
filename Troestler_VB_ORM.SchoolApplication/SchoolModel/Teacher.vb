<EntityAttr(TableName:="TEACHERS")>
Public Class Teacher
    Inherits Person

    <FieldAttr(ColumnName:="HDATE")>
    Public Property HireDate As Date
    Public Property Salary As Integer

    Private _CList As List(Of _Class)


    <FKAttr(ColumnName:="KTEACHER")>
    Public Property CList As List(Of _Class)
        Get
            Return _CList
        End Get
        Private Set(value As List(Of _Class))
            _CList = value
        End Set
    End Property
End Class
