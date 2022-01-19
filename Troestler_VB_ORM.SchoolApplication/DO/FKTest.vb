Module FKTest
    Public Sub DOFKTest()
        Console.WriteLine("FK-Test (Loading Object with Foreign Key):")
        Dim t = [Get](Of Teacher)("t.0")
        Dim c As _Class = New _Class()
        c.ID = "c.0"
        c.Name = "SWE"
        c.Teacher = t
        SaveObj(c)
        c = [Get](Of _Class)("c.0")
        Console.WriteLine(c.Name & " wird von " & c.Teacher.FirstName & " " & c.Teacher.Name & " unterichtet")
        Console.WriteLine(vbLf)
    End Sub
End Module
