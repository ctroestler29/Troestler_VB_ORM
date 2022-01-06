Public Module Insert

    Public Sub DoInsert()

        Dim t As Teacher = New Teacher()
        t.ID = "t.0"
        t.FirstName = "Max"
        t.Name = "Mustermann"
        t.Gender = Gender.MALE
        t.BirthDate = New DateTime(1987, 3, 22)
        t.HireDate = New DateTime(2020, 2, 13)
        t.Salary = 44000
        SaveObj(t)
        Console.WriteLine("Teacher: " + t.FirstName + " " + t.Name + "; successfully created!")
        Console.WriteLine(vbLf)
    End Sub
End Module
