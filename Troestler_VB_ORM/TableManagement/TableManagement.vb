Imports System.Data

Public Class TableManagement
    Implements ITableManagement

    Public Sub CreateTable(Of Type)(Optional tablename As String = Nothing) Implements ITableManagement.CreateTable

        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        Dim t = GetType(Type)
        Dim ent = t.GetEntity

        If tablename Is Nothing Then
            tablename = ent.GetTableName()
        End If

        Dim create = "CREATE TABLE IF NOT EXISTS " + tablename + "( "

        Dim i As Integer = 1
        Dim hv As Integer = ent.GetFields.Length
        For Each item As _Field In ent.GetFields
            create += item.GetColumnName() + " "
            create += GetDBColumType(item.GetColumnType().Name) + " "
            If item.GetIsNullable Then
                create += "NOT NULL"
            End If
            If item.GetIsPrimaryKey Then
                create += "PRIMARY KEY"
            End If

            If i <> hv Then
                create += ","
            End If

            i = i + 1
        Next


        create += ");"

        cmd.CommandText = create
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Public Sub DropTable(Of Type)(Optional tablename As String = Nothing) Implements ITableManagement.DropTable
        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        Dim t = GetType(Type)
        Dim ent = t.GetEntity

        If tablename Is Nothing Then
            tablename = ent.GetTableName()
        End If

        Dim drop = "DROP TABLE " + tablename + ";"

        cmd.CommandText = drop
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Public Sub ResetDBSchema() Implements ITableManagement.ResetDBSchema
        Dim Str As String = "SELECT group_concat(sql,';') FROM sqlite_master;"
        Dim tablequery = "SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%';"
        Dim schema As String = ""
        Dim tables As List(Of String) = New List(Of String)

        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        cmd.CommandText = Str

        Dim re As IDataReader = cmd.ExecuteReader()
        While re.Read
            schema = re.GetString(0)
        End While
        re.Close()
        re.Dispose()


        cmd.CommandText = tablequery
        Dim re2 As IDataReader = cmd.ExecuteReader()
        While re2.Read
            tables.Add(re2.GetString(0))
        End While
        re.Close()
        re.Dispose()

        Dim cmd2 As IDbCommand = GetConnection().CreateCommand()
        Dim drop As String = ""
        For Each item In tables
            drop = "DROP TABLE " + item
            cmd2.CommandText = drop
            cmd2.ExecuteNonQuery()
        Next

        cmd2.CommandText = schema
        cmd2.ExecuteNonQuery()
    End Sub

    Public Function GetDBColumType(t As String)
        If t.Contains("List") Then
            Return "VARCHAR(24)"
        ElseIf t.Contains("DateTime") Then
            Return "TIMESTAMP"
        ElseIf t.Contains("Int") Then
            Return "INTEGER"
        ElseIf t.Contains("String") Then
            Return "VARCHAR(24)"
        ElseIf t.Contains("Gender") Then
            Return "INTEGER"
        End If
        Return ""
    End Function


End Class
