Public Class Persons
    <PKAttr>
    Public Property ID As String

    Public Property FirstName As String
    Public Property Name As String

    Public Property Gender As Gender

    <FieldAttr(ColumnName:="BDate")>
    Public Property BirthDate As Date

    Private _InstanceNumber As Integer
    Protected Shared _N As Integer = 1

    <IgnoreAttr>
    Public Property InstanceNumber As Integer
        Get
            Return _InstanceNumber
        End Get
        Protected Set(value As Integer)
            _InstanceNumber = value
        End Set
    End Property

End Class

Public Enum Gender As Integer
    FEMALE = 0
    MALE = 1
End Enum
