
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLdgmTypeEffects
    Private UpdateDB As SQLiteDBConnection
    Private YAMLTranslationDocument As YAMLDocument

    Public Sub New(ByRef DBRef As String)
        UpdateDB = New SQLiteDBConnection(DBRef)
    End Sub

    Protected Overrides Sub Finalize()
        UpdateDB.CloseDB()
        MyBase.Finalize()
    End Sub

    Public Sub ImportData(FilePath As String, FileName As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0
        Dim TableName As String = FileName.Substring(0, InStr(FileName, ".") - 1)

        ' Get the YAML file - typeIDs is a mapping node root
        YAMLTranslationDocument = New YAMLDocument(FilePath & FileName)

        ' This is a mapped node file
        Dim Languages As YamlSequenceNode = YAMLTranslationDocument.GetFileSequenceNode
        Dim DataField As YamlMappingNode

        Call frmMain.InitalizeProcessing(LabelRef, PGRef, Languages.Count, FileName)

        ' Build table
        SQL = "CREATE TABLE " & TableName & " ("
        SQL = SQL & "typeID INTEGER NOT NULL,"
        SQL = SQL & "effectID NOT NULL,"
        SQL = SQL & "isDefault INTEGER"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        ' Put a unique Primary key
        SQL = "CREATE UNIQUE INDEX IDX_DTE_TID_EID ON " & TableName & " (typeID, effectID)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim typeID As String

        ' Process Data
        For Each DataField In Languages
            typeID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("typeID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & typeID & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("effectID", DataField) & ","
            If YAMLTranslationDocument.GetSQLScalarValueFromMapping("isDefault", DataField) <> "null" Then
                SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("isDefault", DataField))) & ")"
            Else
                SQL = SQL & "null)"
            End If

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & typeID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
