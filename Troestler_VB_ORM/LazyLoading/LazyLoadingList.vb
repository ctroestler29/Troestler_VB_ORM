Public Class LazyLoadingList(Of T)
    Implements IList(Of T), ILazyLoading

    Protected _InternalItems As List(Of T) = Nothing

    Protected sql As String

    Protected parameters As ICollection(Of Tuple(Of String, Object))


    Protected Friend Sub New(ByVal _sql As String, ByVal _parameters As ICollection(Of Tuple(Of String, Object)))
        sql = _sql
        parameters = _parameters
    End Sub


    Public Sub New(ByVal obj As Object, ByVal fieldName As String)
        Dim f = ORMapper.GetTableOf(obj) _
                        .GetFieldByName(fieldName)
        sql = f.Get_FkSql()
        Dim tuples As Tuple(Of String, Object)() = New Tuple(Of String, Object)() {New Tuple(Of String, Object)(":fk", f.GetTable().GetPrimaryKey().GetVal(obj))}
        parameters = tuples
    End Sub

    Protected Function GetItems() As List(Of T)
        If _InternalItems Is Nothing Then
            _InternalItems = New List(Of T)()
            FList(_InternalItems, sql, GetType(T), parameters)
        End If

        Return _InternalItems
    End Function

    Default Public Property Item(index As Integer) As T Implements IList(Of T).Item
        Get
            Return GetItems()(index)
        End Get
        Set(value As T)
            GetItems()(index) = value
        End Set
    End Property

    Public ReadOnly Property Count As Integer Implements ICollection(Of T).Count
        Get
            Return GetItems().Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
        Get
            Return CType(GetItems(), IList(Of T)).IsReadOnly
        End Get
    End Property

    Public Sub Insert(index As Integer, item As T) Implements IList(Of T).Insert
        GetItems().Insert(index, item)
    End Sub

    Public Sub RemoveAt(index As Integer) Implements IList(Of T).RemoveAt
        GetItems().RemoveAt(index)
    End Sub

    Public Sub Add(item As T) Implements ICollection(Of T).Add
        GetItems().Add(item)
    End Sub

    Public Sub Clear() Implements ICollection(Of T).Clear
        GetItems().Clear()
    End Sub

    Public Sub CopyTo(array() As T, arrayIndex As Integer) Implements ICollection(Of T).CopyTo
        GetItems().CopyTo(array, arrayIndex)
    End Sub

    Public Function IndexOf(item As T) As Integer Implements IList(Of T).IndexOf
        Return GetItems().IndexOf(item)
    End Function

    Public Function Contains(item As T) As Boolean Implements ICollection(Of T).Contains
        Return GetItems().Contains(item)
    End Function

    Public Function Remove(item As T) As Boolean Implements ICollection(Of T).Remove
        Return GetItems().Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Return GetItems().GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetItems().GetEnumerator()
    End Function
End Class
