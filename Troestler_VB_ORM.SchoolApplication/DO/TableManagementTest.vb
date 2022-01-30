Module TableManagementTest
    Public Sub DoCreateTable()
        CreateTable(Of Verein)()
        CreateTable(Of Trainer)()
        CreateTable(Of Spieler)()
        CreateTable(Of Position)()
        Console.WriteLine("Successfully created Tables")
        Console.WriteLine(vbLf)
    End Sub

    Public Sub DoDropTable()
        DropTable(Of Verein)()
        DropTable(Of Trainer)()
        DropTable(Of Spieler)()
        DropTable(Of Position)()
        Console.WriteLine("Successfully droped Table")
        Console.WriteLine(vbLf)
    End Sub

    Public Sub DoResetSchema()
        ResetSchema()
        Console.WriteLine("Successfully reset schema")
        Console.WriteLine(vbLf)
    End Sub
End Module
