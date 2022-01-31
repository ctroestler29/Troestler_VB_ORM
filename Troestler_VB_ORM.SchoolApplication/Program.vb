Imports System.Data.SQLite

Module Program
    Sub Main(args As String())
        SetConnection(New SQLiteConnection("Data Source=Transfermarkt.db;"))
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
