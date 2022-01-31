<TableAttr(TableName:="Spieler")>
Public Class Spieler
    Inherits Persons

    Public Property Nummer As Integer

    <FKAttr(ColumnName:="KVerein", IsNullable:=True)>
    Public Property Verein As Verein

    Public Property Marktwert As String

    <FKAttr(AssignmentTable:="Spieler_Positionen", ColumnName:="KSpieler", RemoteColumnName:="KPosition", IsNullable:=True)>
    Public Property PositionList As List(Of Position) = New List(Of Position)()

End Class
