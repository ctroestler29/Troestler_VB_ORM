Public Class LazyLoadingObj(Of _Type)
    Implements ILazyLoading

    Protected pk As Object

    Protected _value As _Type

    Protected initialized As Boolean = False

    Public Sub New(ByVal Optional _pk As Object = Nothing)
        pk = _pk
    End Sub


    Public Property Value As _Type
        Get

            If Not initialized Then
                _value = [GetObjectType](Of _Type)(pk)
                initialized = True
            End If

            Return _value
        End Get
        Set(value As _Type)
            _value = value
            initialized = True
        End Set
    End Property


    Public Shared Widening Operator CType(ByVal lazy As LazyLoadingObj(Of _Type)) As _Type
        Return lazy._value
    End Operator

    Public Shared Widening Operator CType(ByVal obj As _Type) As LazyLoadingObj(Of _Type)
        Dim rval As LazyLoadingObj(Of _Type) = New LazyLoadingObj(Of _Type)()
        rval.Value = obj
        Return rval
    End Operator
End Class
