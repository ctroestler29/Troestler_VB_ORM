Module M_N_Test
    Public Sub Do_M_N()
        Console.WriteLine("Testing M:N relations")
        Console.WriteLine("--------------------------------------")

        Dim position As Position = New Position()
        position.ID = "p.0"
        position.Beschreibung = "Stürmer"
        Save(position)
        position = New Position()
        position.ID = "p.1"
        position.Beschreibung = "Mittelfeld"
        Save(position)
        position = New Position()
        position.ID = "p.2"
        position.Beschreibung = "Verteidigung"
        Save(position)
        position = New Position()
        position.ID = "p.3"
        position.Beschreibung = "Tormann"
        Save(position)


        Dim p As Position = [GetObjectType](Of Position)("p.2")
        Dim p2 As Position = [GetObjectType](Of Position)("p.1")

        Dim spieler As Spieler = New Spieler()
        spieler.ID = "s.1"
        spieler.Name = "Max"
        spieler.FirstName = "Haller"
        spieler.Gender = Gender.MALE
        spieler.BirthDate = New DateTime(2001, 4, 23)
        spieler.Nummer = 2
        spieler.Marktwert = 50000
        spieler.PositionList.Add(p)
        spieler.PositionList.Add(p2)
        Save(spieler)

        Dim s As Spieler = [GetObjectType](Of Spieler)("s.0")
        s.PositionList.Add([GetObjectType](Of Position)("p.0"))
        s.PositionList.Add([GetObjectType](Of Position)("p.1"))
        Save(s)

        s = [GetObjectType](Of Spieler)("s.1")
        Console.WriteLine(s.FirstName + " " + s.Name + " spielt folgende Position(en): ")

        For Each item In s.PositionList
            Console.WriteLine(item.Beschreibung)
        Next
        Console.WriteLine(vbLf)
        s = [GetObjectType](Of Spieler)("s.0")
        Console.WriteLine(s.FirstName + " " + s.Name + " spielt folgende Position(en): ")

        For Each item In s.PositionList
            Console.WriteLine(item.Beschreibung)
        Next


        Console.WriteLine(vbLf)
    End Sub
End Module
