Module Modify
    Public Sub DoModify()

        Console.WriteLine("Modify-Test")
        Dim teacher = [GetObjectType](Of Teacher)("t.0")
        Console.WriteLine(teacher.FirstName & " " & teacher.Name & " hat eine Gehalt von " & teacher.Salary & " Euro.")
        Console.WriteLine("Change first name to Markus.")
        teacher.FirstName = "Markus"
        Save(teacher)
        Dim teacher2 = [GetObjectType](Of Teacher)("t.0")
        Console.WriteLine(teacher2.FirstName & " " & teacher2.Name & " hat eine Gehalt von " & teacher2.Salary & " Euro.")
        Console.WriteLine(vbLf)

    End Sub
End Module
