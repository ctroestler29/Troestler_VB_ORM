Imports System
Imports System.Data
Imports System.Data.SQLite
Module Program
    Sub Main(args As String())
        SetConnection(New SQLiteConnection("Data Source=SchoolDB.sqlite;Version=3;"))
        Call GetConnection().Open()


        SetTableManagement(New TableManagement())
        SetCache(New Cache())
        SetQueryBuilder(New QueryBuilderOperations())
        Call DoCreateTable()
        Call DoDropTable()
        Call DoResetSchema()
        Call DoInsert()
        Call DOQueryTest()
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
