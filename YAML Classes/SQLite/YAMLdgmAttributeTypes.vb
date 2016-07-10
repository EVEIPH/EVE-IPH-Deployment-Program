
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLdgmAttributeTypes
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
        SQL = SQL & "attributeID INTEGER PRIMARY KEY,"
        SQL = SQL & "attributeName VARCHAR(100),"
        SQL = SQL & "description VARCHAR(1000),"
        SQL = SQL & "iconID INTEGER,"
        SQL = SQL & "defaultValue FLOAT,"
        SQL = SQL & "published INTEGER,"
        SQL = SQL & "displayName VARCHAR(1000),"
        SQL = SQL & "unitID INTEGER,"
        SQL = SQL & "stackable INTEGER,"
        SQL = SQL & "highIsGood INTEGER,"
        SQL = SQL & "categoryID INTEGER"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim attributeID As String

        ' Process Data
        For Each DataField In Languages
            attributeID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("attributeID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & attributeID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("attributeName", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("iconID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("defaultValue", DataField) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("published", DataField))) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("displayName", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("unitID", DataField) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("stackable", DataField))) & ","
            SQL = SQL & CInt(CBool(YAMLTranslationDocument.GetSQLScalarValueFromMapping("highIsGood", DataField))) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("categoryID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & attributeID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
