Imports System.Security.Cryptography
Imports System.Text

Public Class Cache
    Implements ICache

    Protected CacheList As Dictionary(Of Type, Dictionary(Of Object, Object)) = New Dictionary(Of Type, Dictionary(Of Object, Object))()
    Protected hash As Dictionary(Of Type, Dictionary(Of Object, String)) = New Dictionary(Of Type, Dictionary(Of Object, String))()

    Public Overridable Function [Get](t As Type, pk As Object) As Object Implements ICache.Get
        Dim cache = GetCache(t)

        Return If(cache.ContainsKey(pk), cache(pk), Nothing)
    End Function

    Public Overridable Function Contains(t As Type, pk As Object) As Boolean Implements ICache.Contains
        Return GetCache(t).ContainsKey(pk)
    End Function

    Protected Overridable Function GetCache(ByVal t As Type) As Dictionary(Of Object, Object)
        If Not CacheList.ContainsKey(t) Then

            Dim value As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
            CacheList.Add(t, value)
            Return value
        End If
        Return CacheList(t)
    End Function

    Public Overridable Sub Put(o As Object) Implements ICache.Put
        If o Is Nothing Then
            Return
        End If
        GetCache(o.GetType())(ORMapper.GetEntity(o).PrimaryKey.GetVal(o)) = o
    End Sub

    Public Overridable Function Changed(o As Object) As Boolean Implements ICache.Changed
        Dim h As Dictionary(Of Object, String) = GetHash(o.GetType())
        Dim pk = ORMapper.GetEntity(o).PrimaryKey.GetVal(o)

        If h.ContainsKey(pk) Then
            Return Equals(h(pk), _ComputeHash(o))
        End If

        Return True
    End Function

    Private Function _ComputeHash(o As Object) As Object
        Dim rval = ""

        For Each i In ORMapper.GetEntity(o).Internals
            Dim m = i.GetVal(o)

            If m IsNot Nothing Then
                If i.IsForeignKey Then
                    If m IsNot Nothing Then
                        rval += ORMapper.GetEntity(m).PrimaryKey.GetVal(m).ToString()
                    End If
                Else
                    rval += (i.ColumnName & "=" & m.ToString() & ";")
                End If
            End If
        Next

        For Each i In ORMapper.GetEntity(o).Externals
            Dim m = CType(i.GetVal(o), IEnumerable)

            If m IsNot Nothing Then
                rval += i.ColumnName & "="

                For Each k In m
                    rval += ORMapper.GetEntity(k).PrimaryKey.GetVal(k).ToString() & ","
                Next
            End If
        Next

        Return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rval)))

    End Function

    Protected Overridable Function GetHash(t As Type) As Dictionary(Of Object, String)
        If hash.ContainsKey(t) Then
            Return hash(t)
        End If

        Dim rval As Dictionary(Of Object, String) = New Dictionary(Of Object, String)()
        hash.Add(t, rval)
        Return rval
    End Function

End Class
