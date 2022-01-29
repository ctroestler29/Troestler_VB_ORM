Module DeleteObjectTest

    Public Sub DoDelete()
        Console.WriteLine("Remove-Object-Test")
        Dim t = [GetObjectType](Of Teacher)("t.1")
        RemoveObj(t)
        Console.WriteLine("Teacher with ID: " + t.ID + " successfully removed from Database!")
        Console.WriteLine(vbLf)

    End Sub
End Module
