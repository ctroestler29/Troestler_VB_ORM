<EntityAttr(TableName:="Verein")>
Public Class Verein

    <PKAttr>
    Public Property ID As String

    Public Property Name As String

    Public Property Adresse As String


    <FKAttr(ColumnName:="KTrainer")>
    Private Property _Trainer As LazyLoadingObj(Of Trainer) = New LazyLoadingObj(Of Trainer)()

    <IgnoreAttr>
    Public Property Trainer As Trainer
        Get
            Return _Trainer.Value
        End Get
        Set(ByVal value As Trainer)
            _Trainer.Value = value
        End Set
    End Property

    <FKAttr(ColumnName:="KVerein")>
    Public Property Spieler As LazyLoadingList(Of Spieler)

    Public Sub New()
        Spieler = New LazyLoadingList(Of Spieler)(Me, "Spieler")
    End Sub

End Class
