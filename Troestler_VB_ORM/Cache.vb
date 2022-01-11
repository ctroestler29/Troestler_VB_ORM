Public Class Cache
    Implements ICache

    Protected CacheList As Dictionary(Of Type, Dictionary(Of Object, Object)) = New Dictionary(Of Type, Dictionary(Of Object, Object))()


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

End Class
