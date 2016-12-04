
Imports System.Data.SQLite

' Class to support SQLite Databases
Public Class SQLiteDBConnection

    Private DB As SQLiteConnection

    ' Initializes the DB connection for the sent file name
    Public Sub New(ByVal DBFileName As String)
        DB = New SQLiteConnection
        DB.ConnectionString = "Data Source=" & DBFileName & ";Version=3;"
        DB.Open()
        Call ExecuteNonQuerySQL("PRAGMA synchronous = NORMAL; PRAGMA locking_mode = NORMAL; PRAGMA cache_size = 10000; PRAGMA page_size = 4096; PRAGMA temp_store = DEFAULT; PRAGMA journal_mode = TRUNCATE; PRAGMA count_changes = OFF")
        Call ExecuteNonQuerySQL("PRAGMA auto_vacuum = FULL;") ' Keep the DB small
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
        DB.Close()
        DB.Dispose()
    End Sub

    ' Provides a reference to the DB
    Public Function DBREf() As SQLiteConnection
        Return DB
    End Function

    Public Sub ClearPools()

    End Sub

    ' Executes the SQL sent, which doesn't require a return value
    Public Sub ExecuteNonQuerySQL(ByVal SQL As String)
        Dim DBExecuteCmd As SQLiteCommand = DB.CreateCommand
        DBExecuteCmd.CommandTimeout = 0
        DBExecuteCmd.CommandText = SQL
        DBExecuteCmd.ExecuteNonQuery()
        DBExecuteCmd.Dispose()
    End Sub

    ' Begins an SQL Transaction
    Public Sub BeginSQLiteTransaction()
        Call ExecuteNonQuerySQL("BEGIN;")
    End Sub

    ' Commits an open transaction where changes were made to data
    Public Sub CommitSQLiteTransaction()
        Call ExecuteNonQuerySQL("END;")
    End Sub

    ' Rollsback changes made during an open transaction
    Public Sub RollbackSQLiteTransaction()
        Call ExecuteNonQuerySQL("ROLLBACK;")
    End Sub

    ' Properly formats a string with apostrophes in it to allow DB inserts
    Public Function FormatDBString(ByVal inStrVar As String) As String
        ' Anything with quote mark in name it won't correctly load - need to replace with double quotes
        If InStr(inStrVar, "'") Then
            inStrVar = Replace(inStrVar, "'", "''")
        End If
        Return inStrVar
    End Function

    ' Runs the sent recordset query and returns true if it returns data
    Public Function DataExists(ByVal SQL As String) As Boolean
        ' Run the query and if it returns rows, return true else false
        Dim SQLQuery As SQLiteCommand
        Dim SQLReader As SQLiteDataReader
        Dim ReturnValue As Boolean

        ' See if the query returns data
        SQLQuery = New SQLiteCommand(SQL, DB)
        SQLReader = SQLQuery.ExecuteReader
        SQLReader.Read()

        If SQLReader.HasRows Then
            ReturnValue = True
        Else
            ReturnValue = False
        End If

        SQLReader.Close()
        SQLReader = Nothing
        SQLQuery = Nothing

        Return ReturnValue

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
        If CheckString = "null" Or CheckString = "" Then
            Return "null"
        Else
            Return "'" & FormatDBString(CheckString) & "'" ' Add quotes and format it for proper insert
        End If
    End Function

End Class
