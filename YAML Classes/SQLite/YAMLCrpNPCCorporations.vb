
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLCrpNPCCorporations
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
        SQL = SQL & "corporationID INTEGER PRIMARY KEY,"
        SQL = SQL & "size VARCHAR(1) ,"
        SQL = SQL & "extent VARCHAR(1) ,"
        SQL = SQL & "solarSystemID INTEGER ,"
        SQL = SQL & "investorID1 INTEGER ,"
        SQL = SQL & "investorShares1 INTEGER ,"
        SQL = SQL & "investorID2 INTEGER ,"
        SQL = SQL & "investorShares2 INTEGER ,"
        SQL = SQL & "investorID3 INTEGER ,"
        SQL = SQL & "investorShares3 INTEGER ,"
        SQL = SQL & "investorID4 INTEGER ,"
        SQL = SQL & "investorShares4 INTEGER ,"
        SQL = SQL & "friendID INTEGER ,"
        SQL = SQL & "enemyID INTEGER ,"
        SQL = SQL & "publicShares INTEGER ,"
        SQL = SQL & "initialPrice INTEGER ,"
        SQL = SQL & "minSecurity FLOAT ,"
        SQL = SQL & "scattered INTEGER ,"
        SQL = SQL & "fringe INTEGER ,"
        SQL = SQL & "corridor INTEGER ,"
        SQL = SQL & "hub INTEGER ,"
        SQL = SQL & "border INTEGER ,"
        SQL = SQL & "factionID INTEGER ,"
        SQL = SQL & "sizeFactor FLOAT ,"
        SQL = SQL & "stationCount INTEGER ,"
        SQL = SQL & "stationSystemCount INTEGER ,"
        SQL = SQL & "description VARCHAR(4000) ,"
        SQL = SQL & "iconID INTEGER "
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim corporationID As String

        ' Process Data
        For Each DataField In Languages
            corporationID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("corporationID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & corporationID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("size", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("extent", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("solarSystemID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorID1", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorShares1", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorID2", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorShares2", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorID3", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorShares3", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorID4", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("investorShares4", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("friendID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("enemyID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("publicShares", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("initialPrice", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("minSecurity", DataField) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("scattered", DataField))) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("fringe", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("corridor", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("hub", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("border", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("factionID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("sizeFactor", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("stationCount", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("stationSystemCount", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("iconID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & corporationID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
