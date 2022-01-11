Public Interface ICache
    Function [Get](t As Type, pk As Object) As Object

    Function Contains(t As Type, pk As Object) As Boolean
    Sub Put(o As Object)

End Interface
