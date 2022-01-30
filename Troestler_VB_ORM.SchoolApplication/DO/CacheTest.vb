Module CacheTest
    Public Sub DOCacheTest()

        Console.WriteLine("Cache-Test:")
        Call GetInstanceNumber()
        Console.WriteLine(vbLf)
    End Sub


    Private Sub GetInstanceNumber()
        For i = 0 To 10 - 1
            Dim s = [GetObjectType](Of Spieler)("s.0")
            Console.WriteLine("Object with SpielerID: " & s.ID & " and instance-number: " & s.InstanceNumber)
        Next
    End Sub
End Module
