Public Class QueryBuilder(Of _type)
    Implements IEnumerable(Of _type)

    Private _op As QueryBuilderOperations.Operation

    Private _prev As QueryBuilder(Of _type)

    Private arguments As Object() = Nothing

    Private _iv As List(Of _type) = Nothing

    Private Function Get_Values() As List(Of _type)
        If _iv Is Nothing Then
            _iv = New List(Of _type)()

            If GetType(_type).IsAbstract Then
                Dim localCache As ICollection(Of Object) = Nothing

                Dim arr = GetChildrenOf(GetType(_type))
                For i1 = 0 To arr.Length - 1
                    Dim i = arr(i1)
                    _Fill(i)
                Next
            Else
                _Fill(GetType(_type))
            End If
        End If

        Return _iv
    End Function

    Public Function GetOp() As QueryBuilderOperations.Operation
        Return _op
    End Function

    Public Sub SetOp(value As QueryBuilderOperations.Operation)
        _op = value
    End Sub

    Friend Sub New(ByVal prev As QueryBuilder(Of _type))
        _prev = prev
    End Sub

    Private Sub _Fill(ByVal t As Type)
        Dim ops As List(Of QueryBuilder(Of _type)) = New List(Of QueryBuilder(Of _type))()
        Dim _me = Me
        Dim ent As Table = t.GetTableOf()
        Dim sql As String = ent.GetSQL()
        Dim parameters As List(Of Tuple(Of String, Object)) = New List(Of Tuple(Of String, Object))()
        Dim conj = " WHERE "
        Dim [not] = False
        Dim opbrk = ""
        Dim clbrk = ""
        Dim n = 0
        Dim operation As String
        Dim field As Column

        While _me IsNot Nothing
            ops.Insert(0, _me)
            _me = _me._prev
        End While


        For i1 = 0 To ops.Count - 1
            Dim i = ops(i1)

            Dim hv As QueryBuilderOperations.Operation = i.GetOp()

            Select Case hv
                Case QueryBuilderOperations.Operation.OR

                    If conj IsNot " WHERE " Then
                        conj = " OR "
                    End If

                Case QueryBuilderOperations.Operation.NOT
                    [not] = True
                Case QueryBuilderOperations.Operation.GRP
                    opbrk += "("
                Case QueryBuilderOperations.Operation.ENDGRP
                    clbrk += ")"
                Case QueryBuilderOperations.Operation.EQUALS, QueryBuilderOperations.Operation.LIKE
                    field = ent.GetFieldByName(CStr(i.arguments(0)))

                    operation = If(i.GetOp() = QueryBuilderOperations.Operation.LIKE, If([not], " NOT LIKE ", " LIKE "), If([not], " != ", " = "))

                    sql += $"{clbrk}{conj}{opbrk}"
                    sql += $"{If(Not i.arguments(2), field.GetColumnName, "Lower(" & field.GetColumnName & ")")}{operation}{If(i.arguments(2), "Lower(:p" & n.ToString() & ")", ":p" & n.ToString())}"

                    If i.arguments(2) Then
                        i.arguments(1) = CStr(i.arguments(1)).ToLower()
                    End If

                    parameters.Add(New Tuple(Of String, Object)(":p" & Math.Min(Threading.Interlocked.Increment(n), n - 1).ToString(), field.ToColumnType(i.arguments(1))))
                    opbrk = clbrk = ""
                    conj = " AND "
                    [not] = False
                Case QueryBuilderOperations.Operation.IN
                    field = ent.GetFieldByName(fieldName:=CStr(i.arguments(0)))
                    sql += $"{clbrk}{conj}{opbrk}"
                    sql += $"{field.GetColumnName}{If([not], " NOT IN (", " IN (")}"


                    For k = 1 To i.arguments.Length - 1

                        If k > 1 Then
                            sql += ", "
                        End If

                        sql += $":p{n}"
                        parameters.Add(New Tuple(Of String, Object)(
                            ":p" & Math.Min(Threading.Interlocked.Increment(n), n - 1).ToString(), field.ToColumnType(i.arguments(k))))
                    Next

                    sql += ")"
                    opbrk = clbrk = ""
                    conj = " AND "
                    [not] = False
                Case QueryBuilderOperations.Operation.GT
                    field = ent.GetFieldByName(CStr(i.arguments(0)))

                    operation = " > "

                    sql += $"{clbrk}{conj}{opbrk}"
                    sql += $"{field.GetColumnName}{operation}:p{n}"
                    parameters.Add(New Tuple(Of String, Object)(":p" & Math.Min(Threading.Interlocked.Increment(n), n - 1).ToString(), field.ToColumnType(i.arguments(1))))
                    opbrk = clbrk = ""
                    conj = " AND "
                    [not] = False
                Case QueryBuilderOperations.Operation.GTOE
                    field = ent.GetFieldByName(CStr(i.arguments(0)))

                    operation = " >= "

                    sql += $"{clbrk}{conj}{opbrk}"
                    sql += $"{field.GetColumnName}{operation}:p{n}"
                    parameters.Add(New Tuple(Of String, Object)(":p" & Math.Min(Threading.Interlocked.Increment(n), n - 1).ToString(), field.ToColumnType(i.arguments(1))))
                    opbrk = clbrk = ""
                    conj = " AND "
                    [not] = False
                Case QueryBuilderOperations.Operation.LT
                    field = ent.GetFieldByName(CStr(i.arguments(0)))

                    operation = " < "

                    sql += $"{clbrk}{conj}{opbrk}"
                    sql += $"{field.GetColumnName}{operation}:p{n}"
                    parameters.Add(New Tuple(Of String, Object)(":p" & Math.Min(Threading.Interlocked.Increment(n), n - 1).ToString(), field.ToColumnType(i.arguments(1))))
                    opbrk = clbrk = ""
                    conj = " AND "
                    [not] = False
                Case QueryBuilderOperations.Operation.LTOE
                    field = ent.GetFieldByName(CStr(i.arguments(0)))

                    operation = " <= "

                    sql += $"{clbrk}{conj}{opbrk}"
                    sql += $"{field.GetColumnName}{operation}:p{n}"
                    parameters.Add(New Tuple(Of String, Object)(":p" & Math.Min(Threading.Interlocked.Increment(n), n - 1).ToString(), field.ToColumnType(i.arguments(1))))
                    opbrk = clbrk = ""
                    conj = " AND "
                    [not] = False
            End Select
        Next

        FList(_iv, sql, t, parameters)
    End Sub

    Private Function SetOperation(ByVal op As QueryBuilderOperations.Operation, ParamArray args As Object()) As QueryBuilder(Of _type)
        SetOp(op)
        arguments = args
        Dim q As QueryBuilder(Of _type) = New QueryBuilder(Of _type)(Me)
        Return q
    End Function


    Public Function [Not]() As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.NOT)
    End Function


    Public Function [Or]() As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.OR)
    End Function


    Public Function BeginGroup() As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.GRP)
    End Function


    Public Function EndGroup() As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.ENDGRP)
    End Function

    Public Function Equals(ByVal field As String, ByVal value As Object, ByVal Optional ignoreCase As Boolean = False) As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.EQUALS, field, value, ignoreCase)
    End Function

    Public Function [Like](ByVal field As String, ByVal value As Object, ByVal Optional ignoreCase As Boolean = False) As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.LIKE, field, value, ignoreCase)
    End Function


    Public Function [In](ByVal field As String, ParamArray values As Object()) As QueryBuilder(Of _type)
        Dim v As List(Of Object) = New List(Of Object)(values)
        v.Insert(0, field)
        Return SetOperation(QueryBuilderOperations.Operation.LIKE, v.ToArray())
    End Function

    Public Function GreaterThan(ByVal field As String, ByVal value As Object) As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.GT, field, value)
    End Function

    Public Function GreaterThanOrEqual(ByVal field As String, ByVal value As Object) As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.GTOE, field, value)
    End Function


    Public Function LowerThan(ByVal field As String, ByVal value As Object) As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.LT, field, value)
    End Function

    Public Function LowerThanOrEqual(ByVal field As String, ByVal value As Object) As QueryBuilder(Of _type)
        Return SetOperation(QueryBuilderOperations.Operation.LTOE, field, value)
    End Function

    Public ReadOnly Property ToList As List(Of _type)
        Get
            Return New List(Of _type)(Get_Values())
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator(Of _type) Implements IEnumerable(Of _type).GetEnumerator
        Dim enumerator As List(Of _type).Enumerator = Get_Values().GetEnumerator()
        Return enumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Dim enumerator As List(Of _type).Enumerator = Get_Values().GetEnumerator()
        Return enumerator
    End Function
End Class
