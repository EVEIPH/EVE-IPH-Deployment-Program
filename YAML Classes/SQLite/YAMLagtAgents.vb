
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLagtAgents
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
        SQL = SQL & "agentID INTEGER PRIMARY KEY,"
        SQL = SQL & "agentTypeID INTEGER NOT NULL,"
        SQL = SQL & "corporationID INTEGER NOT NULL,"
        SQL = SQL & "divisionID INTEGER NOT NULL,"
        SQL = SQL & "isLocator INTEGER NOT NULL," ' Need to convert to number
        SQL = SQL & "level INTEGER NOT NULL,"
        SQL = SQL & "locationID INTEGER NOT NULL,"
        SQL = SQL & "quality INTEGER NOT NULL"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()

        Dim AgentID As String

        ' Process Data
        For Each DataField In Languages
            AgentID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("agentID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & AgentID & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("agentTypeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("corporationID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("divisionID", DataField) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("isLocator", DataField))) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("level", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("locationID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("quality", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & AgentID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
