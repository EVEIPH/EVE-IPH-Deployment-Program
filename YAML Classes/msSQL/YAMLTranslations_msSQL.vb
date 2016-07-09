
Imports YamlDotNet.RepresentationModel
Imports System.Data.SqlClient

' Class imports translation data from YAML and inserts it into the appropriate tables
Public Class YAMLTranslations_msSQL
    Private UpdateDB As msSQLDBConnection

    Public Sub New(ByRef DBRef As String, Optional InstanceName As String = "")
        UpdateDB = New msSQLDBConnection(DBRef, InstanceName)
    End Sub

    Protected Overrides Sub Finalize()
        UpdateDB.CloseDB()
        MyBase.Finalize()
    End Sub

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

                ' translationTables
                ' sourceTable = some Short name For the table Or something
                ' destinationTable = table name With the translations
                ' translatedKey = column name
                ' tcGroupID = Not used
                ' tcID = Unique ID for the translation column

                ' Make sure we aren't inserting the same key - sourceTable and translatedKey
                ' *** Not going to update - this is all dupliate data with trnTranslationColumns except sourceTable, which isn't useful
                'If Not UpdateDB.DataExists("SELECT 'X' FROM translationTables WHERE sourceTable = '" & TableName & "' AND translatedKey = '" & ColumnName & "'") Then
                '    SQL = "INSERT INTO translationTables VALUES ('" & TableName & "','" & TableName & "','" & ColumnName & "',0," & ColumnNumber & ")"
                '    UpdateDB.ExecuteNonQuerySQL(SQL)
                'End If

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

    Public Function GetTranslationGroupNumber(TableName As String) As String
        Dim msSQLQuery As SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim ReturnValue As Integer

        ' See if the groupID exists
        msSQLQuery = New SqlCommand("SELECT tcGroupID FROM trnTranslationColumns WHERE tableName = '" & TableName & "'", UpdateDB.DBREf)
        msSQLReader = msSQLQuery.ExecuteReader
        msSQLReader.Read()

        If msSQLReader.HasRows Then
            ReturnValue = CStr(msSQLReader.GetInt16(0))
        Else
            ' Not found so need to get the next max number
            msSQLReader.Close()

            msSQLQuery = New SqlCommand("SELECT MAX(tcGroupID) FROM trnTranslationColumns", UpdateDB.DBREf)
            msSQLReader = msSQLQuery.ExecuteReader
            msSQLReader.Read()

            If msSQLReader.HasRows Then
                ReturnValue = CStr(msSQLReader.GetInt16(0) + 1)
            Else
                ReturnValue = "1"
            End If
        End If

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        Return ReturnValue

    End Function

    Public Function GetTranslationColumnNumber(TableName As String, ColumnName As String) As String
        Dim msSQLQuery As SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim ReturnValue As Integer

        ' See if the tcID exists
        msSQLQuery = New SqlCommand("SELECT tcGroupID FROM trnTranslationColumns WHERE tableName = '" & TableName & "' AND columnName ='" & ColumnName & "'", UpdateDB.DBREf)
        msSQLReader = msSQLQuery.ExecuteReader
        msSQLReader.Read()

        If msSQLReader.HasRows Then
            ReturnValue = CStr(msSQLReader.GetInt16(0))
        Else
            ' Not found so need to get the next max number
            msSQLReader.Close()

            msSQLQuery = New SqlCommand("SELECT MAX(tcID) FROM trnTranslationColumns", UpdateDB.DBREf)
            msSQLReader = msSQLQuery.ExecuteReader
            msSQLReader.Read()

            If msSQLReader.HasRows Then
                ReturnValue = CStr(msSQLReader.GetInt16(0) + 1)
            Else
                ReturnValue = "1"
            End If
        End If

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        Return ReturnValue

    End Function

End Class
