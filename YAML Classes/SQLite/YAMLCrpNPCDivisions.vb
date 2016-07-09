
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLCrpNPCDivisions
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
        SQL = SQL & "divisionID INTEGER PRIMARY KEY,"
        SQL = SQL & "divisionName VARCHAR(100) NOT NULL,"
        SQL = SQL & "description VARCHAR(1000) NOT NULL,"
        SQL = SQL & "leaderType VARCHAR(100) NOT NULL"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim divisionID As String

        ' Process Data
        For Each DataField In Languages
            divisionID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("divisionID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & divisionID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("divisionName", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("leaderType", DataField)) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & divisionID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
