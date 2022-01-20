Module LockingDBTest
    Public Sub DoDBLocking()
        Console.WriteLine("Locking DB Test")
        Console.WriteLine()
        ORLocking = New LockingDB()
        Dim c = [Get](Of _Class)("c.0")
        LockDBObject(c)
        ORLocking = New LockingDB()
        Console.WriteLine("Object _Class with Name " + c.Name + " successfully locked in DB!")

        Try
            LockDBObject(c)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Console.WriteLine(vbLf)
    End Sub
End Module
