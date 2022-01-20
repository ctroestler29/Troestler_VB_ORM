Imports System.Data

Public Class LockingDB
    Implements ILockDB

    Private _sk As String

    Public Property sk As String
        Get
            Return _sk
        End Get
        Private Set(ByVal value As String)
            _sk = value
        End Set
    End Property

    Public Property LockingTime As Integer = 120

    Public Sub New()
        sk = Guid.NewGuid().ToString()
    End Sub

    Public Sub Release(o As Object) Implements ILockDB.Release
        Dim keys = getKeys(o)
        Dim cmd As IDbCommand = Connection.CreateCommand()
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
        p.Value = sk
        cmd.Parameters.Add(p)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Public Sub Lock(o As Object) Implements ILockDB.Lock
        Dim hv = getLock(o)
        If Equals(hv, sk) Then Return

        If Equals(hv, Nothing) Then
            createLock(o)
            hv = getLock(o)
        End If

        If Not Equals(hv, sk) Then
            Throw New Exception("Object " + o.GetType.Name + " is locked and therefore could not be reached!")
        End If
    End Sub

    Private Function getKeys(o As Object) As (String, String)
        Dim ent As _Entity = o.GetType().GetEntity
        Return (ent.TableName, ent.PrimaryKey.ToColumnType(ent.PrimaryKey.GetVal(o)).ToString())
    End Function

    Private Sub createLock(ByVal o As Object)
        Dim keys = getKeys(o)
        Dim cmd As IDbCommand = Connection.CreateCommand()
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
        p.Value = sk
        cmd.Parameters.Add(p)

        Try
            cmd.ExecuteNonQuery()
        Catch __unusedException1__ As Exception
        End Try

        cmd.Dispose()
    End Sub

    Public Sub deleteLock()
        Dim cmd As IDbCommand = Connection.CreateCommand()
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
        Dim rval As String = Nothing
        Dim cmd As IDbCommand = Connection.CreateCommand()
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
            rval = re.GetString(0)
        End If

        re.Close()
        re.Dispose()
        cmd.Dispose()
        Return rval
    End Function
End Class
