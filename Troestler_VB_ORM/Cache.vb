Public Class Cache
    Implements ICache

    Protected CacheList As Dictionary(Of Type, Dictionary(Of Object, Object)) = New Dictionary(Of Type, Dictionary(Of Object, Object))()


    Public Function [Get](t As Type, pk As Object) As Object Implements ICache.Get
        Dim cache = GetCache(t)

        If cache.ContainsKey(pk) Then
            Return cache(pk)
        End If

        Return Nothing
    End Function

    Public Function Contains(t As Type, pk As Object) As Boolean Implements ICache.Contains
        Return GetCache(t).ContainsKey(pk)
    End Function

    Protected Function GetCache(ByVal t As Type) As Dictionary(Of Object, Object)
        If CacheList.ContainsKey(t) Then
            Return CacheList(t)
        End If

        Dim value As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
        CacheList.Add(t, value)
        Return value
    End Function

    Public Sub Put(o As Object) Implements ICache.Put
        If o IsNot Nothing Then
            GetCache(o.GetType())(ORMapper.GetEntity(o).PrimaryKey.GetVal(o)) = o
        End If
    End Sub
End Class
