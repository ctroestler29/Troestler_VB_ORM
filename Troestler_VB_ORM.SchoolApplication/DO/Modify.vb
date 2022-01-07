Module Modify
    Public Sub DoModify()

        Console.WriteLine("Modify-Test")
        Dim teacher = [Get](Of Teacher)("t.0")
        Console.WriteLine(teacher.FirstName & " " & teacher.Name & " hat eine Gehalt von " & teacher.Salary & " Euro.")
        Console.WriteLine("Change first name to Markus.")
        teacher.FirstName = "Markus"
        SaveObj(teacher)
        Dim teacher2 = [Get](Of Teacher)("t.0")
        Console.WriteLine(teacher2.FirstName & " " & teacher2.Name & " hat eine Gehalt von " & teacher2.Salary & " Euro.")
        Console.WriteLine(vbLf)

    End Sub
End Module
