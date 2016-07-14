
Imports YamlDotNet.RepresentationModel

Public Class YAMLeveIcons

    Private UpdateDB As SQLiteDBConnection ' Reference of the DB we want to update - opened within the class
    Private YAMLIcons As YAMLDocument

    Public Sub New(ByRef DBRef As String)
        UpdateDB = New SQLiteDBConnection(DBRef)
    End Sub

    Protected Overrides Sub Finalize()
        UpdateDB.CloseDB()
        UpdateDB = Nothing
        MyBase.Finalize()
    End Sub

    Public Sub ImportData(FilePath As String, FileName As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0

        ' Build table
        SQL = "CREATE TABLE eveIcons ("
        SQL = SQL & "iconID INTEGER PRIMARY KEY,"
        SQL = SQL & "iconFile VARCHAR(500),"
        SQL = SQL & "description TEXT"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()

        ' Get the YAML file - typeIDs is a mapping node root
        YAMLIcons = New YAMLDocument(FilePath & FileName)

        ' This is a mapped node file
        Dim IconData As YamlMappingNode = YAMLIcons.GetFileMappingNode
        Dim IconID As String = "" ' Save so we can pass to other functions

        Dim CheckString As String = ""

        Call frmMain.InitalizeProcessing(LabelRef, PGRef, IconData.Count, FileName)

        For Each DataNode In IconData.Children
            ' Get the main TypeID first
            IconID = DataNode.Key.ToString

            SQL = "INSERT INTO eveIcons VALUES ("
            SQL = SQL & IconID & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLIcons.GetSQLMappingScalarValue("iconFile", DataNode)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLIcons.GetSQLMappingScalarValue("description", DataNode)) & ")"

            UpdateDB.ExecuteNonQuerySQL(SQL)

            Call frmMain.UpdateProgress(LabelRef, PGRef, Count, FileName & " Record " & IconID)

        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call frmMain.ClearProcessing(LabelRef, PGRef)

    End Sub

End Class