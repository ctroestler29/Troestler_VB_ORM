Imports System
Imports System.Data
Imports System.Data.SQLite
Module Program
    Sub Main(args As String())
        Connection = New SQLiteConnection("Data Source=SchoolDB.sqlite;Version=3;")
        Call Connection.Open()
        Dim Str As String = "SELECT group_concat(sql,';') FROM sqlite_master;"
        Dim schema As String = ""
        Dim cmd As IDbCommand = Connection.CreateCommand()
        cmd.CommandText = Str
        Dim re As IDataReader = cmd.ExecuteReader()
        While re.Read
            schema = re.GetString(0)
        End While
        re.Close()

        Dim drop1 As String = "DROP TABLE TEACHERS"
        Dim drop2 As String = "DROP TABLE CLASSES"
        Dim drop3 As String = "DROP TABLE STUDENTS"
        Dim drop4 As String = "DROP TABLE COURSES"
        Dim drop5 As String = "DROP TABLE STUDENT_COURSES"
        Dim drop6 As String = "DROP TABLE STUDENTS_CLASSES"


        cmd.CommandText = drop1
        cmd.ExecuteNonQuery()

        cmd.CommandText = drop2
        cmd.ExecuteNonQuery()

        cmd.CommandText = drop3
        cmd.ExecuteNonQuery()

        cmd.CommandText = drop4
        cmd.ExecuteNonQuery()

        cmd.CommandText = drop5
        cmd.ExecuteNonQuery()

        cmd.CommandText = drop6
        cmd.ExecuteNonQuery()

        cmd.CommandText = schema
        cmd.ExecuteNonQuery()

        ORMapper.Cache = New Cache()
        Call DoInsert()
        Call DoModify()
        Call DOCacheTest()
        Call DOFKTest()
        Call Do_M_N()
        Call Connection.Close()
    End Sub
End Module
