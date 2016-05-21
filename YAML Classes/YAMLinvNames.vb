Imports System.Data.SQLite
Imports System.IO
Imports YamlDotNet.Serialization
Imports YamlDotNet.RepresentationModel

Public Class YAMLinvNames

    Private UpdateDB As New SQLiteConnection

    Public Sub New(ByRef DBRef As SQLiteConnection)
        UpdateDB = DBRef
    End Sub

    '    internal Static YamlMappingNode ParseYamlFile(String filePath)
    '    {
    '        If (s_isClosing)
    'Return null;

    '        Using (StreamReader tReader = New StreamReader(filePath))
    '        {
    '            YamlStream yStream = New YamlStream();
    '            yStream.Load(tReader);
    '            Return yStream.Documents.First().RootNode As YamlMappingNode;
    '        }
    '    }


    '    internal Static YamlSequenceNode ParsePerTableFileYamlFile(String filePath)
    '    {
    '        If (s_isClosing)
    'Return null;

    '        Using (StreamReader tReader = New StreamReader(filePath))
    '        {
    '            YamlStream yStream = New YamlStream();
    '            yStream.Load(tReader);
    '            Return yStream.Documents.First().RootNode As YamlSequenceNode;
    '        }
    '    }


    Public Sub ImportFile(FilePath As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0
        Dim Names As Dictionary(Of String, invName)

        Dim tReader As New StreamReader(FilePath)
        Dim yStream As New YamlStream()
        yStream.Load(tReader)

        Dim x As YamlSequenceNode

        x = yStream.Documents.First().RootNode

        Try
            Dim Deserializer = New Deserializer() ' Don't use a naming convention
            ' Read the file into a string reader (file returns a string)
            Dim InputText = New StringReader(File.ReadAllText(FilePath))
            ' Now parse the input text
            Names = Deserializer.Deserialize(Of Dictionary(Of String, invName))(InputText)
        Catch ex As Exception
            Dim msg As String
            If IsNothing(ex.InnerException) Then
                msg = ""
            Else
                msg = "Inner Exception: " & ex.InnerException.ToString
            End If
            Debug.Print(ex.Message & vbCrLf & msg)
            MsgBox(ex.Message & vbCrLf & msg)
            Exit Sub
        End Try

        ' Update form
        PGRef.Value = 0
        PGRef.Maximum = Names.Count
        PGRef.Visible = True

        ' Build the table we will insert first
        ' industryActivities
        Call ResetTable("invNames")
        ' Build table
        SQL = "CREATE TABLE invNames (itemID bigint NOT NULL, itemName VARCHAR(50) NOT NULL, "
        SQL = SQL & "PRIMARY KEY (itemID))"
        Call ExecuteSQLiteSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_IN_IID ON industryActivities (itemID)"
        Call ExecuteSQLiteSQL(SQL)

        LabelRef.Text = ""
        PGRef.Visible = False

    End Sub

    Public Sub ExecuteSQLiteSQL(ByVal SQL As String)
        Dim DBExecuteCmd As SQLiteCommand

        DBExecuteCmd = UpdateDB.CreateCommand
        DBExecuteCmd.CommandText = SQL
        DBExecuteCmd.ExecuteNonQuery()

        DBExecuteCmd.Dispose()

    End Sub

    Public Sub ResetTable(TableName As String)
        '  SQL variables
        Dim DBCommand As New SQLiteCommand
        Dim SQLReader As SQLiteDataReader
        Dim SQL As String

        ' See if the table exists and drop if it does
        SQL = "SELECT * FROM sqlite_master WHERE name  = '" & TableName & "'"
        DBCommand = New SQLiteCommand(SQL, UpdateDB)
        SQLReader = DBCommand.ExecuteReader
        SQLReader.Read()

        If SQLReader.HasRows Then
            SQL = "DROP TABLE " & TableName
            SQLReader.Close()
            Call ExecuteSQLiteSQL(SQL)
        Else
            SQLReader.Close()
        End If

    End Sub

End Class

Public Class invName
    Public Property itemID As Long
    Public Property itemName As String
End Class
