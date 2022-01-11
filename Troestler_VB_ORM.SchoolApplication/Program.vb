Imports System
Imports System.Data.SQLite
Module Program
    Sub Main(args As String())
        Connection = New SQLiteConnection("Data Source=SchoolDB.sqlite;Version=3;")
        Call Connection.Open()
        ORMapper.Cache = New Cache()
        Call DoInsert()
        Call DoModify()
        Call DOCacheTest()

        Call Connection.Close()
    End Sub
End Module
