
Imports YamlDotNet.RepresentationModel

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLCharAncestries
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
        SQL = SQL & "ancestryID INTEGER PRIMARY KEY,"
        SQL = SQL & "ancestryName VARCHAR(30) NOT NULL,"
        SQL = SQL & "description VARCHAR(1000) NOT NULL,"
        SQL = SQL & "shortDescription VARCHAR(500),"
        SQL = SQL & "bloodlineID INTEGER NOT NULL,"
        SQL = SQL & "charisma INTEGER NOT NULL,"
        SQL = SQL & "intelligence INTEGER NOT NULL,"
        SQL = SQL & "memory INTEGER NOT NULL,"
        SQL = SQL & "perception INTEGER NOT NULL,"
        SQL = SQL & "willpower INTEGER NOT NULL"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()
        Dim AncestryID As String

        ' Process Data
        For Each DataField In Languages
            AncestryID = YAMLTranslationDocument.GetSQLScalarValueFromMapping("ancestryID", DataField)
            SQL = "INSERT INTO " & TableName & " VALUES ("
            SQL = SQL & AncestryID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("ancestryName", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("description", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("shortDescription", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("bloodlineID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("charisma", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("intelligence", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("memory", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("perception", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("willpower", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & AncestryID)
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class
