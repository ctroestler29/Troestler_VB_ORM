Imports System.Data

Public Class QueryBuilderOperations

    Enum Operation As Integer

        [NOT] = 0
        [OR] = 1
        [LIKE] = 2
        [IN] = 3
        GT = 4
        LT = 5
        GTOE = 6
        LTOE = 7
        EQUALS = 8
        GRP = 9
        ENDGRP = 10


    End Enum

End Class
