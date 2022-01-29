Imports System.Data

Public Class QueryBuilderOperations
    Implements QueryBuilder

    Public Property sql As String = ""
    Sub From(Of type)() Implements QueryBuilder.From
        Dim t = GetType(type)
        Dim ent = t.GetEntity()
        sql += "SELECT * FROM " + ent.GetTableName() + " "
    End Sub

    Public Sub GreaterThan(field As String, value As String) Implements QueryBuilder.GreaterThan
        sql += "WHERE " + field + ">=" + value
    End Sub

    Public Sub is()

    Public Function Find() As String Implements QueryBuilder.Find
        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        sql += ";"
        cmd.CommandText = sql
        Dim re As IDataReader = cmd.ExecuteReader()

        Dim res = ""
        While re.Read
            res += re.GetString(0)
            res += ";"
        End While
        re.Close()
        re.Dispose()

        Return res
    End Function



End Class
