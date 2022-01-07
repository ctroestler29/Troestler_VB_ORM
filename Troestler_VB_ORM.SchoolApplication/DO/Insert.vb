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
        SaveObj(teacher)
        Console.WriteLine("Teacher: " + teacher.FirstName + " " + teacher.Name + "; successfully created!")
        Console.WriteLine(vbLf)
    End Sub
End Module
