
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLCharFactions
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
        SQL = SQL & "factionID INTEGER PRIMARY KEY,"
        SQL = SQL & "factionName VARCHAR(15) NOT NULL,"
        SQL = SQL & "description VARCHAR(1000) NOT NULL,"
        SQL = SQL & "raceIDs INTEGER NOT NULL,"
        SQL = SQL & "solarSystemID INTEGER NOT NULL,"
        SQL = SQL & "corporationID INTEGER NOT NULL,"
        SQL = SQL & "sizeFactor FLOAT NOT NULL,"
        SQL = SQL & "stationCount INTEGER NOT NULL,"
        SQL = SQL & "stationSystemCount INTEGER NOT NULL,"
        SQL = SQL & "militiaCorporationID INTEGER,"
        SQL = SQL & "iconID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim factionID As String

        ' Process Data
        For Each DataField In Languages
            factionID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("factionID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & factionID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("factionName", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("raceIDs", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("solarSystemID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("corporationID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("sizeFactor", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("stationCount", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("stationSystemCount", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("militiaCorporationID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("iconID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & factionID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
