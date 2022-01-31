Module FKTest
    Public Sub DOFKTest()
        Console.WriteLine("FK-Test (Loading Object with Foreign Key):")


        Dim trainer = [GetObjectType](Of Trainer)("t.0")
        Dim spieler = [GetObjectType](Of Spieler)("s.0")
        Dim verein As Verein = New Verein()
        verein.ID = "v.0"
        verein.Name = "SCG Eckartsau"
        verein.Adresse = "Teststrasse 2"
        verein.Trainer = trainer
        verein.Spieler.Add(spieler)
        SaveObject(verein)

        verein = [GetObjectType](Of Verein)("v.0")
        Console.WriteLine(verein.Name & " wird von " & verein.Trainer.FirstName & " " & verein.Trainer.Name & " trainiert")
        Console.WriteLine(verein.Name & " hat folgende Spieler: ")
        For Each s In verein.Spieler
            Console.WriteLine(s.FirstName + " " + s.Name)
        Next
        Console.WriteLine(vbLf)


    End Sub
End Module
