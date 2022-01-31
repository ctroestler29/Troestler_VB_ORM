Public Module Insert

    Public Sub DoInsert()
        Console.WriteLine("Insert-Test")

        Dim trainer As Trainer = New Trainer()
        trainer.ID = "t.0"
        trainer.FirstName = "Markus"
        trainer.Name = "Winzer"
        'trainer.Gender = Gender.MALE
        trainer.BirthDate = New Date(1991, 3, 19)
        trainer.HireDate = New Date(2022, 1, 1)
        trainer.Gehalt = 36000
        SaveObject(trainer)
        trainer = [GetObjectType](Of Trainer)("t.0")
        Console.WriteLine("Trainer: " + trainer.FirstName + " " + trainer.Name + "; successfully created!")

        Dim trainer2 As Trainer = New Trainer()
        trainer2.ID = "t.1"
        trainer2.FirstName = "Chris"
        trainer2.Name = "Muster"
        'trainer2.Gender = Gender.MALE
        trainer2.BirthDate = New Date(1978, 2, 12)
        trainer2.HireDate = New Date(2010, 1, 1)
        trainer2.Gehalt = 66000
        SaveObject(trainer2)
        trainer2 = [GetObjectType](Of Trainer)("t.1")
        Console.WriteLine("Trainer: " + trainer2.FirstName + " " + trainer2.Name + "; successfully created!")

        Dim spieler As Spieler = New Spieler()
        spieler.ID = "s.0"
        spieler.FirstName = "Christoph"
        spieler.Name = "Tröstler"
        'spieler.Gender = Gender.MALE
        spieler.BirthDate = New Date(2000, 1, 27)
        spieler.Nummer = 9
        spieler.Marktwert = 100000
        SaveObject(spieler)
        Console.WriteLine("Spieler: " + spieler.FirstName + " " + spieler.Name + "; successfully created!")
        Console.WriteLine(vbLf)
    End Sub
End Module
