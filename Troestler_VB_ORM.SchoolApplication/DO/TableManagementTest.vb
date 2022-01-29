Module TableManagementTest
    Public Sub DoCreateTable()
        CreateTable(Of TESTCLass)()
        Console.WriteLine("Successfully created Table")
        Console.WriteLine(vbLf)
    End Sub

    Public Sub DoDropTable()
        DropTable(Of TESTCLass)()
        Console.WriteLine("Successfully droped Table")
        Console.WriteLine(vbLf)
    End Sub

    Public Sub DoResetSchema()
        ResetSchema()
        Console.WriteLine("Successfully reset schema")
        Console.WriteLine(vbLf)
    End Sub
End Module
