Module Modify
    Public Sub DoModify()

        Console.WriteLine("Modify-Test")

        Dim spieler = [GetObjectType](Of Spieler)("s.0")
        Console.WriteLine(spieler.FirstName & " " & spieler.Name & " hat die Rückennummer " & spieler.Nummer)
        Console.WriteLine("Ändere die Rückennummer zu 11")
        spieler.Nummer = 11
        SaveObject(spieler)
        spieler = [GetObjectType](Of Spieler)("s.0")
        Console.WriteLine(spieler.FirstName & " " & spieler.Name & " hat nun die Rückennummer " & spieler.Nummer)
        Console.WriteLine(vbLf)

    End Sub
End Module
