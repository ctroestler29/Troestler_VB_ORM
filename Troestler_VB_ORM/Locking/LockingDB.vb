Imports System.Data

Public Class LockingDB
    Implements ILockDB

    Private _sk As String

    Public Function GetSk() As String
        Return _sk
    End Function

    Private Sub SetSk(value As String)
        _sk = value
    End Sub

    Public Property LockingTime As Integer = 120

    Public Sub New()
        SetSk(Guid.NewGuid().ToString())
    End Sub

    Public Sub ReleaseObj(o As Object) Implements ILockDB.ReleaseObj
        Dim keys = getKeys(o)
        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        cmd.CommandText = "DELETE FROM LOCKS WHERE JCLASS = :c AND JOBJECT = :o AND JOWNER = :s"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":c"
        p.Value = keys.Item1
        cmd.Parameters.Add(p)
        p = cmd.CreateParameter()
        p.ParameterName = ":o"
        p.Value = keys.Item2
        cmd.Parameters.Add(p)
        p = cmd.CreateParameter()
        p.ParameterName = ":s"
        p.Value = GetSk()
        cmd.Parameters.Add(p)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Public Sub LockObj(o As Object) Implements ILockDB.LockObj
        Dim hv = getLock(o)
        If Equals(hv, GetSk()) Then Return

        If Equals(hv, Nothing) Then
            createLock(o)
            hv = getLock(o)
        End If

        If Not Equals(hv, GetSk()) Then
            Throw New Exception("Object " + o.GetType.Name + " is locked and therefore could not be reached!")
        End If
    End Sub

    Private Function getKeys(o As Object) As (String, String)
        Dim ent As Table = o.GetType().GetTableOf
        Return (ent.GetTableName(), ent.GetPrimaryKey().ToColumnType(ent.GetPrimaryKey().GetVal(o)).ToString())
    End Function

    Private Sub createLock(ByVal o As Object)
        Dim keys = getKeys(o)
        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        cmd.CommandText = "INSERT INTO LOCKS(JCLASS, JOBJECT, JTIME, JOWNER) VALUES (:c, :o, Current_Timestamp, :s)"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":c"
        p.Value = keys.Item1
        cmd.Parameters.Add(p)
        p = cmd.CreateParameter()
        p.ParameterName = ":o"
        p.Value = keys.Item2
        cmd.Parameters.Add(p)
        p = cmd.CreateParameter()
        p.ParameterName = ":s"
        p.Value = GetSk()
        cmd.Parameters.Add(p)

        Try
            cmd.ExecuteNonQuery()
        Catch __unusedException1__ As Exception
        End Try

        cmd.Dispose()
    End Sub

    Public Sub deleteLock()
        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        cmd.CommandText = "DELETE FROM LOCKS WHERE ((JulianDay(Current_Timestamp) - JulianDay(JTIME)) * 86400) > :t"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":t"
        p.Value = LockingTime
        cmd.Parameters.Add(p)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub



    Private Function getLock(ByVal o As Object) As String
        Dim keys = getKeys(o)
        Dim obj As String = Nothing
        Dim cmd As IDbCommand = GetConnection().CreateCommand()
        cmd.CommandText = "SELECT JOWNER FROM LOCKS WHERE JCLASS = :c AND JOBJECT = :o"
        Dim p As IDataParameter = cmd.CreateParameter()
        p.ParameterName = ":c"
        p.Value = keys.Item1
        cmd.Parameters.Add(p)
        p = cmd.CreateParameter()
        p.ParameterName = ":o"
        p.Value = keys.Item2
        cmd.Parameters.Add(p)
        Dim re As IDataReader = cmd.ExecuteReader()

        If re.Read() Then
            obj = re.GetString(0)
        End If

        re.Close()
        re.Dispose()
        cmd.Dispose()
        Return obj
    End Function
End Class
