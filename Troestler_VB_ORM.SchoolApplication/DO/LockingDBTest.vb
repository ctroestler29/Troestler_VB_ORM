Module LockingDBTest
    Public Sub DoDBLocking()
        Console.WriteLine("Locking DB Test")

        Console.WriteLine()
        SetORLocking(New LockingDB())
        Dim v = [GetObjectType](Of Verein)("v.0")
        LockDBObject(v)
        'ReleaseDBObject(v)
        SetORLocking(New LockingDB())
        Console.WriteLine("Object Verein with Name " + v.Name + " successfully locked in DB!")

        Try
            LockDBObject(v)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        'ReleaseDBObject(v)
        Console.WriteLine(vbLf)



    End Sub
End Module
