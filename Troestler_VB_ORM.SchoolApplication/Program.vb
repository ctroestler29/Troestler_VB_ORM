Imports System
Imports System.Data.SQLite
Module Program
    Sub Main(args As String())
        Connection = New SQLiteConnection("Data Source=SchoolDB.sqlite;Version=3;")
        Call Connection.Open()
        Call DoInsert()
        Call Connection.Close()
    End Sub
End Module
