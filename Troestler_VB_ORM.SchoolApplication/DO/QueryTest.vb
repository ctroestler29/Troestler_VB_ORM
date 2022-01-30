
Imports System
Imports System.Data
Imports System.Data.SQLite
Imports System.Linq
Imports LinqToDB

Module QueryTest
    Public Sub DOQueryTest()
        Console.WriteLine("QUERY_TEST")
        Console.WriteLine()
        Console.WriteLine("Alle Spieler die die Position Mittelfeld spielen: ")
        Dim spielers As QueryBuilder(Of Spieler) = [Select](Of Spieler)()
        Dim position As QueryBuilder(Of Position) = [Select](Of Position)().Like("Beschreibung", "Mittelfeld")

        For Each item In spielers
            If item.PositionList.Contains(position(0)) Then
                Console.WriteLine(item.FirstName + " " + item.Name)
            End If
        Next
        Console.WriteLine(vbLf)

        Dim trainer As QueryBuilder(Of Trainer) = [Select](Of Trainer)().GreaterThanOrEqual("Gehalt", 36000)
        Console.WriteLine("Alle Trainer mit einem mindest Gehalt von 36000: ")
        For Each item In trainer
            Console.WriteLine(item.FirstName + " " + item.Name)
        Next
        Console.WriteLine(vbLf)
    End Sub
End Module
