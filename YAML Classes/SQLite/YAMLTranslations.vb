
Imports YamlDotNet.RepresentationModel
Imports System.Data.SQLite

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLTranslations
    Private UpdateDB As SQLiteDBConnection
    Private YAMLTranslationDocument As YAMLDocument

    Public Sub New(ByRef DBRef As String)
        UpdateDB = New SQLiteDBConnection(DBRef)
    End Sub

    Protected Overrides Sub Finalize()
        UpdateDB.CloseDB()
        MyBase.Finalize()
    End Sub

    Public Sub ImportTranslationLanguages(FilePath As String, FileName As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0

        ' Get the YAML file - typeIDs is a mapping node root
        YAMLTranslationDocument = New YAMLDocument(FilePath & FileName)

        ' This is a mapped node file
        Dim Languages As YamlSequenceNode = YAMLTranslationDocument.GetFileSequenceNode
        Dim DataField As YamlMappingNode

        Call InitalizeProcessing(LabelRef, PGRef, Languages.Count, FileName)

        ' Build table
        SQL = "CREATE TABLE trnTranslationLanguages ("
        SQL = SQL & "languageID VARCHAR(5) PRIMARY KEY,"
        SQL = SQL & "languageName VARCHAR(25) NOT NULL,"
        SQL = SQL & "numericLanguageID INTEGER"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()

        ' Load the data
        For Each DataField In Languages
            SQL = "INSERT INTO trnTranslationLanguages VALUES ("
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("languageID", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("languageName", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("numericLanguageID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call UpdateProgress(LabelRef, PGRef, Count, FileName)

        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call ClearProcessing(LabelRef, PGRef)

    End Sub

    Public Sub ImportTranslationColumns(FilePath As String, FileName As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0

        ' Get the YAML file - typeIDs is a mapping node root
        YAMLTranslationDocument = New YAMLDocument(FilePath & FileName)

        ' This is a mapped node file
        Dim Languages As YamlSequenceNode = YAMLTranslationDocument.GetFileSequenceNode
        Dim DataField As YamlMappingNode

        Call InitalizeProcessing(LabelRef, PGRef, Languages.Count, FileName)

        ' Build table
        SQL = "CREATE TABLE trnTranslationColumns ("
        SQL = SQL & "columnName VARCHAR(50) NOT NULL,"
        SQL = SQL & "masterID VARCHAR(25) NOT NULL,"
        SQL = SQL & "tableName VARCHAR(100) NOT NULL,"
        SQL = SQL & "tcGroupID INTEGER NOT NULL,"
        SQL = SQL & "tcID INTEGER PRIMARY KEY"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()

        ' Insert the Data
        For Each DataField In Languages
            SQL = "INSERT INTO trnTranslationColumns VALUES ("
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("columnName", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("masterID", DataField)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("tableName", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("tcGroupID", DataField) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("tcID", DataField) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call UpdateProgress(LabelRef, PGRef, Count, FileName)

        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call ClearProcessing(LabelRef, PGRef)

    End Sub

    Public Sub ImportTranslations(FilePath As String, FileName As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0

        ' Get the YAML file - typeIDs is a mapping node root
        YAMLTranslationDocument = New YAMLDocument(FilePath & FileName)

        ' This is a mapped node file
        Dim Languages As YamlSequenceNode = YAMLTranslationDocument.GetFileSequenceNode
        Dim DataField As YamlMappingNode

        Call InitalizeProcessing(LabelRef, PGRef, Languages.Count, FileName)

        ' Build table
        SQL = "CREATE TABLE trnTranslations ("
        SQL = SQL & "keyID INTEGER NOT NULL,"
        SQL = SQL & "languageID VARCHAR(5) NOT NULL,"
        SQL = SQL & "tcID INTEGER NOT NULL,"
        SQL = SQL & "text TEXT"
        SQL = SQL & ")"

        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        ' Put a unique Primary key
        SQL = "CREATE UNIQUE INDEX IDX_KID_LID_TCID ON trnTranslations (keyID, languageID, tcID)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.BeginSQLiteTransaction()

        ' Process Data
        For Each DataField In Languages
            SQL = "INSERT INTO trnTranslations VALUES ("
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("keyID", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("languageID", DataField)) & ","
            SQL = SQL & YAMLTranslationDocument.GetSQLScalarValueFromMapping("tcID", DataField) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTranslationDocument.GetSQLScalarValueFromMapping("text", DataField)) & ")"

            Call UpdateDB.ExecuteNonQuerySQL(SQL)

            Call UpdateProgress(LabelRef, PGRef, Count, FileName & "Record " & FormatNumber(Count, 0))
        Next

        Call UpdateDB.CommitSQLiteTransaction()

        Call ClearProcessing(LabelRef, PGRef)

    End Sub

    ' Inserts sent data into the translation tables
    Public Function InsertTranslationData(TypeID As String, MappedTranslationNode As YamlMappingNode, ColumnName As String,
                                          TableName As String, ReturnLanguageCode As String) As String

        Dim SQL As String = ""
        Dim TranslationText As String = ""
        Dim ReturnText As String = "null"
        Dim ColumnNumber As String = "-1"
        Dim GroupNumber As String = "-1"

        ' Note, will need to build these tables later or pull the data from yaml and then keep this as an insert
        If Not IsNothing(MappedTranslationNode) Then
            For Each DataNode In MappedTranslationNode

                ' Get the max column number if we are inserting new data - just increment the column number from what the max is in the table
                ColumnNumber = CStr(GetTranslationColumnNumber(TableName, ColumnName))
                GroupNumber = GetTranslationGroupNumber(TableName)

                ' trnTranslationColumns
                ' tcGroupID = Not used
                ' tcID = Unique ID for the translation column
                ' tableName = table that the translation column Is In
                ' columnName = column In table With translation field

                ' Make sure we aren't inserting the same key - tcID and tableName - if new, increment the column number field
                If Not UpdateDB.DataExists("SELECT 'X' FROM trnTranslationColumns WHERE tcID = " & ColumnNumber & " AND tableName = '" & TableName & "'") Then
                    SQL = "INSERT INTO trnTranslationColumns VALUES (" & GroupNumber & "," & ColumnNumber & ",'" & TableName & "','" & ColumnName & "','typeID')"
                    UpdateDB.ExecuteNonQuerySQL(SQL)
                End If

                ' trnTranslations
                ' tcID = Unique ID for the translation column
                ' keyID = ID Of the item translated In the table (ie. typeID, groupID, etc.)
                ' languageID = language code (sort Of matches the trnTranslationLanguages but Not right Case)
                ' text = actual translation text
                TranslationText = ""

                ' Make sure we aren't inserting the same key - tcID, keyID, and languageID
                If Not UpdateDB.DataExists("SELECT 'X' FROM trnTranslations WHERE tcID = " & ColumnNumber & " AND keyID = " & TypeID & " AND languageID = '" & DataNode.Key.ToString & "'") Then
                    SQL = "INSERT INTO trnTranslations VALUES (" & ColumnNumber & ", " & TypeID & ",'" & DataNode.Key.ToString & "','" & UpdateDB.FormatDBString(DataNode.Value.ToString) & "')"
                    UpdateDB.ExecuteNonQuerySQL(SQL)
                End If

                If DataNode.Key.ToString = ReturnLanguageCode Then
                    ReturnText = DataNode.Value.ToString
                End If
            Next
        End If

        Return ReturnText

    End Function

    ' Gets the current or next group number for the table name passed
    Public Function GetTranslationGroupNumber(TableName As String) As String
        Dim DBCommand As SQLiteCommand
        Dim SQLReader As SQLiteDataReader
        Dim ReturnValue As Integer
        Dim SQL As String = ""

        ' See if the groupID exists
        SQL = "SELECT tcGroupID FROM trnTranslationColumns WHERE tableName = '" & TableName & "'"
        DBCommand = New SQLiteCommand(SQL, UpdateDB.DBREf)
        SQLReader = DBCommand.ExecuteReader
        SQLReader.Read()

        If SQLReader.HasRows Then
            ReturnValue = CStr(SQLReader.GetInt16(0))
        Else
            ' Not found so need to get the next max number
            SQLReader.Close()

            SQL = "SELECT MAX(tcGroupID) FROM trnTranslationColumns"
            DBCommand = New SQLiteCommand(SQL, UpdateDB.DBREf)
            SQLReader = DBCommand.ExecuteReader
            SQLReader.Read()

            If SQLReader.HasRows Then
                ReturnValue = CStr(SQLReader.GetInt16(0) + 1)
            Else
                ReturnValue = "1"
            End If
        End If

        SQLReader.Close()
        SQLReader = Nothing
        DBCommand = Nothing

        Return ReturnValue

    End Function

    ' Gets the current or next ID number for the table name and column name passed
    Public Function GetTranslationColumnNumber(TableName As String, ColumnName As String) As String
        Dim DBCommand As SQLiteCommand
        Dim SQLReader As SQLiteDataReader
        Dim ReturnValue As Integer
        Dim SQL As String = ""

        ' See if the tcID exists
        SQL = "SELECT tcGroupID FROM trnTranslationColumns WHERE tableName = '" & TableName & "' AND columnName ='" & ColumnName & "'"
        DBCommand = New SQLiteCommand(SQL, UpdateDB.DBREf)
        SQLReader = DBCommand.ExecuteReader
        SQLReader.Read()

        If SQLReader.HasRows Then
            ReturnValue = CStr(SQLReader.GetInt16(0))
        Else
            ' Not found so need to get the next max number
            SQLReader.Close()

            SQL = "SELECT MAX(tcID) FROM trnTranslationColumns"
            DBCommand = New SQLiteCommand(SQL, UpdateDB.DBREf)
            SQLReader = DBCommand.ExecuteReader
            SQLReader.Read()

            If SQLReader.HasRows Then
                ReturnValue = CStr(SQLReader.GetInt16(0) + 1)
            Else
                ReturnValue = "1"
            End If
        End If

        SQLReader.Close()
        SQLReader = Nothing
        DBCommand = Nothing

        Return ReturnValue

    End Function

    ' Initializes the form
    Private Sub InitalizeProcessing(ByRef LabelRef As Label, ByRef PGRef As ProgressBar, PGMaxCount As Long, FileName As String)
        LabelRef.Text = "Reading " & FileName
        Application.UseWaitCursor = True
        Application.DoEvents()

        PGRef.Value = 0
        PGRef.Maximum = PGMaxCount
        PGRef.Visible = True
    End Sub

    ' Resets the form
    Private Sub ClearProcessing(ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        PGRef.Visible = False
        LabelRef.Text = ""
        Application.UseWaitCursor = False
        Application.DoEvents()
    End Sub

    ' Increments the progressbar
    Private Sub UpdateProgress(ByRef LabelRef As Label, ByRef PGRef As ProgressBar, ByRef Count As Long, DataUpdatedText As String)
        Count += 1
        If Count < PGRef.Maximum - 1 And Count <> 0 Then
            PGRef.Value = Count
            PGRef.Value = PGRef.Value - 1
            PGRef.Value = Count
        Else
            PGRef.Value = Count
        End If

        LabelRef.Text = "Saving " & DataUpdatedText
        Application.DoEvents()
    End Sub

End Class
