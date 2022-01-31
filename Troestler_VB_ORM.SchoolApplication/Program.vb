Imports System
Imports System.Data
Imports System.Data.SQLite
Imports Npgsql

Module Program
    Sub Main(args As String())
        SetConnection(New SQLiteConnection("Data Source=Transfermarkt.db;"))
        'Call GetConnection().Open()
        'Dim connectionString As String = "Server=127.0.0.1;Port=5432;Database=Troestler_VB_ORM_DB;User Id=postgres;Password=root;"
        'SetConnection(New NpgsqlConnection(connectionString))
        Call GetConnection().Open()

        SetTableManagement(New TableManagement())
        SetLocalCache(New Cache())
        Call DoDropTable()
        Call DoCreateTable()
        Call DoCreateIndex()
        Call DoResetSchema()
        Call DoInsert()
        Call DoDelete()
        Call DoModify()
        Call DOCacheTest()
        Call DOFKTest()
        Call Do_M_N()
        Call DoDBLocking()
        Call DOQueryTest()
        Call GetConnection().Close()
    End Sub
End Module
