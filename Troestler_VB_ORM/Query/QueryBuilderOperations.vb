Imports System.Data

Public Class QueryBuilderOperations

    Enum Operation As Integer
        [AND] = 0
        [NOT] = 1
        [OR] = 2
        [LIKE] = 3
        [IN] = 4
        GT = 5
        LT = 6
        EQUALS = 7
        NOP = 8
        GRP = 9
        ENDGRP = 10
        GTOE = 11
        LTOE = 12

    End Enum

End Class
