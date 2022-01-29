
Imports System
Imports System.Data
Imports System.Data.SQLite
Imports System.Linq
Imports LinqToDB

Module QueryTest
    Public Sub DOQueryTest()
        Console.WriteLine("QUERY_TEST")
        From(Of Teacher)()
        'GreaterThan("Grade", "1")

        Dim res As String = Find()
        Console.WriteLine(vbLf)
    End Sub
End Module
