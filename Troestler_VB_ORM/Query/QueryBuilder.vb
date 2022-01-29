Public Interface QueryBuilder
    Sub From(Of type)()

    Sub GreaterThan(field As String, value As String)

    Function Find() As String
End Interface
