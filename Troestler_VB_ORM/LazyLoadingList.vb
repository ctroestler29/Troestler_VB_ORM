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
        Dim f = ORMapper.GetEntity(obj).GetFieldByName(fieldName)
        sql = f._FkSql
        parameters = New Tuple(Of String, Object)() {New Tuple(Of String, Object)(":fk", f.Entity.PrimaryKey.GetVal(obj))}
    End Sub


    Protected ReadOnly Property _Items As List(Of T)
        Get

            If _InternalItems Is Nothing Then
                _InternalItems = New List(Of T)()
                _FillList(GetType(T), _InternalItems, sql, parameters)
            End If

            Return _InternalItems
        End Get
    End Property


    Default Public Property Item(index As Integer) As T Implements IList(Of T).Item
        Get
            Return _Items(index)
        End Get
        Set(value As T)
            _Items(index) = value
        End Set
    End Property

    Public ReadOnly Property Count As Integer Implements ICollection(Of T).Count
        Get
            Return _Items.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
        Get
            Return CType(_Items, IList(Of T)).IsReadOnly
        End Get
    End Property

    Public Sub Insert(index As Integer, item As T) Implements IList(Of T).Insert
        _Items.Insert(index, item)
    End Sub

    Public Sub RemoveAt(index As Integer) Implements IList(Of T).RemoveAt
        _Items.RemoveAt(index)
    End Sub

    Public Sub Add(item As T) Implements ICollection(Of T).Add
        _Items.Add(item)
    End Sub

    Public Sub Clear() Implements ICollection(Of T).Clear
        _Items.Clear()
    End Sub

    Public Sub CopyTo(array() As T, arrayIndex As Integer) Implements ICollection(Of T).CopyTo
        _Items.CopyTo(array, arrayIndex)
    End Sub

    Public Function IndexOf(item As T) As Integer Implements IList(Of T).IndexOf
        Return _Items.IndexOf(item)
    End Function

    Public Function Contains(item As T) As Boolean Implements ICollection(Of T).Contains
        Return _Items.Contains(item)
    End Function

    Public Function Remove(item As T) As Boolean Implements ICollection(Of T).Remove
        Return _Items.Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Return _Items.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return _Items.GetEnumerator()
    End Function
End Class
