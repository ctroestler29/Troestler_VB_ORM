Imports System.Data.SQLite
Imports NUnit.Framework
Imports Troestler_VB_ORM.SchoolApplication

Namespace Troestler_VB_ORM.Testing

    Public Class ORMTests

        <SetUp>
        Public Sub Setup()

            SetConnection(New SQLiteConnection("Data Source=TestingDB.db;"))
            GetConnection().Open()

            SetTableManagement(New TableManagement())
            SetLocalCache(New Cache())
            CreateTable(Of LOCKS)()
            CreateIndex("INDEX_LOCKS", "LOCKS")
            CreateTable(Of Spieler_Positionen)()
            CreateTable(Of Verein)()
            CreateTable(Of Trainer)()
            CreateTable(Of Spieler)()
            CreateTable(Of Position)()
            ResetSchema()

        End Sub

        <Test>
        Public Sub ORM_SaveObj_no_PK()

            Dim t As Trainer = New Trainer()
            t.FirstName = "Vorname"
            t.Name = "Nachname"
            t.BirthDate = New Date(1950, 6, 28)
            t.HireDate = New Date(2001, 1, 1)
            t.Gehalt = 123000
            Dim hv = Nothing
            Try
                SaveObject(t)
            Catch ex As Exception
                hv = "ID cant be null"
            End Try

            Assert.IsNotNull(hv)

        End Sub

        <Test>
        Public Sub ORM_SaveObj_success()

            Dim t As Trainer = New Trainer()
            t.ID = "t.0"
            t.FirstName = "Vorname"
            t.Name = "Nachname"
            t.BirthDate = New Date(1950, 6, 28)
            t.HireDate = New Date(2001, 1, 1)
            t.Gehalt = 123000

            SaveObject(t)

            Assert.IsInstanceOf(Of Trainer)(GetObjectType(Of Trainer)("t.0"))
        End Sub

        <Test>
        Public Sub ORM_RemoveObj()

            Dim t As Trainer = New Trainer()
            t.ID = "t.0"
            t.FirstName = "Vorname"
            t.Name = "Nachname"
            t.BirthDate = New Date(1950, 6, 28)
            t.HireDate = New Date(2001, 1, 1)
            t.Gehalt = 123000

            SaveObject(t)

            RemoveObj(t)

            t = GetObjectType(Of Trainer)("t.0")

            Assert.IsNull(t)
        End Sub

        <Test>
        Public Sub ORM_Cache_Put_success()

            Dim c As Cache = GetLocalCache()
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            c.StoreObject(s)

            Dim gcs As Spieler = c.GetObjectByPK(s.GetType(), "s.0")
            Assert.AreEqual(s.ID, gcs.ID)

        End Sub

        <Test>
        Public Sub Cache_Contains_isTrue()

            Dim c As Cache = GetLocalCache()
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            c.StoreObject(s)

            Assert.IsTrue(c.ContainsObjectWithPK(s.GetType(), "s.0"))

        End Sub

        <Test>
        Public Sub Cache_RemoveObject_success()

            Dim c As Cache = GetLocalCache()
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            c.StoreObject(s)
            c.RemoveObject(s)

            Assert.IsFalse(c.ContainsObjectWithPK(s.GetType(), "s.0"))

        End Sub

        <Test>
        Public Sub Cache_Contains_isFalse()

            Dim c As Cache = GetLocalCache()
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            c.StoreObject(s)

            Assert.IsFalse(c.ContainsObjectWithPK(s.GetType(), "s.2"))

        End Sub

        <Test>
        Public Sub Cache_ObjectChanged_isTrue()

            Dim c As Cache = GetLocalCache()
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            s.Name = "Christoph"
            s.Nummer = 8
            c.StoreObject(s)
            s.Nummer = 11

            Assert.IsTrue(c.ObjectChanged(s))

        End Sub

        <Test>
        Public Sub Drop_Table_success()

            DropTable(Of Spieler)()
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            s.Name = "Christoph"
            s.Nummer = 8

            Dim hv = Nothing
            Try
                SaveObject(s)
            Catch ex As Exception
                hv = "No such Table"
            End Try
            CreateTable(Of Spieler)()
            Assert.IsNotNull(hv)

        End Sub

        <Test>
        Public Sub LockingObject_success()

            SetORLocking(New LockingDB())
            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            s.Name = "Tröstler"
            s.Nummer = 8
            s.Marktwert = 2000
            s.FirstName = "Christoph"
            s.BirthDate = New Date(2000, 1, 27)
            SaveObject(s)

            LockDBObject(s)
            SetORLocking(New LockingDB())

            Dim hv = Nothing
            Try
                LockDBObject(s)
            Catch ex As Exception
                hv = "Object is locked"
            End Try
            Assert.IsNotNull(hv)

        End Sub

        <Test>
        Public Sub QueryBuilder_Select_success()

            Dim s As Spieler = New Spieler()
            s.ID = "s.0"
            s.Name = "Tröstler"
            s.Nummer = 8
            s.Marktwert = 2000
            s.FirstName = "Christoph"
            s.BirthDate = New Date(2000, 1, 27)
            SaveObject(s)

            Dim spielers As QueryBuilder(Of Spieler) = [Select](Of Spieler)()

            Assert.IsTrue(spielers.Count = 1)

        End Sub

        <Test>
        Public Sub QueryBuilder_Select_no_success()

            Dim spielers As QueryBuilder(Of Spieler) = [Select](Of Spieler)()

            Assert.IsTrue(spielers.Count = 0)

        End Sub


    End Class

End Namespace