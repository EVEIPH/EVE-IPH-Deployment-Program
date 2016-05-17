Imports System.Data.SQLite
Imports System.Data.SqlClient ' For SQL Server Connection

Public Module Globals

    Private SQLExpressConnectionExecute As SqlConnection ' For updating while another connection is open
    Private SQLExpressConnection As SqlConnection

    Private SQLExpressProgressBar As SqlConnection

    Public SQLiteDB As New SQLiteConnection
    Public UniverseDB As New SQLiteConnection

    Private Function CheckNull(ByVal inVariable As Object) As Object
        If IsNothing(inVariable) Then
            Return "null"
        ElseIf DBNull.Value.Equals(inVariable) Then
            Return "null"
        Else
            Return inVariable
        End If
    End Function

    Public Function FormatDBString(ByVal inStrVar As String) As String
        ' Anything with quote mark in name it won't correctly load - need to replace with double quotes
        If InStr(inStrVar, "'") Then
            inStrVar = Replace(inStrVar, "'", "''")
        End If
        Return inStrVar
    End Function

    ' Formats the value sent to what we want to insert inot the table field
    Public Function BuildInsertFieldString(ByVal inValue As Object) As String
        Dim CheckNullValue As Object
        Dim OutputString As String

        ' See if it is null first
        CheckNullValue = CheckNull(inValue)

        If CStr(CheckNullValue) <> "null" Then
            ' Not null, so format
            If CheckNullValue.GetType.Name = "Boolean" Then
                ' Change these to numeric values
                If inValue = True Then
                    OutputString = "1"
                Else
                    OutputString = "0"
                End If
            ElseIf CheckNullValue.GetType.Name <> "String" Then
                OutputString = CStr(inValue)
            Else
                ' String, so check for appostrophes
                OutputString = "'" & FormatDBString(inValue) & "'"
            End If
        Else
            OutputString = "null"
        End If

        Return Trim(OutputString)

    End Function

    Public Sub Execute_msSQL(ByVal SQL As String)
        Dim Command As SqlCommand

        Command = New SqlCommand(SQL, SQLExpressConnectionExecute)
        Command.ExecuteNonQuery()

        Command = Nothing

    End Sub

    Public Sub Execute_SQLiteSQL(ByVal SQL As String, ByRef DBRef As SQLiteConnection)
        Dim DBExecuteCmd As SQLiteCommand

        DBExecuteCmd = DBRef.CreateCommand
        DBExecuteCmd.CommandText = SQL
        DBExecuteCmd.ExecuteNonQuery()

        DBExecuteCmd.Dispose()

    End Sub

    Public Sub BeginSQLiteTransaction(ByRef DBRef As SQLiteConnection)
        Call Execute_SQLiteSQL("BEGIN;", DBRef)
    End Sub

    Public Sub CommitSQLiteTransaction(ByRef DBRef As SQLiteConnection)
        Call Execute_SQLiteSQL("END;", DBRef)
    End Sub

    Public Function GetLenSQLExpField(ByVal FieldName As String, ByVal TableName As String) As String
        Dim SQL As String
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim ColumnLength As Integer

        SQL = "SELECT MAX(LEN(" & FieldName & ")) FROM " & TableName
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If IsDBNull(msSQLReader.GetValue(0)) Then
            ColumnLength = 100
        Else
            ColumnLength = msSQLReader.GetValue(0)
        End If

        msSQLReader.Close()

        Return CStr(ColumnLength)

    End Function

    Public Sub ResetTable(TableName As String)
        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim SQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = '" & TableName & "'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE " & TableName
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

    End Sub

End Module