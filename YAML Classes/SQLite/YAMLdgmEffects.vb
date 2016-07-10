
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLdgmEffects
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
        SQL = SQL & "effectID INTEGER PRIMARY KEY,"
        SQL = SQL & "effectName VARCHAR(400),"
        SQL = SQL & "effectCategory INTEGER,"
        SQL = SQL & "preExpression INTEGER,"
        SQL = SQL & "postExpression INTEGER,"
        SQL = SQL & "description VARCHAR(1000),"
        SQL = SQL & "guid VARCHAR(60),"
        SQL = SQL & "iconID INTEGER,"
        SQL = SQL & "isOffensive INTEGER,"
        SQL = SQL & "isAssistance INTEGER,"
        SQL = SQL & "durationAttributeID INTEGER,"
        SQL = SQL & "trackingSpeedAttributeID INTEGER,"
        SQL = SQL & "dischargeAttributeID INTEGER,"
        SQL = SQL & "rangeAttributeID INTEGER,"
        SQL = SQL & "falloffAttributeID INTEGER,"
        SQL = SQL & "disallowAutoRepeat INTEGER,"
        SQL = SQL & "published INTEGER,"
        SQL = SQL & "displayName VARCHAR(100),"
        SQL = SQL & "isWarpSafe INTEGER,"
        SQL = SQL & "rangeChance INTEGER,"
        SQL = SQL & "electronicChance INTEGER,"
        SQL = SQL & "propulsionChance INTEGER,"
        SQL = SQL & "distribution INTEGER,"
        SQL = SQL & "sfxName VARCHAR(20),"
        SQL = SQL & "npcUsageChanceAttributeID INTEGER,"
        SQL = SQL & "npcActivationChanceAttributeID INTEGER,"
        SQL = SQL & "fittingUsageChanceAttributeID INTEGER,"
        SQL = SQL & "modifierInfo TEXT"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim effectID As String

        ' Process Data
        For Each DataField In Languages
            effectID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("effectID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & effectID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("effectName", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("effectCategory", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("preExpression", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("postExpression", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("guid", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("iconID", DataField) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("isOffensive", DataField))) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("isAssistance", DataField))) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("durationAttributeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("trackingSpeedAttributeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("dischargeAttributeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("rangeAttributeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("falloffAttributeID", DataField) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("disallowAutoRepeat", DataField))) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("published", DataField))) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("displayName", DataField)) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("isWarpSafe", DataField))) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("rangeChance", DataField))) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("electronicChance", DataField))) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("propulsionChance", DataField))) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("distribution", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("sfxName", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("npcUsageChanceAttributeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("npcActivationChanceAttributeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("fittingUsageChanceAttributeID", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("modifierInfo", DataField)) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & effectID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
