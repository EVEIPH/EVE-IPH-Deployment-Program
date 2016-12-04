
Imports System.Data.SqlClient

' Class to support Microsoft SQL Server DB
Public Class msSQLDBConnection

    Private DB As SqlConnection

    Public Sub New(ByVal DBFileName As String, InstanceName As String)
        DB = New SqlConnection(String.Format("Server={0}\{1};Database={2};Trusted_Connection=True; Connection Timeout = 0",
                                             Environment.MachineName, InstanceName, DBFileName))
        DB.Open()
    End Sub

    ' Finalize the class and close the db if needed
    Protected Overrides Sub Finalize()
        On Error Resume Next
        DB.Close()
        On Error GoTo 0
        MyBase.Finalize()
    End Sub

    ' Closes the database
    Public Sub CloseDB()
        On Error Resume Next
        DB.Close()
        On Error GoTo 0
    End Sub

    ' Provides a reference to the DB
    Public Function DBREf() As SqlConnection
        Return DB
    End Function

    ' Executes the SQL sent, which doesn't require a return value
    Public Sub ExecuteNonQuerySQL(ByVal SQL As String)
        Dim Command As New SqlCommand(SQL, DB)
        Command.ExecuteNonQuery()
        Command.CommandTimeout = 0
        Command = Nothing
    End Sub

    ' Runs the sent recordset query and returns true if it returns data
    Public Function DataExists(ByVal SQL As String) As Boolean
        ' Run the query and if it returns rows, return true else false
        Dim msSQLQuery As SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim ReturnValue As Boolean

        ' See if the data exists from the query
        msSQLQuery = New SqlCommand(SQL, DB)
        msSQLReader = msSQLQuery.ExecuteReader
        msSQLReader.Read()

        If msSQLReader.HasRows Then
            ReturnValue = True
        Else
            ReturnValue = False
        End If

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        Return ReturnValue

    End Function

    ' Begins an SQL Transaction
    Public Sub BeginSQLTransaction()
        Call ExecuteNonQuerySQL("BEGIN TRANSACTION;")
    End Sub

    ' Commits an open transaction where changes were made to data
    Public Sub CommitSQLTransaction()
        Call ExecuteNonQuerySQL("END TRANSACTION;")
    End Sub

    ' Rollsback changes made during an open transaction
    Public Sub RollbackSQLiteTransaction()
        Call ExecuteNonQuerySQL("ROLLBACK TRANSACTION;")
    End Sub

    ' Properly formats a string with apostrophes in it to allow DB inserts
    Public Function FormatDBString(ByVal inStrVar As String) As String
        ' Anything with quote mark in name it won't correctly load - need to replace with double quotes
        If InStr(inStrVar, "'") Then
            inStrVar = Replace(inStrVar, "'", "''")
        End If
        Return inStrVar
    End Function

    ' Drops the sent table name if it exists
    Public Sub DropTable(TableName As String)
        Dim SQL As String = ""

        ' See if the table exists and drop if it does
        SQL = "SELECT * FROM sys.tables WHERE name  = '" & TableName & "'"

        If DataExists(SQL) Then
            Call ExecuteNonQuerySQL("DROP TABLE " & TableName)
        End If

    End Sub

    ' Provides a valid insert value for strings
    Public Function BuildSQLInsertStringValue(ByVal CheckString As String) As String
        If CheckString = "null" Then
            Return "null"
        Else
            Return "'" & Trim(FormatDBString(CheckString)) & "'" ' Add quotes and format it for proper insert
        End If
    End Function

End Class
