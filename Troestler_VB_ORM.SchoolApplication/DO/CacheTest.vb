Module CacheTest
    Public Sub DOCacheTest()

        Console.WriteLine("Cache-Test:")
        Call GetInstanceNumber()
        Console.WriteLine(vbLf)
    End Sub


    Private Sub GetInstanceNumber()
        For i = 0 To 10 - 1
            Dim t = [Get](Of Teacher)("t.0")
            Console.WriteLine("Object with TeacherID: " & t.ID & " and instance-number: " & t.InstanceNumber)
        Next
    End Sub
End Module
