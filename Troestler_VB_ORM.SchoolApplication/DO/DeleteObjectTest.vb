Module DeleteObjectTest

    Public Sub DoDelete()
        Console.WriteLine("Remove-Object-Test")
        Dim t = [GetObjectType](Of Trainer)("t.1")
        RemoveObj(t)
        Console.WriteLine("Trainer with ID: " + t.ID + " successfully removed from Database!")
        Console.WriteLine(vbLf)

    End Sub
End Module
