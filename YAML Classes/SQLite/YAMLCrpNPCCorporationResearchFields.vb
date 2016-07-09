
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLCrpNPCCorporationResearchFields
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
        SQL = SQL & "corporationID INTEGER NOT NULL,"
        SQL = SQL & "skillID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        ' Put a unique Primary key
        SQL = "CREATE UNIQUE INDEX IDX_CRF_CID_SID ON " & TableName & " (corporationID, skillID)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim corporationID As String

        ' Process Data
        For Each DataField In Languages
            corporationID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("corporationID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & corporationID & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("skillID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & corporationID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
