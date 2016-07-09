
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLCharBloodLines
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
        SQL = SQL & "bloodlineID INTEGER PRIMARY KEY,"
        SQL = SQL & "bloodlineName VARCHAR(15) NOT NULL,"
        SQL = SQL & "raceID INTEGER NOT NULL,"
        SQL = SQL & "description VARCHAR(1000) NOT NULL,"
        SQL = SQL & "shortDescription VARCHAR(500) NOT NULL,"
        SQL = SQL & "femaleDescription VARCHAR(1000) NOT NULL,"
        SQL = SQL & "shortFemaleDescription VARCHAR(500) NOT NULL,"
        SQL = SQL & "maleDescription VARCHAR(1000) NOT NULL,"
        SQL = SQL & "shortMaleDescription VARCHAR(500) NOT NULL,"
        SQL = SQL & "corporationID INTEGER NOT NULL,"
        SQL = SQL & "shipTypeID INTEGER NOT NULL,"
        SQL = SQL & "charisma INTEGER NOT NULL,"
        SQL = SQL & "intelligence INTEGER NOT NULL,"
        SQL = SQL & "memory INTEGER NOT NULL,"
        SQL = SQL & "perception INTEGER NOT NULL,"
        SQL = SQL & "willpower INTEGER NOT NULL,"
        SQL = SQL & "iconID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim bloodlineID As String

        ' Process Data
        For Each DataField In Languages
            bloodlineID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("bloodlineID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & bloodlineID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("bloodlineName", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("raceID", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("shortDescription", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("femaleDescription", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("shortFemaleDescription", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("maleDescription", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("shortMaleDescription", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("corporationID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("shipTypeID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("charisma", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("intelligence", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("memory", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("perception", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("willpower", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("iconID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & bloodlineID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
