Imports System.Security.Cryptography
Imports System.Text

Public Class Cache
    Implements ICache

    Private _cacheList As Dictionary(Of Type, Dictionary(Of Object, Object)) = New Dictionary(Of Type, Dictionary(Of Object, Object))()
    Private _hash As Dictionary(Of Type, Dictionary(Of Object, String)) = New Dictionary(Of Type, Dictionary(Of Object, String))()

    Protected Function GetCacheList() As Dictionary(Of Type, Dictionary(Of Object, Object))
        Return _cacheList
    End Function

    Protected Sub SetCacheList(value As Dictionary(Of Type, Dictionary(Of Object, Object)))
        _cacheList = value
    End Sub

    Protected Function GetHash() As Dictionary(Of Type, Dictionary(Of Object, String))
        Return _hash
    End Function

    Protected Sub SetHash(value As Dictionary(Of Type, Dictionary(Of Object, String)))
        _hash = value
    End Sub

    Public Overridable Function [GetObjectByPK](t As Type, pk As Object) As Object Implements ICache.GetObjectByPK
        Dim cache = GetCache(t)

        Return If(Not cache.ContainsKey(pk), Nothing, cache(pk))
    End Function

    Public Overridable Function ContainsObjectWithPK(t As Type, pk As Object) As Boolean Implements ICache.ContainsObjectWithPK
        Return GetCache(t).ContainsKey(pk)
    End Function

    Protected Overridable Function GetCache(ByVal t As Type) As Dictionary(Of Object, Object)
        If GetCacheList().ContainsKey(t) Then
            Return GetCacheList()(t)
        End If

        Dim value As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
        GetCacheList().Add(t, value)
        Return value
    End Function

    Public Overridable Sub StoreObject(o As Object) Implements ICache.StoreObject
        If o Is Nothing Then
            Return
        End If
        GetCache(o.GetType())(ORMapper.GetTableOf(o).GetPrimaryKey().GetVal(o)) = o
    End Sub

    Public Overridable Function ObjectChanged(o As Object) As Boolean Implements ICache.ObjectChanged
        Dim h As Dictionary(Of Object, String) = GetHash(o.GetType())
        Dim pk = ORMapper.GetTableOf(o).GetPrimaryKey().GetVal(o)

        If h.ContainsKey(pk) Then
            Return Equals(h(pk), _ComputeHash(o))
        End If

        Return True
    End Function

    Private Function _ComputeHash(o As Object) As Object
        Dim rval = ""

        For Each i In ORMapper.GetTableOf(o).GetInternals()
            Dim m = i.GetVal(o)

            If m IsNot Nothing Then
                If i.GetIsForeignKey() Then
                    If m IsNot Nothing Then
                        rval += ORMapper.GetTableOf(m).GetPrimaryKey().GetVal(m).ToString()
                    End If
                Else
                    rval += (i.GetColumnName() & "=" & m.ToString() & ";")
                End If
            End If
        Next

        For Each i In ORMapper.GetTableOf(o).GetExternals()
            Dim m = CType(i.GetVal(o), IEnumerable)

            If m IsNot Nothing Then
                rval += i.GetColumnName() & "="

                For Each k In m
                    rval += ORMapper.GetTableOf(k).GetPrimaryKey().GetVal(k).ToString() & ","
                Next
            End If
        Next

        Return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rval)))

    End Function

    Protected Overridable Function GetHash(t As Type) As Dictionary(Of Object, String)
        If GetHash().ContainsKey(t) Then
            Return GetHash()(t)
        End If

        Dim rval As Dictionary(Of Object, String) = New Dictionary(Of Object, String)()
        GetHash().Add(t, rval)
        Return rval
    End Function

    Public Overridable Sub RemoveObject(ByVal obj As Object) Implements ICache.RemoveObject
        GetCache(obj.GetType()).Remove(ORMapper.GetTableOf(obj).GetPrimaryKey.GetVal(obj))
    End Sub

End Class
