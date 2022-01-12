Public Class LazyLoadingObj(Of T)
    Implements ILazyLoading

    Protected pk As Object

    Protected _value As T

    Protected initialized As Boolean = False

    Public Sub New(ByVal Optional _pk As Object = Nothing)
        pk = _pk
    End Sub


    Public Property Value As T
        Get

            If Not initialized Then
                _value = [Get](Of T)(pk)
                initialized = True
            End If

            Return _value
        End Get
        Set(value As T)
            _value = value
            initialized = True
        End Set
    End Property


    Public Shared Widening Operator CType(ByVal lazy As LazyLoadingObj(Of T)) As T
        Return lazy._value
    End Operator

    Public Shared Widening Operator CType(ByVal obj As T) As LazyLoadingObj(Of T)
        Dim rval As LazyLoadingObj(Of T) = New LazyLoadingObj(Of T)()
        rval.Value = obj
        Return rval
    End Operator
End Class
