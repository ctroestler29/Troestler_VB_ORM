Module M_N_Test
    Public Sub Do_M_N()
        Console.WriteLine("Testing M:N relations")
        Console.WriteLine("--------------------------------------")
        Dim c As _Class = [GetObjectType](Of _Class)("c.0")
        Dim s As Student = New Student()
        s.ID = "s.0"
        s.Name = "Christoph"
        s.FirstName = "Tröstler"
        s.Gender = Gender.MALE
        s.BirthDate = New DateTime(2000, 1, 27)
        s.Grade = 1
        Save(s)
        c.Students.Add(s)
        s = New Student()
        s.ID = "s.1"
        s.Name = "Jakob"
        s.FirstName = "Huber"
        s.Gender = Gender.MALE
        s.BirthDate = New DateTime(1999, 4, 9)
        s.Grade = 3
        Save(s)
        c.Students.Add(s)
        s = New Student()
        s.ID = "s.2"
        s.Name = "Alicia"
        s.FirstName = "Hager"
        s.Gender = Gender.FEMALE
        s.BirthDate = New DateTime(1998, 2, 2)
        s.Grade = 2
        Save(s)
        c.Students.Add(s)
        Save(c)
        c = [GetObjectType](Of _Class)("c.0")
        Console.WriteLine("Students in " & c.Name & ":")

        For Each i In c.Students
            Console.WriteLine(i.FirstName & " " & i.Name)
        Next

        Console.WriteLine(vbLf)
    End Sub
End Module
