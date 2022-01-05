
Imports System.Reflection
Friend Class _Field

    Private _Entity As _Entity
    Private _Member As MemberInfo
    Private _ColumnName As String
    Private _ColumnType As Type
    Private _IsPrimaryKey As Boolean
    Private _IsForeignKey As Boolean
    Private _AssignmentTable As String
    Private _RemoteColumnName As String
    Private _IsManyToMany As Boolean
    Private _IsNullable As Boolean
    Private _IsExternal As Boolean

    Public Sub New(entity As _Entity)
        Me.Entity = entity
    End Sub

    Public Property Entity As _Entity
        Get
            Return _Entity
        End Get
        Private Set(value As _Entity)
            _Entity = value
        End Set
    End Property

    Public Property Member As MemberInfo
        Get
            Return _Member
        End Get
        Friend Set(value As MemberInfo)
            _Member = value
        End Set
    End Property


    Public ReadOnly Property Type As Type
        Get

            If TypeOf Member Is PropertyInfo Then
                Return CType(Member, PropertyInfo).PropertyType
            End If

            Throw New NotSupportedException("Member type not supported.")
        End Get
    End Property

    Public Property ColumnName As String
        Get
            Return _ColumnName
        End Get
        Friend Set(value As String)
            _ColumnName = value
        End Set
    End Property

    Public Property ColumnType As Type
        Get
            Return _ColumnType
        End Get
        Friend Set(value As Type)
            _ColumnType = value
        End Set
    End Property

    Public Property IsPrimaryKey As Boolean
        Get
            Return _IsPrimaryKey
        End Get
        Friend Set(value As Boolean)
            _IsPrimaryKey = value
        End Set
    End Property

    Public Property IsForeignKey As Boolean
        Get
            Return _IsForeignKey
        End Get
        Friend Set(value As Boolean)
            _IsForeignKey = value
        End Set
    End Property

    Public Property AssignmentTable As String
        Get
            Return _AssignmentTable
        End Get
        Friend Set(value As String)
            _AssignmentTable = value
        End Set
    End Property

    Public Property RemoteColumnName As String
        Get
            Return _RemoteColumnName
        End Get
        Friend Set(value As String)
            _RemoteColumnName = value
        End Set
    End Property

    Public Property IsManyToMany As Boolean
        Get
            Return _IsManyToMany
        End Get
        Friend Set(value As Boolean)
            _IsManyToMany = value
        End Set
    End Property

    Public Property IsNullable As Boolean
        Get
            Return _IsNullable
        End Get
        Friend Set(value As Boolean)
            _IsNullable = value
        End Set
    End Property

    Public Property IsExternal As Boolean
        Get
            Return _IsExternal
        End Get
        Friend Set(value As Boolean)
            _IsExternal = value
        End Set
    End Property


End Class
