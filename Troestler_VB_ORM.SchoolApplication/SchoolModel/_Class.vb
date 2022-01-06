<EntityAttr(TableName:="CLASSES")>
Public Class _Class

    <PKAttr>
    Public Property ID As String

    Public Property Name As String

    <FKAttr(ColumnName:="KTEACHER")>
    Private Property _Teacher As Teacher = New Teacher()

    <IgnoreAttr>
    Public Property Teacher As Teacher
        Get
            Return _Teacher
        End Get
        Set(value As Teacher)
            _Teacher = value
        End Set
    End Property

    <FKAttr(ColumnName:="KCLASS")>
    Public Property Students As List(Of Student)

    Public Sub New()
        Students = New List(Of Student)
    End Sub
End Class
