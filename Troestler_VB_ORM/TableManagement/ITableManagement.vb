Public Interface ITableManagement
    Sub CreateTable(Of Type)(Optional tablename As String = Nothing)
    Sub DropTable(Of Type)(Optional tablename As String = Nothing)

    Sub ResetDBSchema()

    Sub CreateIndex(indexname As String, tablename As String)

End Interface
