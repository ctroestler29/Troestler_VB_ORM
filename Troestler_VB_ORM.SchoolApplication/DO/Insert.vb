Public Module Insert

    Public Sub DoInsert()
        Console.WriteLine("Insert-Test")
        Dim teacher As Teacher = New Teacher()
        teacher.ID = "t.0"
        teacher.FirstName = "Max"
        teacher.Name = "Mustermann"
        teacher.Gender = Gender.MALE
        teacher.BirthDate = New DateTime(1987, 3, 22)
        teacher.HireDate = New DateTime(2020, 2, 13)
        teacher.Salary = 44000
        Save(teacher)
        Console.WriteLine("Teacher: " + teacher.FirstName + " " + teacher.Name + "; successfully created!")

        teacher.ID = "t.1"
        teacher.FirstName = "Markus"
        teacher.Name = "Winzer"
        teacher.Gender = Gender.MALE
        teacher.BirthDate = New DateTime(1991, 3, 19)
        teacher.HireDate = New DateTime(2022, 1, 1)
        teacher.Salary = 36000
        Save(teacher)
        Console.WriteLine("Teacher: " + teacher.FirstName + " " + teacher.Name + "; successfully created!")
        Console.WriteLine(vbLf)
    End Sub
End Module
