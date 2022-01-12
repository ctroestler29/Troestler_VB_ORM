<EntityAttr(TableName:="CLASSES")>
Public Class _Class

    <PKAttr>
    Public Property ID As String

    Public Property Name As String

    <FKAttr(ColumnName:="KTEACHER")>
    Private Property _Teacher As LazyLoadingObj(Of Teacher) = New LazyLoadingObj(Of Teacher)()

    <IgnoreAttr>
    Public Property Teacher As Teacher
        Get
            Return _Teacher.Value
        End Get
        Set(value As Teacher)
            _Teacher.Value = value
        End Set
    End Property

    <FKAttr(ColumnName:="KCLASS")>
    Public Property Students As LazyLoadingList(Of Student)

    Public Sub New()
        Students = New LazyLoadingList(Of Student)(Me, "Students")
    End Sub
End Class
