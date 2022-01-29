Public Interface ICache
    Function [GetObjectByPK](t As Type, pk As Object) As Object

    Function ContainsObjectWithPK(t As Type, pk As Object) As Boolean
    Sub StoreObject(o As Object)

    Function ObjectChanged(o As Object) As Boolean

    Sub RemoveObject(obj As Object)

End Interface
