Imports System.Data.SQLite
Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json
Imports System.Net
Imports System.Text

Public Class frmMain
    Inherits Form

    Public EVEIPHSQLiteDB As SQLiteDBConnection
    Public SDEDB As SQLiteDBConnection

    Private Const SettingsFileName As String = "Settings.txt"

    Private VersionNumber As String = ""

    ' Directory files and paths
    Private RootDirectory As String ' For the debugging process, will copy images here as well
    Private WorkingDirectory As String ' Where the main db, final DB, and image zip is stored 
    Private UploadFileDirectory As String ' Where all the files we want to sync to the server for download are
    Private UploadFileTestDirectory As String

    ' DB
    Private DatabasePath As String ' Where we build the SQLite database
    Private FinalDBPath As String ' Final DB
    Private DatabaseName As String
    Private ImagesVersion As String ' Version of the images we have in the zip
    Private FinalDBName As String = "EVEIPH DB"
    Private SQLInstance As String ' how to log into the SQL server on the host computer
    Private Const DBExtention As String = ".sqlite"

    ' Image folders
    Private ImageExportTypes As String
    Private EVEIPHImageFolder As String
    Private RendersImageFolder As String

    ' When updating the image files to build the zip, update the root directory images as well so we have the updated images for running in debug mode
    Private WorkingImageFolder As String = "Root Directory\EVEIPH Images"
    Private UploadImageFolder As String = "\EVEIPH Images"
    Private MissingImagesFilePath As String

    ' For saving and scanning the github folder for updates - this folder is in the deployment folder (same as installer and binary)
    Private FinalBinaryFolder As String = "EVEIPH\"
    Private FinalBinaryZip As String = "EVEIPH v" & VersionNumber & " Binaries.zip"

    ' File names
    Private MSIInstaller As String = "Eve Isk per Hour " & VersionNumber & ".msi"

    ' Special Processing
    Private Const StructureRigCategory As Integer = -66

    Private JSONDLL As String = "Newtonsoft.Json.dll"
    Private SQLiteDLL As String = "System.Data.SQLite.dll"
    Private EVEIPHEXE As String = "EVE Isk per Hour.exe"
    Private EVEIPHUpdater As String = "EVEIPH Updater.exe"
    Private EVEIPHDB As String = "EVEIPH DB.sqlite"
    Private UpdaterManifest As String = "EVEIPH Updater.exe.manifest"
    Private EXEManifest As String = "EVE Isk per Hour.exe.manifest"
    Private ImageZipFile As String = "EVEIPH Images.zip"
    Private MoreLinqDLL As String = "MoreLinq.Portable.dll"
    Private LatestVersionXML As String
    Private LatestTestVersionXML As String

    Private JSONDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/Newtonsoft.Json.dll"
    Private SQLiteDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/System.Data.SQLite.dll"
    Private EVEIPHEXEURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe"
    Private EVEIPHUpdaterURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe"
    Private EVEIPHDBURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20DB.sqlite"
    Private UpdaterManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe.manifest"
    Private EXEManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe.manifest"
    Private ImageZipFileURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Images.zip"
    Private MoreLinqDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/MoreLinq.Portable.dll"

    Private TestJSONDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/Newtonsoft.Json.dll"
    Private TestSQLiteDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/System.Data.SQLite.dll"
    Private TestEVEIPHEXEURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe"
    Private TestEVEIPHUpdaterURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe"
    Private TestEVEIPHDBURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20DB.sqlite"
    Private TestUpdaterManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe.manifest"
    Private TestEXEManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe.manifest"
    Private TestImageZipFileURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Images.zip"
    Private TestMoreLinqDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/MoreLinq.Portable.dll"

    Private Const SequenceLabel As String = "Sequence"
    Private Const Null As String = "null"
    Private Const ANY_NUMBER As String = "ANY_NUMBER"
    Private ParentNode As String = ""

    Private Const AssemblyArraysTable As String = "ASSEMBLY_ARRAYS"
    Private Const StationFacilitiesTable As String = "STATION_FACILITIES"

    Private INDENT As String = ""
    Private Const COLON As String = ":"
    Private Const COLON_SPACE As String = ": "
    Private Const BLOCK_2SEQUENCE As String = "- "
    Private Const BLOCK_4SEQUENCE As String = "-   "

    Private Const ASCII_Quote_Code As Integer = 39
    Private Const ASCII_DoubleQuote_Code As Integer = 34

    Private FileList As List(Of FileNameDate)

    Const SpaceFlagCode As Integer = 500

    Structure FileNameDate
        Dim FileName As String
        Dim FileDate As DateTime
    End Structure

    Private Class Mapping
        Public MappingName As String
        Public MappingList As List(Of Mapping)

        Public Sub New()
            MappingName = ""
            MappingList = New List(Of Mapping)
        End Sub

    End Class

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Call GetFilePaths()
        Call SetFilePaths()

        ToolTip.SetToolTip(txtDBName, "Name of the database file and database in SQL Server - Use the name saved on the SDE Zip file")
        ToolTip.SetToolTip(txtImageVersion, "Version of the images from the Types directory")
        ToolTip.SetToolTip(btnCopyFilesBuildXML, "Copies all the files from directories and then builds the xml file and saves them all in the github folder for upload")

        ' Set the grid - scrollbar is 21
        lstFileInformation.Columns.Add("File Name", 155, HorizontalAlignment.Left)
        lstFileInformation.Columns.Add("File Date/Time", 136, HorizontalAlignment.Left)

        Call LoadFileGrid()

    End Sub

    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers")>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")>
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub btnExit_Click(sender As System.Object, e As System.EventArgs) Handles btnExit.Click
        Me.Dispose()
        End
    End Sub

    Private Sub GetFilePaths()
        ' Read the settings file and lines
        Dim BPStream As StreamReader = Nothing
        If File.Exists(SettingsFileName) Then
            BPStream = New System.IO.StreamReader(SettingsFileName)

            DatabaseName = BPStream.ReadLine
            ImagesVersion = BPStream.ReadLine
            VersionNumber = BPStream.ReadLine

            RootDirectory = BPStream.ReadLine
            If Not Directory.Exists(RootDirectory) Then
                RootDirectory = ""
            End If
            WorkingDirectory = BPStream.ReadLine
            If Not Directory.Exists(WorkingDirectory) Then
                WorkingDirectory = ""
            End If
            UploadFileDirectory = BPStream.ReadLine
            If Not Directory.Exists(UploadFileDirectory) Then
                UploadFileDirectory = ""
            End If
            UploadFileTestDirectory = BPStream.ReadLine
            If Not Directory.Exists(UploadFileTestDirectory) Then
                UploadFileTestDirectory = ""
            End If

            If Not IsNothing(VersionNumber) Then
                ' Set these if we have a version number
                FinalBinaryZip = "EVEIPH v" & VersionNumber & " Binaries.zip"

                ' File names
                MSIInstaller = "Eve Isk per Hour " & VersionNumber & ".msi"
            Else
                FinalBinaryZip = "EVEIPH v4.0 Binaries.zip"
                MSIInstaller = "Eve Isk per Hour 4.0.msi"
            End If

            SQLInstance = BPStream.ReadLine
            If Not Directory.Exists(RootDirectory) Then
                SQLInstance = ""
            End If

            BPStream.Close()
        Else
            DatabaseName = ""
            ImagesVersion = ""
            RootDirectory = ""
            WorkingDirectory = ""
            UploadFileDirectory = ""
            UploadFileTestDirectory = ""
            VersionNumber = ""
            SQLInstance = ""
        End If
    End Sub

    Private Sub SetFilePaths()

        ' Add the slash if not there
        If RootDirectory <> "" Then
            If RootDirectory.Substring(Len(RootDirectory) - 1) <> "\" Then
                RootDirectory = RootDirectory & "\"
            End If
        End If

        If WorkingDirectory <> "" Then
            If WorkingDirectory.Substring(Len(WorkingDirectory) - 1) <> "\" Then
                WorkingDirectory = WorkingDirectory & "\"
            End If
        End If

        If UploadFileDirectory <> "" Then
            If UploadFileDirectory.Substring(Len(UploadFileDirectory) - 1) <> "\" Then
                UploadFileDirectory = UploadFileDirectory & "\"
            End If
        End If

        If UploadFileTestDirectory <> "" Then
            If UploadFileTestDirectory.Substring(Len(UploadFileTestDirectory) - 1) <> "\" Then
                UploadFileTestDirectory = UploadFileTestDirectory & "\"
            End If
        End If

        WorkingImageFolder = WorkingDirectory
        DatabasePath = WorkingDirectory & DatabaseName
        FinalDBPath = WorkingDirectory & FinalDBName

        txtDBName.Text = DatabaseName
        txtImageVersion.Text = ImagesVersion
        lblDBNameDisplay.Text = DatabaseName
        txtVersionNumber.Text = VersionNumber
        txtSqlInstanceName.Text = SQLInstance

        If WorkingDirectory <> "\" Then
            lblWorkingFolderPath.Text = WorkingDirectory
        End If

        If UploadFileDirectory <> "\" Then
            lblFilesPath.Text = UploadFileDirectory
        End If

        If UploadFileTestDirectory <> "\" Then
            lblTestPath.Text = UploadFileTestDirectory
        End If

        If RootDirectory <> "\" Then
            lblRootDebugFolderPath.Text = RootDirectory
        End If

        LatestVersionXML = "LatestVersionIPH.xml"
        LatestTestVersionXML = "LatestVersionIPH Test.xml"

        ' When updating the image files to build the zip, update the root directory images as well so we have the updated images for running in debug mode
        WorkingImageFolder = RootDirectory & "EVEIPH Images"

        ImageExportTypes = WorkingDirectory & "Types"
        RendersImageFolder = WorkingDirectory & "Renders"
        EVEIPHImageFolder = WorkingDirectory & "EVEIPH Images"
        MissingImagesFilePath = WorkingDirectory & " Missing Images.txt"

    End Sub

    Private Sub SetProgressBarValues(ByVal TableName As String)

        ' SQL variables
        Dim SQL As String
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader

        Dim i As Integer

        ' Now select the count of the final query of data
        SQL = "SELECT COUNT(*) FROM " & TableName
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader
        SQLReader1.Read()

        pgMain.Maximum = SQLReader1.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        SQLReader1.Close()
        SQLCommand = Nothing

    End Sub

    Private Sub btnSelectFilePath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectFilePath.Click
        If UploadFileDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = UploadFileDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblFilesPath.Text = FolderBrowserDialog.SelectedPath
                UploadFileDirectory = FolderBrowserDialog.SelectedPath
                Call SetFilePaths()
            Catch ex As Exception
                MsgBox(Err.Description, vbExclamation, Application.ProductName)
            End Try
        End If
    End Sub

    Private Sub btnSelectTestFilePath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectTestFilePath.Click
        If UploadFileTestDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = UploadFileTestDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblTestPath.Text = FolderBrowserDialog.SelectedPath
                UploadFileTestDirectory = FolderBrowserDialog.SelectedPath
                Call SetFilePaths()
            Catch ex As Exception
                MsgBox(Err.Description, vbExclamation, Application.ProductName)
            End Try
        End If
    End Sub

    Private Sub btnSelectDBImagesPath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectWorkingPath.Click
        If WorkingDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = WorkingDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblWorkingFolderPath.Text = FolderBrowserDialog.SelectedPath
                WorkingDirectory = FolderBrowserDialog.SelectedPath
                Call SetFilePaths()
            Catch ex As Exception
                MsgBox(Err.Description, vbExclamation, Application.ProductName)
            End Try
        End If
    End Sub

    Private Sub btnSaveFilePath_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveFilePath.Click
        Call SaveFilePaths()
    End Sub

    Private Sub SaveFilePaths()
        Dim MyStream As StreamWriter

        If Trim(txtDBName.Text) = "" Then
            MsgBox("Invalid database name", vbExclamation, Application.ProductName)
            txtDBName.Focus()
            Exit Sub
        End If

        If Trim(txtSqlInstanceName.Text) = "" Then
            MsgBox("Invalid SQL Server Instance Name", vbExclamation, Application.ProductName)
            txtSqlInstanceName.Focus()
        End If

        If Trim(lblFilesPath.Text) = "" Then
            MsgBox("Invalid Installer/Binary file path", vbExclamation, Application.ProductName)
            lblFilesPath.Focus()
            Exit Sub
        End If

        If Trim(lblTestPath.Text) = "" Then
            MsgBox("Invalid Installer/Binary test file path", vbExclamation, Application.ProductName)
            lblTestPath.Focus()
            Exit Sub
        End If

        If Trim(lblWorkingFolderPath.Text) = "" Then
            MsgBox("Invalid Images file path", vbExclamation, Application.ProductName)
            lblWorkingFolderPath.Focus()
            Exit Sub
        End If

        If Trim(lblRootDebugFolderPath.Text) = "" Then
            MsgBox("Invalid Root/Debug file path", vbExclamation, Application.ProductName)
            lblRootDebugFolderPath.Focus()
            Exit Sub
        End If

        If Trim(txtVersionNumber.Text) = "" Then
            MsgBox("Invalid version number", vbExclamation, Application.ProductName)
            txtVersionNumber.Focus()
            Exit Sub
        End If

        If Trim(txtImageVersion.Text) = "" Then
            MsgBox("Invalid Images Version number", vbExclamation, Application.ProductName)
            txtImageVersion.Focus()
            Exit Sub
        End If

        DatabaseName = txtDBName.Text
        ImagesVersion = txtImageVersion.Text
        lblDBNameDisplay.Text = DatabaseName
        VersionNumber = txtVersionNumber.Text
        SQLInstance = txtSqlInstanceName.Text

        RootDirectory = lblRootDebugFolderPath.Text
        WorkingDirectory = lblWorkingFolderPath.Text
        UploadFileDirectory = lblFilesPath.Text
        UploadFileTestDirectory = lblTestPath.Text

        ' Set these if we have a version number
        FinalBinaryZip = "EVEIPH v" & VersionNumber & " Binaries.zip"

        ' File names
        MSIInstaller = "Eve Isk per Hour " & VersionNumber & ".msi"

        ' Save the file path as a text file and the database name
        MyStream = File.CreateText(SettingsFileName)
        MyStream.Write(txtDBName.Text & Environment.NewLine)
        MyStream.Write(txtImageVersion.Text & Environment.NewLine)
        MyStream.Write(txtVersionNumber.Text & Environment.NewLine)
        MyStream.Write(lblRootDebugFolderPath.Text & Environment.NewLine)
        MyStream.Write(lblWorkingFolderPath.Text & Environment.NewLine)
        MyStream.Write(lblFilesPath.Text & Environment.NewLine)
        MyStream.Write(lblTestPath.Text & Environment.NewLine)
        MyStream.Write(txtSqlInstanceName.Text & Environment.NewLine)

        MyStream.Flush()
        MyStream.Close()

        ' Reload this incase the folder changed
        Call LoadFileGrid()
        ' Reset all the variables
        Call SetFilePaths()

        MsgBox("Settings Saved", vbInformation, Application.ProductName)

    End Sub

    Private Sub btnSelectRootDebugPath2_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectRootDebugPath.Click
        Call SelectRootDebugPath()
    End Sub

    Private Sub btnSelectRootDebugPath_Click(sender As System.Object, e As System.EventArgs)
        Call SelectRootDebugPath()
    End Sub

    Private Sub SelectRootDebugPath()
        If RootDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = RootDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblRootDebugFolderPath.Text = FolderBrowserDialog.SelectedPath
                lblRootDebugFolderPath.Text = FolderBrowserDialog.SelectedPath
                RootDirectory = FolderBrowserDialog.SelectedPath
                Call SetFilePaths()
            Catch ex As Exception
                MsgBox(Err.Description, vbExclamation, Application.ProductName)
            End Try
        End If
    End Sub

    Private Sub txtDBName_DoubleClick(sender As Object, e As System.EventArgs)
        Call GetFilePaths()
        txtDBName.Text = DatabaseName
    End Sub

    Private Sub txtDBName_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs)
        DatabaseName = txtDBName.Text
        Call SetFilePaths()
    End Sub

    ' Loads up the grid with files in the github directory and shows the date they were last updated
    Private Sub LoadFileGrid()
        Dim lstViewRow As ListViewItem
        Dim TempFile As FileNameDate
        Dim di As DirectoryInfo

        If UploadFileDirectory <> "" Then
            If chkCreateTest.Checked Then
                di = New DirectoryInfo(UploadFileTestDirectory)
            Else
                di = New DirectoryInfo(UploadFileDirectory)
            End If

            Dim fiArr As FileInfo() = di.GetFiles()

            ' Reset
            FileList = New List(Of FileNameDate)

            ' Add the names of the files.
            Dim FI As FileInfo
            For Each FI In fiArr
                If Not FI.Name.Contains("git") Then
                    TempFile.FileDate = FI.LastWriteTime
                    TempFile.FileName = FI.Name
                    FileList.Add(TempFile)
                End If
            Next FI

            ' Sort the names
            Call SortListDesc(FileList, 0, FileList.Count - 1)

            ' Add them to the list
            lstFileInformation.Items.Clear()
            lstFileInformation.BeginUpdate()

            For i = 0 To FileList.Count - 1
                lstViewRow = lstFileInformation.Items.Add(FileList(i).FileName)
                lstViewRow.SubItems.Add(CStr(FileList(i).FileDate))
            Next

            lstFileInformation.EndUpdate()

        End If

    End Sub

    ' Copies just the bp images that I use for EVE IPH from the latest dump into a new folder and zips them up for deployment
    Private Sub btnImageCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImageCopy.Click
        Dim ReaderCount As Long
        Dim SQL As String
        Dim rsReader As SQLiteDataReader
        Dim DBCommand As SQLiteCommand
        Dim MissingImages As Boolean

        ' Make sure we have a DB first
        If DatabaseName = "" Then
            MsgBox("Database Name not defined", vbExclamation, Application.ProductName)
            Call txtDBName.Focus()
            Exit Sub
        End If

        ' Make sure we have a DB first
        If RootDirectory = "" Then
            MsgBox("Root Directory Path not set", vbExclamation, Application.ProductName)
            Call btnSelectRootDebugPath.Focus()
            Exit Sub
        End If

        Me.Cursor = Cursors.WaitCursor
        Call EnableButtons(False)

        If Not ConnectToDBs() Then
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnImageCopy.Enabled = True
            Exit Sub
        End If

        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()

        ' Build the new folder
        If Directory.Exists(EVEIPHImageFolder) Then
            Directory.Delete(EVEIPHImageFolder, True) ' Delete everything for zip in working
        End If

        If Directory.Exists(WorkingImageFolder) Then
            Directory.Delete(WorkingImageFolder, True) ' Delete everything in my working folder
        End If

        Directory.CreateDirectory(EVEIPHImageFolder)
        Directory.CreateDirectory(WorkingImageFolder)

        ' For missing BP ID's
        File.Delete(MissingImagesFilePath)

        Dim OutputFile As New StreamWriter(MissingImagesFilePath)
        OutputFile.WriteLine("Blueprint ID - Blueprint Name")
        MissingImages = False

        ' Get the count first
        SQL = "SELECT COUNT(*) FROM ALL_BLUEPRINTS"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        rsReader = DBCommand.ExecuteReader
        rsReader.Read()
        ReaderCount = rsReader.GetValue(0)
        rsReader.Close()

        ' Get all the BP ID numbers we use in the program and copy those files to the directory
        SQL = "SELECT BLUEPRINT_ID, BLUEPRINT_NAME FROM ALL_BLUEPRINTS"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        rsReader = DBCommand.ExecuteReader

        pgMain.Value = 0
        pgMain.Maximum = ReaderCount
        pgMain.Visible = True

        Application.DoEvents()

        ' Loop through and copy all the images to the new folder - use absolute value for stuff I use negative typeIDs for (outpost stuff)
        While rsReader.Read
            Application.DoEvents()
            Try
                ' For zip use
                File.Copy(ImageExportTypes & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png", EVEIPHImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png")
                ' To root Working Directory
                File.Copy(ImageExportTypes & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png", WorkingImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png")
            Catch
                ' Build a file with the BP ID's and Names that do not have a image
                OutputFile.WriteLine(rsReader(0).ToString & " - " & rsReader(1).ToString)
                MissingImages = True
            End Try

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        ' Final images
        File.Copy(ImageExportTypes & "\17476_64.png", EVEIPHImageFolder & "\17476_64.png")    ' Covetor
        File.Copy(ImageExportTypes & "\17476_64.png", WorkingImageFolder & "\17476_64.png")   ' Covetor
        File.Copy(ImageExportTypes & "\17478_64.png", EVEIPHImageFolder & "\17478_64.png")    ' Retrieverr
        File.Copy(ImageExportTypes & "\17478_64.png", WorkingImageFolder & "\17478_64.png")   ' Retriever
        File.Copy(ImageExportTypes & "\17480_64.png", EVEIPHImageFolder & "\17480_64.png")    ' Procurer
        File.Copy(ImageExportTypes & "\17480_64.png", WorkingImageFolder & "\17480_64.png")   ' Procurer
        File.Copy(ImageExportTypes & "\22544_64.png", EVEIPHImageFolder & "\22544_64.png")    ' Hulk
        File.Copy(ImageExportTypes & "\22544_64.png", WorkingImageFolder & "\22544_64.png")   ' Hulk
        File.Copy(ImageExportTypes & "\22546_64.png", EVEIPHImageFolder & "\22546_64.png")    ' Skiff
        File.Copy(ImageExportTypes & "\22546_64.png", WorkingImageFolder & "\22546_64.png")   ' Skiff
        File.Copy(ImageExportTypes & "\22548_64.png", EVEIPHImageFolder & "\22548_64.png")    ' Mackinaw
        File.Copy(ImageExportTypes & "\22548_64.png", WorkingImageFolder & "\22548_64.png")   ' Mackinaw
        File.Copy(ImageExportTypes & "\22557_32.png", EVEIPHImageFolder & "\42528_32.png") ' T1 Mining Ganglink image
        File.Copy(ImageExportTypes & "\22557_32.png", WorkingImageFolder & "\42528_32.png") ' T1 Mining Ganglink image
        File.Copy(ImageExportTypes & "\24688_64.png", EVEIPHImageFolder & "\24688_64.png")    ' Rokh
        File.Copy(ImageExportTypes & "\24688_64.png", WorkingImageFolder & "\24688_64.png")   ' Rokh
        File.Copy(ImageExportTypes & "\24698_64.png", EVEIPHImageFolder & "\24698_64.png")    ' Drake
        File.Copy(ImageExportTypes & "\24698_64.png", WorkingImageFolder & "\24698_64.png")   ' Drake
        File.Copy(ImageExportTypes & "\28352_64.png", EVEIPHImageFolder & "\28352_64.png")    ' Rorqual
        File.Copy(ImageExportTypes & "\28352_64.png", WorkingImageFolder & "\28352_64.png")   ' Rorqual
        File.Copy(ImageExportTypes & "\28606_64.png", EVEIPHImageFolder & "\28606_64.png")    ' Orca
        File.Copy(ImageExportTypes & "\28606_64.png", WorkingImageFolder & "\28606_64.png")   ' Orca
        File.Copy(ImageExportTypes & "\32880_64.png", EVEIPHImageFolder & "\32880_64.png")    ' Ore Mining Frig
        File.Copy(ImageExportTypes & "\32880_64.png", WorkingImageFolder & "\32880_64.png")   ' Ore Mining Frig
        File.Copy(ImageExportTypes & "\33697_64.png", EVEIPHImageFolder & "\33697_64.png")    ' Prospect
        File.Copy(ImageExportTypes & "\33697_64.png", WorkingImageFolder & "\33697_64.png")   ' Prospect
        File.Copy(ImageExportTypes & "\37135_64.png", EVEIPHImageFolder & "\37135_64.png")    ' Endurance
        File.Copy(ImageExportTypes & "\37135_64.png", WorkingImageFolder & "\37135_64.png")   ' Endurance
        File.Copy(ImageExportTypes & "\42244_64.png", EVEIPHImageFolder & "\42244_64.png")    ' Porpoise
        File.Copy(ImageExportTypes & "\42244_64.png", WorkingImageFolder & "\42244_64.png")   ' Porpoise
        File.Copy(ImageExportTypes & "\4276_32.png", EVEIPHImageFolder & "\43551_32.png") ' T2 Mining Ganglink image
        File.Copy(ImageExportTypes & "\4276_32.png", WorkingImageFolder & "\43551_32.png") ' T2 Mining Ganglink image

        ' Fuel block images
        File.Copy(ImageExportTypes & "\16272_32.png", EVEIPHImageFolder & "\16272_32.png") ' Heavy Water
        File.Copy(ImageExportTypes & "\16272_32.png", WorkingImageFolder & "\16272_32.png") ' Heavy Water
        File.Copy(ImageExportTypes & "\16273_32.png", EVEIPHImageFolder & "\16273_32.png") ' Liquid Ozone
        File.Copy(ImageExportTypes & "\16273_32.png", WorkingImageFolder & "\16273_32.png") ' Liquid Ozone
        File.Copy(ImageExportTypes & "\16274_32.png", EVEIPHImageFolder & "\16274_32.png") ' Helium Isotopes
        File.Copy(ImageExportTypes & "\16274_32.png", WorkingImageFolder & "\16274_32.png") ' Helium Isotopes
        File.Copy(ImageExportTypes & "\16275_32.png", EVEIPHImageFolder & "\16275_32.png") ' Strontium Clathrates
        File.Copy(ImageExportTypes & "\16275_32.png", WorkingImageFolder & "\16275_32.png") ' Strontium Clathrates
        File.Copy(ImageExportTypes & "\17887_32.png", EVEIPHImageFolder & "\17887_32.png") ' Oxygen Isotopes
        File.Copy(ImageExportTypes & "\17887_32.png", WorkingImageFolder & "\17887_32.png") ' Oxygen Isotopes
        File.Copy(ImageExportTypes & "\17888_32.png", EVEIPHImageFolder & "\17888_32.png") ' Nitrogen Isotopes
        File.Copy(ImageExportTypes & "\17888_32.png", WorkingImageFolder & "\17888_32.png") ' Nitrogen Isotopes
        File.Copy(ImageExportTypes & "\17889_32.png", EVEIPHImageFolder & "\17889_32.png") ' Hydrogen Isotopes
        File.Copy(ImageExportTypes & "\17889_32.png", WorkingImageFolder & "\17889_32.png") ' Hydrogen Isotopes
        File.Copy(ImageExportTypes & "\24593_32.png", EVEIPHImageFolder & "\24593_32.png") ' Caldari State Starbase Charter
        File.Copy(ImageExportTypes & "\24593_32.png", WorkingImageFolder & "\24593_32.png") ' Caldari State Starbase Charter
        File.Copy(ImageExportTypes & "\3683_32.png", EVEIPHImageFolder & "\3683_32.png")   ' Oxygen
        File.Copy(ImageExportTypes & "\3683_32.png", WorkingImageFolder & "\3683_32.png")   ' Oxygen
        File.Copy(ImageExportTypes & "\3689_32.png", EVEIPHImageFolder & "\3689_32.png")   ' Mechanical Parts
        File.Copy(ImageExportTypes & "\3689_32.png", WorkingImageFolder & "\3689_32.png")   ' Mechanical Parts
        File.Copy(ImageExportTypes & "\4051_32.png", EVEIPHImageFolder & "\4051_32.png")   ' Nitrogen Fuel Block
        File.Copy(ImageExportTypes & "\4051_32.png", WorkingImageFolder & "\4051_32.png")   ' Nitrogen Fuel Block
        File.Copy(ImageExportTypes & "\4246_32.png", EVEIPHImageFolder & "\4246_32.png")   ' Hydrogen Fuel Block
        File.Copy(ImageExportTypes & "\4246_32.png", WorkingImageFolder & "\4246_32.png")   ' Hydrogen Fuel Block
        File.Copy(ImageExportTypes & "\4247_32.png", EVEIPHImageFolder & "\4247_32.png")   ' Helium Fuel Block
        File.Copy(ImageExportTypes & "\4247_32.png", WorkingImageFolder & "\4247_32.png")   ' Helium Fuel Block
        File.Copy(ImageExportTypes & "\4312_32.png", EVEIPHImageFolder & "\4312_32.png")   ' Oxygen Fuel Block
        File.Copy(ImageExportTypes & "\4312_32.png", WorkingImageFolder & "\4312_32.png")   ' Oxygen Fuel Block
        File.Copy(ImageExportTypes & "\44_32.png", EVEIPHImageFolder & "\44_32.png")   ' Enriched Uranium
        File.Copy(ImageExportTypes & "\44_32.png", WorkingImageFolder & "\44_32.png")   ' Enriched Uranium
        File.Copy(ImageExportTypes & "\9832_32.png", EVEIPHImageFolder & "\9832_32.png")   ' Coolant
        File.Copy(ImageExportTypes & "\9832_32.png", WorkingImageFolder & "\9832_32.png")   ' Coolant
        File.Copy(ImageExportTypes & "\9848_32.png", EVEIPHImageFolder & "\9848_32.png")   ' Robotics
        File.Copy(ImageExportTypes & "\9848_32.png", WorkingImageFolder & "\9848_32.png")   ' Robotics

        SQL = "SELECT COUNT(typeID) FROM INVENTORY_TYPES, INVENTORY_GROUPS WHERE INVENTORY_TYPES.groupID = INVENTORY_GROUPS.groupID "
        SQL &= "AND ABS(categoryID) = 66 AND INVENTORY_TYPES.published <> 0"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        rsReader = DBCommand.ExecuteReader
        rsReader.Read()
        ReaderCount = rsReader.GetValue(0)
        rsReader.Close()

        pgMain.Value = 0
        pgMain.Maximum = ReaderCount
        pgMain.Visible = True

        ' Get all the Engineering Complex icons
        SQL = "SELECT typeID, typeName FROM INVENTORY_TYPES, INVENTORY_GROUPS WHERE INVENTORY_TYPES.groupID = INVENTORY_GROUPS.groupID "
        SQL &= "AND ABS(categoryID) = 66 AND INVENTORY_TYPES.published <> 0"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        rsReader = DBCommand.ExecuteReader

        Application.DoEvents()

        ' Loop through and copy all the images to the new folder for upwell structures 
        While rsReader.Read
            Try
                ' For zip use
                File.Copy(ImageExportTypes & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png", EVEIPHImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png")
                ' To root Working Directory
                File.Copy(ImageExportTypes & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png", WorkingImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & "_64.png")
            Catch
                ' Build a file with the BP ID's and Names that do not have a image
                OutputFile.WriteLine(rsReader(0).ToString & " - " & rsReader(1).ToString)
                MissingImages = True
            End Try

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        ' Finally, get all the upwell structures renders by typeID in the Renders folder - Look up by groupID - if these change or more are added, then need to update
        SQL = "SELECT DISTINCT COUNT(typeID) FROM INVENTORY_TYPES, INVENTORY_GROUPS WHERE INVENTORY_GROUPS.categoryID = 65 
                AND INVENTORY_TYPES.groupID = INVENTORY_GROUPS.groupid AND INVENTORY_TYPES.published = 1"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        rsReader = DBCommand.ExecuteReader
        rsReader.Read()
        ReaderCount = rsReader.GetValue(0)
        rsReader.Close()

        pgMain.Value = 0
        pgMain.Maximum = ReaderCount
        pgMain.Visible = True

        SQL = "Select DISTINCT typeID FROM INVENTORY_TYPES, INVENTORY_GROUPS WHERE INVENTORY_GROUPS.categoryID = 65 
                AND INVENTORY_TYPES.groupID = INVENTORY_GROUPS.groupid AND INVENTORY_TYPES.published = 1"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        rsReader = DBCommand.ExecuteReader

        Application.DoEvents()

        ' Loop through and copy all the images to the new folder for upwell structures items
        While rsReader.Read
            Try
                ' For zip use
                File.Copy(RendersImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & ".png", EVEIPHImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & ".png")
                ' To root Working Directory
                File.Copy(RendersImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & ".png", WorkingImageFolder & "\" & CStr(Math.Abs(rsReader.GetValue(0))) & ".png")
            Catch
                ' Build a file with the BP ID's and Names that do not have a image
                OutputFile.WriteLine(rsReader(0).ToString & " - " & rsReader(1).ToString)
                MissingImages = True
            End Try

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()
        End While

        Call rsReader.Close()

        pgMain.Visible = False
        lblTableName.Text = "Cleaning up files..."
        Application.DoEvents()

        ' Delete the file if it already exists
        File.Delete(WorkingDirectory & "EVEIPH Images.zip")

        ' Compress the images
        Call ZipFile.CreateFromDirectory(EVEIPHImageFolder, WorkingDirectory & "EVEIPH Images.zip", CompressionLevel.Optimal, False)

        ' If we didn't output any missing images, delete the output fille
        If Not MissingImages Then
            OutputFile.Close()
            File.Delete(MissingImagesFilePath)
        End If

        ' Leave working folder for use with binary builder
        Call CloseDBs()

        Me.Cursor = Cursors.Default
        lblTableName.Text = ""
        Call EnableButtons(True)

        MsgBox("Images Copied Successfully", vbInformation, "Complete")

    End Sub

    ' Builds the binary zip file
    Private Sub btnBuildBinary_Click(sender As System.Object, e As System.EventArgs) Handles btnBuildBinary.Click
        ' Build this in the working directory
        Dim FinalBinaryFolderPath As String = WorkingDirectory & FinalBinaryFolder
        Dim FinalBinaryZipPath As String = WorkingDirectory & FinalBinaryZip

        ' Temp working Image folder to zip later
        Dim ImageFolder As String = "EVEIPH Images" ' IN DD Working

        btnBuildBinary.Enabled = False
        Application.UseWaitCursor = True
        Application.DoEvents()
        Call EnableButtons(False)

        ' Make folder to put files in and zip
        If Directory.Exists(FinalBinaryFolderPath) Then
            Directory.Delete(FinalBinaryFolderPath, True)
        End If

        If chkCreateTest.Checked Then
            ' Copy the test.txt to the binary
            File.Copy(RootDirectory & "Test.txt", FinalBinaryFolderPath & "Test.txt")
        End If

        Directory.CreateDirectory(FinalBinaryFolderPath)

        ' Copy all these files from the latest file directory (should be most up to date) to the working directory to make the zip
        File.Copy(UploadFileDirectory & JSONDLL, FinalBinaryFolderPath & JSONDLL)
        File.Copy(UploadFileDirectory & SQLiteDLL, FinalBinaryFolderPath & SQLiteDLL)
        File.Copy(UploadFileDirectory & EVEIPHEXE, FinalBinaryFolderPath & EVEIPHEXE)
        File.Copy(UploadFileDirectory & EVEIPHUpdater, FinalBinaryFolderPath & EVEIPHUpdater)
        File.Copy(UploadFileDirectory & UpdaterManifest, FinalBinaryFolderPath & UpdaterManifest)
        File.Copy(UploadFileDirectory & EXEManifest, FinalBinaryFolderPath & EXEManifest)
        File.Copy(UploadFileDirectory & LatestVersionXML, FinalBinaryFolderPath & LatestVersionXML)
        File.Copy(UploadFileDirectory & MoreLinqDLL, FinalBinaryFolderPath & MoreLinqDLL)

        ' DB
        File.Copy(WorkingDirectory & EVEIPHDB, FinalBinaryFolderPath & EVEIPHDB)

        ' IPH images
        My.Computer.FileSystem.CopyDirectory(WorkingDirectory & ImageFolder, FinalBinaryFolderPath & ImageFolder, True)

        ' Delete the file if it already exists
        File.Delete(FinalBinaryZipPath)
        ' Compress the whole file for download
        Call ZipFile.CreateFromDirectory(FinalBinaryFolderPath, FinalBinaryZipPath, CompressionLevel.Optimal, False)

        File.Delete(UploadFileDirectory & FinalBinaryZip)

        ' Copy binary zip file to the media file directory
        File.Copy(FinalBinaryZipPath, UploadFileDirectory & FinalBinaryZip)

        Application.UseWaitCursor = False
        Application.DoEvents()

        ' Clean up working folder
        If Directory.Exists(FinalBinaryFolderPath) Then
            Directory.Delete(FinalBinaryFolderPath, True)
        End If

        ' Refresh this file in the list
        Call LoadFileGrid()
        Call EnableButtons(True)
        Application.DoEvents()

        MsgBox("Binary Built", vbInformation, "Complete")

    End Sub

#Region "Supporting Functions"

    Private Structure Setting
        Dim FileName As String
        Dim Version As String
        Dim MD5 As String
        Dim URL As String

        Public Sub New(inFileName As String, inVersion As String, inMD5 As String, inURL As String)
            FileName = inFileName
            Version = inVersion
            MD5 = inMD5
            URL = inURL
        End Sub

    End Structure

    Public Sub EnableButtons(EnableValue As Boolean)
        btnBuildDatabase.Enabled = EnableValue
        btnImageCopy.Enabled = EnableValue
        btnCopyFilesBuildXML.Enabled = EnableValue
        btnBuildBinary.Enabled = EnableValue
        btnRefreshList.Enabled = EnableValue
    End Sub

    ' Sorts the material list by quantity
    Private Sub SortListDesc(ByVal Sentlist As List(Of FileNameDate), ByVal First As Integer, ByVal Last As Integer)
        Dim LowIndex As Integer
        Dim HighIndex As Integer
        Dim MidValue As Date

        ' Quicksort
        LowIndex = First
        HighIndex = Last
        MidValue = Sentlist((First + Last) \ 2).FileDate

        Do
            While Sentlist(LowIndex).FileDate > MidValue
                LowIndex = LowIndex + 1
            End While

            While Sentlist(HighIndex).FileDate < MidValue
                HighIndex = HighIndex - 1
            End While

            If LowIndex <= HighIndex Then
                Swap(LowIndex, HighIndex)
                LowIndex = LowIndex + 1
                HighIndex = HighIndex - 1
            End If
        Loop While LowIndex <= HighIndex

        If First < HighIndex Then
            SortListDesc(Sentlist, First, HighIndex)
        End If

        If LowIndex < Last Then
            SortListDesc(Sentlist, LowIndex, Last)
        End If

    End Sub

    ' This swaps the list values
    Private Sub Swap(ByRef IndexA As Integer, ByRef IndexB As Integer)
        Dim Temp As FileNameDate

        Temp = FileList(IndexA)
        FileList(IndexA) = FileList(IndexB)
        FileList(IndexB) = Temp

    End Sub

    ' MD5 Hash - specify the path to a file and this routine will calculate your hash
    Public Function MD5CalcFile(ByVal filepath As String) As String

        ' Open file (as read-only) - If it's not there, return ""
        If IO.File.Exists(filepath) Then
            Using reader As New System.IO.FileStream(filepath, IO.FileMode.Open, IO.FileAccess.Read)
                Using md5 As New System.Security.Cryptography.MD5CryptoServiceProvider

                    ' hash contents of this stream
                    Dim hash() As Byte = md5.ComputeHash(reader)

                    ' return formatted hash
                    Return ByteArrayToString(hash)

                End Using
            End Using
        End If

        ' Something went wrong
        Return ""

    End Function

    ' MD5 Hash - utility function to convert a byte array into a hex string
    Private Function ByteArrayToString(ByVal arrInput() As Byte) As String

        Dim sb As New System.Text.StringBuilder(arrInput.Length * 2)

        For i As Integer = 0 To arrInput.Length - 1
            sb.Append(arrInput(i).ToString("X2"))
        Next

        Return sb.ToString().ToLower

    End Function

    ' Updates the value in the progressbar for a smooth progress - total hack from this: http://stackoverflow.com/questions/977278/how-can-i-make-the-progress-bar-update-fast-enough/1214147#1214147
    Public Sub IncrementProgressBar(ByRef PG As ProgressBar)
        PG.Value = PG.Value + 1
        PG.Value = PG.Value - 1
        PG.Value = PG.Value + 1
    End Sub

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

    ' Formats the value sent to what we want to insert into the table field
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

    Public Sub Execute_SQLiteSQL(ByVal SQL As String, ByRef DBRef As SQLiteConnection)
        Dim DBExecuteCmd As SQLiteCommand

        DBExecuteCmd = DBRef.CreateCommand
        DBExecuteCmd.CommandText = SQL
        DBExecuteCmd.ExecuteNonQuery()

        DBExecuteCmd.Dispose()

    End Sub

    Public Function GetLenSQLExpField(ByVal FieldName As String, ByVal TableName As String) As String
        Dim SQL As String
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim ColumnLength As Integer

        SQL = "SELECT MAX(length(" & FieldName & ")) FROM " & TableName
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader
        SQLReader1.Read()

        If IsDBNull(SQLReader1.GetValue(0)) Then
            ColumnLength = 100
        Else
            ColumnLength = SQLReader1.GetValue(0)
        End If

        SQLReader1.Close()

        Return CStr(ColumnLength)

    End Function

    Public Sub ResetTable(TableName As String)
        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        Dim SQL As String

        ' See if the table exists and drop if it does
        mainSQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = '" & TableName & "' AND type = 'table'"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        If CInt(SQLReader1.GetValue(0)) = 1 Then
            SQL = "DROP TABLE " & TableName
            SQLReader1.Close()
            Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        Else
            SQLReader1.Close()
        End If

    End Sub

#End Region

#Region "Database Update"

    ' Create a new database, build tables and indexes, then populate it with the different updated tables
    Private Sub btnBuildDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuildDatabase.Click

        ' Make sure we have a DB first
        If DatabaseName = "" Then
            MsgBox("Database Name not defined", vbExclamation, Application.ProductName)
            Call txtDBName.Focus()
            Exit Sub
        End If

        lblTableName.Text = "Preparing Database for import"
        Application.UseWaitCursor = True
        Application.DoEvents()

        Call EnableButtons(False)

        ' Set the sde data updates
        Call UpdateSDEData()

        ' Build DB's and open connections
        Call CreateDBFile(FinalDBPath)

        If Not ConnectToDBs() Then
            Try
                ' Delete old one
                File.Delete(DatabasePath & DBExtention)
            Catch
                ' Nothing
            End Try
            lblTableName.Text = ""
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnImageCopy.Enabled = True
            ' Done
            Exit Sub
        End If

        Application.UseWaitCursor = False
        Application.DoEvents()

        Call BuildEVEDatabase()

        lblTableName.Text = ""
        Me.Cursor = Cursors.Default

        Call EnableButtons(True)

        Call CloseDBs()

        Application.DoEvents()
        Call MsgBox("Database Created", vbInformation, "Complete")

    End Sub

    Private Sub CreateDBFile(DBPathandName As String)

        ' Check for SQLite DB
        If File.Exists(DBPathandName & DBExtention) Then
            Try
                EVEIPHSQLiteDB.CloseDB()
            Catch
                ' Nothing
            End Try
            ' Delete old one
            File.Delete(DBPathandName & DBExtention)
        End If

        ' Create new SQLite DB
        SQLiteConnection.CreateFile(DBPathandName & DBExtention)

    End Sub

    Private Function ConnectToDBs() As Boolean
        Application.DoEvents()
        Me.Cursor = Cursors.WaitCursor
        Dim SQLInstanceName = ""

        If (txtSqlInstanceName.Text = "") Then
            Call MsgBox("SQL Server Instance Name not supplied", "Error", vbExclamation)
            Return False
        End If

        SQLInstanceName = txtSqlInstanceName.Text

        Try

            ' SQLite DB for saving data
            If File.Exists(FinalDBPath & DBExtention) Then
                EVEIPHSQLiteDB = New SQLiteDBConnection(FinalDBPath & DBExtention)
                ' Set pragma to make this faster
                Call Execute_SQLiteSQL("PRAGMA synchronous = OFF", EVEIPHSQLiteDB.DBRef)
            End If

            ' SQLite DB for the SDE
            If File.Exists(WorkingDirectory & DatabaseName & DBExtention) Then
                SDEDB = New SQLiteDBConnection(WorkingDirectory & DatabaseName & DBExtention)
                ' Set pragma to make this faster
                Call Execute_SQLiteSQL("PRAGMA synchronous = OFF", SDEDB.DBRef)
            Else
                Me.Cursor = Cursors.Default
                Call MsgBox("Not SDE Database found", vbExclamation, Application.ProductName)
                Return False
            End If

            btnBuildDatabase.Focus()
            Me.Cursor = Cursors.Default
            Return True
        Catch ex As Exception
            MsgBox(Err.Description, vbExclamation, Application.ProductName)
            Me.Cursor = Cursors.Default
            Return False
        End Try

    End Function

    Private Sub CloseDBs()
        On Error Resume Next
        EVEIPHSQLiteDB.CloseDB()
        EVEIPHSQLiteDB.ClearPools()
        On Error GoTo 0
    End Sub

    ' Main Table Building Query
    Private Sub BuildEVEDatabase()
        Dim SQL As String
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader

        Me.Cursor = Cursors.WaitCursor
        pgMain.Minimum = 0
        Application.DoEvents()

        On Error GoTo 0

        ' Set the version value
        SQL = "CREATE TABLE DB_VERSION ("
        SQL &= "VERSION_NUMBER VARCHAR(50)"
        SQL &= ")"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Insert the database name for the version
        Call Execute_SQLiteSQL("INSERT INTO DB_VERSION VALUES ('" & DatabaseName & "')", EVEIPHSQLiteDB.DBRef)

        ' Need a view for industryMaterials, which is used in more than one build function
        Call Execute_SQLiteSQL("DROP VIEW IF EXISTS MY_INDUSTRY_MATERIALS", SDEDB.DBRef)

        SQL = "CREATE VIEW MY_INDUSTRY_MATERIALS AS "
        SQL &= "SELECT blueprintTypeID, activityID, materialTypeID, quantity, 1 AS consume FROM industryActivityMaterials "
        SQL &= "UNION "
        SQL &= "SELECT blueprintTypeID, activityID, skillID AS materialTypeID, level as quantity, 0 AS consume FROM industryActivitySkills"
        Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        'lblTableName.Text = "Building: INVENTORY_NAMES"
        'Call Build_INVENTORY_NAMES()

        lblTableName.Text = "Building: INVENTORY_TYPES"
        Call Build_INVENTORY_TYPES()

        lblTableName.Text = "Building: INVENTORY_GROUPS"
        Call Build_INVENTORY_GROUPS()

        lblTableName.Text = "Building: INVENTORY_CATEGORIES"
        Call Build_INVENTORY_CATEGORIES()

        lblTableName.Text = "Building: ALL_BLUEPRINTS"
        Call Build_ALL_BLUEPRINTS()

        lblTableName.Text = "Building: ALL_BLUEPRINT_MATERIALS"
        Call Build_ALL_BLUEPRINT_MATERIALS()

        lblTableName.Text = "Building: ITEM_PRICES"
        Call Build_ITEM_PRICES()

        lblTableName.Text = "Building: ASSEMBLY_ARRAYS"
        Call Build_ASSEMBLY_ARRAYS()

        lblTableName.Text = "Building: STATIONS"
        Call Build_Stations()

        lblTableName.Text = "Building: REGIONS"
        Call Build_REGIONS()

        lblTableName.Text = "Building: CONSTELLATIONS"
        Call Build_CONSTELLATIONS()

        lblTableName.Text = "Building: SOLAR_SYSTEMS"
        Call Build_SOLAR_SYSTEMS()

        lblTableName.Text = "Building: MARKET_HISTORY"
        Call Build_MARKET_HISTORY()

        lblTableName.Text = "Building: MARKET_HISTORY_UPDATE_CACHE"
        Call Build_MARKET_HISTORY_UPDATE_CACHE()

        lblTableName.Text = "Building: MARKET_ORDERS"
        Call Build_MARKET_ORDERS()

        lblTableName.Text = "Building: MARKET_ORDERS_UPDATE_CACHE"
        Call Build_MARKET_ORDERS_UPDATE_CACHE()

        lblTableName.Text = "Building: INVENTORY_FLAGS"
        Call Build_Inventory_Flags()

        lblTableName.Text = "Building: INDUSTRY_SYSTEMS_COST_INDICIES"
        Call Build_INDUSTRY_SYSTEMS_COST_INDICIES()

        lblTableName.Text = "Building: INDUSTRY_FACILITIES"
        Call Build_INDUSTRY_FACILITIES()

        lblTableName.Text = "Building: RACE_IDS"
        Call Build_RACE_IDS()

        lblTableName.Text = "Building: FW_SYSTEM_UPGRADES"
        Call Build_FW_SYSTEM_UPGRADES()

        lblTableName.Text = "Building: RAM_ACTIVITIES"
        Call Build_RAM_ACTIVITIES()

        lblTableName.Text = "Building: RAM_ASSEMBLY_LINE_STATIONS"
        Call Build_RAM_ASSEMBLY_LINE_STATIONS()

        lblTableName.Text = "Building: RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY"
        Call Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY()

        lblTableName.Text = "Building: RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP"
        Call Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP()

        lblTableName.Text = "Building: RAM_ASSEMBLY_LINE_TYPES"
        Call Build_RAM_ASSEMBLY_LINE_TYPES()

        lblTableName.Text = "Building: RAM_INSTALLATION_TYPE_CONTENTS"
        Call Build_RAM_INSTALLATION_TYPE_CONTENTS()

        lblTableName.Text = "Building: INDUSTRY_ACTIVITY_PRODUCTS"
        Call Build_INDUSTRY_ACTIVITY_PRODUCTS()

        lblTableName.Text = "Building: CHARACTER_SKILLS"
        Call Build_CHARACTER_SKILLS()

        lblTableName.Text = "Building: ESI_CHARACTER_DATA"
        Call Build_ESI_CHARACTER_DATA()

        lblTableName.Text = "Building: ESI_CORPORATION_DATA"
        Call Build_ESI_CORPORATION_DATA()

        lblTableName.Text = "Building: ESI_CORPORATION_ROLES"
        Call Build_ESI_CORPORATION_ROLES()

        lblTableName.Text = "Building: ESI_PUBLIC_CACHE_DATES"
        Call Build_ESI_PUBLIC_CACHE_DATES()

        lblTableName.Text = "Building: PRICE_PROFILES"
        Call Build_PRICE_PROFILES()

        lblTableName.Text = "Building: CHARACTER_STANDINGS"
        Call Build_Character_Standings()

        lblTableName.Text = "Building: OWNED_BLUEPRINTS"
        Call Build_OWNED_BLUEPRINTS()

        lblTableName.Text = "Building: ALL_OWNED_BLUEPRINTS"
        Call Build_ALL_OWNED_BLUEPRINTS()

        lblTableName.Text = "Building: ITEM_PRICES_CACHE"
        Call Build_ITEM_PRICES_CACHE()

        lblTableName.Text = "Building: FACTIONS"
        Call Build_FACTIONS()

        lblTableName.Text = "Building: META_TYPES"
        Call Build_Meta_Types()

        lblTableName.Text = "Building: ATTRIBUTE_TYPES"
        Call Build_Attribute_Types()

        lblTableName.Text = "Building: TYPE_ATTRIBUTES"
        Call Build_Type_Attributes()

        lblTableName.Text = "Building: TYPE_EFFECTS"
        Call Build_Type_Effects()

        lblTableName.Text = "Building: ORES_LOCATIONS"
        Call Build_ORE_LOCATIONS()

        lblTableName.Text = "Building: REPROCESSING"
        Call Build_Reprocessing()

        lblTableName.Text = "Building: ORES"
        Call Build_ORES()

        lblTableName.Text = "Building: OreRefine"
        Call Build_OreRefine()

        lblTableName.Text = "Building: ENGINEERING_RIG_BONUSES"
        Call Build_StructureRigBonuses()

        lblTableName.Text = "Building: REACTIONS"
        Call Build_Reactions()

        lblTableName.Text = "Building: SKILLS"
        Call Build_Skills()

        lblTableName.Text = "Building: RESEARCH_AGENTS"
        Call Build_Research_Agents()

        lblTableName.Text = "Building: INDUSTRY_JOBS"
        Call Build_Industry_Jobs()

        lblTableName.Text = "Building: ASSETS"
        Call Build_Assets()

        lblTableName.Text = "Building: ASSET_LOCATIONS"
        Call Build_Asset_Locations()

        lblTableName.Text = "Building: CURRENT_RESEARCH_AGENTS"
        Call Build_Current_Research_Agents()

        lblTableName.Text = "Building: INDUSTRY_UPGRADE_BELTS"
        Call Build_INDUSTRY_UPGRADE_BELTS()

        lblTableName.Text = "Building: PLANET_SCHEMATICS"
        Call Build_PLANET_SCHEMATICS()

        lblTableName.Text = "Building: PLANET_SCHEMATICS_TYPE_MAP"
        Call Build_PLANET_SCHEMATICS_TYPE_MAP()

        lblTableName.Text = "Building: PLANET_SCHEMATICS_PIN_MAP"
        Call Build_PLANET_SCHEMATICS_PIN_MAP()

        lblTableName.Text = "Building: PLANET_RESOURCES"
        Call Build_PLANET_RESOURCES()

        lblTableName.Text = "Building: INVENTORY_TRAITS"
        Call Build_INVENTORY_TRAITS()

        lblTableName.Text = "Building: STATION_FACILITIES"
        Call Build_STATION_FACILITIES()

        lblTableName.Text = "Building: FACILITY_ACTIVITIES"
        Call Build_FACILITY_ACTIVITIES()

        lblTableName.Text = "Building: UPWELL_STRUCTURE_BONUSES"
        Call Build_UPWELL_STRUCTURES()

        lblTableName.Text = "Building: UPWELL"
        Call Build_UPWELL_STRUCTURES_INSTALLED_MODULES()

        lblTableName.Text = "Building: FACILITY_PRODUCTION_TYPES"
        Call Build_FACILITY_PRODUCTION_TYPES()

        lblTableName.Text = "Building: FACILITY_TYPES"
        Call Build_FACILITY_TYPES()

        lblTableName.Text = "Building: SAVED_FACILITIES"
        Call Build_SAVED_FACILITIES()

        lblTableName.Text = "Building: MAP_DISALLOWED_ANCHOR_CATEGORIES"
        Call Build_MAP_DISALLOWED_ANCHOR_CATEGORIES()

        lblTableName.Text = "Building: MAP_DISALLOWED_ANCHOR_GROUPS"
        Call Build_MAP_DISALLOWED_ANCHOR_GROUPS()

        'lblTableName.Text = "Building: LP_OFFER_REQUIREMENTS"
        'Call Build_LP_OFFER_REQUIREMENTS()

        'lblTableName.Text = "Building: LP_STORE"
        'Call Build_LP_STORE()

        ' After we are done with everything, use the following tables to update the RACE ID value in the ALL_BLUEPRINTS table
        lblTableName.Text = "Updating the Race ID's"

        ' Set null to zero
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 0 WHERE RACE_ID IS NULL "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' 1 = Caldari
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 1 "
        SQL &= "WHERE "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Caldari Encryption Methods') OR "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL &= "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Caldari Encryption Methods')) "
        SQL &= "OR MARKET_GROUP ='Caldari' OR BLUEPRINT_GROUP IN ('Missile Blueprint','Missile Launcher Blueprint') "
        SQL &= "OR BLUEPRINT_NAME LIKE 'Caldari%'  OR BLUEPRINT_NAME LIKE 'Caldari%' "
        SQL &= "AND RACE_ID = 0 "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' 2 = Minmatar
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 2 "
        SQL &= "WHERE "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Minmatar Encryption Methods') OR "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL &= "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Minmatar Encryption Methods')) "
        SQL &= "OR MARKET_GROUP ='Minmatar' OR BLUEPRINT_GROUP IN ('Projectile Ammo Blueprint','Projectile Weapon Blueprint') "
        SQL &= "OR BLUEPRINT_NAME LIKE 'Republic%'  OR BLUEPRINT_NAME LIKE 'Minmatar%' "
        SQL &= "AND RACE_ID = 0 "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' 4 = Amarr
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 4 "
        SQL &= "WHERE "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Amarr Encryption Methods') OR "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL &= "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Amarr Encryption Methods')) "
        SQL &= "OR MARKET_GROUP ='Amarr' OR BLUEPRINT_GROUP IN ('Energy Weapon Blueprint','Frequency Crystal Blueprint') "
        SQL &= "OR BLUEPRINT_NAME LIKE 'Ammatar%' OR BLUEPRINT_NAME LIKE 'Imperial Navy%' OR BLUEPRINT_NAME LIKE 'Khanid Navy%' OR BLUEPRINT_NAME LIKE 'Amarr%' "
        SQL &= "AND RACE_ID = 0"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' 8 = Gallente
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 8 "
        SQL &= "WHERE "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Gallente Encryption Methods') OR "
        SQL &= "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL &= "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Gallente Encryption Methods')) "
        SQL &= "OR MARKET_GROUP ='Gallente' OR BLUEPRINT_GROUP IN ('Hybrid Charge Blueprint','Hybrid Weapon Blueprint', 'Capacitor Booster Charge Blueprint', 'Bomb Blueprint') "
        SQL &= "OR BLUEPRINT_NAME LIKE 'Federation%' OR BLUEPRINT_NAME LIKE 'Gallente%' "
        SQL &= "AND RACE_ID = 0"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 15 WHERE MARKET_GROUP = 'Pirate Faction' OR RACE_ID > 15"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 15 WHERE BLUEPRINT_NAME LIKE 'Serpentis%' OR BLUEPRINT_NAME LIKE 'Angel%' OR BLUEPRINT_NAME LIKE 'Blood%'"
        SQL &= "OR BLUEPRINT_NAME LIKE 'Domination%' OR BLUEPRINT_NAME LIKE 'Dread Guristas%' OR BLUEPRINT_NAME LIKE 'Guristas%' "
        SQL &= "OR BLUEPRINT_NAME LIKE 'True Sansha%' OR BLUEPRINT_NAME LIKE 'Sansha%' OR BLUEPRINT_NAME LIKE 'Shadow%' OR BLUEPRINT_NAME LIKE 'Dark Blood%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Set all the structures now that are zero
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 1 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL &= "AND ITEM_GROUP IN ('Mobile Missile Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 2 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL &= "AND ITEM_GROUP IN ('Mobile Projectile Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 4 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL &= "AND ITEM_GROUP IN ('Mobile Laser Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 8 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL &= "AND ITEM_GROUP IN ('Mobile Hybrid Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Update any remaining by blueprint group
        SQL = "SELECT DISTINCT BLUEPRINT_GROUP, RACE_ID FROM ALL_BLUEPRINTS WHERE RACE_ID <> 0 "
        SQLiteDBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBRef)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        While SQLiteReader.Read
            SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = " & SQLiteReader.GetInt32(1) & " "
            SQL &= "WHERE BLUEPRINT_GROUP = '" & SQLiteReader.GetString(0) & "' "
            SQL &= "AND RACE_ID = 0 AND ITEM_CATEGORY IN ('Module', 'Drone')"
            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        End While

        ' Station Parts should be 'Other'
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 0 WHERE ITEM_GROUP_ID = 536 "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Fix for Pheobe SDE issues - These were removed from game but haven't been deleted from SDE
        SQL = "DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL_CATEGORY = 'Decryptors'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE BLUEPRINT_NAME LIKE '%Data Interface Blueprint'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "DELETE FROM ALL_BLUEPRINTS WHERE ITEM_GROUP = 'Data Interfaces'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        lblTableName.Text = "Finalizing..."
        Application.DoEvents()

        ' Run a vacuum on the new SQL DB
        Call Execute_SQLiteSQL("VACUUM;", EVEIPHSQLiteDB.DBRef)
        'Call EVEIPHSQLiteDB.DBREf.ClearAllPools()

    End Sub

    ' ALL_BLUEPRINTS
    Private Sub Build_ALL_BLUEPRINTS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        ' See if the table exists and delete if so
        SQL = "SELECT COUNT(*) FROM sqlite_master where tbl_name = 'ALL_BLUEPRINTS' AND type = 'table'"
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        If CInt(SQLReader1.GetValue(0)) = 1 Then
            SQL = "DROP TABLE ALL_BLUEPRINTS"
            SQLReader1.Close()
            Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        Else
            SQLReader1.Close()
        End If

        ' Build ALL_BLUEPRINTS from this query
        SQL = "CREATE TABLE ALL_BLUEPRINTS AS SELECT industryBlueprints.blueprintTypeID AS BLUEPRINT_ID, "
        SQL &= "invTypes1.typeName AS BLUEPRINT_NAME, "
        SQL &= "invGroups1.groupName AS  BLUEPRINT_GROUP, "
        SQL &= "invTypes.typeID AS ITEM_ID, "
        SQL &= "invTypes.typeName AS ITEM_NAME, "
        SQL &= "invGroups.groupID AS ITEM_GROUP_ID, "
        SQL &= "invGroups.groupName AS ITEM_GROUP, "
        SQL &= "invCategories.categoryID AS ITEM_CATEGORY_ID, "
        SQL &= "invCategories.categoryName AS ITEM_CATEGORY, "
        SQL &= "invMarketGroups.marketGroupID AS MARKET_GROUP_ID, "
        SQL &= "invMarketGroups.marketGroupName AS MARKET_GROUP, "
        SQL &= "0 AS TECH_LEVEL, "
        SQL &= "industryActivityProducts.quantity AS PORTION_SIZE, "
        SQL &= "industryActivities.time AS BASE_PRODUCTION_TIME, "
        SQL &= "IA1.time AS BASE_RESEARCH_TL_TIME, "
        SQL &= "IA2.time AS BASE_RESEARCH_ML_TIME, "
        SQL &= "IA3.time AS BASE_COPY_TIME, "
        SQL &= "IA4.time AS BASE_INVENTION_TIME, "
        SQL &= "industryBlueprints.maxProductionLimit AS MAX_PRODUCTION_LIMIT, "
        SQL &= "COALESCE(dgmTypeAttributes.valueInt,dgmTypeAttributes.valueFloat) AS ITEM_TYPE, "
        SQL &= "invTypes.raceID AS RACE_ID, "
        SQL &= "invMetaTypes.metaGroupID AS META_GROUP, "
        SQL &= "'XX' AS SIZE_GROUP, "
        SQL &= "0 AS IGNORE "
        SQL &= "FROM invTypes "
        SQL &= "LEFT JOIN invMarketGroups ON invTypes.marketGroupID = invMarketGroups.marketGroupID "
        SQL &= "LEFT JOIN dgmTypeAttributes ON invTypes.typeID = dgmTypeAttributes.typeID AND attributeID = 633 "
        SQL &= "LEFT JOIN invMetaTypes ON invTypes.typeID = invMetaTypes.typeID, "
        SQL &= "invTypes AS invTypes1, invGroups, invGroups AS invGroups1, invCategories, "
        SQL &= "industryActivityProducts, industryActivities, "
        SQL &= "industryBlueprints "
        SQL &= "LEFT JOIN industryActivities AS IA1 ON industryBlueprints.blueprintTypeID = IA1.blueprintTypeID AND IA1.activityID = 3 " ' -- Research TL time
        SQL &= "LEFT JOIN industryActivities AS IA2 ON industryBlueprints.blueprintTypeID = IA2.blueprintTypeID AND IA2.activityID = 4 " ' -- Research ML time
        SQL &= "LEFT JOIN industryActivities AS IA3 ON industryBlueprints.blueprintTypeID = IA3.blueprintTypeID AND IA3.activityID = 5 " ' -- Copy time
        SQL &= "LEFT JOIN industryActivities AS IA4 ON industryBlueprints.blueprintTypeID = IA4.blueprintTypeID AND IA4.activityID = 8 " ' -- Invention time
        SQL &= "WHERE industryActivityProducts.activityID IN (1,11) " ' -- only bps we can build or reactions
        SQL &= "AND industryBlueprints.blueprintTypeID = industryActivityProducts.blueprintTypeID "
        SQL &= "AND invTypes1.typeID = industryBlueprints.blueprintTypeID "
        SQL &= "AND invTypes1.groupID = invGroups1.groupID "
        SQL &= "AND invTypes.typeID = industryActivityProducts.productTypeID "
        SQL &= "AND invTypes.groupID = invGroups.groupID "
        SQL &= "AND invGroups.categoryID = invCategories.categoryID "
        SQL &= "AND industryBlueprints.blueprintTypeID = industryActivities.blueprintTypeID "
        SQL &= "AND industryActivities.activityID IN (1,11) " ' -- Production Time 
        SQL &= "AND (invTypes1.published <> 0 AND invTypes.published <> 0 AND invGroups1.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0 " ' -- 2830 bps
        SQL &= "OR industryBlueprints.blueprintTypeID < 0)" ' For all outpost "blueprints"

        ' Build table
        Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Now that ALL_BLUEPRINTS is created, do some updates to the data before the main query

        '***** TO CHECK LATER *****
        ' Set the tech level of the BPs first by looking at the item type from the query
        ' This is not ideal but meta 5 items are T2; Tengu/Legion/Proteus/Loki items are T3, and all others are T1
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 2, ITEM_TYPE = 2 WHERE ITEM_TYPE = 5"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 3 WHERE ITEM_CATEGORY = 'Subsystem' OR ITEM_GROUP = 'Strategic Cruiser' OR ITEM_GROUP = 'Tactical Destroyer'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 1 WHERE TECH_LEVEL = 0" ' Anything not updated should be a 0
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Tech's first
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1 "
        SQL &= "WHERE (TECH_LEVEL=3 AND ITEM_GROUP='Hybrid Tech Components') "
        SQL &= "OR (TECH_LEVEL=2 AND ITEM_GROUP Like '%Construction Components') "
        SQL &= "OR (ITEM_NAME='Mercoxit Mining Crystal I') "
        SQL &= "OR (ITEM_NAME='Deep Core Mining Laser I') "
        SQL &= "OR (ITEM_GROUP='Tool')"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Alliance Tournament ships added - They are set as T2 (use t2 mats to build) but come up as faction in game ('Mimir','Freki','Adrestia','Utu','Vangel','Malice','Etana','Cambion','Moracha','Chremoas','Whiptail','Chameleon', 'Caedes')
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 1 WHERE BLUEPRINT_ID IN (3517, 3519, 32789, 32791, 32788, 33396, 33398, 33674, 33676, 42525)"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Quick fix to update the sql table for Rubicon - Ascendancy Implant Blueprints (and Low-Grade Ascendancy) are set to T2 implant (Alpha), not invented though so set to T1
        Call Execute_SQLiteSQL("UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1 WHERE BLUEPRINT_ID IN (33536,33543,33545,33546,33547,33548,33556,33558,33560,33562,33564,33566)", SDEDB.DBRef)

        ' Now update the Item Types - Other tables take this item type data item types: 1 = T1, 2 = T2, 14 = Tech 3, 15 = Pirate, 16 = Navy
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 14 WHERE TECH_LEVEL = 3" ' T3 stuff
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 15 WHERE MARKET_GROUP = 'Pirate Faction'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE MARKET_GROUP = 'Navy Faction'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND MARKET_GROUP IS NULL AND ITEM_CATEGORY = 'Ship'" ' Navy Faction Ships
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1 WHERE TECH_LEVEL <> 1 AND META_GROUP = 3" ' Consider storyline a tech 1
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 3 WHERE META_GROUP = 3" ' Storyline
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND MARKET_GROUP = 'Scan Probes'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 15 WHERE META_GROUP = 4 AND ITEM_CATEGORY IN ('Structure', 'Starbase')"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Module'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Drone'" ' Augmented and Integrated drones
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = TECH_LEVEL WHERE ITEM_TYPE = 0 OR ITEM_TYPE IS NULL"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 15 WHERE  BLUEPRINT_GROUP = 'Combat Drone Blueprint' AND ITEM_TYPE = 16" ' Aug/Integrated drones
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Add the S/M/L/XL tag to these here

        ' Drones are light, missiles are rockets and light
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'S' WHERE SIZE_GROUP = 'XX' AND ("
        SQL &= "ITEM_NAME LIKE '% S' OR ITEM_NAME Like '%Small%' "
        SQL &= "OR (ITEM_NAME Like '%Micro%' AND ITEM_GROUP <> 'Propulsion Module' AND ITEM_NAME NOT LIKE 'Microwave%') "
        SQL &= "OR ITEM_NAME Like '%Defender%' "
        SQL &= "OR (ITEM_CATEGORY = 'Implant') "
        SQL &= "OR ITEM_NAME Like '% S-Set%' "
        SQL &= "OR ITEM_NAME IN ('Cap Booster 25','Cap Booster 50') "
        SQL &= "OR MARKET_GROUP IN ('Interdiction Probes', 'Mining Crystals', 'Nanite Repair Paste', 'Scan Probes', 'Survey Probes', 'Scripts') "
        SQL &= "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID from invTypes where packagedVolume = 5)) "
        SQL &= "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '1MN%') "
        SQL &= "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID from invTypes where marketGroupID IN (561,564,567,570,574,577,1671,1672,1037)))  "
        SQL &= "OR (ITEM_CATEGORY IN ('Charge','Module') AND (ITEM_NAME Like '%Rocket%' OR ITEM_NAME Like '%Light Missile%') AND ITEM_GROUP NOT IN ('Propulsion Module', 'Rig Launcher'))  "
        SQL &= "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE groupID IN (324,29,1534,237,830,420,893,1283,25,831,541,1527,1022,31,834,1305))))"

        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Drones are medium, missiles are heavys and hams
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'M' WHERE SIZE_GROUP = 'XX' AND ("
        SQL &= "ITEM_NAME LIKE '% M' OR ITEM_NAME Like '%Medium%' OR ITEM_NAME IN ('Cap Booster 75','Cap Booster 100') "
        SQL &= "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE packagedVolume = 10)) "
        SQL &= "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '10MN%') "
        SQL &= "OR (ITEM_GROUP IN ('Gang Coordinator')) "
        SQL &= "OR ITEM_NAME Like '% M-Set%' "
        SQL &= "OR (ITEM_CATEGORY = 'Subsystem') "
        SQL &= "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (562,565,568,572,575,578,1673,1674))) "
        SQL &= "OR (ITEM_CATEGORY IN ('Charge','Module') AND ITEM_NAME Like '%Heavy%' AND ITEM_NAME Not Like '%Jolt%')  "
        SQL &= "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE groupID IN (1201,1202,419,540,26,380,543,833,358,894,28,832,463,963)))) "
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Drones are Heavy, missiles are cruise/torp, towers are regular towers (Caldari Control Tower)
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'L' "
        SQL &= "WHERE SIZE_GROUP = 'XX' AND (ITEM_NAME LIKE '% L' "
        SQL &= "OR (ITEM_NAME Like '%Large%' AND ITEM_NAME NOT Like '%X-Large%') "
        SQL &= "OR ITEM_NAME IN ('Cap Booster 150','Cap Booster 200')"
        SQL &= "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE packagedVolume >= 25 and packagedVolume <=50)) "
        SQL &= "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '100MN%') "
        SQL &= "OR (ITEM_NAME Like ('%Control Tower')) "
        SQL &= "OR ITEM_NAME Like '% L-Set%' "
        SQL &= "OR (ITEM_CATEGORY = 'Deployable' AND ITEM_GROUP <> 'Mobile Warp Disruptor') "
        SQL &= "OR (ITEM_CATEGORY = 'Structure' AND ITEM_GROUP <> 'Control Tower')"
        SQL &= "OR (ITEM_CATEGORY = 'Module' AND ITEM_NAME Like '%Heavy%' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID NOT IN (563,566,569,573,576,579,1675,1676))) "
        SQL &= "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (563,566,569,573,576,579,1675,1676))) "
        SQL &= "OR (ITEM_CATEGORY IN ('Charge','Module') AND (ITEM_NAME Like '%Cruise%' OR ITEM_NAME Like '%Torpedo%')) "
        SQL &= "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE groupID IN (27, 898, 900))))"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Drones are fighters, missiles are upwell structures
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'XL' "
        SQL &= "WHERE SIZE_GROUP = 'XX' AND (ITEM_NAME LIKE '% XL' "
        SQL &= "OR ITEM_NAME LIKE '%Capital%' "
        SQL &= "OR ITEM_NAME LIKE '%Huge%'"
        SQL &= "OR ITEM_NAME LIKE '%X-Large%' "
        SQL &= "OR ITEM_NAME LIKE '%Giant%' "
        SQL &= "OR ITEM_NAME LIKE '% XL-Set%' "
        SQL &= "OR ITEM_CATEGORY IN ('Infrastructure Upgrades','Sovereignty Structures','Orbitals') "
        SQL &= "OR ITEM_GROUP IN ('Station Components', 'Remote ECM Burst', 'Super Weapon', 'Siege Module')"
        SQL &= "OR ITEM_NAME IN ('Cap Booster 400','Cap Booster 800') "
        SQL &= "OR (ITEM_CATEGORY = 'Fighter') "
        SQL &= "OR (ITEM_CATEGORY = 'Module' AND (ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (771,772,773,774,775,776,1642,1941)))) "
        SQL &= "OR (ITEM_GROUP IN ('Jump Drive Economizer','Drone Control Unit') OR ITEM_NAME LIKE 'Jump Portal%') "
        SQL &= "OR (ITEM_CATEGORY IN ('Charge','Module') AND ITEM_NAME Like '%Citadel%') "
        SQL &= "OR (ITEM_CATEGORY = 'Celestial' AND (ITEM_NAME Like 'Station%' OR ITEM_NAME LIKE '%Outpost%' OR ITEM_NAME LIKE '%Freight%')) "
        SQL &= "OR ITEM_GROUP LIKE 'Bomb%' "
        SQL &= "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE groupID IN (30,485,513,547,659,883,902,941,1538))))"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Anything left update to small (may need to revisit later)
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'S' WHERE SIZE_GROUP = 'XX'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'XL' WHERE ITEM_NAME = 'Orca'"
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Now build the tables
        SQL = "CREATE TABLE ALL_BLUEPRINTS ("
        SQL &= "BLUEPRINT_ID INTEGER PRIMARY KEY,"
        SQL &= "BLUEPRINT_NAME VARCHAR(" & GetLenSQLExpField("BLUEPRINT_NAME", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL &= "BLUEPRINT_GROUP VARCHAR(" & GetLenSQLExpField("BLUEPRINT_GROUP", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL &= "ITEM_ID INTEGER NOT NULL,"
        SQL &= "ITEM_NAME VARCHAR(" & GetLenSQLExpField("ITEM_NAME", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL &= "ITEM_GROUP_ID INTEGER NOT NULL,"
        SQL &= "ITEM_GROUP VARCHAR(" & GetLenSQLExpField("ITEM_GROUP", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL &= "ITEM_CATEGORY_ID INTEGER NOT NULL,"
        SQL &= "ITEM_CATEGORY VARCHAR(" & GetLenSQLExpField("ITEM_CATEGORY", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL &= "MARKET_GROUP_ID INTEGER,"
        SQL &= "MARKET_GROUP VARCHAR(" & GetLenSQLExpField("MARKET_GROUP", "ALL_BLUEPRINTS") & "),"
        SQL &= "TECH_LEVEL INTEGER NOT NULL,"
        SQL &= "PORTION_SIZE INTEGER NOT NULL,"
        SQL &= "BASE_PRODUCTION_TIME INTEGER NOT NULL,"
        SQL &= "BASE_RESEARCH_TL_TIME INTEGER,"
        SQL &= "BASE_RESEARCH_ML_TIME INTEGER,"
        SQL &= "BASE_COPY_TIME INTEGER,"
        SQL &= "BASE_INVENTION_TIME INTEGER,"
        SQL &= "MAX_PRODUCTION_LIMIT INTEGER NOT NULL,"
        SQL &= "ITEM_TYPE INTEGER,"
        SQL &= "RACE_ID INTEGER, "
        SQL &= "META_GROUP INTEGER,"
        SQL &= "SIZE_GROUP VARCHAR(2) NOT NULL, "
        SQL &= "IGNORE INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ALL_BLUEPRINTS")

        ' Now select the final query of data
        mainSQL = "SELECT * FROM ALL_BLUEPRINTS"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Insert the data into the table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO ALL_BLUEPRINTS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(9)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(10)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(11)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(12)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(13)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(14)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(15)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(16)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(17)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(18)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(19)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(20)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(21)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(22)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(23)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_AB_ITEM_ID ON ALL_BLUEPRINTS (ITEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_AB_BP_NAME ON ALL_BLUEPRINTS (BLUEPRINT_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_AB_CAT_ITEM ON ALL_BLUEPRINTS (ITEM_CATEGORY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_AB_GROUP_ITEM ON ALL_BLUEPRINTS (ITEM_GROUP,ITEM_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' ALL_BLUEPRINT_MATERIALS
    Private Sub Build_ALL_BLUEPRINT_MATERIALS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        Application.DoEvents()

        ' See if the table exists and delete if so
        SQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = 'ALL_BLUEPRINT_MATERIALS' AND type = 'table'"
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        If CInt(SQLReader1.GetValue(0)) = 1 Then
            SQL = "DROP TABLE ALL_BLUEPRINT_MATERIALS"
            SQLReader1.Close()
            Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        Else
            SQLReader1.Close()
        End If

        ' Build the temp table in SQL Server first
        SQL = "CREATE TABLE ALL_BLUEPRINT_MATERIALS AS SELECT industryBlueprints.blueprintTypeID AS BLUEPRINT_ID, invTypes.typeName AS BLUEPRINT_NAME, industryActivityProducts.productTypeID AS PRODUCT_ID, "
        SQL &= "MY_INDUSTRY_MATERIALS.materialTypeID AS MATERIAL_ID, matTypes.typeName AS MATERIAL, matGroups.groupName AS MATERIAL_GROUP,  "
        SQL &= "matCategories.categoryName AS MATERIAL_CATEGORY, matTypes.packagedVolume AS MATERIAL_VOLUME, MY_INDUSTRY_MATERIALS.quantity AS QUANTITY, "
        SQL &= "MY_INDUSTRY_MATERIALS.activityID AS ACTIVITY, MY_INDUSTRY_MATERIALS.consume AS CONSUME "
        SQL &= "FROM industryBlueprints, invTypes, industryActivityProducts, MY_INDUSTRY_MATERIALS, invGroups, invCategories, "
        SQL &= "invTypes AS matTypes, invGroups AS matGroups, invCategories AS matCategories "
        SQL &= "WHERE industryBlueprints.blueprintTypeID = invTypes.typeID "
        SQL &= "AND industryBlueprints.blueprintTypeID = industryActivityProducts.blueprintTypeID "
        SQL &= "AND industryBlueprints.blueprintTypeID = MY_INDUSTRY_MATERIALS.blueprintTypeID "
        SQL &= "AND industryActivityProducts.activityID = MY_INDUSTRY_MATERIALS.activityID "
        SQL &= "AND matTypes.typeID = MY_INDUSTRY_MATERIALS.materialTypeID "
        SQL &= "AND matGroups.groupID = matTypes.groupID "
        SQL &= "AND matGroups.categoryID = matCategories.categoryID "
        SQL &= "AND invTypes.groupID = invGroups.groupID "
        SQL &= "AND invGroups.categoryID = invCategories.categoryID "
        SQL &= "AND (invTypes.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0 "
        SQL &= "OR industryBlueprints.blueprintTypeID < 0) " ' For all outpost "blueprints"
        SQL &= "ORDER BY BLUEPRINT_ID, PRODUCT_ID "

        Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Update all the materials that are blueprints - add copy to the name to reduce confusion, only materials required are BPCs
        Call Execute_SQLiteSQL("UPDATE ALL_BLUEPRINT_MATERIALS SET MATERIAL = MATERIAL + ' Copy' WHERE MATERIAL_CATEGORY = 'Blueprint'", SDEDB.DBRef)

        ' Also, find any bp that has the product the same as a material id, this will cause an infinite loop
        SQL = "SELECT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS where PRODUCT_ID = MATERIAL_ID"
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        ' Delete these BPs from the materials and all_blueprints tables before building the final in SQLite
        While SQLReader1.Read
            Call Execute_SQLiteSQL("DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE BLUEPRINT_ID = " & CStr(SQLReader1.GetInt32(0)), SDEDB.DBRef)
            ' This table is already built in sqlite, so delete from there
            Call Execute_SQLiteSQL("DELETE FROM ALL_BLUEPRINTS WHERE BLUEPRINT_ID = " & CStr(SQLReader1.GetInt32(0)), SDEDB.DBRef)
        End While

        SQLReader1.Close()

        ' Create SQLite table
        SQL = "CREATE TABLE ALL_BLUEPRINT_MATERIALS ("
        SQL &= "BLUEPRINT_ID INTEGER,"
        SQL &= "BLUEPRINT_NAME VARCHAR(100) NOT NULL,"
        SQL &= "PRODUCT_ID INTEGER NOT NULL,"
        SQL &= "MATERIAL_ID INTEGER NOT NULL,"
        SQL &= "MATERIAL VARCHAR(100) NOT NULL,"
        SQL &= "MATERIAL_GROUP VARCHAR(100) NOT NULL,"
        SQL &= "MATERIAL_CATEGORY VARCHAR(100) NOT NULL,"
        SQL &= "MATERIAL_VOLUME REAL,"
        SQL &= "QUANTITY INTEGER NOT NULL,"
        SQL &= "ACTIVITY INTEGER NOT NULL,"
        SQL &= "CONSUME INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ALL_BLUEPRINT_MATERIALS")

        ' Now select the final query of data
        mainSQL = "SELECT * FROM ALL_BLUEPRINT_MATERIALS"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Insert the data into the new SQLite table
        While SQLReader1.Read
            SQL = "INSERT INTO ALL_BLUEPRINT_MATERIALS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(9)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(10)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_ABM_BP_ID_ACTIVITY ON ALL_BLUEPRINT_MATERIALS (BLUEPRINT_ID,ACTIVITY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ABM_PRODUCT_ID_ACTIVITY ON ALL_BLUEPRINT_MATERIALS (PRODUCT_ID, ACTIVITY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ABM_REQCOMP_ID_PRODUCT ON ALL_BLUEPRINT_MATERIALS (MATERIAL_ID, PRODUCT_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' ASSEMBLY_ARRAYS
    Private Sub Build_ASSEMBLY_ARRAYS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE ASSEMBLY_ARRAYS ("
        SQL &= "ARRAY_TYPE_ID INTEGER NOT NULL,"
        SQL &= "ARRAY_NAME VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & ") NOT NULL, "
        SQL &= "ACTIVITY_ID INTEGER NOT NULL,"
        SQL &= "MATERIAL_MULTIPLIER REAL NOT NULL,"
        SQL &= "TIME_MULTIPLIER REAL NOT NULL,"
        SQL &= "COST_MULTIPLIER REAL NOT NULL,"
        SQL &= "GROUP_ID INTEGER NOT NULL,"
        SQL &= "CATEGORY_ID INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "SELECT invTypes.typeID AS ARRAY_TYPE_ID, "
        mainSQL = mainSQL & "invTypes.typeName AS ARRAY_NAME, "
        mainSQL = mainSQL & "ramActivities.activityID AS ACTIVITY_ID, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerGroup.materialMultiplier AS MATERIAL_MULTIPLIER, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerGroup.timeMultiplier AS TIME_MULTIPLIER, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerGroup.costMultiplier AS COST_MULTIPLIER, "
        mainSQL = mainSQL & "invGroups.groupID AS GROUP_ID, "
        mainSQL = mainSQL & "0 AS CATEGORY_ID "
        mainSQL = mainSQL & "FROM invTypes, ramInstallationTypeContents, invGroups AS IG1, "
        mainSQL = mainSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerGroup, invGroups "
        mainSQL = mainSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        mainSQL = mainSQL & "AND ramInstallationTypeContents.installationTypeID = invTypes.typeID  "
        mainSQL = mainSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID  "
        mainSQL = mainSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerGroup.assemblyLineTypeID  "
        mainSQL = mainSQL & "AND ramAssemblyLineTypeDetailPerGroup.groupID = invGroups.groupID "
        mainSQL = mainSQL & "AND invTypes.groupID = IG1.groupID  "
        mainSQL = mainSQL & "AND IG1.categoryID  = 23 "
        mainSQL = mainSQL & "UNION "
        mainSQL = mainSQL & "SELECT invTypes.typeID AS ARRAY_TYPE_ID, "
        mainSQL = mainSQL & "invTypes.typeName AS ARRAY_NAME, "
        mainSQL = mainSQL & "ramActivities.activityID AS ACTIVITY_ID, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerCategory.materialMultiplier AS MATERIAL_MULTIPLIER, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerCategory.timeMultiplier AS TIME_MULTIPLIER, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerCategory.costMultiplier AS COST_MULTIPLIER, "
        mainSQL = mainSQL & "0 AS GROUP_ID, "
        mainSQL = mainSQL & "invCategories.categoryID AS CATEGORY_ID "
        mainSQL = mainSQL & "FROM invTypes, invGroups, ramInstallationTypeContents, "
        mainSQL = mainSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerCategory, invCategories "
        mainSQL = mainSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        mainSQL = mainSQL & "AND ramInstallationTypeContents.installationTypeID = invTypes.typeID  "
        mainSQL = mainSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID  "
        mainSQL = mainSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerCategory.assemblyLineTypeID  "
        mainSQL = mainSQL & "AND ramAssemblyLineTypeDetailPerCategory.categoryID = invCategories.categoryID "
        mainSQL = mainSQL & "AND invTypes.groupID = invGroups.groupID  "
        mainSQL = mainSQL & "AND invGroups.categoryID  = 23 "

        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call SetProgressBarValues(" (" & mainSQL & ") AS X ")

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO ASSEMBLY_ARRAYS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        SQLReader1.Close()

        ' Finally, index
        SQL = "CREATE INDEX IDX_AA_AID_CID_GID ON ASSEMBLY_ARRAYS (ACTIVITY_ID, CATEGORY_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_AA_AN_AID_CID_GID ON ASSEMBLY_ARRAYS (ARRAY_NAME, ACTIVITY_ID, CATEGORY_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Assembly arrays had bonuses removed when ECs came out (do this until they are removed)
        SQL = "UPDATE ASSEMBLY_ARRAYS SET MATERIAL_MULTIPLIER = 1, TIME_MULTIPLIER = 1, COST_MULTIPLIER = 1"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    End Sub

    ' STATION_FACILITIES - Temp table, update with CREST
    Private Sub Build_STATION_FACILITIES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String
        Dim i As Integer

        SQL = "CREATE TABLE STATION_FACILITIES ( "
        SQL &= "FACILITY_ID INT NOT NULL, "
        SQL &= "FACILITY_NAME VARCHAR(" & GetLenSQLExpField("stationName", "staStations") & ") NOT NULL, "
        SQL &= "SOLAR_SYSTEM_ID INT NOT NULL, "
        SQL &= "SOLAR_SYSTEM_NAME VARCHAR(" & GetLenSQLExpField("solarSystemName", "mapSolarSystems") & ") NOT NULL, "
        SQL &= "SOLAR_SYSTEM_SECURITY REAL NOT NULL, "
        SQL &= "REGION_ID INT NOT NULL, "
        SQL &= "REGION_NAME VARCHAR(" & GetLenSQLExpField("regionName", "mapRegions") & ") NOT NULL, "
        SQL &= "FACILITY_TYPE_ID INT NOT NULL, "
        SQL &= "FACILITY_TYPE VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & ") NOT NULL, "
        SQL &= "ACTIVITY_ID INT NOT NULL, "
        SQL &= "FACILITY_TAX REAL NOT NULL, "
        SQL &= "MATERIAL_MULTIPLIER REAL NOT NULL, "
        SQL &= "TIME_MULTIPLIER REAL NOT NULL, "
        SQL &= "COST_MULTIPLIER REAL NOT NULL, "
        SQL &= "GROUP_ID INT NOT NULL, "
        SQL &= "CATEGORY_ID INT NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Application.DoEvents()

        pgMain.Maximum = 110000
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True

        ' Pull station data from stations for temp use if they don't load facilities from CREST
        mainSQL = "SELECT staStations.stationID AS FACILITY_ID, stationName AS FACILITY_NAME, "
        mainSQL = mainSQL & "mapSolarSystems.solarSystemID AS SOLAR_SYSTEM_ID, mapSolarSystems.solarSystemName AS SOLAR_SYSTEM_NAME, mapSolarSystems.security AS SOLAR_SYSTEM_SECURITY, "
        mainSQL = mainSQL & "mapRegions.regionID AS REGION_ID, mapRegions.regionName AS REGION_NAME, "
        mainSQL = mainSQL & "staStations.stationTypeID, typeName AS FACILITY_TYPE, ramActivities.activityID AS ACTIVITY_ID, "
        mainSQL = mainSQL & ".1 as FACILITY_TAX, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerGroup.materialMultiplier AS MATERIAL_MULTIPLIER, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerGroup.timeMultiplier AS TIME_MULTIPLIER,  "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerGroup.costMultiplier AS COST_MULTIPLIER,  "
        mainSQL = mainSQL & "invGroups.groupID AS GROUP_ID, 0 AS CATEGORY_ID "
        mainSQL = mainSQL & "FROM staStations, invTypes, ramAssemblyLineStations, mapRegions, mapSolarSystems, "
        mainSQL = mainSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerGroup, invGroups "
        mainSQL = mainSQL & "WHERE staStations.stationTypeID = invTypes.typeID "
        mainSQL = mainSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerGroup.assemblyLineTypeID "
        mainSQL = mainSQL & "AND ramAssemblyLineTypeDetailPerGroup.groupID = invGroups.groupID "
        mainSQL = mainSQL & "AND staStations.regionID = mapRegions.regionID "
        mainSQL = mainSQL & "AND staStations.solarSystemID = mapSolarSystems.solarSystemID "
        mainSQL = mainSQL & "AND staStations.stationID = ramAssemblyLineStations.stationID "
        mainSQL = mainSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID "
        mainSQL = mainSQL & "AND ramAssemblyLineStations.assemblyLineTypeID = ramAssemblyLineTypes.assemblyLineTypeID "
        mainSQL = mainSQL & "UNION "
        mainSQL = mainSQL & "SELECT staStations.stationID, stationName, "
        mainSQL = mainSQL & "mapSolarSystems.solarSystemID AS SOLAR_SYSTEM_ID, mapSolarSystems.solarSystemName AS SOLAR_SYSTEM_NAME, mapSolarSystems.security AS SOLAR_SYSTEM_SECURITY, "
        mainSQL = mainSQL & "mapRegions.regionID AS REGION_ID, mapRegions.regionName AS REGION_NAME, "
        mainSQL = mainSQL & "staStations.stationTypeID, typeName AS FACILITY_TYPE, ramActivities.activityID AS ACTIVITY_ID, "
        mainSQL = mainSQL & ".1 as FACILITY_TAX, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerCategory.materialMultiplier AS MATERIAL_MULTIPLIER, "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerCategory.timeMultiplier AS TIME_MULTIPLIER,  "
        mainSQL = mainSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerCategory.costMultiplier AS COST_MULTIPLIER,    "
        mainSQL = mainSQL & "0 AS GROUP_ID, invCategories.categoryID AS CATEGORY_ID  "
        mainSQL = mainSQL & "FROM staStations, invTypes, ramAssemblyLineStations, mapRegions, mapSolarSystems, "
        mainSQL = mainSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerCategory, invCategories "
        mainSQL = mainSQL & "WHERE staStations.stationTypeID = invTypes.typeID "
        mainSQL = mainSQL & "And ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerCategory.assemblyLineTypeID "
        mainSQL = mainSQL & "And ramAssemblyLineTypeDetailPerCategory.categoryID = invCategories.categoryID "
        mainSQL = mainSQL & "And staStations.regionID = mapRegions.regionID "
        mainSQL = mainSQL & "And staStations.solarSystemID = mapSolarSystems.solarSystemID "
        mainSQL = mainSQL & "And staStations.stationID = ramAssemblyLineStations.stationID "
        mainSQL = mainSQL & "And ramAssemblyLineTypes.activityID = ramActivities.activityID "
        mainSQL = mainSQL & "And ramAssemblyLineStations.assemblyLineTypeID = ramAssemblyLineTypes.assemblyLineTypeID "

        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call SetProgressBarValues(" (" & mainSQL & ") AS X ")

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()
            SQL = "INSERT INTO STATION_FACILITIES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(9)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(10)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(11)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(12)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(13)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(14)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(15)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        SQLReader1.Close()
        Application.DoEvents()

        ' Finally do indexes
        SQL = "CREATE INDEX IDX_SF_FID_AID_GID_CID ON STATION_FACILITIES (FACILITY_ID, ACTIVITY_ID, GROUP_ID, CATEGORY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SF_OP_FN_AID_CID ON STATION_FACILITIES (FACILITY_NAME, ACTIVITY_ID, CATEGORY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SF_OP_FN_AID_GID ON STATION_FACILITIES (FACILITY_NAME, ACTIVITY_ID, GROUP_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SF_SSID_AID ON STATION_FACILITIES (SOLAR_SYSTEM_ID, ACTIVITY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SF_OP_AID_GID_CID_RN_SSN ON STATION_FACILITIES (ACTIVITY_ID, GROUP_ID, CATEGORY_ID, REGION_NAME, SOLAR_SYSTEM_NAME);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    End Sub

    ' Updates the table with categories not included - this makes it easier to run the station_facilities table without joins
    Private Sub UpdateramAssemblyLineTypeDetailPerCategory()
        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLCommand2 As New SQLiteCommand
        Dim SQLCommand3 As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim SQLReader12 As SQLiteDataReader
        Dim SQLReader13 As SQLiteDataReader
        Dim mainSQL As String

        ' Figure out what lines are not in the categories table so that we can add the missing line and categoryID
        mainSQL = "SELECT ramAssemblyLineTypes.assemblyLineTypeID, activityID "
        mainSQL = mainSQL & "FROM ramAssemblyLineTypes, ramInstallationTypeContents, invTypes "
        mainSQL = mainSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID Not IN (SELECT assemblyLineTypeID FROM ramAssemblyLineTypeDetailPerCategory) "
        mainSQL = mainSQL & "And ramAssemblyLineTypes.assemblyLineTypeID Not IN (SELECT assemblyLineTypeID FROM ramAssemblyLineTypeDetailPerGroup) "
        mainSQL = mainSQL & "And ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        mainSQL = mainSQL & "And ramInstallationTypeContents.installationTypeID = invTypes.typeID "
        mainSQL = mainSQL & "GROUP BY ramAssemblyLineTypes.assemblyLineTypeID, activityID "
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        While SQLReader1.Read
            ' Look up all item categoryID's for the activity of all blueprints that have it
            mainSQL = "SELECT invCategories.categoryID "
            mainSQL = mainSQL & "FROM industryActivityProducts, invTypes, invGroups, invCategories "
            ' This line figures out the items made with the bp, and then attaches it to the activities on the bp - not elegant but works with CCPs system
            mainSQL = mainSQL & "WHERE (SELECT typeID FROM invTypes, industryActivityProducts AS X WHERE typeID = X.productTypeID And X.activityID = 1 And X.blueprintTypeID = industryActivityProducts.blueprintTypeID) = invTypes.typeID "
            mainSQL = mainSQL & "And invTypes.groupID = invGroups.groupID "
            mainSQL = mainSQL & "And invGroups.categoryID = invCategories.categoryID "
            mainSQL = mainSQL & "And activityID = " & SQLReader1.GetValue(1) & " "
            mainSQL = mainSQL & "GROUP BY invCategories.categoryID "

            SQLCommand2 = New SQLiteCommand(mainSQL, SDEDB.DBRef)
            SQLReader12 = SQLCommand2.ExecuteReader()

            While SQLReader12.Read
                ' Now insert the data into the ramAssemblyLineTypeDetailPerCategory table if not there
                mainSQL = "SELECT 'X' FROM ramAssemblyLineTypeDetailPerCategory "
                mainSQL = mainSQL & "WHERE assemblyLineTypeID = " & SQLReader1.GetValue(0) & " "
                mainSQL = mainSQL & "AND categoryID = " & SQLReader12.GetValue(0) & " "
                mainSQL = mainSQL & "AND timeMultiplier = 1 AND materialMultiplier = 1 AND costMultiplier = 1"

                SQLCommand3 = New SQLiteCommand(mainSQL, SDEDB.DBRef)
                SQLReader13 = SQLCommand3.ExecuteReader()

                If Not SQLReader13.Read Then
                    mainSQL = "INSERT INTO ramAssemblyLineTypeDetailPerCategory VALUES ("
                    mainSQL = mainSQL & CStr(SQLReader1.GetValue(0)) & ", " ' ramAssemblyLineTypeID
                    mainSQL = mainSQL & CStr(SQLReader12.GetValue(0)) & ", " ' categoryID
                    mainSQL = mainSQL & "1,1,1)" ' timeMultiplier, materialMultiplier, and costMultiplier are all 1 by default since they don't exist
                Else
                    Application.DoEvents()
                End If

                Call Execute_SQLiteSQL(mainSQL, SDEDB.DBRef)

                SQLReader13.Close()

            End While

            SQLReader12.Close()

        End While

        SQLReader1.Close()

        ' Add station's categoryID to table so that we can build in stations - this is for the No POS facility, which might not matter anymore
        On Error Resume Next
        mainSQL = "INSERT INTO ramAssemblyLineTypeDetailPerCategory VALUES (5,3,1,1,1)"
        Call Execute_SQLiteSQL(mainSQL, SDEDB.DBRef)
        mainSQL = "INSERT INTO ramAssemblyLineTypeDetailPerCategory VALUES (35,3,1,1,1)"
        Call Execute_SQLiteSQL(mainSQL, SDEDB.DBRef)
        On Error GoTo 0

    End Sub

    ' STATIONS
    Private Sub Build_Stations()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE STATIONS ("
        SQL &= "STATION_ID INTEGER PRIMARY KEY,"
        SQL &= "STATION_NAME VARCHAR(100) NOT NULL,"
        SQL &= "STATION_TYPE_ID INTEGER NOT NULL,"
        SQL &= "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL &= "SOLAR_SYSTEM_SECURITY FLOAT NOT NULL,"
        SQL &= "REGION_ID INTEGER NOT NULL,"
        SQL &= "CORPORATION_ID INTEGER NOT NULL,"
        SQL &= "REPROCESSING_EFFICIENCY FLOAT NOT NULL,"
        SQL &= "REPROCESSING_TAX_RATE FLOAT NOT NULL,"
        SQL &= "CACHE_DATE VARCHAR(23) NOT NULL" ' Date for updating upwell structure names
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("staStations")

        ' Pull new data and insert
        mainSQL = "SELECT stationID, stationName, stationTypeID, solarSystemID, security, regionID, corporationID, reprocessingEfficiency, reprocessingStationsTake FROM staStations"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO STATIONS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ","
            SQL &= "'2200-01-01 00:00:00')" ' no expiration for static data

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQL = "CREATE INDEX IDX_S_FID ON STATIONS (STATION_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQLReader1.Close()

    End Sub

    ' RACE_IDS
    Private Sub Build_RACE_IDS()
        Dim SQL As String

        SQL = "CREATE TABLE RACE_IDS (ID INTEGER PRIMARY KEY, RACE VARCHAR(8) Not NULL)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "INSERT INTO RACE_IDS VALUES (1, 'Caldari')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "INSERT INTO RACE_IDS VALUES (2, 'Minmatar')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "INSERT INTO RACE_IDS VALUES (4, 'Amarr')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "INSERT INTO RACE_IDS VALUES (8, 'Gallente')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' FW_SYSTEM_UPGRADES
    Private Sub Build_FW_SYSTEM_UPGRADES()
        Dim SQL As String

        SQL = "CREATE TABLE FW_SYSTEM_UPGRADES (SOLAR_SYSTEM_ID INTEGER PRIMARY KEY, UPGRADE_LEVEL INTEGER NOT NULL)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' CHARACTER_SKILLS
    Private Sub Build_CHARACTER_SKILLS()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_SKILLS ("
        SQL &= "CHARACTER_ID INTEGER NOT NULL,"
        SQL &= "SKILL_TYPE_ID INTEGER NOT NULL,"
        SQL &= "SKILL_NAME VARCHAR(50) NOT NULL,"
        SQL &= "SKILL_POINTS INTEGER NOT NULL,"
        SQL &= "SKILL_LEVEL INTEGER NOT NULL,"
        SQL &= "OVERRIDE_SKILL INTEGER NOT NULL,"
        SQL &= "OVERRIDE_LEVEL INTEGER NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CSKILLS_CHARACTER_ID ON CHARACTER_SKILLS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CSKILLS_SKILL_TYPE_ID ON CHARACTER_SKILLS (SKILL_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' INDUSTRY_UPGRADE_BELTS
    Private Sub Build_INDUSTRY_UPGRADE_BELTS()
        Dim SQL As String

        ' Build the table
        SQL = "CREATE TABLE INDUSTRY_UPGRADE_BELTS ("
        SQL &= "AMOUNT INTEGER NOT NULL,"
        SQL &= "BELT_NAME VARCHAR(8) NOT NULL,"
        SQL &= "ORE VARCHAR(21) NOT NULL,"
        SQL &= "NUMBER_ASTEROIDS INTEGER NOT NULL,"
        SQL &= "TRUESEC_BONUS INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Since this is all data I created, just do inserts here 
        ' Data from: https://forums.eveonline.com/default.aspx?g=posts&t=418719
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Arkonor',4,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Crimson Arkonor',4,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Prime Arkonor',4,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Bistot',7,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Triclinic Bistot',7,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Monoclinic Bistot',7,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Crokite',9,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Sharp Crokite',9,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Crystalline Crokite',9,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Dark Ochre',6,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Onyx Ochre',6,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Obsidian Ochre',6,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Gneiss',13,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Iridescent Gneiss',13,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Prismatic Gneiss',13,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Spodumain',14,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Bright Spodumain',14,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Gleaming Spodumain',14,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Mercoxit',5,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Magma Mercoxit',5,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Vitreous Mercoxit',5,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (1,'Testing','Vitreous Mercoxit',5,10)", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Arkonor',4,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Crimson Arkonor',4,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Prime Arkonor',4,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Bistot',5,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Monoclinic Bistot',5,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Triclinic Bistot',5,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Crokite',7,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Sharp Crokite',7,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Crystalline Crokite',7,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Dark Ochre',10,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Obsidian Ochre',10,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Onyx Ochre',10,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Gneiss',10,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Prismatic Gneiss',10,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Iridescent Gneiss',10,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (578000,'Enormous','Spodumain',10,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (578000,'Enormous','Gleaming Spodumain',10,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (578000,'Enormous','Bright Spodumain',10,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Mercoxit',4,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Magma Mercoxit',4,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Vitreous Mercoxit',4,10)", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Arkonor',3,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Crimson Arkonor',3,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Prime Arkonor',3,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Bistot',5,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Monoclinic Bistot',5,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Triclinic Bistot',5,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Crokite',6,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Sharp Crokite',6,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Crystalline Crokite',6,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Dark Ochre',4,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Obsidian Ochre',4,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Onyx Ochre',4,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Gneiss',9,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Prismatic Gneiss',9,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Iridescent Gneiss',9,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Spodumain',9,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Gleaming Spodumain',9,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Bright Spodumain',9,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Mercoxit',3,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Magma Mercoxit',3,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Vitreous Mercoxit',3,10)", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Medium','Arkonor',3,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Medium','Crimson Arkonor',3,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Medium','Prime Arkonor',3,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Medium','Bistot',4,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Medium','Monoclinic Bistot',4,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Medium','Triclinic Bistot',4,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Medium','Crokite',5,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Medium','Sharp Crokite',5,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Medium','Crystalline Crokite',5,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Medium','Dark Ochre',3,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Medium','Obsidian Ochre',3,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Medium','Onyx Ochre',3,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Medium','Gneiss',8,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Medium','Prismatic Gneiss',8,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Medium','Iridescent Gneiss',8,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Medium','Spodumain',8,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Medium','Gleaming Spodumain',8,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Medium','Bright Spodumain',8,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Medium','Mercoxit',2,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Medium','Magma Mercoxit',2,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Medium','Vitreous Mercoxit',2,10)", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9700,'Small','Arkonor',3,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9700,'Small','Crimson Arkonor',3,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9700,'Small','Prime Arkonor',3,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Bistot',3,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Monoclinic Bistot',3,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Triclinic Bistot',3,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crokite',5,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Sharp Crokite',5,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crystalline Crokite',5,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Dark Ochre',4,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Obsidian Ochre',4,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Onyx Ochre',4,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Gneiss',6,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Prismatic Gneiss',6,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Iridescent Gneiss',6,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Spodumain',7,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Gleaming Spodumain',7,10)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Bright Spodumain',7,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Mercoxit',0,0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Magma Mercoxit',0,5)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Vitreous Mercoxit',0,10)", EVEIPHSQLiteDB.DBRef)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQL = "CREATE INDEX IDX_BP_ID_BELT_NAME ON INDUSTRY_UPGRADE_BELTS (BELT_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' ESI_CHARACTER_DATA
    Private Sub Build_ESI_CHARACTER_DATA()
        Dim SQL As String

        SQL = "CREATE TABLE ESI_CHARACTER_DATA ("
        SQL &= "CHARACTER_ID INTEGER NOT NULL,"
        SQL &= "CHARACTER_NAME VARCHAR(100) NOT NULL,"
        SQL &= "CORPORATION_ID INTEGER NOT NULL,"
        SQL &= "BIRTHDAY VARCHAR(23) NOT NULL," ' Date
        SQL &= "GENDER VARCHAR(6) NOT NULL,"
        SQL &= "RACE_ID INTEGER NOT NULL,"
        SQL &= "BLOODLINE_ID INTEGER NOT NULL,"
        SQL &= "ANCESTRY_ID INTEGER,"
        SQL &= "DESCRIPTION VARCHAR(500),"
        SQL &= "ACCESS_TOKEN VARCHAR(100) NOT NULL,"
        SQL &= "ACCESS_TOKEN_EXPIRE_DATE_TIME VARCHAR(23) NOT NULL," ' Date
        SQL &= "TOKEN_TYPE VARCHAR(20) NOT NULL,"
        SQL &= "REFRESH_TOKEN VARCHAR(100) NOT NULL,"
        SQL &= "SCOPES VARCHAR(1000) NOT NULL,"
        SQL &= "OVERRIDE_SKILLS INTEGER NOT NULL,"
        SQL &= "PUBLIC_DATA_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "SKILLS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "STANDINGS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "RESEARCH_AGENTS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "BLUEPRINTS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "ASSETS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "INDUSTRY_JOBS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "IS_DEFAULT Integer NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ECD_CHARACTER_ID On ESI_CHARACTER_DATA (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ESI_CORPORATION_DATA
    Private Sub Build_ESI_CORPORATION_DATA()
        Dim SQL As String

        SQL = "CREATE TABLE ESI_CORPORATION_DATA ("
        SQL &= "CORPORATION_ID INTEGER NOT NULL,"
        SQL &= "CORPORATION_NAME VARCHAR(100) NOT NULL,"
        SQL &= "TICKER VARCHAR(5) NOT NULL,"
        SQL &= "MEMBER_COUNT INTEGER NOT NULL,"
        SQL &= "FACTION_ID INTEGER,"
        SQL &= "ALLIANCE_ID INTEGER, "
        SQL &= "CEO_ID INTEGER NOT NULL,"
        SQL &= "CREATOR_ID INTEGER NOT NULL,"
        SQL &= "HOME_STATION_ID INTEGER,"
        SQL &= "SHARES INTEGER,"
        SQL &= "TAX_RATE REAL NOT NULL,"
        SQL &= "DESCRIPTION VARCHAR(1000),"
        SQL &= "DATE_FOUNDED VARCHAR(23)," ' Date
        SQL &= "URL VARCHAR(100),"
        SQL &= "PUBLIC_DATA_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "BLUEPRINTS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "ASSETS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "INDUSTRY_JOBS_CACHE_DATE VARCHAR(23)," ' Date
        SQL &= "CORP_ROLES_CACHE_DATE VARCHAR(23)" ' Date
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ECRD_CHARACTER_ID On ESI_CORPORATION_DATA (CORPORATION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ESI_CORPORATION_ROLES
    Private Sub Build_ESI_CORPORATION_ROLES()
        Dim SQL As String

        SQL = "CREATE TABLE ESI_CORPORATION_ROLES ("
        SQL &= "CORPORATION_ID INTEGER NOT NULL,"
        SQL &= "CHARACTER_ID INTEGER NOT NULL,"
        SQL &= "ROLE VARCHAR(25) NOT NULL,"
        SQL &= "ROLE_TYPE VARCHAR(25) NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ESI_CORPORATION_ROLES ON ESI_CORPORATION_ROLES (CORPORATION_ID, CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' EVEIPH DATA
    Private Sub Build_ESI_PUBLIC_CACHE_DATES()
        Dim SQL As String

        SQL = "CREATE TABLE ESI_PUBLIC_CACHE_DATES ("
        SQL &= "INDUSTRY_SYSTEMS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL &= "INDUSTRY_FACILITIES_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL &= "MARKET_PRICES_CACHED_UNTIL VARCHAR(23)" ' Date
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' PRICE_PROFILES
    Private Sub Build_PRICE_PROFILES()
        Dim SQL As String

        SQL = "CREATE TABLE PRICE_PROFILES ("
        SQL &= "ID Integer Not NULL,"
        SQL &= "GROUP_NAME VARCHAR(50) Not NULL,"
        SQL &= "PRICE_TYPE VARCHAR(25) Not NULL,"
        SQL &= "REGION_NAME VARCHAR(50) Not NULL,"
        SQL &= "SOLAR_SYSTEM_NAME VARCHAR(50) Not NULL,"
        SQL &= "PRICE_MODIFIER FLOAT Not NULL,"
        SQL &= "RAW_MATERIAL Integer Not NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Insert the base data, this will be default until they change it and it's copied in the updater - start with raw, in Jita
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Minerals','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ice Products','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Gas','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Abyssal Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Datacores','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Decryptors','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Planetary','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Asteroids','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Salvage','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ancient Salvage','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ancient Relics','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Hybrid Polymers','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Misc.','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Raw Moon Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Processed Moon Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Advanced Moon Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Materials & Compounds','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Rogue Drone Components','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Booster Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ships','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Charges','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Modules','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Drones','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Rigs','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Deployables','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Subsystems','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Boosters','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Structures','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Structure Modules','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Celestials','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Station Parts','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Adv. Capital Construction Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Capital Construction Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Construction Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Hybrid Tech Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Tools','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Fuel Blocks','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Implants','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Structure Rigs','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_PP_ID ON PRICE_PROFILES (ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' INVENTORY_NAMES
    Private Sub Build_INVENTORY_NAMES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE INVENTORY_NAMES ("
        SQL &= "ITEM_ID INTEGER NOT NULL,"
        SQL &= "ITEM_NAME VARCHAR(500)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("invNames")

        ' Pull new data and insert
        mainSQL = "SELECT itemID, itemName FROM invNames"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to new table
        While SQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_NAMES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader.Close()

        SQL = "CREATE INDEX IDX_ITEM_ID ON INVENTORY_NAMES (ITEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' INDUSTRY_ACTIVITY_PRODUCTS
    Private Sub Build_INDUSTRY_ACTIVITY_PRODUCTS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        ' Build table
        SQL = "CREATE TABLE INDUSTRY_ACTIVITY_PRODUCTS ("
        SQL &= "blueprintTypeID INTEGER NOT NULL,"
        SQL &= "activityID INTEGER NOT NULL,"
        SQL &= "productTypeID INTEGER NOT NULL,"
        SQL &= "quantity INTEGER NOT NULL,"
        SQL &= "probability FLOAT NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        mainSQL = "SELECT blueprintTypeID, activityID, productTypeID, quantity, probability FROM industryActivityProducts"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO INDUSTRY_ACTIVITY_PRODUCTS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        SQL = "CREATE INDEX IDX_IAP_BTID_AID ON INDUSTRY_ACTIVITY_PRODUCTS (blueprintTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' RAM_ACTIVITIES
    Private Sub Build_RAM_ACTIVITIES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE RAM_ACTIVITIES ("
        SQL &= "activityID INTEGER NOT NULL,"
        SQL &= "activityName VARCHAR(100),"
        SQL &= "iconNo VARCHAR(5),"
        SQL &= "description VARCHAR(1000),"
        SQL &= "published INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("ramActivities")

        ' Pull new data and insert
        mainSQL = "SELECT activityID, activityName, iconNo, description, published FROM ramActivities"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ACTIVITIES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetString(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetString(3)) & ","
            SQL &= BuildInsertFieldString(CInt(SQLReader1.GetBoolean(4))) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        ' Add two special cases
        Call Execute_SQLiteSQL("INSERT INTO RAM_ACTIVITIES VALUES(-1,'Drilling',NULL,'Moon Mining',0)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO RAM_ACTIVITIES VALUES(-2,'Reprocessing',NULL,'Ore Reprocessing',0)", EVEIPHSQLiteDB.DBRef)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_ACTIVITY_ID ON RAM_ACTIVITIES (activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' RAM_ASSEMBLY_LINE_STATIONS
    Private Sub Build_RAM_ASSEMBLY_LINE_STATIONS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_STATIONS ("
        SQL &= "stationID INTEGER NOT NULL,"
        SQL &= "assemblyLineTypeID INTEGER NOT NULL,"
        SQL &= "quantity INTEGER,"
        SQL &= "stationTypeID INTEGER, "
        SQL &= "ownerID INTEGER,"
        SQL &= "solarSystemID INTEGER,"
        SQL &= "regionID INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("ramAssemblyLineStations")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM ramAssemblyLineStations"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_STATIONS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_RALS_SID ON RAM_ASSEMBLY_LINE_STATIONS (stationID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_RALS_SSID ON RAM_ASSEMBLY_LINE_STATIONS (solarSystemID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_RALS_ALTID ON RAM_ASSEMBLY_LINE_STATIONS (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY ("
        SQL &= "assemblyLineTypeID INTEGER NOT NULL,"
        SQL &= "categoryID INTEGER NOT NULL,"
        SQL &= "timeMultiplier FLOAT,"
        SQL &= "materialMultiplier FLOAT, "
        SQL &= "costMultiplier FLOAT"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("ramAssemblyLineTypeDetailPerCategory")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM ramAssemblyLineTypeDetailPerCategory"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALC_ALTID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ALC_CID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY (categoryID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP ("
        SQL &= "assemblyLineTypeID INTEGER NOT NULL,"
        SQL &= "groupID INTEGER NOT NULL,"
        SQL &= "timeMultiplier FLOAT,"
        SQL &= "materialMultiplier FLOAT, "
        SQL &= "costMultiplier FLOAT"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("ramAssemblyLineTypeDetailPerGroup")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM ramAssemblyLineTypeDetailPerGroup"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALG_ALTID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ALG_GID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP (groupID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPES
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String



        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPES ("
        SQL &= "assemblyLineTypeID INTEGER NOT NULL,"
        SQL &= "assemblyLineTypeName VARCHAR(100),"
        SQL &= "description VARCHAR(1000),"
        SQL &= "baseTimeMultiplier FLOAT, "
        SQL &= "baseMaterialMultiplier FLOAT,"
        SQL &= "baseCostMultiplier FLOAT,"
        SQL &= "volume FLOAT,"
        SQL &= "activityID INTEGER,"
        SQL &= "minCostPerHour FLOAT"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("ramAssemblyLineTypes")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM ramAssemblyLineTypes"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALT_ALTID_AID ON RAM_ASSEMBLY_LINE_TYPES (assemblyLineTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ALT_AID ON RAM_ASSEMBLY_LINE_TYPES (activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' RAM_INSTALLATION_TYPE_CONTENTS
    Private Sub Build_RAM_INSTALLATION_TYPE_CONTENTS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE RAM_INSTALLATION_TYPE_CONTENTS ("
        SQL &= "installationTypeID INTEGER NOT NULL,"
        SQL &= "assemblyLineTypeID INTEGER NOT NULL,"
        SQL &= "quantity INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("ramInstallationTypeContents")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM ramInstallationTypeContents"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_INSTALLATION_TYPE_CONTENTS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_RITC_ITID_ALTID ON RAM_INSTALLATION_TYPE_CONTENTS (installationTypeID, assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_RITC_ALTID ON RAM_INSTALLATION_TYPE_CONTENTS (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' CHARACTER_STANDINGS
    Private Sub Build_Character_Standings()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_STANDINGS ("
        SQL &= "CHARACTER_ID INTEGER NOT NULL,"
        SQL &= "NPC_TYPE_ID INTEGER NOT NULL,"
        SQL &= "NPC_TYPE VARCHAR(50) NOT NULL,"
        SQL &= "NPC_NAME VARCHAR(500) NOT NULL,"
        SQL &= "STANDING REAL NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CS_CHARACTER_ID ON CHARACTER_STANDINGS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CS_NPC_TYPE_ID ON CHARACTER_STANDINGS (NPC_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' OWNED_BLUEPRINTS
    Private Sub Build_OWNED_BLUEPRINTS()
        Dim SQL As String

        SQL = "CREATE TABLE OWNED_BLUEPRINTS ("
        SQL &= "USER_ID INTEGER NOT NULL,"
        SQL &= "ITEM_ID INTEGER NOT NULL,"
        SQL &= "LOCATION_ID INTEGER NOT NULL,"
        SQL &= "BLUEPRINT_ID INTEGER NOT NULL,"
        SQL &= "BLUEPRINT_NAME VARCHAR(100) NOT NULL,"
        SQL &= "QUANTITY INTEGER NOT NULL,"
        SQL &= "FLAG_ID INTEGER NOT NULL,"
        SQL &= "ME INTEGER NOT NULL,"
        SQL &= "TE INTEGER NOT NULL,"
        SQL &= "RUNS INTEGER NOT NULL,"
        SQL &= "BP_TYPE INTEGER NOT NULL,"
        SQL &= "OWNED INTEGER NOT NULL,"
        SQL &= "SCANNED INTEGER NOT NULL,"
        SQL &= "FAVORITE INTEGER NOT NULL,"
        SQL &= "ADDITIONAL_COSTS FLOAT"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Indexes
        SQL = "CREATE INDEX IDX_OBP_USER_ID ON OWNED_BLUEPRINTS (USER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ALL_OWNED_BLUEPRINTS
    Private Sub Build_ALL_OWNED_BLUEPRINTS()
        Dim SQL As String

        SQL = "CREATE TABLE ALL_OWNED_BLUEPRINTS ("
        SQL &= "OWNER_ID INTEGER NOT NULL,"
        SQL &= "ITEM_ID INTEGER NOT NULL,"
        SQL &= "LOCATION_ID INTEGER NOT NULL,"
        SQL &= "BLUEPRINT_ID INTEGER NOT NULL,"
        SQL &= "BLUEPRINT_NAME VARCHAR(100) NOT NULL,"
        SQL &= "QUANTITY INTEGER NOT NULL,"
        SQL &= "FLAG_ID INTEGER NOT NULL,"
        SQL &= "ME INTEGER NOT NULL,"
        SQL &= "TE INTEGER NOT NULL,"
        SQL &= "RUNS INTEGER NOT NULL,"
        SQL &= "BP_TYPE INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Indexes
        SQL = "CREATE INDEX IDX_AOBP_OWNER_ID ON ALL_OWNED_BLUEPRINTS (OWNER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' FACTIONS
    Private Sub Build_FACTIONS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE FACTIONS ("
        SQL &= "factionID INTEGER PRIMARY KEY,"
        SQL &= "factionName VARCHAR(" & GetLenSQLExpField("factionName", "chrFactions") & ") NOT NULL,"
        SQL &= "raceID INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("chrFactions")

        Application.DoEvents()

        ' Pull new data and insert
        mainSQL = "SELECT factionID, factionName, raceIDs FROM chrFactions"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()
            SQL = "INSERT INTO FACTIONS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_F_FACTION_NAME ON FACTIONS (factionName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' INVENTORY_TRAITS
    Private Sub Build_INVENTORY_TRAITS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE INVENTORY_TRAITS ("
        SQL &= "bonusID INTEGER,"
        SQL &= "typeID INTEGER,"
        SQL &= "skilltypeID INTEGER,"
        SQL &= "bonus FLOAT,"
        SQL &= "bonusText TEXT,"
        SQL &= "importance INTEGER,"
        SQL &= "nameID INTEGER,"
        SQL &= "unitID INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("invTraits")

        Application.DoEvents()

        ' Pull new data and insert
        mainSQL = "SELECT * FROM invTraits"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()
            SQL = "INSERT INTO INVENTORY_TRAITS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_INVENTORY_TRAITS_BID ON INVENTORY_TRAITS (bonusID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_INVENTORY_TRAITS_TID ON INVENTORY_TRAITS (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' META_TYPEs
    Private Sub Build_Meta_Types()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE META_TYPES ("
        SQL &= "typeID INTEGER PRIMARY KEY,"
        SQL &= "parentTypeID INTEGER,"
        SQL &= "metaGroupID INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("invMetaTypes")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM invMetaTypes"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO META_TYPES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        pgMain.Visible = False

    End Sub

    ' USER_SETTINGS
    Private Sub Build_User_Settings()
        Dim SQL As String
        Dim i As Integer

        SQL = "CREATE TABLE USER_SETTINGS ("
        SQL &= "CHECK_FOR_UPDATES INTEGER,"
        SQL &= "EXPORT_CVS INTEGER,"
        SQL &= "DEFAULT_PRICE_SYSTEM VARCHAR(" & GetLenSQLExpField("solarSystemName", "mapSolarSystems") & "),"
        SQL &= "DEFAULT_PRICE_REGION VARCHAR(" & GetLenSQLExpField("regionName", "mapRegions") & "),"
        SQL &= "SHOW_TOOL_TIPS INTEGER,"
        SQL &= "REFINING_IMPLANT_VALUE REAL,"
        SQL &= "MANUFACTURING_IMPLANT_VALUE REAL,"
        SQL &= "BUILD_BASE_INSTALL REAL,"
        SQL &= "BUILD_BASE_HOURLY REAL,"
        SQL &= "BUILD_STANDING_DISCOUNT REAL,"
        SQL &= "INVENT_BASE_INSTALL REAL,"
        SQL &= "INVENT_BASE_HOURLY REAL,"
        SQL &= "INVENT_STANDING_DISCOUNT REAL,"
        SQL &= "BUILD_CORP_STANDING REAL,"
        SQL &= "INVENT_CORP_STANDING REAL,"
        SQL &= "BROKER_CORP_STANDING REAL,"
        SQL &= "BROKER_FACTION_STANDING REAL,"
        SQL &= "DEFAULT_POS_FUEL_COST REAL,"
        SQL &= "DEFAULT_BUILD_BUY INTEGER,"
        SQL &= "INCLUDE_COPY_TIMES INTEGER,"
        SQL &= "INCLUDE_INVENTION_TIMES INTEGER,"
        SQL &= "USE_MAX_BPC_RUNS_SHIP INTEGER,"
        SQL &= "USE_MAX_BPC_RUNS_NONSHIP INTEGER,"
        SQL &= "DEFAULT_COPY_COST REAL,"
        SQL &= "DEFAULT_COPY_SLOT_MODIFIER REAL,"
        SQL &= "COPY_IMPLANT_VALUE REAL,"
        SQL &= "DEFAULT_INVENTION_SLOT_MODIFIER REAL,"
        SQL &= "DEFAULT_ME INTEGER,"
        SQL &= "DEFAULT_PE INTEGER,"
        SQL &= "INCLUDE_RE_TIMES INTEGER,"
        SQL &= "REFINE_CORP_STANDING REAL,"
        For i = 13 To 50
            ' Null all the unused
            SQL &= "UNUSED_SETTING_" & CStr(i) & ","
        Next
        SQL = SQL.Substring(0, Len(SQL) - 1) & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ATTRIBUTE_TYPES
    Private Sub Build_Attribute_Types()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE ATTRIBUTE_TYPES ("
        SQL &= "attributeID INTEGER PRIMARY KEY,"
        SQL &= "attributeName VARCHAR(" & GetLenSQLExpField("attributeName", "dgmAttributeTypes") & "),"
        SQL &= "displayName VARCHAR(" & GetLenSQLExpField("displayName", "dgmAttributeTypes") & ")"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("dgmAttributeTypes")

        ' Pull new data and insert
        mainSQL = "SELECT attributeID, attributeName, displayName FROM dgmAttributeTypes"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO ATTRIBUTE_TYPES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        pgMain.Visible = False

    End Sub

    ' TYPE_ATTRIBUTES
    Private Sub Build_Type_Attributes()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE TYPE_ATTRIBUTES ("
        SQL &= "typeID INTEGER NOT NULL,"
        SQL &= "attributeID INTEGER,"
        SQL &= "valueInt INTEGER,"
        SQL &= "valueFloat REAL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("dgmTypeAttributes")

        ' Pull new data and insert
        mainSQL = "SELECT typeID, attributeID, valueInt, valueFloat FROM dgmTypeAttributes"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO TYPE_ATTRIBUTES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_TA_ATTRIBUTE_ID ON TYPE_ATTRIBUTES (attributeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_TA_TYPE_ID ON TYPE_ATTRIBUTES (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' TYPE_EFFECTS    
    Private Sub Build_Type_Effects()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE TYPE_EFFECTS ("
        SQL &= "typeID INTEGER NOT NULL,"
        SQL &= "effectID INTEGER,"
        SQL &= "isDefault INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("dgmTypeEffects")

        ' Pull new data and insert
        mainSQL = "SELECT typeID, effectID, isDefault FROM dgmTypeEffects"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO TYPE_EFFECTS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_TE_TYPE_ID ON TYPE_EFFECTS (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' OreRefine
    Private Sub Build_OreRefine()
        Dim SQL As String = ""

        Application.DoEvents()

        ' Build table
        SQL = "CREATE TABLE ORE_REFINE ("
        SQL &= "OreID INTEGER,"
        SQL &= "MineralID INTEGER,"
        SQL &= "MineralQuantity INTEGER)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Add Data
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (18,35,213)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES  (18,34,107)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (18,36,107)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,34,56000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,35,12050)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,36,2100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,37,450)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (20,34,134)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (20,36,267)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (20,37,134)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,35,1000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,37,200)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,38,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,39,19)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (22,34,22000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (22,36,2500)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (22,40,320)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1223,35,12000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1223,39,450)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1223,40,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,34,351)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,35,25)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,36,50)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,38,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1225,34,21000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1225,38,760)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1225,39,135)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1226,36,350)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1226,38,75)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1226,39,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1227,34,800)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1227,35,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1227,37,85)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1228,34,346)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1228,35,173)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1229,35,2200)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1229,36,2400)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1229,37,300)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1230,34,415)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,34,2200)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,37,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,38,120)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,39,15)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1232,34,10000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1232,37,1600)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1232,38,120)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (11396,11399,300)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28422,35,213)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28422,34,107)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28422,36,107)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,34,56000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,35,12050)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,36,2100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,37,450)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28410,34,134)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28410,36,267)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28410,37,134)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,35,1000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,37,200)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,38,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,39,19)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28367,34,22000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28367,36,2500)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28367,40,320)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28388,35,12000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28388,39,450)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28388,40,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,34,351)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,35,25)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,36,50)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,38,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28391,34,21000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28391,38,760)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28391,39,135)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28406,36,350)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28406,38,75)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28406,39,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28416,34,800)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28416,35,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28416,37,85)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28429,34,346)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28429,35,173)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28397,35,2200)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28397,36,2400)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28397,37,300)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28432,34,415)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,34,2200)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,37,100)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,38,120)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,39,15)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28394,34,10000)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28394,37,1600)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28394,38,120)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28413,11399,300)", EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ORE_REFINE_ORE_ID ON ORE_REFINE (OreID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ORES
    Private Sub Build_ORES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        Application.DoEvents()

        SQL = "CREATE TABLE ORES ("
        SQL &= "ORE_ID INTEGER PRIMARY KEY,"
        SQL &= "ORE_NAME VARCHAR(50),"
        SQL &= "ORE_VOLUME REAL,"
        SQL &= "UNITS_TO_REFINE INTEGER,"
        SQL &= "BELT_TYPE VARCHAR(20),"
        SQL &= "HIGH_YIELD_ORE INTEGER)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "SELECT invTypes.typeID, invTypes.typeName, invTypes.packagedVolume, invTypes.portionSize, "
        mainSQL = mainSQL & "CASE WHEN invTypes.groupID = 465 THEN 'Ice' WHEN invTypes.groupID = 711 THEN 'Gas' "
        mainSQL = mainSQL & "WHEN invTypes.groupID IN (1884,1920,1921,1922,1923) THEN invGroups.groupName "
        mainSQL = mainSQL & "ELSE 'Ore' END, "
        mainSQL = mainSQL & "CASE WHEN invTypes.typeName IN ('Arkonor','Bistot','Crokite','Dark Ochre','Gneiss','Hedbergite',  "
        mainSQL = mainSQL & "'Hemorphite','Jaspet','Kernite','Mercoxit','Omber','Plagioclase','Pyroxeres','Scordite','Spodumain','Veldspar') THEN 0 "
        mainSQL = mainSQL & "WHEN invTypes.groupID = 465 THEN -1 WHEN invTypes.groupID = 711 THEN -2 ELSE 1 END " ' 465 is ice, 711 is gas - neither have yield components
        mainSQL = mainSQL & "FROM invTypes, invGroups "
        mainSQL = mainSQL & "WHERE invTypes.groupID = invGroups.groupID "
        mainSQL = mainSQL & "AND (invGroups.categoryID = 25 OR invGroups.groupID = 711) " ' Clouds and Ores
        mainSQL = mainSQL & "AND invTypes.groupID NOT IN (903, 1911) " ' make sure this isn't ores for mining missions or legacy ice ('Ancient')
        mainSQL = mainSQL & "GROUP BY invTypes.typeID, invTypes.typeName, invTypes.packagedVolume, invTypes.portionSize, "
        mainSQL = mainSQL & "CASE WHEN invTypes.groupID = 465 THEN 'Ice' WHEN invTypes.groupID = 711 THEN 'Gas' "
        mainSQL = mainSQL & "WHEN invTypes.groupID IN (1884,1920,1921,1922,1923) THEN invGroups.groupName "
        mainSQL = mainSQL & "ELSE 'Ore' END, "
        mainSQL = mainSQL & "CASE WHEN invTypes.typeName IN ('Arkonor','Bistot','Crokite','Dark Ochre','Gneiss','Hedbergite', "
        mainSQL = mainSQL & "'Hemorphite','Jaspet','Kernite','Mercoxit','Omber','Plagioclase','Pyroxeres','Scordite','Spodumain','Veldspar') THEN 0 "
        mainSQL = mainSQL & "WHEN invTypes.groupID = 465 THEN -1 WHEN invTypes.groupID = 711 THEN -2 ELSE 1 END"

        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLCommand.CommandTimeout = 300
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO ORES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Moon ore versions of typical ores don't have a bonus, so are not high_yield. Plus need to mark them as moon asteroids
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 0, BELT_TYPE = 'Moon Asteroids' WHERE  "
        SQL = SQL & "ORE_NAME LIKE '%Flawless Arkonor' OR "
        SQL = SQL & "ORE_NAME LIKE '%Cubic Bistot' OR "
        SQL = SQL & "ORE_NAME LIKE '%Pellucid Crokite' OR "
        SQL = SQL & "ORE_NAME LIKE '%Brilliant Gneiss' OR "
        SQL = SQL & "ORE_NAME LIKE '%Lustrous Hedbergite' OR "
        SQL = SQL & "ORE_NAME LIKE '%Scintillating Hemorphite' OR "
        SQL = SQL & "ORE_NAME LIKE '%Resplendant Kernite' OR "
        SQL = SQL & "ORE_NAME LIKE '%Immaculate Jaspet' OR "
        SQL = SQL & "ORE_NAME LIKE '%Platinoid Omber' OR "
        SQL = SQL & "ORE_NAME LIKE '%Sparkling Plagioclase' OR "
        SQL = SQL & "ORE_NAME LIKE '%Opulent Pyroxeres' OR "
        SQL = SQL & "ORE_NAME LIKE '%Glossy Scordite' OR "
        SQL = SQL & "ORE_NAME LIKE '%Dazzling Spodumain' OR "
        SQL = SQL & "ORE_NAME LIKE '%Stable Veldspar' OR "
        SQL = SQL & "ORE_NAME LIKE '%Jet Ochre' "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now set the 5%/10% flag for standard ores
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Crimson Arkonor'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Triclinic Bistot'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Sharp Crokite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Onyx Ochre'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Vitric Hedbergite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Vivid Hemorphite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Pure Jaspet'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Luminous Kernite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Azure Plagioclase'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Solid Pyroxeres'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Condensed Scordite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Bright Spodumain'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Concentrated Veldspar'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Iridescent Gneiss'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Magma Mercoxit'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Silvery Omber'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Prime Arkonor'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Monoclinic Bistot'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Crystalline Crokite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Obsidian Ochre'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Glazed Hedbergite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Radiant Hemorphite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Pristine Jaspet'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Fiery Kernite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Rich Plagioclase'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Viscous Pyroxeres'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Massive Scordite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Gleaming Spodumain'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Dense Veldspar'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Prismatic Gneiss'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Vitreous Mercoxit'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Golden Omber'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Update the mooon mining ores for 5%/10% yield 
        ' Set all to 0 first
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 0 WHERE BELT_TYPE LIKE '%Moon Asteroids'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Brimful%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Glistening%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Lavish%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Shimmering%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Replete%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Glowing%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Bountiful%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Shining%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Copious%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Twinkling%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ORES_ORE_ID ON ORES (ORE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' ORE_LOCATIONS
    Private Sub Build_ORE_LOCATIONS()
        Dim SQL As String

        Application.DoEvents()

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand

        SQL = "CREATE TABLE ORE_LOCATIONS ("
        SQL &= "ORE_ID INTEGER NOT NULL,"
        SQL &= "SYSTEM_SECURITY VARCHAR(10),"
        SQL &= "SPACE VARCHAR(20),"
        SQL &= "HIGH_YIELD_ORE INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        ' Now open the saved table and insert all the values into this new table
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C5','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C6','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C3','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C4','WH',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'High Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Low Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'High Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'High Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'Low Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'High Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'High Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17975,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17976,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17977,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17978,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25274,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25274,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25276,'Low Sec','Amarr',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25276,'Null Sec','Amarr',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25277,'Null Sec','Amarr',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25277,'Low Sec','Amarr',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Null Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Null Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25279,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28694,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28694,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28695,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28695,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28696,'High Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28696,'High Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28697,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28697,'High Sec','Caldari',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28698,'Low Sec','Amarr',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28699,'High Sec','Amarr',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28700,'High Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28700,'High Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28701,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28701,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C1','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C2','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C1','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C2','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C1','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C2','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C1','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C2','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C1','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C2','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C3','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C4','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30377,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30377,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30378,'C5','WH',-2)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30378,'C6','WH',-2)", EVEIPHSQLiteDB.DBRef)

        ' Moon ores - all systems, all space
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45495,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45496,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45497,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45498,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45499,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45500,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45501,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45502,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45503,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45504,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45506,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45510,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45511,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45512,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45513,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46280,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46281,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46282,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46283,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46284,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46285,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46286,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46287,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46288,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46289,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46290,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46291,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46292,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46293,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46294,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46295,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46296,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46297,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46298,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46299,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46300,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46301,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46302,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46303,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46304,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46305,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46306,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46307,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46308,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46309,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46310,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46311,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46312,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46313,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46314,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46315,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46316,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46317,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46318,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46319,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46675,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45495,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45496,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45497,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45498,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45499,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45500,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45501,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45502,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45503,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45504,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45506,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45510,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45511,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45512,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45513,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46280,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46281,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46282,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46283,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46284,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46285,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46286,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46287,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46288,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46289,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46290,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46291,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46292,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46293,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46294,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46295,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46296,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46297,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46298,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46299,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46300,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46301,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46302,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46303,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46304,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46305,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46306,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46307,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46308,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46309,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46310,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46311,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46312,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46313,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46314,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46315,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46316,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46317,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46318,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46319,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46675,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45495,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45496,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45497,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45498,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45499,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45500,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45501,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45502,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45503,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45504,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45506,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45510,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45511,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45512,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45513,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46280,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46281,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46282,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46283,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46284,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46285,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46286,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46287,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46288,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46289,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46290,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46291,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46292,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46293,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46294,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46295,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46296,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46297,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46298,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46299,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46300,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46301,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46302,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46303,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46304,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46305,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46306,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46307,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46308,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46309,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46310,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46311,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46312,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46313,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46314,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46315,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46316,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46317,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46318,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46319,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46675,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45495,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45496,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45497,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45498,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45499,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45500,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45501,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45502,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45503,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45504,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45506,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45510,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45511,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45512,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45513,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46280,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46281,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46282,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46283,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46284,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46285,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46286,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46287,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46288,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46289,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46290,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46291,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46292,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46293,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46294,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46295,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46296,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46297,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46298,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46299,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46300,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46301,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46302,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46303,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46304,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46305,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46306,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46307,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46308,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46309,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46310,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46311,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46312,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46313,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46314,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46315,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46316,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46317,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46318,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46319,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46675,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45495,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45496,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45497,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45498,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45499,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45500,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45501,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45502,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45503,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45504,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45506,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45510,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45511,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45512,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45513,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46280,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46281,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46282,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46283,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46284,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46285,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46286,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46287,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46288,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46289,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46290,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46291,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46292,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46293,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46294,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46295,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46296,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46297,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46298,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46299,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46300,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46301,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46302,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46303,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46304,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46305,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46306,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46307,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46308,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46309,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46310,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46311,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46312,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46313,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46314,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46315,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46316,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46317,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46318,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46319,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46675,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45494,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45495,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45496,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45497,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45498,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45499,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45500,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45501,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45502,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45503,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45504,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45506,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45510,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45511,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45512,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45513,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46280,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46281,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46282,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46283,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46284,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46285,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46286,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46287,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46288,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46289,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46290,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46291,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46292,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46293,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46294,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46295,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46296,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46297,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46298,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46299,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46300,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46301,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46302,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46303,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46304,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46305,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46306,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46307,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46308,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46309,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46310,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46311,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46312,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46313,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46314,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46315,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46316,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46317,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46318,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46319,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46675,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45490,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45491,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45492,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (45493,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46676,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46677,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46678,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46679,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46680,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46681,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46682,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46683,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46684,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46685,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46686,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46687,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46688,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (46689,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ORE_LOCS_ORE_ID ON ORE_LOCATIONS (ORE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' ENGINEERING_RIG_BONUSES
    Private Sub Build_StructureRigBonuses()
        Dim SQL As String = ""

        Application.DoEvents()

        ' Build table
        SQL = "CREATE TABLE ENGINEERING_RIG_BONUSES ("
        SQL &= "typeID INTEGER,"
        SQL &= "groupID INTEGER,"
        SQL &= "categoryID INTEGER,"
        SQL &= "activityID INTEGER)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Add Data

        ' XL-Set Rigs
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37178,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37179,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37183,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37183,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37183,NULL,9,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37183,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37182,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37182,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37182,NULL,9,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37182,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37180,NULL,6,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37181,NULL,6,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43704,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43705,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45548,964,NULL,1)", EVEIPHSQLiteDB.DBRef)

        ' L-Set Rigs
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37174,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37174,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37174,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37174,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37175,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37175,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37175,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37175,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45641,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45641,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45641,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45641,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37168,898,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37168,900,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37168,902,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37169,898,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37169,900,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37169,902,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,NULL,32,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,358,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,380,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,540,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,543,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,832,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,833,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,894,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,906,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,963,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43709,1202,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,NULL,32,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,358,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,380,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,540,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,543,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,832,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,833,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,894,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,906,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,963,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43711,1202,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,324,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,541,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,830,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,831,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,834,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,893,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,1283,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,1305,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,1527,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43707,1534,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,324,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,541,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,830,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,831,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,834,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,893,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,1283,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,1305,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,1527,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43708,1534,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37164,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37165,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43718,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43719,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45546,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37166,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37166,513,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37166,941,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37167,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37167,513,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37167,941,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43716,26,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43716,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43716,28,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43716,419,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43716,463,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43716,1201,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43717,26,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43717,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43717,28,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43717,419,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43717,463,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43717,1201,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43714,25,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43714,29,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43714,31,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43714,237,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43714,420,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43714,1022,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43715,25,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43715,29,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43715,31,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43715,237,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43715,420,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43715,1022,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37173,30,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37173,485,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37173,547,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37173,659,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37173,883,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37173,1538,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37172,30,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37172,485,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37172,547,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37172,659,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37172,883,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37172,1538,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43712,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43712,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43713,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43713,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37170,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37170,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37170,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37170,12,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37170,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37171,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37171,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37171,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37171,12,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37171,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43720,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43721,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43722,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43723,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43724,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43725,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43726,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43727,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43729,NULL,9,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43730,NULL,9,5)", EVEIPHSQLiteDB.DBRef)

        ' M-Set Rigs
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43867,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43867,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43867,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43867,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43866,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43866,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43866,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43866,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45640,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45640,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45640,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45640,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43869,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43869,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43869,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43869,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43868,332,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43868,334,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43868,913,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43868,964,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43862,898,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43862,900,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43862,902,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43863,898,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43863,900,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43863,902,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43865,898,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43865,900,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43865,902,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43864,898,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43864,900,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43864,902,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,NULL,32,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,358,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,380,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,540,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,543,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,832,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,833,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,894,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,906,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,963,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43858,1202,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,NULL,32,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,358,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,380,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,540,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,543,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,832,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,833,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,894,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,906,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,963,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43859,1202,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,NULL,32,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,358,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,380,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,540,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,543,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,832,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,833,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,894,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,906,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,963,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43860,1202,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,NULL,32,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,358,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,380,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,540,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,543,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,832,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,833,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,894,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,906,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,963,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43861,1202,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,324,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,541,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,830,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,831,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,834,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,893,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,1283,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,1305,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,1527,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43855,1534,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,324,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,541,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,830,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,831,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,834,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,893,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,1283,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,1305,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,1527,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43854,1534,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,324,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,541,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,830,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,831,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,834,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,893,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,1283,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,1305,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,1527,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43856,1534,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,324,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,541,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,830,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,831,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,834,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,893,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,1283,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,1305,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,1527,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43857,1534,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37159,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37150,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37151,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37158,NULL,8,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43870,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43871,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43872,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43873,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(45544,873,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43732,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43732,513,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43732,941,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37152,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37152,513,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37152,941,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43733,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43733,513,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43733,941,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43734,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43734,513,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43734,941,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37146,26,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37146,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37146,28,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37146,419,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37146,463,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37146,1201,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37147,26,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37147,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37147,28,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37147,419,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37147,463,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37147,1201,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43919,26,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43919,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43919,28,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43919,419,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43919,463,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43919,1201,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37153,26,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37153,27,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37153,28,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37153,419,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37153,463,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37153,1201,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37154,25,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37154,29,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37154,31,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37154,237,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37154,420,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37154,1022,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37155,25,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37155,29,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37155,31,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37155,237,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37155,420,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37155,1022,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37162,25,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37162,29,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37162,31,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37162,237,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37162,420,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37162,1022,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37163,25,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37163,29,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37163,31,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37163,237,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37163,420,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37163,1022,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37156,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37156,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37157,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37157,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37148,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37148,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37149,NULL,87,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37149,NULL,18,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43920,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43920,12,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43920,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43920,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43920,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43921,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43921,12,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43921,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43921,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43921,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37160,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37160,12,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37160,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37160,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37160,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37161,448,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37161,12,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37161,NULL,7,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37161,NULL,20,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(37161,NULL,22,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43875,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43874,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43876,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,NULL,39,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,NULL,40,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,NULL,65,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,NULL,66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,NULL,-66,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,NULL,23,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43877,1136,NULL,1)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43878,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43879,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43880,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43881,NULL,9,8)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43882,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43883,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43884,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43885,NULL,9,4)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43886,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43887,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43888,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43889,NULL,9,3)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43890,NULL,9,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43891,NULL,9,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43892,NULL,9,5)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(43893,NULL,9,5)", EVEIPHSQLiteDB.DBRef)

        ' Reactions
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46484,429,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46485,429,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46486,429,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46487,429,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46488,974,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46489,974,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46490,974,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46491,974,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46492,712,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46493,712,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46494,712,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46495,712,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46496,429,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46496,974,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46496,712,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46497,429,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46497,974,NULL,11)", EVEIPHSQLiteDB.DBRef)
        Call Execute_SQLiteSQL("INSERT INTO ENGINEERING_RIG_BONUSES VALUES(46497,712,NULL,11)", EVEIPHSQLiteDB.DBRef)


        SQL = "CREATE INDEX IDX_ERB_TID ON ENGINEERING_RIG_BONUSES (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' REPROCESSING
    Private Sub Build_Reprocessing()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mySQLReader2 As SQLiteDataReader
        Dim mainSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE REPROCESSING ("
        SQL &= "ITEM_ID INTEGER NOT NULL,"
        SQL &= "ITEM_NAME VARCHAR(100),"
        SQL &= "ITEM_VOLUME REAL,"
        SQL &= "UNITS_TO_REPROCESS INTEGER,"
        SQL &= "REFINED_MATERIAL_ID INTEGER,"
        SQL &= "REFINED_MATERIAL VARCHAR(100),"
        SQL &= "REFINED_MATERIAL_GROUP VARCHAR(100),"
        SQL &= "REFINED_MATERIAL_VOLUME REAL,"
        SQL &= "REFINED_MATERIAL_QUANTITY INTEGER"

        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        mainSQL = "SELECT IT.typeID, IT.typeName, IT.packagedVolume, IT.portionSize, "
        mainSQL = mainSQL & "IT2.typeID, IT2.typeName, IG2.groupName, IT2.packagedVolume, ITM.quantity "
        mainSQL = mainSQL & "FROM invTypes AS IT, invTypeMaterials AS ITM, invGroups AS IG, invCategories as IC, "
        mainSQL = mainSQL & "invTypes AS IT2, invGroups AS IG2 "
        mainSQL = mainSQL & "WHERE IT.typeID = ITM.typeID AND IT.groupID = IG.groupID AND IG.categoryID = IC.categoryID "
        mainSQL = mainSQL & "AND ITM.materialTypeID = IT2.typeID AND IT2.groupID = IG2.groupID "

        ' Get the count
        SQLCommand = New SQLiteCommand("SELECT COUNT(*) FROM (" & mainSQL & ") AS X", SDEDB.DBRef)
        mySQLReader2 = SQLCommand.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO REPROCESSING VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ", "
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_REPRO_ITEM_MAT_ID On REPROCESSING (ITEM_ID, REFINED_MATERIAL_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' REACTIONS
    Private Sub Build_Reactions()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mySQLReader2 As SQLiteDataReader
        Dim mainSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE REACTIONS ("
        SQL &= "REACTION_TYPE_ID Integer Not NULL,"
        SQL &= "REACTION_NAME VARCHAR(100),"
        SQL &= "REACTION_GROUP VARCHAR(100),"
        SQL &= "REACTION_TYPE VARCHAR(255),"
        SQL &= "MATERIAL_TYPE_ID Integer,"
        SQL &= "MATERIAL_NAME VARCHAR(100),"
        SQL &= "MATERIAL_GROUP VARCHAR(100),"
        SQL &= "MATERIAL_CATEGORY  VARCHAR(100),"
        SQL &= "MATERIAL_QUANTITY Integer,"
        SQL &= "MATERIAL_VOLUME REAL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "Select invTypeReactions.reactionTypeID, "
        mainSQL = mainSQL & "invTypes.typeName, "
        mainSQL = mainSQL & "invGroups.groupName, "
        mainSQL = mainSQL & "CASE When invTypeReactions.input = 0 Then 'Output' ELSE 'Input' END, "
        mainSQL = mainSQL & "invTypeReactions.typeID, invTypes_1.typeName, "
        mainSQL = mainSQL & "invGroups_1.groupName, "
        mainSQL = mainSQL & "invCategories.categoryName, "
        mainSQL = mainSQL & "CASE WHEN COALESCE(valueInt, valueFloat) IS NULL THEN invTypeReactions.quantity ELSE COALESCE(valueInt, valueFloat) * invTypeReactions.quantity END AS Quantity, "
        mainSQL = mainSQL & "invTypes_1.packagedVolume "

        msSQL2 = "FROM invTypeReactions LEFT JOIN dgmTypeAttributes ON invTypeReactions.typeID = dgmTypeAttributes.typeID AND dgmTypeAttributes.attributeID =726,"
        msSQL2 = msSQL2 & "invTypes, invTypes AS invTypes_1, invGroups, invGroups AS invGroups_1, invCategories "
        msSQL2 = msSQL2 & "WHERE invTypeReactions.reactionTypeID = invTypes.typeID "
        msSQL2 = msSQL2 & "AND invTypeReactions.typeID = invTypes_1.typeID "
        msSQL2 = msSQL2 & "AND invTypes.groupID = invGroups.groupID "
        msSQL2 = msSQL2 & "AND invTypes_1.groupID = invGroups_1.groupID "
        msSQL2 = msSQL2 & "AND invGroups_1.categoryID = invCategories.categoryID "
        msSQL2 = msSQL2 & "AND invTypes.published <> 0 AND invTypes_1.published <> 0 AND invCategories.published <> 0 AND invGroups.published <> 0 AND invGroups_1.published <> 0 "

        SQLCommand = New SQLiteCommand("SELECT COUNT(*) " & msSQL2, SDEDB.DBRef)
        mySQLReader2 = SQLCommand.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        SQLCommand = New SQLiteCommand(mainSQL & msSQL2, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO REACTIONS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(9)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_REACTION_MAT_TYPE_ID ON REACTIONS (MATERIAL_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_REACTION_MAT_GROUP ON REACTIONS (MATERIAL_GROUP)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' CURRENT_RESEARCH_AGENTS
    Private Sub Build_Current_Research_Agents()
        Dim SQL As String

        SQL = "CREATE TABLE CURRENT_RESEARCH_AGENTS ("
        SQL &= "AGENT_ID INTEGER NOT NULL,"
        SQL &= "SKILL_TYPE_ID INTEGER NOT NULL,"
        SQL &= "RP_PER_DAY FLOAT NOT NULL,"
        SQL &= "RESEARCH_START_DATE VARCHAR(23) NOT NULL,"
        SQL &= "REMAINDER_POINTS FLOAT NOT NULL,"
        SQL &= "CHARACTER_ID INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CRA_AGENTID ON CURRENT_RESEARCH_AGENTS (AGENT_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CRA_CHARID ON CURRENT_RESEARCH_AGENTS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' SKILLS
    Private Sub Build_Skills()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mySQLReader2 As SQLiteDataReader
        Dim mainSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE SKILLS ("
        SQL &= "SKILL_TYPE_ID INTEGER PRIMARY KEY,"
        SQL &= "SKILL_NAME VARCHAR(100),"
        SQL &= "SKILL_GROUP VARCHAR(100)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "SELECT invTypes.typeID, invTypes.typeName, invGroups.groupName "
        msSQL2 = "FROM (invTypes INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) INNER JOIN invCategories ON invGroups.categoryID = invCategories.categoryID "
        msSQL2 = msSQL2 & "WHERE invCategories.categoryName='Skill' AND invTypes.published<>0 AND invGroups.published<>0 AND invCategories.published<>0"

        ' Get the count
        SQLCommand = New SQLiteCommand("SELECT COUNT(*) " & msSQL2, SDEDB.DBRef)
        mySQLReader2 = SQLCommand.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        SQLCommand = New SQLiteCommand(mainSQL & msSQL2, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO SKILLS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

    End Sub

    ' RESEARCH_AGENTS
    Private Sub Build_Research_Agents()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mySQLReader2 As SQLiteDataReader
        Dim mainSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE RESEARCH_AGENTS ("
        SQL &= "FACTION VARCHAR(100) NOT NULL,"
        SQL &= "CORPORATION_ID INTEGER,"
        SQL &= "CORPORATION_NAME VARCHAR(100),"
        SQL &= "AGENT_ID INTEGER,"
        SQL &= "AGENT_NAME VARCHAR(100) NOT NULL,"
        SQL &= "LEVEL INTEGER,"
        SQL &= "QUALITY INTEGER,"
        SQL &= "RESEARCH_TYPE_ID INTEGER,"
        SQL &= "RESEARCH_TYPE VARCHAR(100) NOT NULL,"
        SQL &= "REGION_ID INTEGER,"
        SQL &= "REGION_NAME VARCHAR(100),"
        SQL &= "SOLAR_SYSTEM_ID INTEGER,"
        SQL &= "SOLAR_SYSTEM_NAME VARCHAR(100),"
        SQL &= "SECURITY REAL,"
        SQL &= "STATION VARCHAR(100)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "SELECT chrFactions.factionName, agtAgents.corporationID, "
        mainSQL = mainSQL & "invNames_1.itemName, "
        mainSQL = mainSQL & "invNames.itemID, "
        mainSQL = mainSQL & "invNames.itemName, "
        mainSQL = mainSQL & "agtAgents.level, "
        mainSQL = mainSQL & "agtAgents.quality, "
        mainSQL = mainSQL & "invTypes.typeID, "
        mainSQL = mainSQL & "invTypes.typeName, "
        mainSQL = mainSQL & "mapRegions.regionID, "
        mainSQL = mainSQL & "mapRegions.regionName, "
        mainSQL = mainSQL & "mapSolarSystems.solarSystemID, "
        mainSQL = mainSQL & "mapSolarSystems.solarSystemName, "
        mainSQL = mainSQL & "mapSolarSystems.security, "
        mainSQL = mainSQL & "mapDenormalize.itemName AS Station "
        msSQL2 = "FROM (((((((((agtAgents INNER JOIN agtResearchAgents ON agtAgents.agentID = agtResearchAgents.agentID) "
        msSQL2 = msSQL2 & "INNER JOIN invTypes ON agtResearchAgents.typeID = invTypes.typeID) "
        msSQL2 = msSQL2 & "INNER JOIN crpNPCCorporations ON agtAgents.corporationID = crpNPCCorporations.corporationID) "
        msSQL2 = msSQL2 & "INNER JOIN invNames ON agtResearchAgents.agentID = invNames.itemID) "
        msSQL2 = msSQL2 & "INNER JOIN mapDenormalize ON agtAgents.locationID = mapDenormalize.itemID) "
        msSQL2 = msSQL2 & "INNER JOIN mapSolarSystems ON mapDenormalize.solarSystemID = mapSolarSystems.solarSystemID) "
        msSQL2 = msSQL2 & "INNER JOIN mapConstellations ON mapDenormalize.constellationID = mapConstellations.constellationID) "
        msSQL2 = msSQL2 & "INNER JOIN mapRegions ON mapDenormalize.regionID = mapRegions.regionID) "
        msSQL2 = msSQL2 & "INNER JOIN invNames AS invNames_1 ON crpNPCCorporations.corporationID = invNames_1.itemID) "
        msSQL2 = msSQL2 & "INNER JOIN chrFactions ON crpNPCCorporations.factionID = chrFactions.factionID "
        msSQL2 = msSQL2 & "WHERE agtAgents.agentTypeID= 4"

        ' Get the count
        SQLCommand = New SQLiteCommand("SELECT COUNT(*) " & msSQL2, SDEDB.DBRef)
        mySQLReader2 = SQLCommand.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        SQLCommand = New SQLiteCommand(mainSQL & msSQL2, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO RESEARCH_AGENTS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(9)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(10)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(11)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(12)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(13)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(14)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_RA_TYPE_CORP_ID ON RESEARCH_AGENTS (RESEARCH_TYPE, CORPORATION_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_RA_REGION_ID ON RESEARCH_AGENTS (REGION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' Industry Jobs
    Private Sub Build_Industry_Jobs()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_JOBS ("
        SQL &= "jobID INTEGER PRIMARY KEY, "
        SQL &= "installerID INTEGER, "
        SQL &= "facilityID INTEGER, "
        SQL &= "locationID INTEGER, "
        SQL &= "activityID INTEGER, "
        SQL &= "blueprintID INTEGER, "
        SQL &= "blueprintTypeID INTEGER, "
        SQL &= "blueprintLocationID INTEGER, "
        SQL &= "outputLocationID INTEGER, "
        SQL &= "runs INTEGER, "
        SQL &= "cost FLOAT, "
        SQL &= "licensedRuns INTEGER, "
        SQL &= "probability FLOAT, "
        SQL &= "productTypeID INTEGER, "
        SQL &= "status INTEGER, "
        SQL &= "duration INTEGER, "

        ' Dates
        SQL &= "startDate VARCHAR(23), "
        SQL &= "endDate VARCHAR(23), "
        SQL &= "pauseDate VARCHAR(23), "
        SQL &= "completedDate VARCHAR(23), "

        SQL &= "completedCharacterID INTEGER,"
        SQL &= "successfulRuns INTEGER, "
        SQL &= "JobType INTEGER " ' corp or personal flag
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ASSETS
    Private Sub Build_Assets()
        Dim SQL As String

        SQL = "CREATE TABLE ASSETS ("
        SQL &= "ID INTEGER NOT NULL,"
        SQL &= "ItemID INTEGER NOT NULL,"
        SQL &= "LocationID INTEGER NOT NULL,"
        SQL &= "TypeID INTEGER NOT NULL,"
        SQL &= "Quantity INTEGER NOT NULL,"
        SQL &= "Flag INTEGER NOT NULL,"
        SQL &= "IsSingleton INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC ON ASSETS (LocationID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ITEM_TYPEID_ID ON ASSETS (TypeID, ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' ASSET_LOCATIONS
    Private Sub Build_Asset_Locations()
        Dim SQL As String

        SQL = "CREATE TABLE ASSET_LOCATIONS ("
        SQL &= "EnumAssetType INTEGER NOT NULL,"
        SQL &= "ID INTEGER NOT NULL,"
        SQL &= "LocationID INTEGER NOT NULL,"
        SQL &= "FlagID INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC_TYPE_ACCID ON ASSET_LOCATIONS (EnumAssetType, ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC_ACCOUNT_ID ON ASSET_LOCATIONS (ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' REGIONS
    Private Sub Build_REGIONS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE REGIONS ("
        SQL &= "regionID INTEGER PRIMARY KEY,"
        SQL &= "regionName VARCHAR(20) NOT NULL,"
        SQL &= "factionID INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef) ' SQLite table

        Application.DoEvents()

        mainSQL = "SELECT regionID, regionName, factionID FROM mapRegions ORDER BY regionName"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            If SQLReader1.GetValue(0) = 11000001 Then
                Application.DoEvents()
            End If
            SQL = "INSERT INTO REGIONS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)
        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        SQL = "CREATE INDEX IDX_R_REGION_NAME ON REGIONS (regionName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_R_REGION_ID ON REGIONS (regionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_R_FID ON REGIONS (factionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' CONSTELLATIONS
    Private Sub Build_CONSTELLATIONS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE CONSTELLATIONS ("
        SQL &= "regionID INTEGER NOT NULL,"
        SQL &= "constellationID INTEGER PRIMARY KEY,"
        SQL &= "constellationName VARCHAR(20) NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "SELECT regionID, constellationID, constellationName FROM mapConstellations"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO CONSTELLATIONS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()


        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        SQL = "CREATE INDEX IDX_C_REGION_ID ON CONSTELLATIONS (regionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' SOLAR_SYSTEMS
    Private Sub Build_SOLAR_SYSTEMS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE SOLAR_SYSTEMS ("
        SQL &= "regionID INTEGER NOT NULL,"
        SQL &= "constellationID INTEGER NOT NULL,"
        SQL &= "solarSystemID INTEGER PRIMARY KEY,"
        SQL &= "solarSystemName VARCHAR(17) NOT NULL,"
        SQL &= "security REAL NOT NULL,"
        SQL &= "factionWarzone INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Pull new data and insert
        mainSQL = "SELECT regionID, constellationID, solarSystemID, solarSystemName, security, "
        mainSQL = mainSQL & "CASE WHEN solarSystemID in (SELECT solarSystemID FROM warCombatZoneSystems) THEN 1 ELSE 0 END as fwsystem "
        mainSQL = mainSQL & "FROM mapSolarSystems "
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO SOLAR_SYSTEMS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SS_REGION_ID ON SOLAR_SYSTEMS (regionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SS_SS_ID ON SOLAR_SYSTEMS (solarSystemID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SS_CONSTELLATION_ID ON SOLAR_SYSTEMS (constellationID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_SS_SYSTEM_NAME ON SOLAR_SYSTEMS (solarSystemName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

#Region "Inventory Tables"

    ' INVENTORY_TYPES
    Private Sub Build_INVENTORY_TYPES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE INVENTORY_TYPES ("
        SQL &= "typeID INTEGER PRIMARY KEY,"
        SQL &= "groupID INTEGER,"
        SQL &= "typeName VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & "),"
        SQL &= "description VARCHAR(" & GetLenSQLExpField("description", "invTypes") & "),"
        SQL &= "mass REAL,"
        SQL &= "volume REAL,"
        SQL &= "packagedVolume REAL,"
        SQL &= "capacity REAL,"
        SQL &= "portionSize INTEGER,"
        SQL &= "factionID INTEGER,"
        SQL &= "raceID INTEGER,"
        SQL &= "basePrice REAL,"
        SQL &= "published INTEGER,"
        SQL &= "marketGroupID INTEGER,"
        SQL &= "graphicID INTEGER,"
        SQL &= "radius REAL,"
        SQL &= "iconID INTEGER,"
        SQL &= "soundID INTEGER,"
        SQL &= "sofFactionName INTEGER,"
        SQL &= "sofMaterialSetID INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("invTypes")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM invTypes "
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_TYPES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & "," ' TypeID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & "," ' GroupID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & "," ' TypeName
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & "," ' Description
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & "," ' Mass
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & "," ' Volume
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & "," ' Packaged Volume
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & "," ' Capacity
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & "," ' PortionSize
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(9)) & "," ' FactionID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(10)) & "," ' RaceID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(11)) & "," ' BasePrice
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(12)) & "," ' published
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(13)) & "," ' marketGroupID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(14)) & "," ' graphicID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(15)) & "," ' radius
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(16)) & "," ' iconID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(17)) & "," ' soundID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(18)) & "," ' sofFactionName
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(19)) & ")" ' sofMaterialSetID

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_IT_GROUP_ID ON INVENTORY_TYPES (groupID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IT_TYPE_NAME_ID ON INVENTORY_TYPES (typeName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IT_TYPE_ID ON INVENTORY_TYPES (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' INVENTORY_GROUPS
    Private Sub Build_INVENTORY_GROUPS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE INVENTORY_GROUPS ("
        SQL &= "groupID INTEGER PRIMARY KEY,"
        SQL &= "categoryID INTEGER,"
        SQL &= "groupName VARCHAR(" & GetLenSQLExpField("groupName", "invGroups") & "),"
        SQL &= "iconID INTEGER,"
        SQL &= "useBasePrice INTEGER,"
        SQL &= "anchored INTEGER,"
        SQL &= "anchorable INTEGER,"
        SQL &= "fittableNonSingleton INTEGER,"
        SQL &= "published INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("invGroups")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM invGroups"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_GROUPS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & "," ' groupID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & "," ' categoryID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & "," ' groupName
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & "," ' iconID
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & "," ' useBasePrice
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & "," ' anchored
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & "," ' anchorable
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & "," ' fittableNonSingleton
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ")" ' published

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_IG_GROUP_ID ON INVENTORY_GROUPS (groupID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IG_CATEGORY_ID ON INVENTORY_GROUPS (categoryID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

    End Sub

    ' INVENTORY_CATEGORIES
    Public Sub Build_INVENTORY_CATEGORIES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE INVENTORY_CATEGORIES ("
        SQL &= "categoryID INTEGER PRIMARY KEY,"
        SQL &= "categoryName VARCHAR(" & GetLenSQLExpField("categoryName", "invCategories") & "),"
        SQL &= "published INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("invCategories")

        ' Pull new data and insert
        mainSQL = "SELECT categoryID, categoryName, published FROM invCategories"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_CATEGORIES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(CInt(SQLReader1.GetValue(2))) & ")" ' A bit value, but reads as a boolean for some reason

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQL = "CREATE INDEX IDX_IC_CATEGORY_ID ON INVENTORY_GROUPS (categoryID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQLReader1.Close()

        pgMain.Visible = False

    End Sub

    ' INVENTORY_FLAGS
    Private Sub Build_INVENTORY_FLAGS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String
        Dim Temp As String

        SQL = "CREATE TABLE INVENTORY_FLAGS ("
        SQL &= "flagID INTEGER NOT NULL,"
        SQL &= "flagName VARCHAR(200) NOT NULL,"
        SQL &= "flagText VARCHAR(100) NOT NULL,"
        SQL &= "orderID INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call SetProgressBarValues("invFlags")

        ' Pull new data and insert
        mainSQL = "SELECT * FROM invFlags"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_FLAGS VALUES ("

            Select Case CInt(SQLReader1.GetValue(0))
                Case 63, 64, 146, 147
                    ' Set these to None flag text
                    SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & "," & "'None','None',0)"
                Case Else
                    ' Just whatever is in the table
                    SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
                    SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
                    If CStr(SQLReader1.GetValue(2)).Contains("Corp Security Access Group") Then
                        ' Change name to corp hanger - save the number
                        Temp = BuildInsertFieldString(SQLReader1.GetValue(2))
                        SQL &= "'Corp Hanger " & Temp.Substring(Len(Temp) - 2, 1) & "',"
                    Else
                        SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
                    End If
                    SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ")"
            End Select

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        ' Add a final flag for space
        SQL = "INSERT INTO INVENTORY_FLAGS VALUES (" & CStr(SpaceFlagCode) & ",'Space','Space',0)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        SQL = "CREATE INDEX IDX_ITEM_FLAG_ID ON INVENTORY_FLAGS (FlagID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

#End Region

#Region "Item Price Tables"

    ' ITEM_PRICES
    Private Sub Build_ITEM_PRICES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        Application.DoEvents()

        ' See if the view exists and drop if it does
        SQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = 'PRICES_BUILD' AND type = 'view'"
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        If CInt(SQLReader1.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_BUILD"
            SQLReader1.Close()
            Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        Else
            SQLReader1.Close()
        End If

        ' Build 2 queries and the Union, then pull data
        mainSQL = "CREATE VIEW PRICES_BUILD AS "
        mainSQL = mainSQL & "SELECT ALL_BLUEPRINTS.ITEM_ID, "
        mainSQL = mainSQL & "ALL_BLUEPRINTS.ITEM_NAME, "
        mainSQL = mainSQL & "ALL_BLUEPRINTS.TECH_LEVEL, "
        mainSQL = mainSQL & "0 AS PRICE, "
        mainSQL = mainSQL & "ALL_BLUEPRINTS.ITEM_CATEGORY, "
        mainSQL = mainSQL & "ALL_BLUEPRINTS.ITEM_GROUP, "
        mainSQL = mainSQL & "1 AS MANUFACTURE, "
        mainSQL = mainSQL & "ALL_BLUEPRINTS.ITEM_TYPE,"
        mainSQL = mainSQL & "'None' AS PRICE_TYPE "
        mainSQL = mainSQL & "FROM ALL_BLUEPRINTS "
        mainSQL = mainSQL & "WHERE ITEM_ID <> 33195" ' For some reason spatial attunement Units are getting in here and NO Build, but they are no build items only

        Execute_SQLiteSQL(mainSQL, SDEDB.DBRef)

        ' See if the view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = 'PRICES_NOBUILD' AND type = 'view'"
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        If CInt(SQLReader1.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_NOBUILD"
            SQLReader1.Close()
            Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        Else
            SQLReader1.Close()
        End If

        mainSQL = "CREATE VIEW PRICES_NOBUILD AS SELECT * FROM ("
        ' Get all the materials used to build stuff
        mainSQL = mainSQL & "SELECT DISTINCT MATERIAL_ID, MATERIAL, 0 AS TECH_LEVEL, 0 AS PRICE, MATERIAL_CATEGORY, MATERIAL_GROUP, 0 AS MANUFACTURE, "
        mainSQL = mainSQL & "0 AS ITEM_TYPE, 'None' AS PRICE_TYPE "
        mainSQL = mainSQL & "FROM ALL_BLUEPRINT_MATERIALS "
        mainSQL = mainSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM ALL_BLUEPRINTS) "
        mainSQL = mainSQL & "AND MATERIAL_CATEGORY <> 'Skill' "
        mainSQL = mainSQL & "UNION "
        ' Get specific materials for later use or other areas in IPH (ie asteroids) - include items for LP Store
        mainSQL = mainSQL & "SELECT DISTINCT typeID AS MATERIAL_ID, typeName AS MATERIAL, 0 AS TECH_LEVEL, 0 AS PRICE, categoryName AS MATERIAL_CATEGORY, "
        mainSQL = mainSQL & "groupName AS MATERIAL_GROUP, 0 AS MANUFACTURE, 0 AS ITEM_TYPE, 'None' AS PRICE_TYPE "
        mainSQL = mainSQL & "FROM invTypes, invGroups, invCategories "
        mainSQL = mainSQL & "WHERE invTypes.groupID = invGroups.groupID "
        mainSQL = mainSQL & "AND invGroups.categoryID = invCategories.categoryID "
        mainSQL = mainSQL & "AND invTypes.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0 "
        mainSQL = mainSQL & "AND invTypes.marketGroupID IS NOT NULL "
        mainSQL = mainSQL & "AND (categoryName IN ('Asteroid','Decryptors','Planetary Commodities','Planetary Resources') "
        mainSQL = mainSQL & "OR groupName in ('Moon Materials','Ice Product','Harvestable Cloud','Intermediate Materials') "
        ' The last IDs are random items that come out of booster production or needed in station building (Quafe), or for the LP Store
        mainSQL = mainSQL & "OR typeID in (41, 3699, 3773, 9850, 33195))) AS X " ' Garbage, Quafe, Hydrochloric Acid, Spirits, Spatial Attunment
        mainSQL = mainSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD)"
        Execute_SQLiteSQL(mainSQL, SDEDB.DBRef)

        '' See if the view exists and delete if so
        'SQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = 'PRICES_LP_OFFERS' AND type = 'view'"
        'SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        'SQLReader1 = SQLCommand.ExecuteReader()
        'SQLReader1.Read()

        'If CInt(SQLReader1.GetValue(0)) = 1 Then
        '    SQL = "DROP VIEW PRICES_LP_OFFERS"
        '    SQLReader1.Close()
        '    Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        'Else
        '    SQLReader1.Close()
        'End If

        '' Build a view for items that are LP offers and requirements for those items
        'mainSQL = "CREATE VIEW PRICES_LP_OFFERS AS SELECT * FROM ("
        'mainSQL = mainSQL & "SELECT DISTINCT lpOfferRequirements.typeID AS MATERIAL_ID, typeName AS MATERIAL, "
        '' If we can build this group, then mark the item as a tech 1 (lp stores don't have tech 2 items) and manufacture as 1 too
        'mainSQL = mainSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS TECH_LEVEL, "
        'mainSQL = mainSQL & "0 AS PRICE, categoryName AS MATERIAL_CATEGORY, groupName AS MATERIAL_GROUP, "
        'mainSQL = mainSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS MANUFACTURE, "
        'mainSQL = mainSQL & "CASE WHEN (typeName LIKE '%Navy%' OR typeName LIKE '%Fleet%' OR typeName LIKE '%Sisters%' OR typeName LIKE '%ORE%' OR typeName LIKE '%Thukker%') THEN 15 ELSE 16 END as ITEM_TYPE, 'None' AS PRICE_TYPE "
        'mainSQL = mainSQL & "FROM invTypes, invGroups, invCategories, lpOfferRequirements "
        'mainSQL = mainSQL & "WHERE lpOfferRequirements.typeID = invTypes.typeID "
        'mainSQL = mainSQL & "AND invTypes.groupID = invGroups.groupID "
        'mainSQL = mainSQL & "AND invGroups.categoryID = invCategories.categoryID "
        'mainSQL = mainSQL & "UNION "
        'mainSQL = mainSQL & "SELECT DISTINCT lpOffers.typeID AS MATERIAL_ID, typeName AS MATERIAL, "
        'mainSQL = mainSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS TECH_LEVEL, "
        'mainSQL = mainSQL & "0 AS PRICE, categoryName AS MATERIAL_CATEGORY, groupName AS MATERIAL_GROUP, "
        'mainSQL = mainSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS MANUFACTURE, "
        'mainSQL = mainSQL & "CASE WHEN (typeName LIKE '%Navy%' OR typeName LIKE '%Fleet%' OR typeName LIKE '%Sisters%' OR typeName LIKE '%ORE%' OR typeName LIKE '%Thukker%') THEN 15 ELSE 16 END as ITEM_TYPE, 'None' AS PRICE_TYPE "
        'mainSQL = mainSQL & "FROM invTypes, invGroups, invCategories, lpOffers "
        'mainSQL = mainSQL & "WHERE lpOffers.typeID = invTypes.typeID "
        'mainSQL = mainSQL & "AND invTypes.groupID = invGroups.groupID "
        'mainSQL = mainSQL & "AND invGroups.categoryID = invCategories.categoryID) AS X "
        'mainSQL = mainSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD) AND MATERIAL_ID NOT IN (SELECT MATERIAL_ID FROM PRICES_NOBUILD)"
        'Execute_SQLiteSQL(mainSQL, EVEIPHSQLiteDB.DBRef)

        '' See if the view exists and delete if so
        'SQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = 'PRICES_LP_BP_ITEMS' AND type = 'view'"
        'SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        'SQLReader1 = SQLCommand.ExecuteReader()
        'SQLReader1.Read()

        'If CInt(SQLReader1.GetValue(0)) = 1 Then
        '    SQL = "DROP VIEW PRICES_LP_BP_ITEMS"
        '    SQLReader1.Close()
        '    Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        'Else
        '    SQLReader1.Close()
        'End If

        '' Finally make sure all the items that we can build from BP's are included
        'mainSQL = "CREATE VIEW PRICES_LP_BP_ITEMS AS SELECT * FROM ("
        'mainSQL = mainSQL & "SELECT DISTINCT industryActivityProducts.productTypeID AS MATERIAL_ID, typeName AS MATERIAL, 1 AS TECH_LEVEL, 0 AS PRICE, "
        'mainSQL = mainSQL & "categoryName AS MATERIAL_CATEGORY, groupName AS MATERIAL_GROUP, 1 AS MANUFACTURE, 0 AS ITEM_TYPE, 'None' AS PRICE_TYPE "
        'mainSQL = mainSQL & "FROM lpOffers, industryActivityProducts, invTypes, invGroups, invCategories "
        'mainSQL = mainSQL & "WHERE lpOffers.typeID = industryActivityProducts.blueprintTypeID "
        'mainSQL = mainSQL & "AND industryActivityProducts.productTypeID = invTypes.typeID "
        'mainSQL = mainSQL & "AND invTypes.groupID = invGroups.groupID "
        'mainSQL = mainSQL & "AND invGroups.categoryID = invCategories.categoryID "
        'mainSQL = mainSQL & "AND activityID = 1) AS X "
        'mainSQL = mainSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD) AND MATERIAL_ID NOT IN (SELECT MATERIAL_ID FROM PRICES_NOBUILD) "
        'mainSQL = mainSQL & "AND MATERIAL_ID NOT IN (SELECT MATERIAL_ID FROM PRICES_LP_OFFERS)"
        'Execute_SQLiteSQL(mainSQL, EVEIPHSQLiteDB.DBRef)

        ' See if the union view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sqlite_master WHERE tbl_name = 'ITEM_PRICES_UNION' AND type = 'table'"
        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        If CInt(SQLReader1.GetValue(0)) = 1 Then
            SQL = "DROP TABLE ITEM_PRICES_UNION"
            SQLReader1.Close()
            Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        Else
            SQLReader1.Close()
        End If

        SQL = "CREATE TABLE ITEM_PRICES_UNION AS SELECT * FROM (SELECT * FROM PRICES_BUILD UNION SELECT * FROM PRICES_NOBUILD) "
        Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Create SQLite table
        SQL = "CREATE TABLE ITEM_PRICES ("
        SQL &= "ITEM_ID INTEGER PRIMARY KEY,"
        SQL &= "ITEM_NAME VARCHAR(" & GetLenSQLExpField("ITEM_NAME", "ITEM_PRICES_UNION") & ") NOT NULL,"
        SQL &= "TECH_LEVEL INTEGER NOT NULL,"
        SQL &= "PRICE REAL,"
        SQL &= "ITEM_CATEGORY VARCHAR(" & GetLenSQLExpField("ITEM_CATEGORY", "ITEM_PRICES_UNION") & ") NOT NULL,"
        SQL &= "ITEM_GROUP VARCHAR(" & GetLenSQLExpField("ITEM_GROUP", "ITEM_PRICES_UNION") & ") NOT NULL,"
        SQL &= "MANUFACTURE INTEGER NOT NULL,"
        SQL &= "ITEM_TYPE INTEGER NOT NULL,"
        SQL &= "PRICE_TYPE VARCHAR(20) NOT NULL,"
        SQL &= "ADJUSTED_PRICE FLOAT NOT NULL,"
        SQL &= "AVERAGE_PRICE FLOAT NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ITEM_PRICES_UNION")

        ' Now select the final query of data into a temp table
        mainSQL = "SELECT ITEM_ID, ITEM_NAME, TECH_LEVEL, PRICE, ITEM_CATEGORY, ITEM_GROUP, MANUFACTURE, ITEM_TYPE, PRICE_TYPE FROM ITEM_PRICES_UNION "
        mainSQL = mainSQL & "GROUP BY ITEM_ID, ITEM_NAME, TECH_LEVEL, PRICE, ITEM_CATEGORY, ITEM_GROUP, MANUFACTURE, ITEM_TYPE, PRICE_TYPE"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Insert the data into the table
        While SQLReader1.Read
            SQL = "INSERT INTO ITEM_PRICES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ",0,0)" ' For Adjusted market price and Average market price from CREST

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()

        ' Update the item types fields to make sure they are all able to be found
        SQL = "UPDATE ITEM_PRICES SET ITEM_TYPE = 1 WHERE ITEM_TYPE = 0 AND TECH_LEVEL = 1"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_IP_GROUP ON ITEM_PRICES (ITEM_GROUP)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IP_TYPE ON ITEM_PRICES (ITEM_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IP_CATEGORY ON ITEM_PRICES (ITEM_CATEGORY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Drop the Temp table
        SQL = "DROP TABLE ITEM_PRICES_UNION"
        Call SDEDB.ExecuteNonQuerySQL(SQL)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' ITEM_PRICES_CACHE
    Private Sub Build_ITEM_PRICES_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE ITEM_PRICES_CACHE ("
        SQL &= "typeID INTEGER NOT NULL,"
        SQL &= "buyVolume REAL NOT NULL,"
        SQL &= "buyweightedAvg REAL NOT NULL,"
        SQL &= "buyAvg REAL NOT NULL,"
        SQL &= "buyMax REAL NOT NULL,"
        SQL &= "buyMin REAL NOT NULL,"
        SQL &= "buyStdDev REAL NOT NULL,"
        SQL &= "buyMedian REAL NOT NULL,"
        SQL &= "buyPercentile REAL NOT NULL,"
        SQL &= "buyVariance REAL NOT NULL,"
        SQL &= "sellVolume REAL NOT NULL,"
        SQL &= "sellweightedAvg REAL NOT NULL,"
        SQL &= "sellAvg REAL NOT NULL,"
        SQL &= "sellMax REAL NOT NULL,"
        SQL &= "sellMin REAL NOT NULL,"
        SQL &= "sellStdDev REAL NOT NULL,"
        SQL &= "sellMedian REAL NOT NULL,"
        SQL &= "sellPercentile REAL NOT NULL,"
        SQL &= "sellVariance REAL NOT NULL,"
        SQL &= "RegionORSystem INTEGER NOT NULL,"
        SQL &= "UpdateDate VARCHAR(23) NOT NULL" ' Date
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IPC_TYPEID ON ITEM_PRICES_CACHE (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IPC_ID_REGION ON ITEM_PRICES_CACHE (typeID, RegionORSystem)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' MARKET_HISTORY
    Private Sub Build_MARKET_HISTORY()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_HISTORY ("
        SQL &= "TYPE_ID INTEGER,"
        SQL &= "REGION_ID INTEGER,"
        SQL &= "PRICE_HISTORY_DATE VARCHAR(23)," ' Date
        SQL &= "LOW_PRICE FLOAT,"
        SQL &= "HIGH_PRICE FLOAT,"
        SQL &= "AVG_PRICE FLOAT,"
        SQL &= "TOTAL_ORDERS_FILLED INTEGER,"
        SQL &= "TOTAL_VOLUME_FILLED INTEGER"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE UNIQUE INDEX IDX_MH_TID_RID ON MARKET_HISTORY (TYPE_ID, REGION_ID, PRICE_HISTORY_DATE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' MARKET_HISTORY_UPDATE_CACHE
    Private Sub Build_MARKET_HISTORY_UPDATE_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_HISTORY_UPDATE_CACHE ("
        SQL &= "TYPE_ID INTEGER NOT NULL,"
        SQL &= "REGION_ID INTEGER NOT NULL,"
        SQL &= "CACHE_DATE VARCHAR(23)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE UNIQUE INDEX IDX_MHUC_TID_RID ON MARKET_HISTORY_UPDATE_CACHE (TYPE_ID, REGION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' MARKET_ORDERS
    Private Sub Build_MARKET_ORDERS()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_ORDERS ("
        SQL &= "ORDER_ID INTEGER,"
        SQL &= "TYPE_ID INTEGER,"
        SQL &= "LOCATION_ID INTEGER,"
        SQL &= "REGION_ID INTEGER,"
        SQL &= "SOLAR_SYSTEM_ID INTEGER,"
        SQL &= "ORDER_ISSUED VARCHAR(23)," ' Date
        SQL &= "DURATION INTEGER,"
        SQL &= "IS_BUY_ORDER INTEGER," ' boolean
        SQL &= "PRICE FLOAT,"
        SQL &= "VOLUME_TOTAL INTEGER,"
        SQL &= "MINIMUM_VOLUME INTEGER,"
        SQL &= "VOLUME_REMAINING INTEGER,"
        SQL &= "RANGE VARCHAR(15) "
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_MO_TID_RID_SID ON MARKET_ORDERS (TYPE_ID, REGION_ID, SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' MARKET_ORDERS_UPDATE_CACHE
    Private Sub Build_MARKET_ORDERS_UPDATE_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_ORDERS_UPDATE_CACHE ("
        SQL &= "TYPE_ID INTEGER NOT NULL,"
        SQL &= "REGION_ID INTEGER NOT NULL,"
        SQL &= "CACHE_DATE VARCHAR(23)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE UNIQUE INDEX IDX_MOUC_TID_RID ON MARKET_ORDERS_UPDATE_CACHE (TYPE_ID, REGION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

#End Region

#Region "Facility Tables"

    ' SAVED_FACILITIES
    Private Sub Build_SAVED_FACILITIES()
        Dim SQL As String

        ' Build table here - tax and bonuses are null until they save them, else pulled from other data or default 1 or 0
        SQL = "CREATE TABLE [SAVED_FACILITIES](
                    [CHARACTER_ID] INT NOT NULL, 
                    [PRODUCTION_TYPE] INT NOT NULL, 
                    [FACILITY_VIEW] INT NOT NULL, 
                    [FACILITY_ID] INT NOT NULL, 
                    [FACILITY_TYPE] INT NOT NULL, 
                    [FACILITY_TYPE_ID] INT NOT NULL, 
                    [REGION_ID] INT NOT NULL, 
                    [SOLAR_SYSTEM_ID] INT NOT NULL, 
                    [ACTIVITY_COST_PER_SECOND] REAL NOT NULL, 
                    [INCLUDE_ACTIVITY_COST] INT NOT NULL, 
                    [INCLUDE_ACTIVITY_TIME] INT NOT NULL, 
                    [INCLUDE_ACTIVITY_USAGE] INT NOT NULL, 
                    [FACILITY_TAX] REAL, 
                    [MATERIAL_MULTIPLIER] REAL, 
                    [TIME_MULTIPLIER] REAL, 
                    [COST_MULTIPLIER] REAL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_CID_PT ON SAVED_FACILITIES (CHARACTER_ID, PRODUCTION_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Add default data
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,1,0,60003760,0,1529,10000002,30000142,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,1,1,60003760,0,1529,10000002,30000142,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,2,0,60003760,0,1529,10000002,30000142,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,2,1,60003760,0,1529,10000002,30000142,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,3,0,60003760,0,1529,10000002,30000142,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,3,1,60003760,0,1529,10000002,30000142,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,4,0,60003043,0,1529,10000002,30000163,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,4,1,60003043,0,1529,10000002,30000163,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,5,0,12236,1,24575,10000047,30003713,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,5,1,12236,1,24575,10000047,30003713,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,6,0,12236,1,30389,10000002,30000142,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,6,1,12236,1,30389,10000002,30000142,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,7,0,12236,1,30389,10000002,30000142,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,7,1,12236,1,30389,10000002,30000142,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,8,0,12236,1,25305,10000047,30003713,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,8,1,12236,1,25305,10000047,30003713,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,9,0,60001786,0,54,10000002,30000187,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,9,1,60001786,0,54,10000002,30000187,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,10,0,60001786,0,54,10000002,30000187,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,10,1,60001786,0,54,10000002,30000187,0,1,1,1,0.1,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,11,0,35835,3,35835,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,11,1,35835,3,35835,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,12,0,12236,1,24567,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,12,1,12236,1,24567,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,13,0,12236,1,24653,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,13,1,12236,1,24653,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,14,0,12236,1,13780,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,14,1,12236,1,13780,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,15,0,12236,1,24660,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,15,1,12236,1,24660,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,16,0,12236,1,29613,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO SAVED_FACILITIES VALUES (0,16,1,12236,1,29613,10000002,30000163,0,1,1,1,0,NULL, NULL, NULL)", EVEIPHSQLiteDB.DBRef)

    End Sub

    ' FACILITY_PRODUCTION_TYPES
    Private Sub Build_FACILITY_PRODUCTION_TYPES()
        Dim SQL As String

        ' Build table here
        SQL = "CREATE TABLE FACILITY_PRODUCTION_TYPES (
                PRODUCTION_TYPE INT NOT NULL,
                DESCRIPTION VARCHAR(20) NOT NULL,
                ACTIVITY_ID INT NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_PT ON FACILITY_PRODUCTION_TYPES (PRODUCTION_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (0,'None',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (1,'Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (2,'Component Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (3,'Capital Component Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (4,'Capitial Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (5,'Super Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (6,'T3 Cruiser Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (7,'Subsystem Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (8,'Booster Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (9,'Copying',5);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (10,'Invention',8);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (11,'Cannot build in POS - No POS Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (12,'T3 Invention',8);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (13,'T3 Destroyer Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (14,'POS Module Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (15,'Fuel Block Manufacturing',1);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_PRODUCTION_TYPES VALUES (16,'Large Ship Manufacturing',1)", EVEIPHSQLiteDB.DBRef)

    End Sub

    ' FACILITY_TYPES
    Private Sub Build_FACILITY_TYPES()
        Dim SQL As String

        ' Build table here
        SQL = "CREATE TABLE FACILITY_TYPES (
                FACILITY_TYPE_ID INT NOT NULL,
                FACILITY_TYPE_NAME VARCHAR(10) NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO FACILITY_TYPES VALUES (-1,'None');", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_TYPES VALUES (0,'Station');", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_TYPES VALUES (1,'POS');", EVEIPHSQLiteDB.DBRef)
        'Execute_SQLiteSQL("INSERT INTO FACILITY_TYPES VALUES (2,'Outpost');", EVEIPHSQLiteDB.DBRef) ' no more outposts after july 2018
        Execute_SQLiteSQL("INSERT INTO FACILITY_TYPES VALUES (3,'Structure');", EVEIPHSQLiteDB.DBRef)

    End Sub

    ' UPWELL_STRUCTURES_INSTALLED_MODULES
    Private Sub Build_UPWELL_STRUCTURES_INSTALLED_MODULES()
        Dim SQL As String

        ' Build table here
        SQL = "CREATE TABLE UPWELL_STRUCTURES_INSTALLED_MODULES (
                CHARACTER_ID INT NOT NULL,
                PRODUCTION_TYPE INT NOT NULL,
                SOLAR_SYSTEM_ID INT NOT NULL,
                FACILITY_VIEW INT NOT NULL,
                FACILITY_ID INT NOT NULL,
                INSTALLED_MODULE_ID INT NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' FACILITY_ACTIVITIES
    Private Sub Build_FACILITY_ACTIVITIES()
        Dim SQL As String

        ' Build table here
        SQL = "CREATE TABLE FACILITY_ACTIVITIES (
                FACILITY_TYPE_ID INT NOT NULL,
                FACILITY_TYPE_NAME VARCHAR(10) NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO FACILITY_ACTIVITIES VALUES(1,'Manufacturing');", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_ACTIVITIES VALUES(1,'Component Manufacturing');", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_ACTIVITIES VALUES(1,'Cap Component Manufacturing');", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_ACTIVITIES VALUES(5,'Copying');", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO FACILITY_ACTIVITIES VALUES(8,'Invention');", EVEIPHSQLiteDB.DBRef)

    End Sub

    ' UPWELL_STRUCTURES
    Private Sub Build_UPWELL_STRUCTURES()
        Dim SQL As String

        ' Build table here
        SQL = "CREATE TABLE UPWELL_STRUCTURES (
                UPWELL_STRUCTURE_TYPE_ID INT NOT NULL,
                UPWELL_STRUCTURE_NAME VARCHAR(25) NOT NULL,
                ACTIVITY_ID INT NOT NULL,
                MATERIAL_MULTIPLIER DOUBLE NOT NULL,
                TIME_MULTIPLIER DOUBLE NOT NULL,
                COST_MULTIPLIER DOUBLE NOT NULL,
                GROUP_ID INT NOT NULL,
                CATEGORY_ID INT NOT NULL)" ' group and category IDs are for processing, but will just be zero to accept all types

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' ECs
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35825,'Raitaru',1,.99,.85,.97,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35825,'Raitaru',5,1,.85,.97,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35825,'Raitaru',8,1,.85,.97,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35826,'Azbel',1,.99,.80,.96,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35826,'Azbel',5,1,.80,.96,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35826,'Azbel',8,1,.80,.96,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35827,'Sotiyo',1,.99,.70,.95,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35827,'Sotiyo',5,1,.70,.95,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35827,'Sotiyo',8,1,.70,.95,0,0);", EVEIPHSQLiteDB.DBRef)

        ' Citadels
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35832,'Astrahus',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35832,'Astrahus',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35832,'Astrahus',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35833,'Fortizar',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35833,'Fortizar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35833,'Fortizar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35834,'Keepstar',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35834,'Keepstar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35834,'Keepstar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(40340,'Upwell Palatine Keepstar',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(40340,'Upwell Palatine Keepstar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(40340,'Upwell Palatine Keepstar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        ' Refineries
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35835,'Athanor',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35835,'Athanor',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35835,'Athanor',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35835,'Athanor',11,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35836,'Tatara',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35836,'Tatara',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35836,'Tatara',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(35836,'Tatara',11,1,.75,1,0,0);", EVEIPHSQLiteDB.DBRef)

        ' Faction Citadels
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47512,'''Moreau'' Fortizar',1,.9,1,.98,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47512,'''Moreau'' Fortizar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47512,'''Moreau'' Fortizar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47513,'''Draccous'' Fortizar',1,1,.85,.97,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47513,'''Draccous'' Fortizar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47513,'''Draccous'' Fortizar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47514,'''Horizon'' Fortizar',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47514,'''Horizon'' Fortizar',5,1,.85,.97,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47514,'''Horizon'' Fortizar',8,1,.85,.97,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47515,'''Marginis'' Fortizar',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47515,'''Marginis'' Fortizar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47515,'''Marginis'' Fortizar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47516,'''Prometheus'' Fortizar',1,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47516,'''Prometheus'' Fortizar',5,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO UPWELL_STRUCTURES VALUES(47516,'''Prometheus'' Fortizar',8,1,1,1,0,0);", EVEIPHSQLiteDB.DBRef)

    End Sub

    ' MAP_DISALLOWED_ANCHOR_CATEGORIES
    Private Sub Build_MAP_DISALLOWED_ANCHOR_CATEGORIES()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        ' Build table here
        SQL = "CREATE TABLE MAP_DISALLOWED_ANCHOR_CATEGORIES (
                SOLAR_SYSTEM_ID INT NOT NULL,
                CATEGORY_ID INT NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        mainSQL = "SELECT * FROM mapDisallowedAnchorCategories"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO MAP_DISALLOWED_ANCHOR_CATEGORIES VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_MDAC_SS_ID ON MAP_DISALLOWED_ANCHOR_CATEGORIES (SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' MAP_DISALLOWED_ANCHOR_GROUPS
    Private Sub Build_MAP_DISALLOWED_ANCHOR_GROUPS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        ' Build table here
        SQL = "CREATE TABLE MAP_DISALLOWED_ANCHOR_GROUPS (
                SOLAR_SYSTEM_ID INT NOT NULL,
                GROUP_ID INT NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        mainSQL = "SELECT * FROM mapDisallowedAnchorGroups"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO MAP_DISALLOWED_ANCHOR_GROUPS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_MDAG_SS_ID ON MAP_DISALLOWED_ANCHOR_GROUPS (SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' INDUSTRY_SYSTEMS_COST_INDICIES
    Private Sub Build_INDUSTRY_SYSTEMS_COST_INDICIES()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_SYSTEMS_COST_INDICIES ("
        SQL &= "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL &= "SOLAR_SYSTEM_NAME VARCHAR(100) NOT NULL,"
        SQL &= "ACTIVITY_ID INTEGER NOT NULL,"
        SQL &= "ACTIVITY_NAME VARCHAR(100) NOT NULL,"
        SQL &= "COST_INDEX FLOAT NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_ISCI_SSID_AID ON INDUSTRY_SYSTEMS_COST_INDICIES (SOLAR_SYSTEM_ID, ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

    ' INDUSTRY_FACILITIES
    Private Sub Build_INDUSTRY_FACILITIES()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_FACILITIES ("
        SQL &= "FACILITY_ID INTEGER NOT NULL,"
        SQL &= "FACILITY_NAME VARCHAR(100) NOT NULL,"
        SQL &= "FACILITY_TYPE_ID INTEGER NOT NULL,"
        SQL &= "FACILITY_TAX FLOAT NOT NULL,"
        SQL &= "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL &= "REGION_ID INTEGER NOT NULL,"
        SQL &= "OWNER_ID INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IF_MAIN ON INDUSTRY_FACILITIES (FACILITY_TYPE_ID, REGION_ID, SOLAR_SYSTEM_ID, FACILITY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IF_SSID ON INDUSTRY_FACILITIES (SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_IF_FTID ON INDUSTRY_FACILITIES (FACILITY_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    End Sub

#End Region

#Region "PI Tables"

    ' PLANET_RESOURCES
    Private Sub Build_PLANET_RESOURCES()
        Dim SQL As String
        SQL = "CREATE TABLE PLANET_RESOURCES ("
        SQL &= "PLANET_TYPE_ID INTEGER NOT NULL,"
        SQL &= "RESOURCE_TYPE_ID INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2073)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2073)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2073)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2073)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2267)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2267)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2267)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2267)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2267)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2268)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2268)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2268)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2268)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2268)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2268)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2270)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2270)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2272)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2272)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2272)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2286)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2286)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2287)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2287)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2288)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2288)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2288)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2305)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2306)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2306)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2307)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2308)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2308)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2308)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2309)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2309)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2310)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2310)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2310)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2310)", EVEIPHSQLiteDB.DBRef)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2311)", EVEIPHSQLiteDB.DBRef)

        ' Indexes
        SQL = "CREATE INDEX IDX_PR_PTID_RTID ON PLANET_RESOURCES (PLANET_TYPE_ID, RESOURCE_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        SQL = "CREATE INDEX IDX_PR_PTID ON PLANET_RESOURCES (PLANET_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' PLANET_SCHEMATICS
    Private Sub Build_PLANET_SCHEMATICS()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE PLANET_SCHEMATICS ("
        SQL &= "schematicID INTEGER PRIMARY KEY,"
        SQL &= "schematicName VARCHAR(50) NOT NULL,"
        SQL &= "cycleTime INTEGER NOT NULL"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        mainSQL = "SELECT * FROM planetSchematics"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID ON PLANET_SCHEMATICS (schematicID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' PLANET_SCHEMATICS_TYPE_MAP
    Private Sub Build_PLANET_SCHEMATICS_TYPE_MAP()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE PLANET_SCHEMATICS_TYPE_MAP ("
        SQL &= "schematicID INTEGER NOT NULL,"
        SQL &= "typeID INTEGER NOT NULL,"
        SQL &= "quantity INTEGER NOT NULL,"
        SQL &= "isInput INTEGER NOT NULL,"
        SQL &= "PRIMARY KEY (schematicID, typeID)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        mainSQL = "SELECT * FROM planetSchematicsTypeMap"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS_TYPE_MAP VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID_TMAP ON PLANET_SCHEMATICS_TYPE_MAP (schematicID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

    ' PLANET_SCHEMATICS_PIN_MAP
    Private Sub Build_PLANET_SCHEMATICS_PIN_MAP()
        Dim SQL As String

        ' SQL variables
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader
        Dim mainSQL As String

        SQL = "CREATE TABLE PLANET_SCHEMATICS_PIN_MAP ("
        SQL &= "schematicID INTEGER NOT NULL,"
        SQL &= "pintypeID INTEGER NOT NULL,"
        SQL &= "PRIMARY KEY (schematicID, pintypeID)"
        SQL &= ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        mainSQL = "SELECT * FROM planetSchematicsPinMap"
        SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While SQLReader1.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS_PIN_MAP VALUES ("
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
            SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQLReader1.Close()
        SQLReader1 = Nothing
        SQLCommand = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID_PIN_MAP ON PLANET_SCHEMATICS_PIN_MAP (schematicID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

        pgMain.Visible = False

        Application.DoEvents()

    End Sub

#End Region

#Region "LP Store Tables"

    ' LP_OFFER_REQUIREMENTS
    'Private Sub Build_LP_OFFER_REQUIREMENTS()
    '    Dim SQL As String

    '    ' SQL variables
    '    Dim SQLCommand As New SQLiteCommand
    '    Dim SQLReader1 As SQLiteDataReader
    '    Dim mainSQL As String

    '    SQL = "CREATE TABLE LP_OFFER_REQUIREMENTS ("
    '    SQL &= "OFFER_ID INTEGER NOT NULL,"
    '    SQL &= "REQ_TYPE_ID INTEGER NOT NULL,"
    '    SQL &= "REQ_QUANTITY INTEGER NOT NULL"
    '    SQL &= ")"

    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    ' Now select the count of the final query of data

    '    ' Pull new data and insert
    '    mainSQL = "SELECT * FROM lpOfferRequirements WHERE offerID <> 1201" '1201 seems to be extra
    '    SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
    '    SQLReader1 = SQLCommand.ExecuteReader()

    '    Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

    '    While SQLReader1.Read
    '        Application.DoEvents()

    '        SQL = "INSERT INTO LP_OFFER_REQUIREMENTS VALUES ("
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

    '        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    End While

    '    Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    '    SQLReader1.Close()
    '    SQLReader1 = Nothing
    '    SQLCommand = Nothing

    '    ' Now index and PK the table
    '    SQL = "CREATE INDEX IDX_LO_OID_RTID ON LP_OFFER_REQUIREMENTS (OFFER_ID, REQ_TYPE_ID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    SQL = "CREATE INDEX IDX_LO_RTID ON LP_OFFER_REQUIREMENTS (REQ_TYPE_ID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    pgMain.Visible = False

    '    Application.DoEvents()

    'End Sub

    '' LP_OFFERS
    'Private Sub Build_LP_OFFERS()
    '    Dim SQL As String

    '    ' SQL variables
    '    Dim SQLCommand As New SQLiteCommand
    '    Dim SQLReader1 As SQLiteDataReader
    '    Dim mainSQL As String

    '    SQL = "CREATE TABLE LP_OFFERS ("
    '    SQL &= "offerID INTEGER PRIMARY KEY,"
    '    SQL &= "typeID INTEGER NOT NULL,"
    '    SQL &= "quantity INTEGER NOT NULL,"
    '    SQL &= "lpCost FLOAT NOT NULL,"
    '    SQL &= "iskCost FLOAT NOT NULL"
    '    SQL &= ")"

    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    ' Now select the count of the final query of data

    '    ' Pull new data and insert
    '    mainSQL = "SELECT * FROM lpOffers"
    '    SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
    '    SQLReader1 = SQLCommand.ExecuteReader()

    '    Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

    '    While SQLReader1.Read
    '        Application.DoEvents()

    '        SQL = "INSERT INTO LP_OFFERS VALUES ("
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ")"

    '        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    End While

    '    Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    '    SQLReader1.Close()
    '    SQLReader1 = Nothing
    '    SQLCommand = Nothing

    '    ' Now index and PK the table
    '    SQL = "CREATE INDEX IDX_LO_OID ON LP_OFFERS (offerID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    pgMain.Visible = False
    '    Application.DoEvents()

    'End Sub

    '' LP_STORE
    'Private Sub Build_LP_STORE()
    '    Dim SQL As String

    '    ' SQL variables
    '    Dim SQLCommand As New SQLiteCommand
    '    Dim SQLReader1 As SQLiteDataReader
    '    Dim mainSQL As String

    '    SQL = "CREATE TABLE LP_STORE ("
    '    SQL &= "OFFER_ID INTEGER NOT NULL,"
    '    SQL &= "CORP_ID INTEGER NOT NULL,"
    '    SQL &= "CORP_NAME VARCHAR(200) NOT NULL,"
    '    SQL &= "ITEM_ID INTEGER NOT NULL,"
    '    SQL &= "ITEM VARCHAR (100) NOT NULL,"
    '    SQL &= "ITEM_QUANTITY INTEGER NOT NULL,"
    '    SQL &= "LP_COST INTEGER NOT NULL,"
    '    SQL &= "ISK_COST INTEGER NOT NULL,"
    '    SQL &= "RACE_ID INTEGER NOT NULL"
    '    SQL &= ")"

    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    ' Now select the count of the final query of data

    '    ' Pull new data and insert
    '    mainSQL = "SELECT lpOffers.offerID AS OFFER_ID, lpStore.corporationID AS CORP_ID, invNames.itemName AS CORP_NAME, "
    '    mainSQL = mainSQL & "invTypes.typeID AS ITEM_ID, invTypes.typeName AS ITEM, lpOffers.quantity AS ITEM_QUANTITY, "
    '    mainSQL = mainSQL & "lpCost AS LP_COST, iskCost AS ISK_COST, chrFactions.raceIDs AS RACE_ID "
    '    mainSQL = mainSQL & "FROM invTypes, lpOffers, lpStore, invNames, crpNPCCorporations, chrFactions "
    '    mainSQL = mainSQL & "WHERE lpOffers.typeID = invTypes.typeID "
    '    mainSQL = mainSQL & "AND lpStore.corporationID = invNames.itemID "
    '    mainSQL = mainSQL & "AND lpOffers.offerID = lpStore.offerID "
    '    mainSQL = mainSQL & "AND lpStore.corporationID = crpNPCCorporations.corporationID "
    '    mainSQL = mainSQL & "AND crpNPCCorporations.factionID = chrFactions.factionID "

    '    SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
    '    SQLReader1 = SQLCommand.ExecuteReader()

    '    Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

    '    While SQLReader1.Read
    '        Application.DoEvents()

    '        SQL = "INSERT INTO LP_STORE VALUES ("
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(3)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(4)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(5)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(6)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(7)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(8)) & ")"

    '        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    End While

    '    Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    '    SQLReader1.Close()
    '    SQLReader1 = Nothing
    '    SQLCommand = Nothing

    '    ' Now index and PK the table
    '    SQL = "CREATE INDEX IDX_LS_OID ON LP_STORE (OFFER_ID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    SQL = "CREATE INDEX IDX_LS_CID ON LP_STORE (CORP_ID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    SQL = "CREATE INDEX IDX_LS_IID ON LP_STORE (ITEM_ID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBRef)

    '    pgMain.Visible = False

    '    Application.DoEvents()

    'End Sub

    '' LP_VERIFIED
    'Private Sub Build_LP_VERIFIED()
    '    Dim SQL As String

    '    ' SQL variables
    '    Dim SQLCommand As New SQLiteCommand
    '    Dim SQLReader1 As SQLiteDataReader
    '    Dim mainSQL As String

    '    SQL = "CREATE TABLE LP_VERIFIED ("
    '    SQL &= "corporationID INTEGER PRIMARY KEY,"
    '    SQL &= "verified INTEGER NOT NULL,"
    '    SQL &= "verifiedWith INTEGER"
    '    SQL &= ")"

    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    ' Now select the count of the final query of data

    '    ' Pull new data and insert
    '    mainSQL = "SELECT * FROM lpVerified"
    '    SQLCommand = New SQLiteCommand(mainSQL, SDEDB.DBRef)
    '    SQLReader1 = SQLCommand.ExecuteReader()

    '    Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

    '    While SQLReader1.Read
    '        Application.DoEvents()

    '        SQL = "INSERT INTO LP_VERIFIED VALUES ("
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(0)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(1)) & ","
    '        SQL &= BuildInsertFieldString(SQLReader1.GetValue(2)) & ")"

    '        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    End While

    '    Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    '    SQLReader1.Close()
    '    SQLReader1 = Nothing
    '    SQLCommand = Nothing

    '    ' Now index and PK the table
    '    SQL = "CREATE INDEX IDX_LO_COID ON LP_VERIFIED (corporationID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    pgMain.Visible = False
    '    Application.DoEvents()

    'End Sub

#End Region

#Region "Build SQL DB"

    ' Updates the SDE data for use
    Private Sub UpdateSDEData()

        ' Make sure we have a DB first
        If DatabaseName = "" Then
            MsgBox("Database Name not defined", vbExclamation, Application.ProductName)
            Call txtDBName.Focus()
            Exit Sub
        Else
            txtDBName.Text = DatabaseName
        End If

        Me.Cursor = Cursors.WaitCursor

        Call EnableButtons(False)

        If Not ConnectToDBs() Then
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnImageCopy.Enabled = True
            Exit Sub
        End If

        'Do all random updates here first

        ' Chinese named ships in invtypes for some reason
        Call Execute_SQLiteSQL("DELETE FROM invTypes where typeID IN (34480,34478,34476,34474,34472,34470,34468,34466,34464,34462,34460,34458)", SDEDB.DBRef)
        Call Execute_SQLiteSQL("DELETE FROM invTypes where typeID IN (34457,34459,34461,34463,34465,34467,34469,34471,34473,34475,34477,34479)", SDEDB.DBRef)

        ' If any typenames are null, look them up with ESI
        Call UpdateNullTypeNames()

        ' Update the T3 relic "blueprints" to require the relic blueprint as a material for its invention activity
        Call UpdateT3Relics()

        ' Need to insert blueprints as products for copy, ME/TE 
        Call UpdateIndustryActivityProducts()

        ' Special processing - Add a Category for 'Structure Rigs' to separate from 'Structure Modules' - use the same code just the negative
        Dim rsCheck As SQLiteDataReader
        Dim SQLCommand As SQLiteCommand

        SQLCommand = New SQLiteCommand("SELECT 'X' FROM invCategories WHERE categoryID = " & CStr(StructureRigCategory), SDEDB.DBRef)
        rsCheck = SQLCommand.ExecuteReader()
        rsCheck.Read()

        If Not rsCheck.HasRows Then
            Call Execute_SQLiteSQL(String.Format("INSERT INTO invCategories VALUES ({0},'Structure Rigs',-1,NULL)", CStr(StructureRigCategory)), SDEDB.DBRef)
        End If
        rsCheck.Close()

        ' Special processing - Update all Structure Rigs to use the new code
        Call Execute_SQLiteSQL(String.Format("UPDATE invGroups SET categoryID = {0} WHERE categoryID = 66 AND groupName LIKE '%Rig%'", CStr(StructureRigCategory)), SDEDB.DBRef)

        ' When rebuilding the DB, update the ramAssemblyLineTypeDetailPerCategory table
        ' so it is complete and not missing categories of blueprints for assembly lines in 
        ' the ramAssemblyLineTypes table - this will (should!) speed up updates for CREST facilities
        lblTableName.Text = "Updating ramAssemblyLineTypeDetailPerCategory"
        Call UpdateramAssemblyLineTypeDetailPerCategory()

        ' Add the LP Store tables from script - TODO convert to CREST Look ups
        'Call Execute_SQLiteSQL(File.OpenText(WorkingDirectory & "\lpDatabase_v0.11\lpDatabase_v0.11.sql").ReadToEnd(), SDEDB.DBRef)

        pgMain.Visible = False
        lblTableName.Text = ""

        Call CloseDBs()

        Me.Cursor = Cursors.Default
        Application.UseWaitCursor = False
        Call EnableButtons(True)

        Application.DoEvents()

    End Sub

    Public Sub UpdateNullTypeNames()
        Dim rsIDs As SQLiteDataReader
        Dim SQLCommand As SQLiteCommand
        Dim IDs As String = "["
        Dim PublicData As String
        Dim ESIPublicURL As String = "https://esi.tech.ccp.is/latest/"
        Dim TranquilityDataSource As String = "?datasource=tranquility"
        Dim ESIData As new List(Of ESINameData)

        SQLCommand = New SQLiteCommand("SELECT typeID FROM invTypes WHERE typeName IS NULL", SDEDB.DBRef)
        rsIDs = SQLCommand.ExecuteReader()
        While rsIDs.Read
            IDs &= CStr(rsIDs.GetInt32(0)) & ","
        End While
        IDs = IDs.Substring(0, Len(IDs) - 1) ' strip comma
        IDs &= "]"

        PublicData = GetPublicData(ESIPublicURL & "universe/names/" & TranquilityDataSource, IDs)

        ESIData = JsonConvert.DeserializeObject(Of List(Of ESINameData))(PublicData)

        For Each Record In ESIData
            Call Execute_SQLiteSQL(String.Format("UPDATE invTypes SET typeName = '{0}' WHERE typeID = {1}", FormatDBString(Record.name), CStr(Record.id)), SDEDB.DBRef)
        Next

    End Sub

    ''' <summary>
    ''' Queries the server for public data for the URL sent. If not found, returns nothing
    ''' </summary>
    ''' <param name="URL">Full public data URL as a string</param>
    ''' <returns>Byte Array of response or nothing if call fails</returns>
    Private Function GetPublicData(ByVal URL As String, Optional BodyData As String = "") As String
        Dim Response As String = ""
        Dim WC As New WebClient
        Dim ErrorCode As Integer = 0
        Dim ErrorResponse As String = ""

        WC.Proxy = Nothing

        Try

            If BodyData <> "" Then
                Response = Encoding.UTF8.GetString(WC.UploadData(URL, Encoding.UTF8.GetBytes(BodyData)))
            Else
                Response = WC.DownloadString(URL)
            End If

            Return Response

        Catch ex As WebException
            MsgBox("Web Request failed to get Public data. Code: " & ErrorCode & ", " & ex.Message & " - " & ErrorResponse)
        Catch ex As Exception
            If ex.Message <> "Thread was being aborted." Then
                MsgBox("The request failed to get Public data. " & ex.Message, vbInformation, Application.ProductName)
            End If
        End Try

        If Response <> "" Then
            Return Response
        Else
            Return Nothing
        End If

    End Function


    Public Class ESINameData
        <JsonProperty("category")> Public category As String '[ alliance, character, constellation, corporation, inventory_type, region, solar_system, station ]
        <JsonProperty("id")> Public id As Integer
        <JsonProperty("name")> Public name As String
    End Class

    ' Inserts a relic into the activities for itself requiring the material
    Private Sub UpdateT3Relics()

        ' Delete if any there already
        Dim SQL As String = "DELETE FROM industryActivityMaterials WHERE blueprintTypeID IN "
        SQL &= "(SELECT DISTINCT typeID FROM invTypes, invGroups WHERE categoryID = 34 AND invTypes.groupID = invGroups.groupID) "
        SQL &= "AND activityID = 8 AND blueprintTypeID = materialTypeID"

        Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        ' Now insert a record for each 
        SQL = "SELECT DISTINCT typeID FROM invTypes, invGroups WHERE categoryID = 34 AND invTypes.groupID = invGroups.groupID"
        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader

        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()
        SQLReader1.Read()

        While SQLReader1.Read
            SQL = String.Format("INSERT INTO industryActivityMaterials VALUES ({0},8,{0},1)", SQLReader1.GetInt32(0))
            Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)
        End While

    End Sub

    ' Inserts blueprints as output products for copy and ME/TE activities
    Private Sub UpdateIndustryActivityProducts()
        Dim SQL As String = "SELECT blueprintTypeID, activityID FROM industryActivities WHERE activityID NOT IN (1,8,11)" ' Manufacturing, Invention, Reactions

        Dim SQLCommand As New SQLiteCommand
        Dim SQLReader1 As SQLiteDataReader

        SQLCommand = New SQLiteCommand(SQL, SDEDB.DBRef)
        SQLReader1 = SQLCommand.ExecuteReader()

        While SQLReader1.Read
            ' Delete if it exists, then insert
            SQL = String.Format("DELETE FROM industryActivityProducts WHERE blueprintTypeID ={0} AND activityID = {1}", SQLReader1.GetValue(0), SQLReader1.GetValue(1))
            Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)

            ' Insert the blueprintTypeID as an output
            SQL = String.Format("INSERT INTO industryActivityProducts VALUES ({0},{1},{0},1,1)", SQLReader1.GetValue(0), SQLReader1.GetValue(1))
            Call Execute_SQLiteSQL(SQL, SDEDB.DBRef)

        End While

    End Sub

    ' InsertNegativeBPTypeIDRecord
    Private Sub InsertNegativeBPTypeIDRecord(BPID As Long)
        Dim mainSQL As String

        ' Look up the current record and add the negative type id so we don't get into a recursive loop for outposts
        mainSQL = "INSERT INTO invTypes SELECT typeID * -1 AS typeID, groupID, typeName, description, mass, volume, packagedVolume, capacity, portionSize, factionID, raceID, "
        mainSQL = mainSQL & "basePrice, published, marketGroupID, graphicID, radius, iconID, soundID, sofFactionName, sofMaterialSetID "
        mainSQL = mainSQL & "FROM invTypes WHERE typeID = " & BPID

        Call Execute_SQLiteSQL(mainSQL, SDEDB.DBRef)

    End Sub

#End Region

#End Region

#Region "Deploy files"

    Private Sub btnCopyFilesBuildXML_Click(sender As System.Object, e As System.EventArgs) Handles btnCopyFilesBuildXML.Click
        Call CopyFilesBuildXML()
    End Sub

    ' Copies all the files from directories and then builds the xml file and saves it here for upload to github
    Private Sub CopyFilesBuildXML()
        Dim NewFilesAdded As Boolean = False
        Dim FileDirectory As String = ""

        If chkCreateTest.Checked Then
            FileDirectory = UploadFileTestDirectory
        Else
            FileDirectory = UploadFileDirectory
        End If

        On Error Resume Next
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        Call EnableButtons(False)

        If MD5CalcFile(RootDirectory & JSONDLL) <> MD5CalcFile(FileDirectory & JSONDLL) Then
            File.Copy(RootDirectory & JSONDLL, FileDirectory & JSONDLL, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & SQLiteDLL) <> MD5CalcFile(FileDirectory & SQLiteDLL) Then
            File.Copy(RootDirectory & SQLiteDLL, FileDirectory & SQLiteDLL, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & EVEIPHEXE) <> MD5CalcFile(FileDirectory & EVEIPHEXE) Then
            File.Copy(RootDirectory & EVEIPHEXE, FileDirectory & EVEIPHEXE, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & EVEIPHUpdater) <> MD5CalcFile(FileDirectory & EVEIPHUpdater) Then
            File.Copy(RootDirectory & EVEIPHUpdater, FileDirectory & EVEIPHUpdater, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(WorkingDirectory & EVEIPHDB) <> MD5CalcFile(FileDirectory & EVEIPHDB) Then
            File.Copy(WorkingDirectory & EVEIPHDB, FileDirectory & EVEIPHDB, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(WorkingDirectory & ImageZipFile) <> MD5CalcFile(FileDirectory & ImageZipFile) Then
            File.Copy(WorkingDirectory & ImageZipFile, FileDirectory & ImageZipFile, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & UpdaterManifest) <> MD5CalcFile(FileDirectory & UpdaterManifest) Then
            File.Copy(RootDirectory & UpdaterManifest, FileDirectory & UpdaterManifest, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & EXEManifest) <> MD5CalcFile(FileDirectory & EXEManifest) Then
            File.Copy(RootDirectory & EXEManifest, FileDirectory & EXEManifest, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & MoreLinqDLL) <> MD5CalcFile(FileDirectory & MoreLinqDLL) Then
            File.Copy(RootDirectory & MoreLinqDLL, FileDirectory & MoreLinqDLL, True)
            NewFilesAdded = True
        End If

        On Error GoTo 0

        ' Output the Latest XML File 
        Call WriteLatestXMLFile()

        ' Refresh the grid
        Call LoadFileGrid()

        Me.Cursor = Cursors.Default
        Application.DoEvents()
        Call EnableButtons(True)

        MsgBox("Files Deployed", vbInformation, "Complete")

    End Sub

    ' Writes the sent settings to the sent file name
    Private Sub WriteLatestXMLFile()
        Dim VersionXMLFileName As String = ""
        Dim FileDirectory As String = ""

        ' Create XmlWriterSettings.
        Dim XMLSettings As XmlWriterSettings = New XmlWriterSettings()
        XMLSettings.Indent = True

        ' Delete and make a fresh copy
        If chkCreateTest.Checked Then
            File.Delete(LatestTestVersionXML)

            VersionXMLFileName = LatestTestVersionXML
            FileDirectory = UploadFileTestDirectory

            ' Loop through the settings sent and output each name and value
            ' Copy the new XML file into the root directory - so I don't get updates and then manually upload this to media fire so people don't get crazy updates
            Using writer As XmlWriter = XmlWriter.Create(RootDirectory & VersionXMLFileName, XMLSettings)
                writer.WriteStartDocument()
                writer.WriteStartElement("EVEIPH") ' Root.
                writer.WriteAttributeString("Version", VersionNumber)
                writer.WriteStartElement("LastUpdated")
                writer.WriteString(CStr(Now))
                writer.WriteEndElement()

                writer.WriteStartElement("result")
                writer.WriteStartElement("rowset")
                writer.WriteAttributeString("name", "filelist")
                writer.WriteAttributeString("key", "version")
                writer.WriteAttributeString("columns", "Name,Version,MD5,URL")

                ' Add each file 
                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EVEIPHEXE)
                writer.WriteAttributeString("Version", VersionNumber)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EVEIPHEXE))
                writer.WriteAttributeString("URL", TestEVEIPHEXEURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EVEIPHUpdater)
                writer.WriteAttributeString("Version", "2.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EVEIPHUpdater))
                writer.WriteAttributeString("URL", TestEVEIPHUpdaterURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EVEIPHDB)
                writer.WriteAttributeString("Version", DatabaseName)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EVEIPHDB))
                writer.WriteAttributeString("URL", TestEVEIPHDBURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", ImageZipFile)
                writer.WriteAttributeString("Version", ImagesVersion)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & ImageZipFile))
                writer.WriteAttributeString("URL", TestImageZipFileURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", JSONDLL)
                writer.WriteAttributeString("Version", FileVersionInfo.GetVersionInfo(JSONDLL).FileVersion)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & JSONDLL))
                writer.WriteAttributeString("URL", TestJSONDLLURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", SQLiteDLL)
                writer.WriteAttributeString("Version", FileVersionInfo.GetVersionInfo(SQLiteDLL).FileVersion)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & SQLiteDLL))
                writer.WriteAttributeString("URL", TestSQLiteDLLURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", UpdaterManifest)
                writer.WriteAttributeString("Version", "1.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & UpdaterManifest))
                writer.WriteAttributeString("URL", TestUpdaterManifestURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EXEManifest)
                writer.WriteAttributeString("Version", "1.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EXEManifest))
                writer.WriteAttributeString("URL", TestEXEManifestURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", MoreLinqDLL)
                writer.WriteAttributeString("Version", FileVersionInfo.GetVersionInfo(MoreLinqDLL).FileVersion)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & MoreLinqDLL))
                writer.WriteAttributeString("URL", MoreLinqDLLURL)
                writer.WriteEndElement()

                ' End document.
                writer.WriteEndDocument()
            End Using
        Else
            File.Delete(LatestVersionXML)
            VersionXMLFileName = LatestVersionXML
            FileDirectory = UploadFileDirectory

            ' Loop through the settings sent and output each name and value
            ' Copy the new XML file into the root directory - so I don't get updates and then manually upload this to media fire so people don't get crazy updates
            Using writer As XmlWriter = XmlWriter.Create(RootDirectory & VersionXMLFileName, XMLSettings)
                writer.WriteStartDocument()
                writer.WriteStartElement("EVEIPH") ' Root.
                writer.WriteAttributeString("Version", VersionNumber)
                writer.WriteStartElement("LastUpdated")
                writer.WriteString(CStr(Now))
                writer.WriteEndElement()

                writer.WriteStartElement("result")
                writer.WriteStartElement("rowset")
                writer.WriteAttributeString("name", "filelist")
                writer.WriteAttributeString("key", "version")
                writer.WriteAttributeString("columns", "Name,Version,MD5,URL")

                ' Add each file 
                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EVEIPHEXE)
                writer.WriteAttributeString("Version", VersionNumber)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EVEIPHEXE))
                writer.WriteAttributeString("URL", EVEIPHEXEURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EVEIPHUpdater)
                writer.WriteAttributeString("Version", "2.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EVEIPHUpdater))
                writer.WriteAttributeString("URL", EVEIPHUpdaterURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EVEIPHDB)
                writer.WriteAttributeString("Version", DatabaseName)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EVEIPHDB))
                writer.WriteAttributeString("URL", EVEIPHDBURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", ImageZipFile)
                writer.WriteAttributeString("Version", ImagesVersion)
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & ImageZipFile))
                writer.WriteAttributeString("URL", ImageZipFileURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", JSONDLL)
                writer.WriteAttributeString("Version", "6.03")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & JSONDLL))
                writer.WriteAttributeString("URL", JSONDLLURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", SQLiteDLL)
                writer.WriteAttributeString("Version", "1.07.9.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & SQLiteDLL))
                writer.WriteAttributeString("URL", SQLiteDLLURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", UpdaterManifest)
                writer.WriteAttributeString("Version", "1.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & UpdaterManifest))
                writer.WriteAttributeString("URL", UpdaterManifestURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", EXEManifest)
                writer.WriteAttributeString("Version", "1.0")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & EXEManifest))
                writer.WriteAttributeString("URL", EXEManifestURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", MoreLinqDLL)
                writer.WriteAttributeString("Version", "1.4")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & MoreLinqDLL))
                writer.WriteAttributeString("URL", MoreLinqDLLURL)
                writer.WriteEndElement()

                ' End document.
                writer.WriteEndDocument()
            End Using
        End If

        ' Finally, replace all the update file's crlf with lf so that when it's uploaded to git, it works properly on download
        Dim FileText As String = File.ReadAllText(RootDirectory & VersionXMLFileName)
        FileText = FileText.Replace(vbCrLf, Chr(10))

        ' Write the file back out with new formatting
        File.WriteAllText(RootDirectory & VersionXMLFileName, FileText)
        File.WriteAllText(FileDirectory & VersionXMLFileName, FileText)

    End Sub

    Private Sub btnRefreshList_Click(sender As System.Object, e As System.EventArgs) Handles btnRefreshList.Click
        ' Refresh the grid
        Call LoadFileGrid()
    End Sub

#End Region

    ' Initializes the form
    Public Sub InitalizeProcessing(ByRef LabelRef As Label, ByRef PGRef As ProgressBar, PGMaxCount As Long, FileName As String)
        LabelRef.Text = "Reading " & FileName
        Application.UseWaitCursor = True
        Application.DoEvents()

        PGRef.Value = 0
        PGRef.Maximum = PGMaxCount
        PGRef.Visible = True
    End Sub

    ' Resets the form
    Public Sub ClearProcessing(ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        PGRef.Visible = False
        LabelRef.Text = ""
        Application.UseWaitCursor = False
        Application.DoEvents()
    End Sub

    ' Increments the progressbar
    Public Sub UpdateProgress(ByRef LabelRef As Label, ByRef PGRef As ProgressBar, ByRef Count As Long, DataUpdatedText As String)
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
