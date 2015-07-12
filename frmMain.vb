
Imports System.Data.SqlClient ' For SQL Server Connection
Imports System.Data.SQLite
Imports System.IO
Imports System.Xml

Imports ComponentAce.Compression.ZipForge
' This namespace contains ArchiverException class required for error handling
Imports ComponentAce.Compression.Archiver

Public Class frmMain
    Inherits System.Windows.Forms.Form

    Private Const SettingsFileName As String = "Settings.txt"

    Private VersionNumber As String = ""

    ' Directory files and paths
    Private RootDirectory As String ' For the debugging process, will copy images here as well
    Private WorkingDirectory As String ' Where the final DB and image zip is stored 
    Private MediaFireDirectory As String ' Where all the files we want to sync to the Media fire server for download are

    ' DB
    Private DatabasePath As String ' Where we build the SQLite database
    Private FinalDBPath As String ' Final DB
    Private DatabaseName As String ' also folder name to update YAML and Universe DB stuff
    Private ImagesVersion As String ' Version of the images we have in the zip
    Private FinalDBName As String = "EVEIPH DB"

    ' Image folders
    Private IECFOlder As String
    Private EVEIPHImageFolder As String

    ' When updating the image files to build the zip, update the root directory images as well so we have the updated images for running in debug mode
    Private WorkingImageFolder As String = "Root Directory\EVEIPH Images"
    Private MissingImagesFilePath As String

    ' For saving and scanning the github folder for updates - this folder is in the deployment folder (same as installer and binary)
    Private FinalBinaryFolder As String = "EVEIPH\"
    Private FinalBinaryZip As String = "EVEIPH v" & VersionNumber & " Binaries.zip"

    ' File names
    Private MSIInstaller As String = "Eve Isk per Hour " & VersionNumber & ".msi"

    Private ZipForgeDLL As String = "ZipForge.dll"
    Private JSONDLL As String = "Newtonsoft.Json.dll"
    Private SQLiteDLL As String = "System.Data.SQLite.dll"
    Private EVEIPHEXE As String = "EVE Isk per Hour.exe"
    Private EVEIPHUpdater As String = "EVEIPH Updater.exe"
    Private EVEIPHDB As String = "EVEIPH DB.s3db"
    Private UpdaterManifest As String = "EVEIPH Updater.exe.manifest"
    Private EXEManifest As String = "EVE Isk per Hour.exe.manifest"
    Private ImageZipFile As String = "EVEIPH Images.zip"
    Private LatestVersionXML As String ' Set with scan for text.txt

    ' File URLs
    Private ZipForgeDLLURL As String = "http://www.mediafire.com/download/va7avejtnl5boto/ZipForge.dll"
    Private JSONDLLURL As String = "http://www.mediafire.com/download/7a6ml9gwu14616d/Newtonsoft.Json.dll"
    Private SQLiteDLLURL As String = "http://www.mediafire.com/download/b0px46xwaa8jgx4/System.Data.SQLite.dll"
    Private EVEIPHEXEURL As String = "http://www.mediafire.com/download/2ckpd2th8xlpysv/EVE_Isk_per_Hour.exe"
    Private EVEIPHUpdaterURL As String = "http://www.mediafire.com/download/r9innfrf287mnd7/EVEIPH_Updater.exe"
    Private EVEIPHDBURL As String = "http://www.mediafire.com/download/cfylxmlq6v8i26c/EVEIPH_DB.s3db"
    Private UpdaterManifestURL As String = "http://www.mediafire.com/download/c149x7vcf1gab2p/EVEIPH_Updater.exe.manifest"
    Private EXEManifestURL As String = "http://www.mediafire.com/download/sdlrk28t18gv8z0/EVE_Isk_per_Hour.exe.manifest"
    Private ImageZipFileURL As String = "http://www.mediafire.com/download/duq6nw4d0p59rci/EVEIPH_Images.zip"

    ' YAML files
    Private Const YAMLBlueprints As String = "blueprints.yaml"
    Private Const YAMLCertificates As String = "certificates.yaml"
    Private Const YAMLGraphicIDs As String = "graphicIDs.yaml"
    Private Const YAMLiconIDs As String = "iconIDs.yaml" ' Old eveIcons
    Private Const YAMLtypeIDs As String = "typeIDs.yaml"
    Private Const YAMLgroupIDs As String = "groupIDs.yaml"
    Private Const YAMLcategoryIDs As String = "categoryIDs.yaml"

    Private Const SequenceLabel As String = "Sequence"
    Private Const Null As String = "null"
    Private Const ANY_NUMBER As String = "ANY_NUMBER"
    Private ParentNode As String = ""

    Private Const AssemblyArraysTable As String = "ASSEMBLY_ARRAYS"
    Private Const StationFacilitiesTable As String = "STATION_FACILITIES"

    Private SQLExpressConnection As SqlConnection
    Private SQLExpressConnection2 As SqlConnection ' For updating while another connection is open
    Private SQLExpressConnection3 As SqlConnection ' For updating while another connection is open
    Private SQLExpressConnectionExecute As SqlConnection ' For updating while another connection is open
    Private SQLExpressProgressBar As SqlConnection

    Private SQLiteDB As New SQLiteConnection
    Private UniverseDB As New SQLiteConnection

    Private INDENT As String = ""
    Private Const COLON As String = ":"
    Private Const COLON_SPACE As String = ": "
    Private Const BLOCK_2SEQUENCE As String = "- "
    Private Const BLOCK_4SEQUENCE As String = "-   "

    Private Const ASCII_Quote_Code As Integer = 39
    Private Const ASCII_DoubleQuote_Code As Integer = 34

    Private TestBuild As Boolean
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
        lstFileInformation.Columns.Add("File Name", 250, HorizontalAlignment.Left)
        lstFileInformation.Columns.Add("File Date/Time", 150, HorizontalAlignment.Left)

        Call LoadFileGrid()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        SQLExpressConnection.Close()
        SQLExpressConnection2.Close()
        SQLExpressConnection3.Close()
        SQLExpressProgressBar.Close()
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
            MediaFireDirectory = BPStream.ReadLine
            If Not Directory.Exists(MediaFireDirectory) Then
                MediaFireDirectory = ""
            End If

            If Not IsNothing(VersionNumber) Then
                ' Set these if we have a version number
                FinalBinaryZip = "EVEIPH v" & VersionNumber & " Binaries.zip"

                ' File names
                MSIInstaller = "Eve Isk per Hour " & VersionNumber & ".msi"
            Else
                FinalBinaryZip = "EVEIPH v3.1 Binaries.zip"
                MSIInstaller = "Eve Isk per Hour 3.1.msi"
            End If

            BPStream.Close()
        Else
            DatabaseName = ""
            ImagesVersion = ""
            RootDirectory = ""
            WorkingDirectory = ""
            MediaFireDirectory = ""
            VersionNumber = ""
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

        If MediaFireDirectory <> "" Then
            If MediaFireDirectory.Substring(Len(MediaFireDirectory) - 1) <> "\" Then
                MediaFireDirectory = MediaFireDirectory & "\"
            End If
        End If

        WorkingImageFolder = WorkingDirectory
        DatabasePath = WorkingDirectory & DatabaseName
        FinalDBPath = WorkingDirectory & FinalDBName

        txtDBName.Text = DatabaseName
        txtImageVersion.Text = ImagesVersion
        lblDBNameDisplay.Text = DatabaseName
        txtVersionNumber.Text = VersionNumber

        If WorkingDirectory <> "\" Then
            lblWorkingFolderPath.Text = WorkingDirectory
        End If

        If MediaFireDirectory <> "\" Then
            lblMediaFirePath.Text = MediaFireDirectory
        End If

        If RootDirectory <> "\" Then
            lblRootDebugFolderPath.Text = RootDirectory
        End If

        If File.Exists(RootDirectory & "Test.txt") Then
            TestBuild = True
            LatestVersionXML = "LatestVersionIPH Test.xml"
        Else
            TestBuild = False
            LatestVersionXML = "LatestVersionIPH.xml"
        End If

        ' When updating the image files to build the zip, update the root directory images as well so we have the updated images for running in debug mode
        WorkingImageFolder = RootDirectory & "EVEIPH Images"

        IECFOlder = WorkingDirectory & "Types"
        EVEIPHImageFolder = WorkingDirectory & "EVEIPH Images"
        MissingImagesFilePath = WorkingDirectory & DatabaseName & " Missing Images.txt"

    End Sub

    Private Sub SetProgressBarValues(ByVal TableName As String)

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim mySQLReader1 As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        ' Now select the count of the final query of data
        msSQL = "SELECT COUNT(*) FROM " & TableName
        msSQLQuery = New SqlCommand(msSQL, SQLExpressProgressBar)
        mySQLReader1 = msSQLQuery.ExecuteReader()
        mySQLReader1.Read()
        pgMain.Maximum = mySQLReader1.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader1.Close()
        msSQLQuery = Nothing

    End Sub

    Private Sub btnSelectInstallerBinaryPath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectInstallerBinaryPath.Click
        If MediaFireDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = MediaFireDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblMediaFirePath.Text = FolderBrowserDialog.SelectedPath
                MediaFireDirectory = FolderBrowserDialog.SelectedPath
                Call SetFilePaths()
            Catch ex As Exception
                MsgBox(Err.Description, vbExclamation, Application.ProductName)
            End Try
        End If
    End Sub

    Private Sub btnSelectDBImagesPath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectDBImagesPath.Click
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
            TabPage2.Select()
            txtDBName.Focus()
            Exit Sub
        End If

        If Trim(lblMediaFirePath.Text) = "" Then
            MsgBox("Invalid Installer/Binary file path", vbExclamation, Application.ProductName)
            TabPage2.Select()
            Exit Sub
        End If

        If Trim(lblWorkingFolderPath.Text) = "" Then
            MsgBox("Invalid Images file path", vbExclamation, Application.ProductName)
            TabPage2.Select()
            Exit Sub
        End If

        If Trim(lblRootDebugFolderPath.Text) = "" Then
            MsgBox("Invalid Root/Debug file path", vbExclamation, Application.ProductName)
            TabPage2.Select()
            Exit Sub
        End If

        If Trim(txtVersionNumber.Text) = "" Then
            MsgBox("Invalid version number", vbExclamation, Application.ProductName)
            TabPage2.Select()
            txtVersionNumber.Focus()
            Exit Sub
        End If

        If Trim(txtImageVersion.Text) = "" Then
            MsgBox("Invalid Images Version number", vbExclamation, Application.ProductName)
            TabPage2.Select()
            txtImageVersion.Focus()
            Exit Sub
        End If

        DatabaseName = txtDBName.Text
        ImagesVersion = txtVersionNumber.Text
        lblDBNameDisplay.Text = DatabaseName
        VersionNumber = txtVersionNumber.Text

        RootDirectory = lblRootDebugFolderPath.Text
        WorkingDirectory = lblWorkingFolderPath.Text
        MediaFireDirectory = lblMediaFirePath.Text

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
        MyStream.Write(lblMediaFirePath.Text & Environment.NewLine)

        MyStream.Flush()
        MyStream.Close()

        ' Reload this incase the folder changed
        Call LoadFileGrid()

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

        If MediaFireDirectory <> "" Then
            Dim di As New DirectoryInfo(MediaFireDirectory)
            Dim fiArr As FileInfo() = di.GetFiles()

            ' Reset
            FileList = New List(Of FileNameDate)

            ' Add the names of the files.
            Dim fri As FileInfo
            For Each fri In fiArr
                TempFile.FileDate = fri.LastWriteTime
                TempFile.FileName = fri.Name
                FileList.Add(TempFile)
            Next fri

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
        Dim i As Long
        Dim SQL As String
        Dim readerBPs As SQLite.SQLiteDataReader
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
        btnBuildDatabase.Enabled = False
        btnBuildSQLServerDB.Enabled = False
        btnImageCopy.Enabled = False

        If Not ConnectToDBs() Then
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnBuildSQLServerDB.Enabled = True
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
        If File.Exists(MissingImagesFilePath) Then
            File.Delete(MissingImagesFilePath)
        End If

        Dim OutputFile As New StreamWriter(MissingImagesFilePath)
        OutputFile.WriteLine("Blueprint ID - Blueprint Name")
        MissingImages = False

        ' Get the count first
        SQL = "SELECT COUNT(*) FROM ALL_BLUEPRINTS"
        DBCommand = New SQLiteCommand(SQL, SQLiteDB)
        readerBPs = DBCommand.ExecuteReader
        readerBPs.Read()
        ReaderCount = readerBPs.GetValue(0)
        readerBPs.Close()

        ' Get all the BP ID numbers we use in the program and copy those files to the directory
        SQL = "SELECT BLUEPRINT_ID, BLUEPRINT_NAME FROM ALL_BLUEPRINTS"
        DBCommand = New SQLiteCommand(SQL, SQLiteDB)
        readerBPs = DBCommand.ExecuteReader

        i = 0
        pgMain.Value = i
        pgMain.Maximum = ReaderCount
        pgMain.Visible = True

        Application.DoEvents()

        ' Loop through and copy all the images to the new folder
        While readerBPs.Read
            Application.DoEvents()
            Try
                ' For zip use
                File.Copy(IECFOlder & "\" & readerBPs(0).ToString & "_64.png", EVEIPHImageFolder & "\" & CStr(readerBPs.GetValue(0)) & "_64.png")
                ' To root Working Directory
                File.Copy(IECFOlder & "\" & readerBPs(0).ToString & "_64.png", WorkingImageFolder & "\" & CStr(readerBPs.GetValue(0)) & "_64.png")
            Catch
                ' Build a file with the BP ID's and Names that do not have a image
                OutputFile.WriteLine(readerBPs(0).ToString & " - " & readerBPs(1).ToString)
                MissingImages = True
            End Try
            i += 1
            pgMain.Value = i
        End While

        ' Final images
        File.Copy(IECFOlder & "\4276_32.png", EVEIPHImageFolder & "\4276_32.png") ' T2 Mining Ganglink image
        File.Copy(IECFOlder & "\4276_32.png", WorkingImageFolder & "\4276_32.png") ' T2 Mining Ganglink image
        File.Copy(IECFOlder & "\22557_32.png", EVEIPHImageFolder & "\22557_32.png") ' T1 Mining Ganglink image
        File.Copy(IECFOlder & "\22557_32.png", WorkingImageFolder & "\22557_32.png") ' T1 Mining Ganglink image
        File.Copy(IECFOlder & "\32880_64.png", EVEIPHImageFolder & "\32880_64.png")    ' Ore Mining Frig
        File.Copy(IECFOlder & "\32880_64.png", WorkingImageFolder & "\32880_64.png")   ' Ore Mining Frig
        File.Copy(IECFOlder & "\17476_64.png", EVEIPHImageFolder & "\17476_64.png")    ' Covetor
        File.Copy(IECFOlder & "\17476_64.png", WorkingImageFolder & "\17476_64.png")   ' Covetor
        File.Copy(IECFOlder & "\17478_64.png", EVEIPHImageFolder & "\17478_64.png")    ' Retriever
        File.Copy(IECFOlder & "\17478_64.png", WorkingImageFolder & "\17478_64.png")   ' Retriever
        File.Copy(IECFOlder & "\22544_64.png", EVEIPHImageFolder & "\22544_64.png")    ' Hulk
        File.Copy(IECFOlder & "\22544_64.png", WorkingImageFolder & "\22544_64.png")   ' Hulk
        File.Copy(IECFOlder & "\22546_64.png", EVEIPHImageFolder & "\22546_64.png")    ' Skiff
        File.Copy(IECFOlder & "\22546_64.png", WorkingImageFolder & "\22546_64.png")   ' Skiff
        File.Copy(IECFOlder & "\22548_64.png", EVEIPHImageFolder & "\22548_64.png")    ' Mackinaw
        File.Copy(IECFOlder & "\22548_64.png", WorkingImageFolder & "\22548_64.png")   ' Mackinaw
        File.Copy(IECFOlder & "\28352_64.png", EVEIPHImageFolder & "\28352_64.png")    ' Rorqual
        File.Copy(IECFOlder & "\28352_64.png", WorkingImageFolder & "\28352_64.png")   ' Rorqual
        File.Copy(IECFOlder & "\28606_64.png", EVEIPHImageFolder & "\28606_64.png")    ' Orca
        File.Copy(IECFOlder & "\28606_64.png", WorkingImageFolder & "\28606_64.png")   ' Orca
        File.Copy(IECFOlder & "\17480_64.png", EVEIPHImageFolder & "\17480_64.png")    ' Procurer
        File.Copy(IECFOlder & "\17480_64.png", WorkingImageFolder & "\17480_64.png")   ' Procurer
        File.Copy(IECFOlder & "\24698_64.png", EVEIPHImageFolder & "\24698_64.png")    ' Drake
        File.Copy(IECFOlder & "\24698_64.png", WorkingImageFolder & "\24698_64.png")   ' Drake
        File.Copy(IECFOlder & "\24688_64.png", EVEIPHImageFolder & "\24688_64.png")    ' Rokh
        File.Copy(IECFOlder & "\24688_64.png", WorkingImageFolder & "\24688_64.png")   ' Rokh
        File.Copy(IECFOlder & "\33697_64.png", EVEIPHImageFolder & "\33697_64.png")    ' Prospect
        File.Copy(IECFOlder & "\33697_64.png", WorkingImageFolder & "\33697_64.png")   ' Prospect

        ' Now Zip the images
        ' Create an instance of the ZipForge class
        Dim archiver As New ZipForge()

        If File.Exists(DatabasePath & "EVEIPH Images.zip") Then
            File.Delete(DatabasePath & "EVEIPH Images.zip")
        End If

        ' Set the name of the archive file we want to create
        archiver.FileName = WorkingDirectory & "EVEIPH Images.zip"
        ' Because we create a new archive, 
        ' we set fileMode to System.IO.FileMode.Create
        archiver.OpenArchive(System.IO.FileMode.Create)
        ' Set base (default) directory for all archive operations
        archiver.BaseDir = EVEIPHImageFolder
        ' Add files to the archive by mask
        archiver.AddFiles("*.*")
        archiver.CloseArchive()

        OutputFile.Close()
        pgMain.Visible = False

        ' If we didn't output any missing images, delete the output fille
        If Not MissingImages Then
            File.Delete(MissingImagesFilePath)
        End If

        ' Leave working folder for use with binary builder

        Call CloseDBs()

        Me.Cursor = Cursors.Default
        btnBuildDatabase.Enabled = True
        btnBuildSQLServerDB.Enabled = True
        btnImageCopy.Enabled = True

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

        ' Make folder to put files in and zip
        If Directory.Exists(FinalBinaryFolderPath) Then
            Directory.Delete(FinalBinaryFolderPath, True)
        End If

        If TestBuild Then
            ' Copy the test.txt to the binary
            File.Copy(RootDirectory & "Test.txt", FinalBinaryFolderPath & "Test.txt")
        End If

        Directory.CreateDirectory(FinalBinaryFolderPath)

        ' Copy all these files from the media file directory (should be most up to date) to the working directory to make the zip
        File.Copy(MediaFireDirectory & ZipForgeDLL, FinalBinaryFolderPath & ZipForgeDLL)
        File.Copy(MediaFireDirectory & JSONDLL, FinalBinaryFolderPath & JSONDLL)
        File.Copy(MediaFireDirectory & SQLiteDLL, FinalBinaryFolderPath & SQLiteDLL)
        File.Copy(MediaFireDirectory & EVEIPHEXE, FinalBinaryFolderPath & EVEIPHEXE)
        File.Copy(MediaFireDirectory & EVEIPHUpdater, FinalBinaryFolderPath & EVEIPHUpdater)
        File.Copy(MediaFireDirectory & UpdaterManifest, FinalBinaryFolderPath & UpdaterManifest)
        File.Copy(MediaFireDirectory & EXEManifest, FinalBinaryFolderPath & EXEManifest)
        File.Copy(MediaFireDirectory & LatestVersionXML, FinalBinaryFolderPath & LatestVersionXML)

        ' DB
        File.Copy(WorkingDirectory & EVEIPHDB, FinalBinaryFolderPath & EVEIPHDB)

        ' IPH images
        My.Computer.FileSystem.CopyDirectory(WorkingDirectory & ImageFolder, FinalBinaryFolderPath & ImageFolder, True)

        ' Zip the whole folder up for download
        ' Create an instance of the ZipForge class
        Dim archiver As New ZipForge()

        If File.Exists(FinalBinaryZipPath) Then
            File.Delete(FinalBinaryZipPath)
        End If

        ' Set the name of the archive file we want to create
        archiver.FileName = FinalBinaryZipPath
        ' Because we create a new archive, 
        ' we set fileMode to System.IO.FileMode.Create
        archiver.OpenArchive(System.IO.FileMode.Create)
        ' Set base (default) directory for all archive operations
        archiver.BaseDir = FinalBinaryFolderPath
        ' Add files to the archive by mask
        archiver.AddFiles("*.*")
        archiver.CloseArchive()

        If File.Exists(MediaFireDirectory & FinalBinaryZip) Then
            File.Delete(MediaFireDirectory & FinalBinaryZip)
        End If

        ' Copy binary zip file to the media file directory
        File.Copy(FinalBinaryZipPath, MediaFireDirectory & FinalBinaryZip)

        Application.UseWaitCursor = False
        Application.DoEvents()

        ' Clean up working folder
        If Directory.Exists(FinalBinaryFolderPath) Then
            Directory.Delete(FinalBinaryFolderPath, True)
        End If

        ' Refresh this file in the list
        Call LoadFileGrid()

        MsgBox("Binary Build", vbInformation, "Complete")
        btnBuildBinary.Enabled = True

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

        btnBuildDatabase.Enabled = False
        btnBuildSQLServerDB.Enabled = False
        btnImageCopy.Enabled = False

        ' Build DB's and open connections
        Call BuildDB()

        If Not ConnectToDBs() Then
            Try
                ' Delete old one
                File.Delete(DatabasePath & ".s3db")
            Catch
                ' Nothing
            End Try
            lblTableName.Text = ""
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnBuildSQLServerDB.Enabled = True
            btnImageCopy.Enabled = True
            ' Done
            Exit Sub
        End If

        Call BuildEVEDatabase()

        lblTableName.Text = ""
        Me.Cursor = Cursors.Default
        btnBuildDatabase.Enabled = True
        btnBuildSQLServerDB.Enabled = True
        btnImageCopy.Enabled = True

        Call CloseDBs()

        Application.DoEvents()
        Call MsgBox("Database Created", vbInformation, "Complete")

    End Sub

    Private Sub BuildDB()

        ' Check for SQLite DB
        If File.Exists(FinalDBPath & ".s3db") Then
            Try
                SQLiteDB.Close()
            Catch
                ' Nothing
            End Try
            ' Delete old one
            File.Delete(FinalDBPath & ".s3db")
        End If

        ' Create new SQLite DB
        SQLiteConnection.CreateFile(FinalDBPath & ".s3db")

    End Sub

    Private Function ConnectToDBs() As Boolean
        Application.DoEvents()
        Me.Cursor = Cursors.WaitCursor

        Try
            ' Open connections
            SQLExpressConnection = New SqlConnection("Server=" & Environment.MachineName & "\EVESDE;Database=" & DatabaseName & ";Trusted_Connection=True; Connection Timeout=300;")
            SQLExpressConnection.Open()
            SQLExpressConnection2 = New SqlConnection("Server=" & Environment.MachineName & "\EVESDE;Database=" & DatabaseName & ";Trusted_Connection=True; Connection Timeout=300;")
            SQLExpressConnection2.Open()
            SQLExpressConnection3 = New SqlConnection("Server=" & Environment.MachineName & "\EVESDE;Database=" & DatabaseName & ";Trusted_Connection=True; Connection Timeout=300;")
            SQLExpressConnection3.Open()
            SQLExpressProgressBar = New SqlConnection("Server=" & Environment.MachineName & "\EVESDE;Database=" & DatabaseName & ";Trusted_Connection=True; Connection Timeout=300;")
            SQLExpressProgressBar.Open()
            SQLExpressConnectionExecute = New SqlConnection("Server=" & Environment.MachineName & "\EVESDE;Database=" & DatabaseName & ";Trusted_Connection=True; Connection Timeout=300;")
            SQLExpressConnectionExecute.Open()

            ' SQL Lite DB
            If File.Exists(FinalDBPath & ".s3db") Then
                SQLiteDB.ConnectionString = "Data Source=" & FinalDBPath & ".s3db"
                SQLiteDB.Open()
                ' Set pragma to make this faster
                Call Execute_SQLiteSQL("PRAGMA synchronous = OFF", SQLiteDB)
            End If

            ' SQL Lite DB for new universe data
            If File.Exists(DatabasePath & "\universeDataDx.db") Then
                UniverseDB.ConnectionString = "Data Source=" & DatabasePath & "\universeDataDx.db"
                UniverseDB.Open()
            End If

            btnBuildDatabase.Focus()
            Me.Cursor = Cursors.Default
            Return True
        Catch ex As Exception
            MsgBox(Err.Description, vbExclamation, Application.ProductName)
            Return False
        End Try

    End Function

    Private Sub CloseDBs()
        On Error Resume Next
        SQLExpressConnection.Close()
        SQLExpressConnection2.Close()
        SQLExpressConnection3.Close()
        SQLExpressProgressBar.Close()
        SQLExpressConnectionExecute.Close()
        SQLiteDB.Close()
        UniverseDB.Close()
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

        Call UpdateramAssemblyLineTypeDetailPerCategory()

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

        lblTableName.Text = "Building: ASSEMBLY_ARRAYS"
        Call Build_ASSEMBLY_ARRAYS()

        lblTableName.Text = "Building: STATION_FACILITIES"
        Call Build_STATION_FACILITIES()

        lblTableName.Text = "Building: STATIONS"
        Call Build_Stations()

        lblTableName.Text = "Building: REGIONS"
        Call Build_REGIONS()

        lblTableName.Text = "Building: CONSTELLATIONS"
        Call Build_CONSTELLATIONS()

        lblTableName.Text = "Building: SOLAR_SYSTEMS"
        Call Build_SOLAR_SYSTEMS()

        lblTableName.Text = "Building: ITEM_PRICES"
        Call Build_ITEM_PRICES()

        lblTableName.Text = "Building: MARKET_HISTORY"
        Call Build_MARKET_HISTORY()

        lblTableName.Text = "Building: MARKET_HISTORY_UPDATE_CACHE"
        Call Build_MARKET_HISTORY_UPDATE_CACHE()

        lblTableName.Text = "Building: INVENTORY_FLAGS"
        Call Build_Inventory_Flags()

        lblTableName.Text = "Building: CREST_CACHE_DATES"
        Call Build_CREST_CACHE_DATES()

        lblTableName.Text = "Building: INDUSTRY_SYSTEMS_COST_INDICIES"
        Call Build_INDUSTRY_SYSTEMS_COST_INDICIES()

        lblTableName.Text = "Building: INDUSTRY_TEAMS_AUCTIONS"
        Call Build_INDUSTRY_TEAMS_AUCTIONS()

        lblTableName.Text = "Building: INDUSTRY_TEAMS"
        Call Build_INDUSTRY_TEAMS()

        lblTableName.Text = "Building: INDUSTRY_SPECIALTIES"
        Call Build_INDUSTRY_SPECIALTIES()

        lblTableName.Text = "Building: INDUSTRY_FACILITIES"
        Call Build_INDUSTRY_FACILITIES()

        lblTableName.Text = "Building: RACE_IDS"
        Call Build_RACE_IDS()

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

        lblTableName.Text = "Building: CHARACTER_API"
        Call Build_API()

        lblTableName.Text = "Building: CHARACTER_STANDINGS"
        Call Build_Character_Standings()

        lblTableName.Text = "Building: OWNED_BLUEPRINTS"
        Call Build_OWNED_BLUEPRINTS()

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

        lblTableName.Text = "Building: ORES_LOCATIONS"
        Call Build_ORE_LOCATIONS()

        lblTableName.Text = "Building: ORES"
        Call Build_ORES()

        lblTableName.Text = "Building: REPROCESSING"
        Call Build_Reprocessing()

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

        lblTableName.Text = "Building: EMD_ITEM_PRICE_HISTORY"
        Call Build_EMD_Item_Price_History()

        lblTableName.Text = "Building: EMD_UPDATE_HISTORY"
        Call Build_EMD_Update_History()

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

        ' After we are done with everything, use the following tables to update the RACE ID value in the ALL_BLUEPRINTS table
        lblTableName.Text = "Updating the Race ID's"

        ' Set null to zero
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 0 WHERE RACE_ID IS NULL "
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' 1 = Caldari
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 1 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Caldari Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Caldari Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Caldari' OR BLUEPRINT_GROUP IN ('Missile Blueprint','Missile Launcher Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Caldari%'  OR BLUEPRINT_NAME LIKE 'Caldari%' "
        SQL = SQL & "AND RACE_ID = 0 "
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' 2 = Minmatar
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 2 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Minmatar Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Minmatar Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Minmatar' OR BLUEPRINT_GROUP IN ('Projectile Ammo Blueprint','Projectile Weapon Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Republic%'  OR BLUEPRINT_NAME LIKE 'Minmatar%' "
        SQL = SQL & "AND RACE_ID = 0 "
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' 4 = Amarr
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 4 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Amarr Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Amarr Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Amarr' OR BLUEPRINT_GROUP IN ('Energy Weapon Blueprint','Frequency Crystal Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Ammatar%' OR BLUEPRINT_NAME LIKE 'Imperial Navy%' OR BLUEPRINT_NAME LIKE 'Khanid Navy%' OR BLUEPRINT_NAME LIKE 'Amarr%' "
        SQL = SQL & "AND RACE_ID = 0"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' 8 = Gallente
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 8 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Gallente Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Gallente Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Gallente' OR BLUEPRINT_GROUP IN ('Hybrid Charge Blueprint','Hybrid Weapon Blueprint', 'Capacitor Booster Charge Blueprint', 'Bomb Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Federation%' OR BLUEPRINT_NAME LIKE 'Gallente%' "
        SQL = SQL & "AND RACE_ID = 0"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 15 WHERE MARKET_GROUP = 'Pirate Faction' OR RACE_ID > 15"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 15 WHERE BLUEPRINT_NAME LIKE 'Serpentis%' OR BLUEPRINT_NAME LIKE 'Angel%' OR BLUEPRINT_NAME LIKE 'Blood%'"
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Domination%' OR BLUEPRINT_NAME LIKE 'Dread Guristas%' OR BLUEPRINT_NAME LIKE 'Guristas%' "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'True Sansha%' OR BLUEPRINT_NAME LIKE 'Sansha%' OR BLUEPRINT_NAME LIKE 'Shadow%' OR BLUEPRINT_NAME LIKE 'Dark Blood%'"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Set all the structures now that are zero
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 1 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Missile Sentry')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 2 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Projectile Sentry')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 4 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Laser Sentry')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 8 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Hybrid Sentry')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Update any remaining by blueprint group
        SQL = "SELECT DISTINCT BLUEPRINT_GROUP, RACE_ID FROM ALL_BLUEPRINTS WHERE RACE_ID <> 0 "
        SQLiteDBCommand = New SQLiteCommand(SQL, SQLiteDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        While SQLiteReader.Read
            SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = " & SQLiteReader.GetInt32(1) & " "
            SQL = SQL & "WHERE BLUEPRINT_GROUP = '" & SQLiteReader.GetString(0) & "' "
            SQL = SQL & "AND RACE_ID = 0 AND ITEM_CATEGORY IN ('Module', 'Drone')"
            Call Execute_SQLiteSQL(SQL, SQLiteDB)
        End While

        ' Station Parts should be 'Other'
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 0 WHERE ITEM_GROUP_ID = 536 "
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Fix for Pheobe SDE issues - These were removed from game but haven't been deleted from SDE
        SQL = "DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL_CATEGORY = 'Decryptors'"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE BLUEPRINT_NAME LIKE '%Data Interface Blueprint'"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "DELETE FROM ALL_BLUEPRINTS WHERE ITEM_GROUP = 'Data Interfaces'"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        lblTableName.Text = "Finalizing..."
        Application.DoEvents()

        ' Run a vacuum on the new SQL DB
        Call Execute_SQLiteSQL("VACUUM;", SQLiteDB)

    End Sub

    ' ALL_BLUEPRINTS
    Private Sub Build_ALL_BLUEPRINTS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Long

        ' See if the table exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.tables where name = 'ALL_BLUEPRINTS'"
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE ALL_BLUEPRINTS"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Build ALL_BLUEPRINTS from this query
        SQL = "SELECT industryBlueprints.blueprintTypeID AS BLUEPRINT_ID, "
        SQL = SQL & "invTypes1.typeName AS BLUEPRINT_NAME, "
        SQL = SQL & "invGroups1.groupName AS  BLUEPRINT_GROUP, "
        SQL = SQL & "invTypes.typeID AS ITEM_ID, "
        SQL = SQL & "invTypes.typeName AS ITEM_NAME, "
        SQL = SQL & "invGroups.groupID AS ITEM_GROUP_ID, "
        SQL = SQL & "invGroups.groupName AS ITEM_GROUP, "
        SQL = SQL & "invCategories.categoryID AS ITEM_CATEGORY_ID, "
        SQL = SQL & "invCategories.categoryName AS ITEM_CATEGORY, "
        SQL = SQL & "invMarketGroups.marketGroupID AS MARKET_GROUP_ID, "
        SQL = SQL & "invMarketGroups.marketGroupName AS MARKET_GROUP, "
        SQL = SQL & "0 AS TECH_LEVEL, "
        SQL = SQL & "invTypes.portionSize AS PORTION_SIZE, "
        SQL = SQL & "industryActivities.time AS BASE_PRODUCTION_TIME, "
        SQL = SQL & "IA1.time AS BASE_RESEARCH_TL_TIME, "
        SQL = SQL & "IA2.time AS BASE_RESEARCH_ML_TIME, "
        SQL = SQL & "IA3.time AS BASE_COPY_TIME, "
        SQL = SQL & "IA4.time AS BASE_INVENTION_TIME, "
        SQL = SQL & "industryBlueprints.maxProductionLimit AS MAX_PRODUCTION_LIMIT, "
        SQL = SQL & "isNULL(dgmTypeAttributes.valueInt,dgmTypeAttributes.valueFloat) AS ITEM_TYPE, "
        SQL = SQL & "invTypes.raceID AS RACE_ID, "
        SQL = SQL & "invMetaTypes.metaGroupID AS META_GROUP, "
        SQL = SQL & "'  ' AS SIZE_GROUP, "
        SQL = SQL & "0 AS IGNORE "
        SQL = SQL & "INTO ALL_BLUEPRINTS "
        SQL = SQL & "FROM invTypes "
        SQL = SQL & "LEFT JOIN invMarketGroups ON invTypes.marketGroupID = invMarketGroups.marketGroupID "
        SQL = SQL & "LEFT JOIN dgmTypeAttributes ON invTypes.typeID = dgmTypeAttributes.typeID AND attributeID = 633 "
        SQL = SQL & "LEFT JOIN invMetaTypes ON invTypes.typeID = invMetaTypes.typeID, "
        SQL = SQL & "invTypes AS invTypes1, invGroups, invGroups AS invGroups1, invCategories, "
        SQL = SQL & "industryActivityProducts, industryActivities, "
        SQL = SQL & "industryBlueprints "
        SQL = SQL & "LEFT JOIN industryActivities AS IA1 ON industryBlueprints.blueprintTypeID = IA1.blueprintTypeID AND IA1.activityID = 3 " ' -- Research TL time
        SQL = SQL & "LEFT JOIN industryActivities AS IA2 ON industryBlueprints.blueprintTypeID = IA2.blueprintTypeID AND IA2.activityID = 4 " ' -- Research ML time
        SQL = SQL & "LEFT JOIN industryActivities AS IA3 ON industryBlueprints.blueprintTypeID = IA3.blueprintTypeID AND IA3.activityID = 5 " ' -- Copy time
        SQL = SQL & "LEFT JOIN industryActivities AS IA4 ON industryBlueprints.blueprintTypeID = IA4.blueprintTypeID AND IA4.activityID = 8 " ' -- Invention time
        SQL = SQL & "WHERE industryActivityProducts.activityID = 1 " ' -- only bps we can build
        SQL = SQL & "AND industryBlueprints.blueprintTypeID = industryActivityProducts.blueprintTypeID "
        SQL = SQL & "AND invTypes1.typeID = industryBlueprints.blueprintTypeID "
        SQL = SQL & "AND invTypes1.groupID = invGroups1.groupID "
        SQL = SQL & "AND invTypes.typeID = industryActivityProducts.productTypeID "
        SQL = SQL & "AND invTypes.groupID = invGroups.groupID "
        SQL = SQL & "AND invGroups.categoryID = invCategories.categoryID "
        SQL = SQL & "AND industryBlueprints.blueprintTypeID = industryActivities.blueprintTypeID "
        SQL = SQL & "AND industryActivities.activityID = 1 " ' -- Production Time 
        SQL = SQL & "AND invTypes1.published <> 0 AND invTypes.published <> 0 AND invGroups1.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0" ' -- 2830 bps

        ' Build table
        Call Execute_msSQL(SQL)

        ' Now that ALL_BLUEPRINTS is created, do some updates to the data before the main query

        '***** TO CHECK LATER *****
        ' Set the tech level of the BPs first by looking at the item type from the query
        ' This is not ideal but meta 5 items are T2; Tengu/Legion/Proteus/Loki items are T3, and all others are T1
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 2, ITEM_TYPE = 2 WHERE ITEM_TYPE = 5"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 3 WHERE ITEM_CATEGORY = 'Subsystem' OR ITEM_GROUP = 'Strategic Cruiser' OR ITEM_GROUP = 'Tactical Destroyer'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 1 WHERE TECH_LEVEL = 0" ' Anything not updated should be a 0
        Execute_msSQL(SQL)

        ' Tech's first
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1 "
        SQL = SQL & "WHERE (TECH_LEVEL=3 AND ITEM_GROUP='Hybrid Tech Components') "
        SQL = SQL & "OR (TECH_LEVEL=2 AND ITEM_GROUP Like '%Construction Components') "
        SQL = SQL & "OR (ITEM_NAME='Mercoxit Mining Crystal I') "
        SQL = SQL & "OR (ITEM_NAME='Deep Core Mining Laser I') "
        SQL = SQL & "OR (ITEM_GROUP='Tool')"
        Execute_msSQL(SQL)

        ' Alliance Tournament ships added - They are set as T2 (use t2 mats to build) but come up as faction in game ('Mimir','Freki','Adrestia','Utu','Vangel','Malice','Etana','Cambion','Moracha','Chremoas','Whiptail','Chameleon')
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 1 WHERE BLUEPRINT_ID IN (3517, 3519, 32789, 32791, 32788, 33396, 33398, 33674, 33676)"
        Execute_msSQL(SQL)

        ' Quick fix to update the sql table for Rubicon - Ascendancy Implant Blueprints (and Low-Grade Ascendancy) are set to T2 implant (Alpha), not invented though so set to T1
        Call Execute_msSQL("UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1 WHERE BLUEPRINT_ID IN (33536,33543,33545,33546,33547,33548,33556,33558,33560,33562,33564,33566)")

        ' Now update the Item Types - Other tables take this item type data item types: 1 = T1, 2 = T2, 14 = Tech 3, 15 = Pirate, 16 = Navy
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 14 WHERE TECH_LEVEL = 3" ' T3 stuff
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 15 WHERE MARKET_GROUP = 'Pirate Faction'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE MARKET_GROUP = 'Navy Faction'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND MARKET_GROUP IS NULL AND ITEM_CATEGORY = 'Ship'" ' Navy Faction Ships
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1 WHERE TECH_LEVEL <> 1 AND META_GROUP = 3" ' Consider storyline a tech 1
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 3 WHERE META_GROUP = 3" ' Storyline
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND MARKET_GROUP = 'Scan Probes'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 15 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Structure'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Module'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Drone'" ' Augmented and Integrated drones
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = TECH_LEVEL WHERE ITEM_TYPE = 0 OR ITEM_TYPE IS NULL"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE =15 WHERE  BLUEPRINT_GROUP = 'Combat Drone Blueprint' AND ITEM_TYPE = 16" ' Aug/Integrated drones
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 1 WHERE BLUEPRINT_ID = 33684" ' ORE version of Mack, it's just a skin not a T2 BP to invent
        Execute_msSQL(SQL)

        ' Add the S/M/L/XL tag to these here

        ' Drones are light, missiles are rockets and light
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'S' WHERE "
        SQL = SQL & "ITEM_NAME LIKE '% S' OR ITEM_NAME Like '%Small%' "
        SQL = SQL & "OR (ITEM_NAME Like '%Micro%' AND ITEM_GROUP <> 'Propulsion Module' AND ITEM_NAME NOT LIKE 'Microwave%') "
        SQL = SQL & "OR ITEM_NAME Like '%Defender%' "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Implant') "
        SQL = SQL & "OR ITEM_NAME IN ('Cap Booster 25','Cap Booster 50') "
        SQL = SQL & "OR MARKET_GROUP IN ('Interdiction Probes', 'Mining Crystals', 'Nanite Repair Paste', 'Scan Probes', 'Survey Probes', 'Scripts') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID from invTypes where volume = 5)) "
        SQL = SQL & "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '1MN%') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID from invTypes where marketGroupID IN (561,564,567,570,574,577,1671,1672,1037)))  "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND (ITEM_NAME Like '%Rocket%' OR ITEM_NAME Like '%Light Missile%') AND ITEM_GROUP NOT IN ('Propulsion Module', 'Rig Launcher'))  "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE volume < 10000))"

        Execute_msSQL(SQL)

        ' Drones are medium, missiles are heavys and hams
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'M' WHERE "
        SQL = SQL & "ITEM_NAME LIKE '% M' OR ITEM_NAME Like '%Medium%' OR ITEM_NAME IN ('Cap Booster 75','Cap Booster 100') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE volume = 10)) "
        SQL = SQL & "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '10MN%') "
        SQL = SQL & "OR (ITEM_GROUP IN ('Gang Coordinator')) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Subsystem') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (562,565,568,572,575,578,1673,1674))) "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND ITEM_NAME Like '%Heavy%' AND ITEM_NAME Not Like '%Jolt%')  "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE (volume >= 10000 AND volume < 50000) OR ITEM_GROUP_ID = 963)) "
        Execute_msSQL(SQL)

        ' Drones are Heavy, missiles are cruise/torp, towers are regular towers (Caldari Control Tower)
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'L' "
        SQL = SQL & "WHERE ITEM_NAME LIKE '% L' "
        SQL = SQL & "OR (ITEM_NAME Like '%Large%' AND ITEM_NAME NOT Like '%X-Large%') "
        SQL = SQL & "OR ITEM_NAME IN ('Cap Booster 150','Cap Booster 200')"
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE volume >= 25 and volume <=50)) "
        SQL = SQL & "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '100MN%') "
        SQL = SQL & "OR (ITEM_NAME Like ('%Control Tower')) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Deployable' AND ITEM_GROUP <> 'Mobile Warp Disruptor') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Structure' AND ITEM_GROUP <> 'Control Tower')"
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_NAME Like '%Heavy%' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID NOT IN (563,566,569,573,576,579,1675,1676))) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (563,566,569,573,576,579,1675,1676))) "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND (ITEM_NAME Like '%Cruise%' OR ITEM_NAME Like '%Torpedo%')) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE (volume >= 50000 AND volume < 500000)))"
        Execute_msSQL(SQL)

        ' Drones are fighters, missiles are citadel
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'XL' "
        SQL = SQL & "WHERE ITEM_NAME LIKE '% XL' "
        SQL = SQL & "OR ITEM_NAME Like '%Capital%' "
        SQL = SQL & "OR ITEM_NAME Like '%Huge%'"
        SQL = SQL & "OR ITEM_NAME Like '%X-Large%' "
        SQL = SQL & "OR ITEM_NAME Like '%Giant%' "
        SQL = SQL & "OR ITEM_CATEGORY IN ('Infrastructure Upgrades','Sovereignty Structures','Orbitals') "
        SQL = SQL & "OR ITEM_GROUP IN ('Station Components', 'Remote ECM Burst', 'Super Weapon', 'Siege Module')"
        SQL = SQL & "OR ITEM_NAME IN ('Cap Booster 400','Cap Booster 800') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE volume >= 5000))"
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND (ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (771,772,773,774,775,776,1642,1941)))) "
        SQL = SQL & "OR (ITEM_GROUP IN ('Jump Drive Economizer','Drone Control Unit') OR ITEM_NAME LIKE 'Jump Portal%') "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND ITEM_NAME Like '%Citadel%') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Celestial' AND (ITEM_NAME Like 'Station%' OR ITEM_NAME LIKE '%Outpost%' OR ITEM_NAME LIKE '%Freight%')) "
        SQL = SQL & "OR ITEM_GROUP LIKE 'Bomb%' "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE volume >= 500000))"
        Execute_msSQL(SQL)

        ' Anything left update to small (may need to revisit later)
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'S' WHERE SIZE_GROUP = '  '"
        Execute_msSQL(SQL)

        ' Now build the tables
        SQL = "CREATE TABLE ALL_BLUEPRINTS ("
        SQL = SQL & "BLUEPRINT_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "BLUEPRINT_NAME VARCHAR(" & GetLenSQLExpField("BLUEPRINT_NAME", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL = SQL & "BLUEPRINT_GROUP VARCHAR(" & GetLenSQLExpField("BLUEPRINT_GROUP", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL = SQL & "ITEM_ID INTEGER NOT NULL,"
        SQL = SQL & "ITEM_NAME VARCHAR(" & GetLenSQLExpField("ITEM_NAME", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL = SQL & "ITEM_GROUP_ID INTEGER NOT NULL,"
        SQL = SQL & "ITEM_GROUP VARCHAR(" & GetLenSQLExpField("ITEM_GROUP", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL = SQL & "ITEM_CATEGORY_ID INTEGER NOT NULL,"
        SQL = SQL & "ITEM_CATEGORY VARCHAR(" & GetLenSQLExpField("ITEM_CATEGORY", "ALL_BLUEPRINTS") & ") NOT NULL,"
        SQL = SQL & "MARKET_GROUP_ID INTEGER,"
        SQL = SQL & "MARKET_GROUP VARCHAR(" & GetLenSQLExpField("MARKET_GROUP", "ALL_BLUEPRINTS") & "),"
        SQL = SQL & "TECH_LEVEL INTEGER NOT NULL,"
        SQL = SQL & "PORTION_SIZE INTEGER NOT NULL,"
        SQL = SQL & "BASE_PRODUCTION_TIME INTEGER NOT NULL,"
        SQL = SQL & "BASE_RESEARCH_TL_TIME INTEGER,"
        SQL = SQL & "BASE_RESEARCH_ML_TIME INTEGER,"
        SQL = SQL & "BASE_COPY_TIME INTEGER,"
        SQL = SQL & "BASE_INVENTION_TIME INTEGER,"
        SQL = SQL & "MAX_PRODUCTION_LIMIT INTEGER NOT NULL,"
        SQL = SQL & "ITEM_TYPE INTEGER,"
        SQL = SQL & "RACE_ID INTEGER, "
        SQL = SQL & "META_GROUP INTEGER,"
        SQL = SQL & "SIZE_GROUP VARCHAR(2) NOT NULL, "
        SQL = SQL & "IGNORE INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ALL_BLUEPRINTS")

        ' Now select the final query of data
        msSQL = "SELECT * FROM ALL_BLUEPRINTS"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Insert the data into the table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO ALL_BLUEPRINTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(10)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(11)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(12)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(13)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(14)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(15)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(16)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(17)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(18)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(19)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(20)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(21)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(22)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(23)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_AB_ITEM_ID ON ALL_BLUEPRINTS (ITEM_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AB_BP_NAME ON ALL_BLUEPRINTS (BLUEPRINT_NAME)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AB_CAT_ITEM ON ALL_BLUEPRINTS (ITEM_CATEGORY)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AB_GROUP_ITEM ON ALL_BLUEPRINTS (ITEM_GROUP,ITEM_TYPE)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' ALL_BLUEPRINT_MATERIALS
    Private Sub Build_ALL_BLUEPRINT_MATERIALS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Long

        Application.DoEvents()

        ' See if the table exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.tables where name = 'ALL_BLUEPRINT_MATERIALS'"
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE ALL_BLUEPRINT_MATERIALS"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Build the temp table in SQL Server first
        SQL = "SELECT industryBlueprints.blueprintTypeID AS BLUEPRINT_ID, invTypes.typeName AS BLUEPRINT_NAME, industryActivityProducts.productTypeID AS PRODUCT_ID, "
        SQL = SQL & "industryActivityMaterials.materialTypeID AS MATERIAL_ID, matTypes.typeName AS MATERIAL, matGroups.groupName AS MATERIAL_GROUP,  "
        SQL = SQL & "matCategories.categoryName AS MATERIAL_CATEGORY, matTypes.volume AS MATERIAL_VOLUME, industryActivityMaterials.quantity AS QUANTITY, "
        SQL = SQL & "industryActivityMaterials.activityID AS ACTIVITY, industryActivityMaterials.consume AS CONSUME "
        SQL = SQL & "INTO ALL_BLUEPRINT_MATERIALS "
        SQL = SQL & "FROM industryBlueprints, invTypes, industryActivityProducts, industryActivityMaterials, invGroups, invCategories, "
        SQL = SQL & "invTypes AS matTypes, invGroups AS matGroups, invCategories AS matCategories "
        SQL = SQL & "WHERE industryBlueprints.blueprintTypeID = invTypes.typeID "
        SQL = SQL & "AND industryBlueprints.blueprintTypeID = industryActivityProducts.blueprintTypeID "
        SQL = SQL & "AND industryBlueprints.blueprintTypeID = industryActivityMaterials.blueprintTypeID "
        SQL = SQL & "AND industryActivityProducts.activityID = industryActivityMaterials.activityID "
        SQL = SQL & "AND matTypes.typeID = industryActivityMaterials.materialTypeID "
        SQL = SQL & "AND matGroups.groupID = matTypes.groupID "
        SQL = SQL & "AND matGroups.categoryID = matCategories.categoryID "
        SQL = SQL & "AND invTypes.groupID = invGroups.groupID "
        SQL = SQL & "AND invGroups.categoryID = invCategories.categoryID "
        SQL = SQL & "AND invTypes.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0 "
        SQL = SQL & "ORDER BY BLUEPRINT_ID, PRODUCT_ID "

        Call Execute_msSQL(SQL)

        ' Create SQLite table
        SQL = "CREATE TABLE ALL_BLUEPRINT_MATERIALS ("
        SQL = SQL & "BLUEPRINT_ID INTEGER,"
        SQL = SQL & "BLUEPRINT_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "PRODUCT_ID INTEGER NOT NULL,"
        SQL = SQL & "MATERIAL_ID INTEGER NOT NULL,"
        SQL = SQL & "MATERIAL VARCHAR(100) NOT NULL,"
        SQL = SQL & "MATERIAL_GROUP VARCHAR(100) NOT NULL,"
        SQL = SQL & "MATERIAL_CATEGORY VARCHAR(100) NOT NULL,"
        SQL = SQL & "MATERIAL_VOLUME REAL,"
        SQL = SQL & "QUANTITY INTEGER NOT NULL,"
        SQL = SQL & "ACTIVITY INTEGER NOT NULL,"
        SQL = SQL & "CONSUME INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ALL_BLUEPRINT_MATERIALS")

        ' Now select the final query of data
        msSQL = "SELECT * FROM ALL_BLUEPRINT_MATERIALS"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Insert the data into the new SQLite table
        While msSQLReader.Read
            SQL = "INSERT INTO ALL_BLUEPRINT_MATERIALS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(10)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i
        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_ABM_BP_ID_ACTIVITY ON ALL_BLUEPRINT_MATERIALS (BLUEPRINT_ID,ACTIVITY)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ABM_PRODUCT_ID_ACTIVITY ON ALL_BLUEPRINT_MATERIALS (PRODUCT_ID, ACTIVITY)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ABM_REQCOMP_ID_PRODUCT ON ALL_BLUEPRINT_MATERIALS (MATERIAL_ID, PRODUCT_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' ASSEMBLY_ARRAYS
    Private Sub Build_ASSEMBLY_ARRAYS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE ASSEMBLY_ARRAYS ("
        SQL = SQL & "ARRAY_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "ARRAY_NAME VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & ") NOT NULL, "
        SQL = SQL & "ACTIVITY_ID INTEGER NOT NULL,"
        SQL = SQL & "MATERIAL_MULTIPLIER REAL NOT NULL,"
        SQL = SQL & "TIME_MULTIPLIER REAL NOT NULL,"
        SQL = SQL & "COST_MULTIPLIER REAL NOT NULL,"
        SQL = SQL & "GROUP_ID INTEGER NOT NULL,"
        SQL = SQL & "CATEGORY_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT invTypes.typeID AS ARRAY_TYPE_ID, "
        msSQL = msSQL & "invTypes.typeName AS ARRAY_NAME, "
        msSQL = msSQL & "ramActivities.activityID AS ACTIVITY_ID, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerGroup.materialMultiplier AS MATERIAL_MULTIPLIER, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerGroup.timeMultiplier AS TIME_MULTIPLIER, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerGroup.costMultiplier AS COST_MULTIPLIER, "
        msSQL = msSQL & "invGroups.groupID AS GROUP_ID, "
        msSQL = msSQL & "0 AS CATEGORY_ID "
        msSQL = msSQL & "FROM invTypes, ramInstallationTypeContents, invGroups AS IG1, "
        msSQL = msSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerGroup, invGroups "
        msSQL = msSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        msSQL = msSQL & "AND ramInstallationTypeContents.installationTypeID = invTypes.typeID  "
        msSQL = msSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID  "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerGroup.assemblyLineTypeID  "
        msSQL = msSQL & "AND ramAssemblyLineTypeDetailPerGroup.groupID = invGroups.groupID "
        msSQL = msSQL & "AND invTypes.groupID = IG1.groupID  "
        msSQL = msSQL & "AND IG1.categoryID  = 23 "
        msSQL = msSQL & "UNION "
        msSQL = msSQL & "SELECT invTypes.typeID AS ARRAY_TYPE_ID, "
        msSQL = msSQL & "invTypes.typeName AS ARRAY_NAME, "
        msSQL = msSQL & "ramActivities.activityID AS ACTIVITY_ID, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerCategory.materialMultiplier AS MATERIAL_MULTIPLIER, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerCategory.timeMultiplier AS TIME_MULTIPLIER, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerCategory.costMultiplier AS COST_MULTIPLIER, "
        msSQL = msSQL & "0 AS GROUP_ID, "
        msSQL = msSQL & "invCategories.categoryID AS CATEGORY_ID "
        msSQL = msSQL & "FROM invTypes, invGroups, ramInstallationTypeContents, "
        msSQL = msSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerCategory, invCategories "
        msSQL = msSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        msSQL = msSQL & "AND ramInstallationTypeContents.installationTypeID = invTypes.typeID  "
        msSQL = msSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID  "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerCategory.assemblyLineTypeID  "
        msSQL = msSQL & "AND ramAssemblyLineTypeDetailPerCategory.categoryID = invCategories.categoryID "
        msSQL = msSQL & "AND invTypes.groupID = invGroups.groupID  "
        msSQL = msSQL & "AND invGroups.categoryID  = 23 "

        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call SetProgressBarValues(" (" & msSQL & ") AS X ")

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO ASSEMBLY_ARRAYS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        msSQLReader.Close()

        ' Finally, index
        SQL = "CREATE INDEX IDX_AA_AID_CID_GID ON ASSEMBLY_ARRAYS (ACTIVITY_ID, CATEGORY_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AA_AN_AID_CID_GID ON ASSEMBLY_ARRAYS (ARRAY_NAME, ACTIVITY_ID, CATEGORY_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call CommitSQLiteTransaction(SQLiteDB)

    End Sub

    ' STATION_FACILITIES - Temp table, update with CREST
    Private Sub Build_STATION_FACILITIES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String
        Dim i As Integer

        SQL = "CREATE TABLE STATION_FACILITIES ( "
        SQL = SQL & "FACILITY_ID INT NOT NULL, "
        SQL = SQL & "FACILITY_NAME VARCHAR(" & GetLenSQLExpField("stationName", "staStations") & ") NOT NULL, "
        SQL = SQL & "SOLAR_SYSTEM_ID INT NOT NULL, "
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(" & GetLenSQLExpField("solarSystemName", "mapSolarSystems") & ") NOT NULL, "
        SQL = SQL & "SOLAR_SYSTEM_SECURITY REAL NOT NULL, "
        SQL = SQL & "REGION_ID INT NOT NULL, "
        SQL = SQL & "REGION_NAME VARCHAR(" & GetLenSQLExpField("regionName", "mapRegions") & ") NOT NULL, "
        SQL = SQL & "FACILITY_TYPE_ID INT NOT NULL, "
        SQL = SQL & "FACILITY_TYPE VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & ") NOT NULL, "
        SQL = SQL & "ACTIVITY_ID INT NOT NULL, "
        SQL = SQL & "FACILITY_TAX REAL NOT NULL, "
        SQL = SQL & "MATERIAL_MULTIPLIER REAL NOT NULL, "
        SQL = SQL & "TIME_MULTIPLIER REAL NOT NULL, "
        SQL = SQL & "COST_MULTIPLIER REAL NOT NULL, "
        SQL = SQL & "GROUP_ID INT NOT NULL, "
        SQL = SQL & "CATEGORY_ID INT NOT NULL, "
        SQL = SQL & "COST_INDEX REAL NOT NULL, "
        SQL = SQL & "OUTPOST INT NOT NULL "
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Application.DoEvents()

        pgMain.Maximum = 110000
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True

        ' Pull station data from stations for temp use if they don't load facilities from CREST
        msSQL = "SELECT staStations.stationID AS FACILITY_ID, stationName AS FACILITY_NAME, "
        msSQL = msSQL & "mapSolarSystems.solarSystemID AS SOLAR_SYSTEM_ID, mapSolarSystems.solarSystemName AS SOLAR_SYSTEM_NAME, mapSolarSystems.security AS SOLAR_SYSTEM_SECURITY, "
        msSQL = msSQL & "mapRegions.regionID AS REGION_ID, mapRegions.regionName AS REGION_NAME, "
        msSQL = msSQL & "staStations.stationTypeID, typeName AS FACILITY_TYPE, ramActivities.activityID AS ACTIVITY_ID, "
        msSQL = msSQL & ".1 as FACILITY_TAX, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerGroup.materialMultiplier AS MATERIAL_MULTIPLIER, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerGroup.timeMultiplier AS TIME_MULTIPLIER,  "
        msSQL = msSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerGroup.costMultiplier AS COST_MULTIPLIER,  "
        msSQL = msSQL & "invGroups.groupID AS GROUP_ID, "
        msSQL = msSQL & "0 AS CATEGORY_ID, 0 AS COST_INDEX, 0 AS OUTPOST "
        msSQL = msSQL & "FROM staStations, invTypes, ramAssemblyLineStations, mapRegions, mapSolarSystems, "
        msSQL = msSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerGroup, invGroups "
        msSQL = msSQL & "WHERE staStations.stationTypeID = invTypes.typeID "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerGroup.assemblyLineTypeID "
        msSQL = msSQL & "AND ramAssemblyLineTypeDetailPerGroup.groupID = invGroups.groupID "
        msSQL = msSQL & "AND staStations.regionID = mapRegions.regionID "
        msSQL = msSQL & "AND staStations.solarSystemID = mapSolarSystems.solarSystemID "
        msSQL = msSQL & "AND staStations.stationID = ramAssemblyLineStations.stationID "
        msSQL = msSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID "
        msSQL = msSQL & "AND ramAssemblyLineStations.assemblyLineTypeID = ramAssemblyLineTypes.assemblyLineTypeID "
        msSQL = msSQL & "UNION "
        msSQL = msSQL & "SELECT staStations.stationID, stationName, "
        msSQL = msSQL & "mapSolarSystems.solarSystemID AS SOLAR_SYSTEM_ID, mapSolarSystems.solarSystemName AS SOLAR_SYSTEM_NAME, mapSolarSystems.security AS SOLAR_SYSTEM_SECURITY, "
        msSQL = msSQL & "mapRegions.regionID AS REGION_ID, mapRegions.regionName AS REGION_NAME, "
        msSQL = msSQL & "staStations.stationTypeID, typeName AS FACILITY_TYPE, ramActivities.activityID AS ACTIVITY_ID, "
        msSQL = msSQL & ".1 as FACILITY_TAX, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseMaterialMultiplier * ramAssemblyLineTypeDetailPerCategory.materialMultiplier AS MATERIAL_MULTIPLIER, "
        msSQL = msSQL & "ramAssemblyLineTypes.baseTimeMultiplier * ramAssemblyLineTypeDetailPerCategory.timeMultiplier AS TIME_MULTIPLIER,  "
        msSQL = msSQL & "ramAssemblyLineTypes.baseCostMultiplier * ramAssemblyLineTypeDetailPerCategory.costMultiplier AS COST_MULTIPLIER,    "
        msSQL = msSQL & "0 AS GROUP_ID, "
        msSQL = msSQL & "invCategories.categoryID AS CATEGORY_ID, 0 AS COST_INDEX, 0 AS OUTPOST "
        msSQL = msSQL & "FROM staStations, invTypes, ramAssemblyLineStations, mapRegions, mapSolarSystems, "
        msSQL = msSQL & "ramActivities, ramAssemblyLineTypes, ramAssemblyLineTypeDetailPerCategory, invCategories "
        msSQL = msSQL & "WHERE staStations.stationTypeID = invTypes.typeID "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramAssemblyLineTypeDetailPerCategory.assemblyLineTypeID "
        msSQL = msSQL & "AND ramAssemblyLineTypeDetailPerCategory.categoryID = invCategories.categoryID "
        msSQL = msSQL & "AND staStations.regionID = mapRegions.regionID "
        msSQL = msSQL & "AND staStations.solarSystemID = mapSolarSystems.solarSystemID "
        msSQL = msSQL & "AND staStations.stationID = ramAssemblyLineStations.stationID "
        msSQL = msSQL & "AND ramAssemblyLineTypes.activityID = ramActivities.activityID "
        msSQL = msSQL & "AND ramAssemblyLineStations.assemblyLineTypeID = ramAssemblyLineTypes.assemblyLineTypeID "

        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call SetProgressBarValues(" (" & msSQL & ") AS X ")

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()
            SQL = "INSERT INTO STATION_FACILITIES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(10)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(11)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(12)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(13)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(14)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(15)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(16)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(17)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        msSQLReader.Close()
        Application.DoEvents()

        ' Finally do indexes
        SQL = "CREATE INDEX IDX_SF_FN_AID ON STATION_FACILITIES (FACILITY_NAME, ACTIVITY_ID);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SF_FID_AID_GID_CID ON STATION_FACILITIES (FACILITY_ID, ACTIVITY_ID, GROUP_ID, CATEGORY_ID);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SF_OP_FN_AID_CID ON STATION_FACILITIES (OUTPOST, FACILITY_NAME, ACTIVITY_ID, CATEGORY_ID);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SF_OP_FN_AID_GID ON STATION_FACILITIES (OUTPOST, FACILITY_NAME, ACTIVITY_ID, GROUP_ID);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SF_SSID_AID ON STATION_FACILITIES (SOLAR_SYSTEM_ID, ACTIVITY_ID);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SF_OP_AID_GID_CID_RN_SSN ON STATION_FACILITIES (OUTPOST, ACTIVITY_ID, GROUP_ID, CATEGORY_ID, REGION_NAME, SOLAR_SYSTEM_NAME);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call CommitSQLiteTransaction(SQLiteDB)

    End Sub

    ' Updates the table with categories not included - this makes it easier to run the station_facilities table without joins
    Private Sub UpdateramAssemblyLineTypeDetailPerCategory()
        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLQuery2 As New SqlCommand
        Dim msSQLQuery3 As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQLReader2 As SqlDataReader
        Dim msSQLReader3 As SqlDataReader
        Dim msSQL As String

        ' Figure out what lines are not in the categories table so that we can add the missing line and categoryID
        msSQL = "SELECT ramAssemblyLineTypes.assemblyLineTypeID, activityID "
        msSQL = msSQL & "FROM ramAssemblyLineTypes, ramInstallationTypeContents, invTypes "
        msSQL = msSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID NOT IN (SELECT assemblyLineTypeID FROM ramAssemblyLineTypeDetailPerCategory) "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID NOT IN (SELECT assemblyLineTypeID FROM ramAssemblyLineTypeDetailPerGroup) "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        msSQL = msSQL & "AND ramInstallationTypeContents.installationTypeID = invTypes.typeID "
        msSQL = msSQL & "GROUP BY ramAssemblyLineTypes.assemblyLineTypeID, activityID "
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        While msSQLReader.Read
            ' Look up all item categoryID's for the activity of all blueprints that have it
            msSQL = "SELECT invCategories.categoryID "
            msSQL = msSQL & "FROM industryActivityProducts, invTypes, invGroups, invCategories "
            ' This line figures out the items made with the bp, and then attaches it to the activities on the bp - not elegant but works with CCPs system
            msSQL = msSQL & "WHERE (SELECT typeID FROM invTypes, industryActivityProducts AS X WHERE typeID = X.productTypeID AND X.activityID = 1 AND X.blueprintTypeID = industryActivityProducts.blueprintTypeID) = invTypes.typeID "
            msSQL = msSQL & "AND invTypes.groupID = invGroups.groupID "
            msSQL = msSQL & "AND invGroups.categoryID = invCategories.categoryID "
            msSQL = msSQL & "AND activityID = " & msSQLReader.GetValue(1) & " "
            msSQL = msSQL & "GROUP BY invCategories.categoryID "

            msSQLQuery2 = New SqlCommand(msSQL, SQLExpressConnection2)
            msSQLReader2 = msSQLQuery2.ExecuteReader()

            While msSQLReader2.Read
                ' Now insert the data into the ramAssemblyLineTypeDetailPerCategory table if not there
                msSQL = "SELECT 'X' FROM ramAssemblyLineTypeDetailPerCategory "
                msSQL = msSQL & "WHERE assemblyLineTypeID = " & msSQLReader.GetValue(0) & " "
                msSQL = msSQL & "AND categoryID = " & msSQLReader2.GetValue(0) & " "
                msSQL = msSQL & "AND timeMultiplier = 1 AND materialMultiplier = 1 AND costMultiplier = 1"

                msSQLQuery3 = New SqlCommand(msSQL, SQLExpressConnection3)
                msSQLReader3 = msSQLQuery3.ExecuteReader()

                If Not msSQLReader3.Read Then
                    msSQL = "INSERT INTO ramAssemblyLineTypeDetailPerCategory VALUES ("
                    msSQL = msSQL & CStr(msSQLReader.GetValue(0)) & ", " ' ramAssemblyLineTypeID
                    msSQL = msSQL & CStr(msSQLReader2.GetValue(0)) & ", " ' categoryID
                    msSQL = msSQL & "1,1,1)" ' timeMultiplier, materialMultiplier, and costMultiplier are all 1 by default since they don't exist
                Else
                    Application.DoEvents()
                End If

                Call Execute_msSQL(msSQL)

                msSQLReader3.Close()

            End While

            msSQLReader2.Close()

        End While

        msSQLReader.Close()

    End Sub

    ' STATIONS - Temp table, update with CREST
    Private Sub Build_Stations()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE STATIONS ("
        SQL = SQL & "STATION_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "STATION_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "STATION_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER,"
        SQL = SQL & "SOLAR_SYSTEM_SECURITY FLOAT NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("staStations")

        ' Pull new data and insert
        msSQL = "SELECT stationID, stationName, stationTypeID, solarSystemID, security, regionID FROM staStations"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO STATIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        SQL = "CREATE INDEX IDX_S_FID ON STATIONS (STATION_ID);"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        msSQLReader.Close()

    End Sub

    ' RACE_IDS
    Private Sub Build_RACE_IDS()
        Dim SQL As String

        SQL = "CREATE TABLE RACE_IDS (ID INTEGER PRIMARY KEY, RACE VARCHAR(8) NOT NULL)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "INSERT INTO RACE_IDS VALUES (1, 'Caldari')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "INSERT INTO RACE_IDS VALUES (2, 'Minmatar')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "INSERT INTO RACE_IDS VALUES (4, 'Amarr')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "INSERT INTO RACE_IDS VALUES (8, 'Gallente')"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' CHARACTER_SKILLS
    Private Sub Build_CHARACTER_SKILLS()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_SKILLS ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "SKILL_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "SKILL_NAME VARCHAR(50) NOT NULL,"
        SQL = SQL & "SKILL_POINTS INTEGER NOT NULL,"
        SQL = SQL & "SKILL_LEVEL INTEGER NOT NULL,"
        SQL = SQL & "OVERRIDE_SKILL INTEGER NOT NULL,"
        SQL = SQL & "OVERRIDE_LEVEL INTEGER NOT NULL)"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        lblTableName.Text = "Building: CHARACTER_SKILLS"

        SQL = "CREATE INDEX IDX_CSKILLS_CHARACTER_ID ON CHARACTER_SKILLS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CSKILLS_SKILL_TYPE_ID ON CHARACTER_SKILLS (SKILL_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)


    End Sub

    ' INDUSTRY_UPGRADE_BELTS
    Private Sub Build_INDUSTRY_UPGRADE_BELTS()
        Dim SQL As String

        ' Build the table
        SQL = "CREATE TABLE INDUSTRY_UPGRADE_BELTS ("
        SQL = SQL & "AMOUNT INTEGER NOT NULL,"
        SQL = SQL & "BELT_NAME VARCHAR(8) NOT NULL,"
        SQL = SQL & "ORE VARCHAR(21) NOT NULL,"
        SQL = SQL & "NUMBER_ASTEROIDS INTEGER NOT NULL,"
        SQL = SQL & "TRUESEC_BONUS INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Since this is all data I created, just do inserts here - Double check when possible with ore scanner (numbers of asteroids likely wrong)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Arkonor',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Bistot',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Crokite',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Dark Ochre',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Gneiss',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (200000,'Colossal','Hedbergite',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Colossal','Hemorphite',3,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Colossal','Jaspet',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (600000,'Colossal','Kernite',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Mercoxit',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Colossal','Omber',3,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Plagioclase',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (480000,'Colossal','Pyroxeres',6,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Scordite',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Spodumain',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Veldspar',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Crimson Arkonor',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Dense Veldspar',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (600000,'Colossal','Fiery Kernite',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (200000,'Colossal','Glazed Hedbergite',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Gleaming Spodumain',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Colossal','Golden Omber',3,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Massive Scordite',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Monoclinic Bistot',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Obsidian Ochre',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Prismatic Gneiss',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Colossal','Pristine Jaspet',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Colossal','Radiant Hemorphite',3,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Rich Plagioclase',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Sharp Crokite',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (480000,'Colossal','Viscous Pyroxeres',6,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Vitreous Mercoxit',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Azure Plagioclase',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Bright Spodumain',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Concentrated Veldspar',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Colossal','Condensed Scordite',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Crystalline Crokite',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Iridescent Gneiss',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (600000,'Colossal','Luminous Kernite',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Magma Mercoxit',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Onyx Ochre',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Prime Arkonor',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Colossal','Pure Jaspet',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Colossal','Silvery Omber',3,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (480000,'Colossal','Solid Pyroxeres',6,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Triclinic Bistot',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (200000,'Colossal','Vitric Hedbergite',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Colossal','Vivid Hemorphite',3,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Arkonor',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Bistot',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Crokite',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (50000,'Enormous','Dark Ochre',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Gneiss',6,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (200000,'Enormous','Hedbergite',7,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Enormous','Hemorphite',10,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (470000,'Enormous','Jaspet',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Kernite',12,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Mercoxit',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Omber',13,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (780000,'Enormous','Plagioclase',12,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (805000,'Enormous','Pyroxeres',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (660000,'Enormous','Scordite',8,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (542000,'Enormous','Spodumain',8,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (840500,'Enormous','Veldspar',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Crimson Arkonor',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (840500,'Enormous','Dense Veldspar',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Fiery Kernite',12,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (200000,'Enormous','Glazed Hedbergite',7,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (542000,'Enormous','Gleaming Spodumain',8,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Golden Omber',13,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (660000,'Enormous','Massive Scordite',8,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Monoclinic Bistot',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (50000,'Enormous','Obsidian Ochre',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Prismatic Gneiss',6,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (470000,'Enormous','Pristine Jaspet',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Enormous','Radiant Hemorphite',10,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (780000,'Enormous','Rich Plagioclase',12,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Sharp Crokite',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (805000,'Enormous','Viscous Pyroxeres',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Vitreous Mercoxit',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (780000,'Enormous','Azure Plagioclase',12,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (542000,'Enormous','Bright Spodumain',8,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (840500,'Enormous','Concentrated Veldspar',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (660000,'Enormous','Condensed Scordite',8,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Crystalline Crokite',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Iridescent Gneiss',6,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Luminous Kernite',12,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Magma Mercoxit',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (50000,'Enormous','Onyx Ochre',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Prime Arkonor',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (470000,'Enormous','Pure Jaspet',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Silvery Omber',13,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (805000,'Enormous','Solid Pyroxeres',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Triclinic Bistot',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (200000,'Enormous','Vitric Hedbergite',7,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Enormous','Vivid Hemorphite',10,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Arkonor',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Bistot',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Crokite',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Dark Ochre',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Gneiss',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (80000,'Large','Hedbergite',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (80000,'Large','Hemorphite',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Large','Jaspet',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Large','Kernite',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Mercoxit',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Large','Omber',3,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Plagioclase',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Pyroxeres',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Large','Scordite',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Spodumain',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Veldspar',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Crimson Arkonor',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Dense Veldspar',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Large','Fiery Kernite',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (80000,'Large','Glazed Hedbergite',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Gleaming Spodumain',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Large','Golden Omber',3,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Large','Massive Scordite',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Monoclinic Bistot',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Obsidian Ochre',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Prismatic Gneiss',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Large','Pristine Jaspet',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (80000,'Large','Radiant Hemorphite',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Rich Plagioclase',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Sharp Crokite',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Viscous Pyroxeres',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Vitreous Mercoxit',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Azure Plagioclase',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Bright Spodumain',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Concentrated Veldspar',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Large','Condensed Scordite',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Crystalline Crokite',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Iridescent Gneiss',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Large','Luminous Kernite',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Magma Mercoxit',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Onyx Ochre',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Prime Arkonor',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Large','Pure Jaspet',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Large','Silvery Omber',3,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Large','Solid Pyroxeres',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Triclinic Bistot',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (80000,'Large','Vitric Hedbergite',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (80000,'Large','Vivid Hemorphite',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Moderate','Arkonor',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Moderate','Bistot',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Moderate','Crokite',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Moderate','Dark Ochre',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Moderate','Gneiss',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (100000,'Moderate','Hedbergite',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (100000,'Moderate','Hemorphite',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Moderate','Jaspet',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Moderate','Kernite',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Moderate','Mercoxit',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Moderate','Omber',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (925000,'Moderate','Plagioclase',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (965000,'Moderate','Pyroxeres',11,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (940000,'Moderate','Scordite',13,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Moderate','Spodumain',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (865000,'Moderate','Veldspar',13,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Moderate','Arkonor',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Moderate','Bistot',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Moderate','Crokite',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Moderate','Dark Ochre',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Moderate','Gneiss',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (100000,'Moderate','Hedbergite',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (100000,'Moderate','Hemorphite',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Moderate','Jaspet',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Moderate','Kernite',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Moderate','Mercoxit',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Moderate','Omber',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (925000,'Moderate','Plagioclase',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (965000,'Moderate','Pyroxeres',11,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (940000,'Moderate','Scordite',13,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Moderate','Spodumain',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (865000,'Moderate','Veldspar',13,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Moderate','Arkonor',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Moderate','Bistot',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Moderate','Crokite',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Moderate','Dark Ochre',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Moderate','Gneiss',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (100000,'Moderate','Hedbergite',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (100000,'Moderate','Hemorphite',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Moderate','Jaspet',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Moderate','Kernite',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Moderate','Mercoxit',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (400000,'Moderate','Omber',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (925000,'Moderate','Plagioclase',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (965000,'Moderate','Pyroxeres',11,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (940000,'Moderate','Scordite',13,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Moderate','Spodumain',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (865000,'Moderate','Veldspar',13,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9600,'Small','Arkonor',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Bistot',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crokite',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Dark Ochre',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Gneiss',1,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (103000,'Small','Hedbergite',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (104000,'Small','Hemorphite',8,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Small','Jaspet',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (295000,'Small','Kernite',6,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Mercoxit',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Omber',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (230000,'Small','Plagioclase',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (250000,'Small','Pyroxeres',4,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Scordite',0,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Spodumain',2,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (438000,'Small','Veldspar',5,0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9600,'Small','Arkonor',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Bistot',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crokite',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Dark Ochre',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Gneiss',1,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (103000,'Small','Hedbergite',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (104000,'Small','Hemorphite',8,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Small','Jaspet',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (295000,'Small','Kernite',6,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Mercoxit',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Omber',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (230000,'Small','Plagioclase',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (250000,'Small','Pyroxeres',4,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Scordite',0,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Spodumain',2,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (438000,'Small','Veldspar',5,10)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9600,'Small','Arkonor',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Bistot',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crokite',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Dark Ochre',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Gneiss',1,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (103000,'Small','Hedbergite',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (104000,'Small','Hemorphite',8,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (120000,'Small','Jaspet',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (295000,'Small','Kernite',6,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Mercoxit',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Omber',5,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (230000,'Small','Plagioclase',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (250000,'Small','Pyroxeres',4,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Scordite',0,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Spodumain',2,5)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (438000,'Small','Veldspar',5,5)", SQLiteDB)

        Call CommitSQLiteTransaction(SQLiteDB)

        SQL = "CREATE INDEX IDX_BP_ID_BELT_NAME ON INDUSTRY_UPGRADE_BELTS (BELT_NAME)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' API
    Private Sub Build_API()
        Dim SQL As String

        SQL = "CREATE TABLE API ("
        SQL = SQL & "CACHED_UNTIL VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "KEY_ID INTEGER NOT NULL,"
        SQL = SQL & "API_KEY VARCHAR(64) NOT NULL,"
        SQL = SQL & "API_TYPE VARCHAR(20) NOT NULL,"
        SQL = SQL & "ACCESS_MASK INTEGER NOT NULL,"
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "CHARACTER_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "CORPORATION_ID INTEGER NOT NULL,"
        SQL = SQL & "CORPORATION_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "OVERRIDE_SKILLS INTEGER NOT NULL,"
        SQL = SQL & "IS_DEFAULT INTEGER NOT NULL,"
        SQL = SQL & "KEY_EXPIRATION_DATE VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "ASSETS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "INDUSTRY_JOBS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "RESEARCH_AGENTS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "FACILITIES_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "BLUEPRINTS_CACHED_UNTIL VARCHAR(23)" ' Date
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CAPI_CHARACTER_ID ON API (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CAPI_KEY_ID ON API (KEY_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' EVEIPH DATA
    Private Sub Build_CREST_CACHE_DATES()
        Dim SQL As String

        SQL = "CREATE TABLE CREST_CACHE_DATES ("
        SQL = SQL & "CREST_INDUSTRY_SPECIALIZATIONS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "CREST_INDUSTRY_TEAMS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "CREST_INDUSTRY_TEAM_AUCTIONS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "CREST_INDUSTRY_SYSTEMS_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "CREST_INDUSTRY_FACILITIES_CACHED_UNTIL VARCHAR(23)," ' Date
        SQL = SQL & "CREST_MARKET_PRICES_CACHED_UNTIL VARCHAR(23)" ' Date
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' industryBlueprints
    Private Sub Build_industryBlueprints()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE industryBlueprints ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL PRIMARY KEY,"
        SQL = SQL & "maxProductionLimit INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, maxProductionLimit FROM industryBlueprints"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO industryBlueprints VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_blueprintTypeID ON industryBlueprints (blueprintTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' industryActivities
    Private Sub Build_industryActivities()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE industryActivities ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL,"
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "time INTEGER NOT NULL,"
        SQL = SQL & "PRIMARY KEY (blueprintTypeID, activityID)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, activityID, time FROM industryActivities"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO industryActivities VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_activityID ON industryActivities (activityID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' industryActivityMaterials
    Private Sub Build_industryActivityMaterials()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' Build table
        SQL = "CREATE TABLE industryActivityMaterials ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL,"
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "materialTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER NOT NULL,"
        SQL = SQL & "consume INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, activityID, materialTypeID, quantity, consume FROM industryActivityMaterials"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO industryActivityMaterials VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_BPIDactivityID1 ON industryActivityMaterials (blueprintTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' INDUSTRY_ACTIVITY_PRODUCTS
    Private Sub Build_INDUSTRY_ACTIVITY_PRODUCTS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' Build table
        SQL = "CREATE TABLE INDUSTRY_ACTIVITY_PRODUCTS ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL,"
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "productTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER NOT NULL,"
        SQL = SQL & "probability FLOAT NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, activityID, productTypeID, quantity, probability FROM industryActivityProducts"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INDUSTRY_ACTIVITY_PRODUCTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_IAP_BTID_AID ON INDUSTRY_ACTIVITY_PRODUCTS (blueprintTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' RAM_ACTIVITIES
    Private Sub Build_RAM_ACTIVITIES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE RAM_ACTIVITIES ("
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "activityName VARCHAR(100),"
        SQL = SQL & "iconNo VARCHAR(5),"
        SQL = SQL & "description VARCHAR(1000),"
        SQL = SQL & "published INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("ramActivities")

        ' Pull new data and insert
        msSQL = "SELECT activityID, activityName, iconNo, description, published FROM ramActivities"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ACTIVITIES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetString(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetString(3)) & ","
            SQL = SQL & BuildInsertFieldString(CInt(msSQLReader.GetBoolean(4))) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_ACTIVITY_ID ON RAM_ACTIVITIES (activityID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' RAM_ASSEMBLY_LINE_STATIONS
    Private Sub Build_RAM_ASSEMBLY_LINE_STATIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_STATIONS ("
        SQL = SQL & "stationID INTEGER NOT NULL,"
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER,"
        SQL = SQL & "stationTypeID INTEGER, "
        SQL = SQL & "ownerID INTEGER,"
        SQL = SQL & "solarSystemID INTEGER,"
        SQL = SQL & "regionID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("ramAssemblyLineStations")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineStations"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_STATIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_RALS_SID ON RAM_ASSEMBLY_LINE_STATIONS (stationID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_RALS_SSID ON RAM_ASSEMBLY_LINE_STATIONS (solarSystemID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_RALS_ALTID ON RAM_ASSEMBLY_LINE_STATIONS (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY ("
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "categoryID INTEGER NOT NULL,"
        SQL = SQL & "timeMultiplier FLOAT,"
        SQL = SQL & "materialMultiplier FLOAT, "
        SQL = SQL & "costMultiplier FLOAT"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("ramAssemblyLineTypeDetailPerCategory")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineTypeDetailPerCategory"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALC_ALTID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ALC_CID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY (categoryID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP ("
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "groupID INTEGER NOT NULL,"
        SQL = SQL & "timeMultiplier FLOAT,"
        SQL = SQL & "materialMultiplier FLOAT, "
        SQL = SQL & "costMultiplier FLOAT"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("ramAssemblyLineTypeDetailPerGroup")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineTypeDetailPerGroup"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALG_ALTID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ALG_GID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP (groupID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPES
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPES ("
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "assemblyLineTypeName VARCHAR(100),"
        SQL = SQL & "description VARCHAR(1000),"
        SQL = SQL & "baseTimeMultiplier FLOAT, "
        SQL = SQL & "baseMaterialMultiplier FLOAT,"
        SQL = SQL & "baseCostMultiplier FLOAT,"
        SQL = SQL & "volume FLOAT,"
        SQL = SQL & "activityID INTEGER,"
        SQL = SQL & "minCostPerHour FLOAT"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("ramAssemblyLineTypes")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineTypes"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALT_ALTID_AID ON RAM_ASSEMBLY_LINE_TYPES (assemblyLineTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ALT_AID ON RAM_ASSEMBLY_LINE_TYPES (activityID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' RAM_INSTALLATION_TYPE_CONTENTS
    Private Sub Build_RAM_INSTALLATION_TYPE_CONTENTS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE RAM_INSTALLATION_TYPE_CONTENTS ("
        SQL = SQL & "installationTypeID INTEGER NOT NULL,"
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("ramInstallationTypeContents")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramInstallationTypeContents"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_INSTALLATION_TYPE_CONTENTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_RITC_ITID_ALTID ON RAM_INSTALLATION_TYPE_CONTENTS (installationTypeID, assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_RITC_ALTID ON RAM_INSTALLATION_TYPE_CONTENTS (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' CHARACTER_STANDINGS
    Private Sub Build_Character_Standings()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_STANDINGS ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "NPC_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "NPC_TYPE VARCHAR(50) NOT NULL,"
        SQL = SQL & "NPC_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "STANDING REAL NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CS_CHARACTER_ID ON CHARACTER_STANDINGS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CS_NPC_TYPE_ID ON CHARACTER_STANDINGS (NPC_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' OWNED_BLUEPRINTS
    Private Sub Build_OWNED_BLUEPRINTS()
        Dim SQL As String

        SQL = "CREATE TABLE OWNED_BLUEPRINTS ("
        SQL = SQL & "USER_ID INTEGER NOT NULL,"
        SQL = SQL & "ITEM_ID INTEGER NOT NULL,"
        SQL = SQL & "LOCATION_ID INTEGER NOT NULL,"
        SQL = SQL & "BLUEPRINT_ID INTEGER NOT NULL,"
        SQL = SQL & "BLUEPRINT_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "QUANTITY INTEGER NOT NULL,"
        SQL = SQL & "FLAG_ID INTEGER NOT NULL,"
        SQL = SQL & "ME INTEGER NOT NULL,"
        SQL = SQL & "TE INTEGER NOT NULL,"
        SQL = SQL & "RUNS INTEGER NOT NULL,"
        SQL = SQL & "BP_TYPE INTEGER NOT NULL,"
        SQL = SQL & "OWNED INTEGER NOT NULL,"
        SQL = SQL & "SCANNED INTEGER NOT NULL,"
        SQL = SQL & "FAVORITE INTEGER NOT NULL,"
        SQL = SQL & "ADDITIONAL_COSTS FLOAT"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Indexes
        SQL = "CREATE INDEX IDX_OBP_USER_ID ON OWNED_BLUEPRINTS (USER_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' FACTIONS
    Private Sub Build_FACTIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE FACTIONS ("
        SQL = SQL & "factionID INTEGER PRIMARY KEY,"
        SQL = SQL & "factionName VARCHAR(" & GetLenSQLExpField("factionName", "chrFactions") & ") NOT NULL,"
        SQL = SQL & "raceID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("chrFactions")

        Application.DoEvents()

        ' Pull new data and insert
        msSQL = "SELECT factionID, factionName, raceIDs FROM chrFactions"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()
            SQL = "INSERT INTO FACTIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_F_FACTION_NAME ON FACTIONS (factionName)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' META_TYPEs
    Private Sub Build_Meta_Types()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE META_TYPES ("
        SQL = SQL & "typeID INTEGER PRIMARY KEY,"
        SQL = SQL & "parentTypeID INTEGER,"
        SQL = SQL & "metaGroupID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("invMetaTypes")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invMetaTypes"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO META_TYPES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        pgMain.Visible = False

    End Sub

    ' CONTROL_TOWER_RESOURCES
    Private Sub Build_Control_Tower_Resources()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE CONTROL_TOWER_RESOURCES ("
        SQL = SQL & "controlTowerTypeID INTEGER NOT NULL,"
        SQL = SQL & "resourceTypeID INTEGER NOT NULL,"
        SQL = SQL & "purpose INTEGER,"
        SQL = SQL & "quantity INTEGER,"
        SQL = SQL & "minSecurityLevel FLOAT,"
        SQL = SQL & "factionID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("invControlTowerResources")

        ' Pull new data and insert
        msSQL = "SELECT controlTowerTypeID, resourceTypeID, purpose, quantity, minSecurityLevel, factionID FROM invControlTowerResources"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO CONTROL_TOWER_RESOURCES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_CT_TYPE_ID ON CONTROL_TOWER_RESOURCES (controlTowerTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_RESOURCE_TYPE_ID ON CONTROL_TOWER_RESOURCES (resourceTypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' USER_SETTINGS
    Private Sub Build_User_Settings()
        Dim SQL As String
        Dim i As Integer

        SQL = "CREATE TABLE USER_SETTINGS ("
        SQL = SQL & "CHECK_FOR_UPDATES INTEGER,"
        SQL = SQL & "EXPORT_CVS INTEGER,"
        SQL = SQL & "DEFAULT_PRICE_SYSTEM VARCHAR(" & GetLenSQLExpField("solarSystemName", "mapSolarSystems") & "),"
        SQL = SQL & "DEFAULT_PRICE_REGION VARCHAR(" & GetLenSQLExpField("regionName", "mapRegions") & "),"
        SQL = SQL & "SHOW_TOOL_TIPS INTEGER,"
        SQL = SQL & "REFINING_IMPLANT_VALUE REAL,"
        SQL = SQL & "MANUFACTURING_IMPLANT_VALUE REAL,"
        SQL = SQL & "BUILD_BASE_INSTALL REAL,"
        SQL = SQL & "BUILD_BASE_HOURLY REAL,"
        SQL = SQL & "BUILD_STANDING_DISCOUNT REAL,"
        SQL = SQL & "INVENT_BASE_INSTALL REAL,"
        SQL = SQL & "INVENT_BASE_HOURLY REAL,"
        SQL = SQL & "INVENT_STANDING_DISCOUNT REAL,"
        SQL = SQL & "BUILD_CORP_STANDING REAL,"
        SQL = SQL & "INVENT_CORP_STANDING REAL,"
        SQL = SQL & "BROKER_CORP_STANDING REAL,"
        SQL = SQL & "BROKER_FACTION_STANDING REAL,"
        SQL = SQL & "DEFAULT_POS_FUEL_COST REAL,"
        SQL = SQL & "DEFAULT_BUILD_BUY INTEGER,"
        SQL = SQL & "INCLUDE_COPY_TIMES INTEGER,"
        SQL = SQL & "INCLUDE_INVENTION_TIMES INTEGER,"
        SQL = SQL & "USE_MAX_BPC_RUNS_SHIP INTEGER,"
        SQL = SQL & "USE_MAX_BPC_RUNS_NONSHIP INTEGER,"
        SQL = SQL & "DEFAULT_COPY_COST REAL,"
        SQL = SQL & "DEFAULT_COPY_SLOT_MODIFIER REAL,"
        SQL = SQL & "COPY_IMPLANT_VALUE REAL,"
        SQL = SQL & "DEFAULT_INVENTION_SLOT_MODIFIER REAL,"
        SQL = SQL & "DEFAULT_ME INTEGER,"
        SQL = SQL & "DEFAULT_PE INTEGER,"
        SQL = SQL & "INCLUDE_RE_TIMES INTEGER,"
        SQL = SQL & "REFINE_CORP_STANDING REAL,"
        For i = 13 To 50
            ' Null all the unused
            SQL = SQL & "UNUSED_SETTING_" & CStr(i) & ","
        Next
        SQL = SQL.Substring(0, Len(SQL) - 1) & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' ATTRIBUTE_TYPES
    Private Sub Build_Attribute_Types()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE ATTRIBUTE_TYPES ("
        SQL = SQL & "attributeID INTEGER PRIMARY KEY,"
        SQL = SQL & "attributeName VARCHAR(" & GetLenSQLExpField("attributeName", "dgmAttributeTypes") & "),"
        SQL = SQL & "displayName VARCHAR(" & GetLenSQLExpField("displayName", "dgmAttributeTypes") & ")"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("dgmAttributeTypes")

        ' Pull new data and insert
        msSQL = "SELECT attributeID, attributeName, displayName FROM dgmAttributeTypes"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO ATTRIBUTE_TYPES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        pgMain.Visible = False

    End Sub

    ' TYPE_ATTRIBUTES
    Private Sub Build_Type_Attributes()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE TYPE_ATTRIBUTES ("
        SQL = SQL & "typeID INTEGER NOT NULL,"
        SQL = SQL & "attributeID INTEGER,"
        SQL = SQL & "valueInt INTEGER,"
        SQL = SQL & "valueFloat REAL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("dgmTypeAttributes")

        ' Pull new data and insert
        msSQL = "SELECT typeID, attributeID, valueInt, valueFloat FROM dgmTypeAttributes"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO TYPE_ATTRIBUTES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_TA_ATTRIBUTE_ID ON TYPE_ATTRIBUTES (attributeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_TA_TYPE_ID ON TYPE_ATTRIBUTES (typeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' ORES
    Private Sub Build_ORES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Application.DoEvents()

        SQL = "CREATE TABLE ORES ("
        SQL = SQL & "ORE_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "ORE_NAME VARCHAR(50),"
        SQL = SQL & "ORE_VOLUME REAL,"
        SQL = SQL & "UNITS_TO_REFINE INTEGER,"
        SQL = SQL & "BELT_TYPE VARCHAR(3),"
        SQL = SQL & "HIGH_YIELD_ORE INTEGER)"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT invTypes.typeID, invTypes.typeName, invTypes.volume, invTypes.portionSize, "
        msSQL = msSQL & "CASE WHEN invTypes.groupID = 465 THEN 'Ice' WHEN invTypes.groupID = 711 THEN 'Gas' ELSE 'Ore' END, "
        msSQL = msSQL & "CASE WHEN invTypes.typeName IN ('Arkonor','Bistot','Crokite','Dark Ochre','Gneiss','Hedbergite',  "
        msSQL = msSQL & "'Hemorphite','Jaspet','Kernite','Mercoxit','Omber','Plagioclase','Pyroxeres','Scordite','Spodumain','Veldspar') THEN 0 "
        msSQL = msSQL & "WHEN invTypes.groupID = 465 THEN -1 WHEN invTypes.groupID = 711 THEN -2 ELSE 1 END "
        msSQL = msSQL & "FROM invTypes, invGroups "
        msSQL = msSQL & "WHERE invTypes.groupID = invGroups.groupID "
        msSQL = msSQL & "AND (invGroups.categoryID = 25 OR invGroups.groupID = 711) " ' Clouds and Ores
        msSQL = msSQL & "AND invTypes.marketGroupID <> 0 "
        msSQL = msSQL & "GROUP BY invTypes.typeID, invTypes.typeName, invTypes.volume, invTypes.portionSize, "
        msSQL = msSQL & "CASE WHEN invTypes.groupID = 465 THEN 'Ice' WHEN invTypes.groupID = 711 THEN 'Gas' ELSE 'Ore' END, "
        msSQL = msSQL & "CASE WHEN invTypes.typeName IN ('Arkonor','Bistot','Crokite','Dark Ochre','Gneiss','Hedbergite', "
        msSQL = msSQL & "'Hemorphite','Jaspet','Kernite','Mercoxit','Omber','Plagioclase','Pyroxeres','Scordite','Spodumain','Veldspar') THEN 0 "
        msSQL = msSQL & "WHEN invTypes.groupID = 465 THEN -1 WHEN invTypes.groupID = 711 THEN -2 ELSE 1 END "

        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLQuery.CommandTimeout = 300
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO ORES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_ORES_ORE_ID ON ORES (ORE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' ORE_LOCATIONS
    Private Sub Build_ORE_LOCATIONS()
        Dim SQL As String

        Application.DoEvents()

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand

        SQL = "CREATE TABLE ORE_LOCATIONS ("
        SQL = SQL & "ORE_ID INTEGER NOT NULL,"
        SQL = SQL & "SYSTEM_SECURITY VARCHAR(10),"
        SQL = SQL & "SPACE VARCHAR(20),"
        SQL = SQL & "HIGH_YIELD_ORE INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)
        ' Now open the saved table and insert all the values into this new table

        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'High Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Low Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Low Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'High Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Low Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'High Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Low Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Low Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Low Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'High Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Low Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'High Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Low Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Low Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Low Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Amarr',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Caldari',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Minmatar',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Gallente',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C5','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C6','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C3','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C4','WH',0)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'High Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Low Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'High Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Low Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Low Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'High Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'Low Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'High Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'High Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Low Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Low Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'High Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Low Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'High Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Low Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Amarr',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Caldari',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Minmatar',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Gallente',1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17975,'Null Sec','Gallente',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17976,'Null Sec','Caldari',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17977,'Null Sec','Minmatar',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17978,'Null Sec','Amarr',-1)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Low Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Null Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Null Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Low Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25274,'Null Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25274,'Low Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Low Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25276,'Low Sec','Amarr',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25276,'Null Sec','Amarr',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25277,'Null Sec','Amarr',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25277,'Low Sec','Amarr',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Null Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Null Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Low Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25279,'Low Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28694,'Low Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28694,'Low Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28695,'Low Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28695,'Low Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28696,'High Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28696,'High Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28697,'Low Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28697,'High Sec','Caldari',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28698,'Low Sec','Amarr',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28699,'High Sec','Amarr',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28700,'High Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28700,'High Sec','Minmatar',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28701,'Low Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28701,'Low Sec','Gallente',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C1','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C2','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C1','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C2','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C1','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C2','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C1','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C2','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C1','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C2','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C3','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C4','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30377,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30377,'C6','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30378,'C5','WH',-2)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30378,'C6','WH',-2)", SQLiteDB)

        SQL = "CREATE INDEX IDX_ORE_LOCS_ORE_ID ON ORE_LOCATIONS (ORE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' REPROCESSING
    Private Sub Build_Reprocessing()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim mySQLReader2 As SqlDataReader
        Dim msSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE REPROCESSING ("
        SQL = SQL & "ITEM_ID INTEGER NOT NULL,"
        SQL = SQL & "ITEM_NAME VARCHAR(100),"
        SQL = SQL & "ITEM_VOLUME REAL,"
        SQL = SQL & "UNITS_TO_REPROCESS INTEGER,"
        SQL = SQL & "REFINED_MATERIAL_ID INTEGER,"
        SQL = SQL & "REFINED_MATERIAL VARCHAR(100),"
        SQL = SQL & "REFINED_MATERIAL_GROUP VARCHAR(100),"
        SQL = SQL & "REFINED_MATERIAL_VOLUME REAL,"
        SQL = SQL & "REFINED_MATERIAL_QUANTITY INTEGER"

        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        msSQL = "SELECT invTypes.typeID, invTypes.typeName, invTypes.volume, "
        msSQL = msSQL & "invTypes.portionSize, invTypes_1.typeID, invTypes_1.typeName, "
        msSQL = msSQL & "invGroups_1.groupName, invTypes_1.volume, "
        msSQL = msSQL & "invTypeMaterials.quantity "
        msSQL2 = "FROM ((((invTypes INNER JOIN invTypeMaterials ON invTypes.typeID = invTypeMaterials.typeID) "
        msSQL2 = msSQL2 & "INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) INNER JOIN invCategories ON invGroups.categoryID = invCategories.categoryID) "
        msSQL2 = msSQL2 & "INNER JOIN invTypes AS invTypes_1 ON invTypeMaterials.materialTypeID = invTypes_1.typeID) INNER JOIN (invGroups AS invGroups_1 "
        msSQL2 = msSQL2 & "INNER JOIN invCategories AS invCategories_1 ON invGroups_1.categoryID = invCategories_1.categoryID) ON invTypes_1.groupID = invGroups_1.groupID "

        ' Get the count
        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, SQLExpressConnection)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO REPROCESSING VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_REPRO_ITEM_MAT_ID ON REPROCESSING (ITEM_ID, REFINED_MATERIAL_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' REACTIONS
    Private Sub Build_Reactions()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim mySQLReader2 As SqlDataReader
        Dim msSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE REACTIONS ("
        SQL = SQL & "REACTION_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "REACTION_NAME VARCHAR(100),"
        SQL = SQL & "REACTION_GROUP VARCHAR(100),"
        SQL = SQL & "REACTION_TYPE VARCHAR(255),"
        SQL = SQL & "MATERIAL_TYPE_ID INTEGER,"
        SQL = SQL & "MATERIAL_NAME VARCHAR(100),"
        SQL = SQL & "MATERIAL_GROUP VARCHAR(100),"
        SQL = SQL & "MATERIAL_CATEGORY  VARCHAR(100),"
        SQL = SQL & "MATERIAL_QUANTITY INTEGER,"
        SQL = SQL & "MATERIAL_VOLUME REAL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT invTypeReactions.reactionTypeID, "
        msSQL = msSQL & "invTypes.typeName, "
        msSQL = msSQL & "invGroups.groupName, "
        msSQL = msSQL & "CASE WHEN invTypeReactions.input = 0 THEN 'Output' ELSE 'Input' END, "
        msSQL = msSQL & "invTypeReactions.typeID, invTypes_1.typeName, "
        msSQL = msSQL & "invGroups_1.groupName, "
        msSQL = msSQL & "invCategories_1.categoryName, "
        msSQL = msSQL & "CASE WHEN dgmTypeAttributes.valueFloat IS NULL THEN "
        msSQL = msSQL & "CASE WHEN dgmTypeAttributes.valueInt IS NULL THEN invTypeReactions.quantity ELSE (dgmTypeAttributes.valueInt * invTypeReactions.quantity) END "
        msSQL = msSQL & "ELSE dgmTypeAttributes.valueFloat END as quantity, "
        msSQL = msSQL & "invTypes_1.volume "

        msSQL2 = "FROM (((((invTypeReactions "
        msSQL2 = msSQL2 & "INNER JOIN invTypes ON invTypeReactions.reactionTypeID = invTypes.typeID) "
        msSQL2 = msSQL2 & "INNER JOIN invTypes AS invTypes_1 ON invTypeReactions.typeID = invTypes_1.typeID) "
        msSQL2 = msSQL2 & "INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) "
        msSQL2 = msSQL2 & "INNER JOIN invGroups AS invGroups_1 ON invTypes_1.groupID = invGroups_1.groupID) "
        msSQL2 = msSQL2 & "INNER JOIN invCategories AS invCategories_1 ON invGroups_1.categoryID = invCategories_1.categoryID) "
        msSQL2 = msSQL2 & "LEFT JOIN dgmTypeAttributes ON invTypeReactions.typeID = dgmTypeAttributes.typeID "
        msSQL2 = msSQL2 & "WHERE invTypes.published <> 0 AND invTypes_1.published <> 0 AND invCategories_1.published <> 0 AND invGroups.published <> 0 AND invGroups_1.published <> 0 "

        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, SQLExpressConnection)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO REACTIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_REACTION_MAT_TYPE_ID ON REACTIONS (MATERIAL_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_REACTION_MAT_GROUP ON REACTIONS (MATERIAL_GROUP)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' CURRENT_RESEARCH_AGENTS
    Private Sub Build_Current_Research_Agents()
        Dim SQL As String

        SQL = "CREATE TABLE CURRENT_RESEARCH_AGENTS ("
        SQL = SQL & "AGENT_ID INTEGER NOT NULL,"
        SQL = SQL & "SKILL_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "RP_PER_DAY FLOAT NOT NULL,"
        SQL = SQL & "RESEARCH_START_DATE VARCHAR(23) NOT NULL,"
        SQL = SQL & "REMAINDER_POINTS FLOAT NOT NULL,"
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CRA_AGENTID ON CURRENT_RESEARCH_AGENTS (AGENT_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CRA_CHARID ON CURRENT_RESEARCH_AGENTS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' SKILLS
    Private Sub Build_Skills()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim mySQLReader2 As SqlDataReader
        Dim msSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE SKILLS ("
        SQL = SQL & "SKILL_TYPE_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "SKILL_NAME VARCHAR(100),"
        SQL = SQL & "SKILL_GROUP VARCHAR(100)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT invTypes.typeID, invTypes.typeName, invGroups.groupName "
        msSQL2 = "FROM (invTypes INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) INNER JOIN invCategories ON invGroups.categoryID = invCategories.categoryID "
        msSQL2 = msSQL2 & "WHERE invCategories.categoryName='Skill' AND invTypes.published<>0 AND invGroups.published<>0 AND invCategories.published<>0"

        ' Get the count
        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, SQLExpressConnection)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO SKILLS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

    End Sub

    ' RESEARCH_AGENTS
    Private Sub Build_Research_Agents()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim mySQLReader2 As SqlDataReader
        Dim msSQL As String
        Dim msSQL2 As String

        Dim i As Integer

        SQL = "CREATE TABLE RESEARCH_AGENTS ("
        SQL = SQL & "FACTION VARCHAR(100) NOT NULL,"
        SQL = SQL & "CORPORATION_ID INTEGER,"
        SQL = SQL & "CORPORATION_NAME VARCHAR(100),"
        SQL = SQL & "AGENT_ID INTEGER,"
        SQL = SQL & "AGENT_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "LEVEL INTEGER,"
        SQL = SQL & "QUALITY INTEGER,"
        SQL = SQL & "RESEARCH_TYPE_ID INTEGER,"
        SQL = SQL & "RESEARCH_TYPE VARCHAR(100) NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER,"
        SQL = SQL & "REGION_NAME VARCHAR(100),"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER,"
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(100),"
        SQL = SQL & "SECURITY REAL,"
        SQL = SQL & "STATION VARCHAR(100)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT chrFactions.factionName, agtAgents.corporationID, "
        msSQL = msSQL & "invNames_1.itemName, "
        msSQL = msSQL & "invNames.itemID, "
        msSQL = msSQL & "invNames.itemName, "
        msSQL = msSQL & "agtAgents.level, "
        msSQL = msSQL & "agtAgents.quality, "
        msSQL = msSQL & "invTypes.typeID, "
        msSQL = msSQL & "invTypes.typeName, "
        msSQL = msSQL & "mapRegions.regionID, "
        msSQL = msSQL & "mapRegions.regionName, "
        msSQL = msSQL & "mapSolarSystems.solarSystemID, "
        msSQL = msSQL & "mapSolarSystems.solarSystemName, "
        msSQL = msSQL & "mapSolarSystems.security, "
        msSQL = msSQL & "mapDenormalize.itemName AS Station "
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
        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, SQLExpressConnection)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RESEARCH_AGENTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(10)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(11)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(12)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(13)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(14)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_RA_TYPE_CORP_ID ON RESEARCH_AGENTS (RESEARCH_TYPE, CORPORATION_NAME)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_RA_REGION_ID ON RESEARCH_AGENTS (REGION_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' Industry Jobs
    Private Sub Build_Industry_Jobs()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_JOBS ("
        SQL = SQL & "jobID INTEGER PRIMARY KEY, "
        SQL = SQL & "installerID INTEGER, "
        SQL = SQL & "installerName VARCHAR(100), "
        SQL = SQL & "facilityID INTEGER, "
        SQL = SQL & "solarSystemID INTEGER, "
        SQL = SQL & "solarSystemName VARCHAR(100), "
        SQL = SQL & "stationID INTEGER, "
        SQL = SQL & "activityID INTEGER, "
        SQL = SQL & "licensedRuns INTEGER, "
        SQL = SQL & "probability FLOAT, "
        SQL = SQL & "productTypeID INTEGER, "
        SQL = SQL & "productTypeName VARCHAR(100), "
        SQL = SQL & "status INTEGER, "
        SQL = SQL & "timeInSeconds INTEGER, "

        ' Dates
        SQL = SQL & "startDate VARCHAR(23), "
        SQL = SQL & "endDate VARCHAR(23), "
        SQL = SQL & "pauseDate VARCHAR(23), "
        SQL = SQL & "completedDate VARCHAR(23), "

        SQL = SQL & "completedCharacterID INTEGER,"
        SQL = SQL & "blueprintID INTEGER, "
        SQL = SQL & "blueprintTypeID INTEGER, "
        SQL = SQL & "blueprintTypeName VARCHAR(100), "
        SQL = SQL & "blueprintLocationID INTEGER, "
        SQL = SQL & "outputLocationID INTEGER, "
        SQL = SQL & "runs INTEGER, "
        SQL = SQL & "successfulRuns INTEGER, " ' Added Phoebe
        SQL = SQL & "cost FLOAT, "
        SQL = SQL & "teamID INTEGER, "
        SQL = SQL & "JobType INTEGER "
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' ASSETS
    Private Sub Build_Assets()
        Dim SQL As String

        SQL = "CREATE TABLE ASSETS ("
        SQL = SQL & "ID INTEGER NOT NULL,"
        SQL = SQL & "ItemID INTEGER NOT NULL,"
        SQL = SQL & "LocationID INTEGER NOT NULL,"
        SQL = SQL & "TypeID INTEGER NOT NULL,"
        SQL = SQL & "Quantity INTEGER NOT NULL,"
        SQL = SQL & "Flag INTEGER NOT NULL,"
        SQL = SQL & "Singleton INTEGER NOT NULL,"
        SQL = SQL & "RawQuantity INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC ON ASSETS (LocationID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ITEM_TYPEID ON ASSETS (TypeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' ASSET_LOCATIONS
    Private Sub Build_Asset_Locations()
        Dim SQL As String

        SQL = "CREATE TABLE ASSET_LOCATIONS ("
        SQL = SQL & "EnumAssetType INTEGER NOT NULL,"
        SQL = SQL & "ID INTEGER NOT NULL,"
        SQL = SQL & "LocationID INTEGER NOT NULL,"
        SQL = SQL & "FlagID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC_TYPE_ACCID ON ASSET_LOCATIONS (EnumAssetType, ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC_ACCOUNT_ID ON ASSET_LOCATIONS (ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' REGIONS
    Private Sub Build_REGIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE REGIONS ("
        SQL = SQL & "regionID INTEGER PRIMARY KEY,"
        SQL = SQL & "regionName VARCHAR(20) NOT NULL,"
        SQL = SQL & "factionID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB) ' SQLite table

        Application.DoEvents()

        msSQL = "SELECT regionID, regionName, factionID FROM mapRegions ORDER BY regionName"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            If msSQLReader.GetValue(0) = 11000001 Then
                Application.DoEvents()
            End If
            SQL = "INSERT INTO REGIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)
        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_R_REGION_NAME ON REGIONS (regionName)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_R_REGION_ID ON REGIONS (regionID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_R_FID ON REGIONS (factionID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' CONSTELLATIONS
    Private Sub Build_CONSTELLATIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE CONSTELLATIONS ("
        SQL = SQL & "regionID INTEGER NOT NULL,"
        SQL = SQL & "constellationID INTEGER PRIMARY KEY,"
        SQL = SQL & "constellationName VARCHAR(20) NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT regionID, constellationID, constellationName FROM mapConstellations"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO CONSTELLATIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)


        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_C_REGION_ID ON CONSTELLATIONS (regionID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' SOLAR_SYSTEMS
    Private Sub Build_SOLAR_SYSTEMS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE SOLAR_SYSTEMS ("
        SQL = SQL & "regionID INTEGER NOT NULL,"
        SQL = SQL & "constellationID INTEGER NOT NULL,"
        SQL = SQL & "solarSystemID INTEGER PRIMARY KEY,"
        SQL = SQL & "solarSystemName VARCHAR(17) NOT NULL,"
        SQL = SQL & "security REAL NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Pull new data and insert
        msSQL = "SELECT regionID, constellationID, solarSystemID, solarSystemName, security FROM mapSolarSystems"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO SOLAR_SYSTEMS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SS_REGION_ID ON SOLAR_SYSTEMS (regionID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SS_SS_ID ON SOLAR_SYSTEMS (solarSystemID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SS_CONSTELLATION_ID ON SOLAR_SYSTEMS (constellationID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SS_SYSTEM_NAME ON SOLAR_SYSTEMS (solarSystemName)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

#Region "Inventory Tables"

    ' INVENTORY_TYPES
    Private Sub Build_INVENTORY_TYPES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE INVENTORY_TYPES ("
        SQL = SQL & "typeID INTEGER PRIMARY KEY,"
        SQL = SQL & "groupID INTEGER,"
        SQL = SQL & "typeName VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & "),"
        SQL = SQL & "description VARCHAR(" & GetLenSQLExpField("description", "invTypes") & "),"
        SQL = SQL & "mass REAL,"
        SQL = SQL & "volume REAL,"
        SQL = SQL & "capacity REAL,"
        SQL = SQL & "portionSize INTEGER,"
        SQL = SQL & "factionID INTEGER,"
        SQL = SQL & "raceID INTEGER,"
        SQL = SQL & "basePrice REAL,"
        SQL = SQL & "published INTEGER,"
        SQL = SQL & "marketGroupID INTEGER,"
        SQL = SQL & "chanceOfDuplicating REAL,"
        SQL = SQL & "graphicID INTEGER,"
        SQL = SQL & "radius REAL,"
        SQL = SQL & "iconID INTEGER,"
        SQL = SQL & "soundID INTEGER,"
        SQL = SQL & "sofFactionName INTEGER,"
        SQL = SQL & "sofDnaAddition INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("invTypes")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invTypes "
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_TYPES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & "," ' TypeID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & "," ' GroupID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & "," ' TypeName
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & "," ' Description
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & "," ' Mass
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & "," ' Volume
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & "," ' Capacity
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & "," ' PortionSize
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & "," ' FactionID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & "," ' RaceID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(10)) & "," ' BasePrice
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(11)) & "," ' published
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(12)) & "," ' marketGroupID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(13)) & "," ' chanceofDuplicating
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(14)) & "," ' graphicID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(15)) & "," ' radius
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(16)) & "," ' iconID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(17)) & "," ' soundID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(18)) & "," ' sofFactionName
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(19)) & ")" ' sofDnaAddition

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_IT_GROUP_ID ON INVENTORY_TYPES (groupID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IT_TYPE_NAME_ID ON INVENTORY_TYPES (typeName)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IT_TYPE_ID ON INVENTORY_TYPES (typeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' INVENTORY_GROUPS
    Private Sub Build_INVENTORY_GROUPS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE INVENTORY_GROUPS ("
        SQL = SQL & "groupID INTEGER PRIMARY KEY,"
        SQL = SQL & "categoryID INTEGER,"
        SQL = SQL & "groupName VARCHAR(" & GetLenSQLExpField("groupName", "invGroups") & "),"
        SQL = SQL & "iconID INTEGER,"
        SQL = SQL & "useBasePrice INTEGER,"
        SQL = SQL & "anchored INTEGER,"
        SQL = SQL & "anchorable INTEGER,"
        SQL = SQL & "fittableNonSingleton INTEGER,"
        SQL = SQL & "published INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("invGroups")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invGroups"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_GROUPS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & "," ' groupID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & "," ' categoryID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & "," ' groupName
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & "," ' iconID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & "," ' useBasePrice
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & "," ' anchored
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & "," ' anchorable
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & "," ' fittableNonSingleton
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ")" ' published

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_IG_GROUP_ID ON INVENTORY_GROUPS (groupID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IG_CATEGORY_ID ON INVENTORY_GROUPS (categoryID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False

    End Sub

    ' INVENTORY_CATEGORIES
    Public Sub Build_INVENTORY_CATEGORIES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Integer

        SQL = "CREATE TABLE INVENTORY_CATEGORIES ("
        SQL = SQL & "categoryID INTEGER PRIMARY KEY,"
        SQL = SQL & "categoryName VARCHAR(" & GetLenSQLExpField("categoryName", "invCategories") & "),"
        SQL = SQL & "published INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("invCategories")

        ' Pull new data and insert
        msSQL = "SELECT categoryID, categoryName, published FROM invCategories"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_CATEGORIES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(CInt(msSQLReader.GetValue(2))) & ")" ' A bit value, but reads as a boolean for some reason

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        SQL = "CREATE INDEX IDX_IC_CATEGORY_ID ON INVENTORY_GROUPS (categoryID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        msSQLReader.Close()

        pgMain.Visible = False

    End Sub

    ' INVENTORY_FLAGS
    Private Sub Build_Inventory_Flags()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String
        Dim Temp As String

        Dim i As Integer

        SQL = "CREATE TABLE INVENTORY_FLAGS ("
        SQL = SQL & "FlagID INTEGER NOT NULL,"
        SQL = SQL & "FlagName VARCHAR(200) NOT NULL,"
        SQL = SQL & "FlagText VARCHAR(100) NOT NULL,"
        SQL = SQL & "OrderID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call SetProgressBarValues("invFlags")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invFlags"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_FLAGS VALUES ("

            Select Case CInt(msSQLReader.GetValue(0))
                Case 63, 64, 146, 147
                    ' Set these to None flag text
                    SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & "," & "'None','None',0)"
                Case Else
                    ' Just whatever is in the table
                    SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
                    SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
                    If CStr(msSQLReader.GetValue(2)).Contains("Corp Security Access Group") Then
                        ' Change name to corp hanger - save the number
                        Temp = BuildInsertFieldString(msSQLReader.GetValue(2))
                        SQL = SQL & "'Corp Hanger " & Temp.Substring(Len(Temp) - 2, 1) & "',"
                    Else
                        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
                    End If
                    SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ")"
            End Select

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i

        End While

        ' Add a final flag for space
        SQL = "INSERT INTO INVENTORY_FLAGS VALUES (" & CStr(SpaceFlagCode) & ",'Space','Space',0)"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_ITEM_FLAG_ID ON INVENTORY_FLAGS (FlagID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

#End Region

#Region "Item Price Tables"

    ' ITEM_PRICES
    Private Sub Build_ITEM_PRICES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim i As Long

        Application.DoEvents()

        ' See if the view exists and drop if it does
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'PRICES_BUILD'"
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_BUILD"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Build 2 queries and the Union, then pull data
        msSQL = "CREATE VIEW PRICES_BUILD AS "
        msSQL = msSQL & "SELECT ALL_BLUEPRINTS.ITEM_ID, "
        msSQL = msSQL & "ALL_BLUEPRINTS.ITEM_NAME, "
        msSQL = msSQL & "ALL_BLUEPRINTS.TECH_LEVEL, "
        msSQL = msSQL & "0 AS PRICE, "
        msSQL = msSQL & "ALL_BLUEPRINTS.ITEM_CATEGORY, "
        msSQL = msSQL & "ALL_BLUEPRINTS.ITEM_GROUP, "
        msSQL = msSQL & "1 AS MANUFACTURE, "
        msSQL = msSQL & "ALL_BLUEPRINTS.ITEM_TYPE,"
        msSQL = msSQL & "'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM ALL_BLUEPRINTS "
        msSQL = msSQL & "WHERE ITEM_ID <> 33195" ' For some reason spatial attunement Units are getting in here and NO Build, but they are no build items only

        Execute_msSQL(msSQL)

        ' See if the view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'PRICES_NOBUILD'"
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_NOBUILD"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE VIEW PRICES_NOBUILD AS "
        msSQL = msSQL & "SELECT invTypes.typeID AS ITEM_ID, "
        msSQL = msSQL & "invTypes.typeName AS ITEM_NAME, "
        msSQL = msSQL & "0 AS TECH_LEVEL, 0 AS PRICE, "
        msSQL = msSQL & "invCategories.categoryName AS ITEM_CATEGORY, "
        msSQL = msSQL & "invGroups.groupName AS ITEM_GROUP, "
        msSQL = msSQL & "0 AS MANUFACTURE, "
        msSQL = msSQL & "0 AS ITEM_TYPE, "
        msSQL = msSQL & "'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM (invTypes INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) "
        msSQL = msSQL & "INNER JOIN invCategories ON invGroups.categoryID = invCategories.categoryID "
        msSQL = msSQL & "WHERE ((invTypes.typeID Not In (SELECT ITEM_ID FROM ALL_BLUEPRINTS)) "
        msSQL = msSQL & "AND invCategories.categoryName Not In ('Module','Charge','Ship') "
        msSQL = msSQL & "AND invTypes.published <> 0 AND invGroups.published <> 0 and invCategories.published <> 0 "
        msSQL = msSQL & "AND invTypes.marketGroupID Is Not Null) "
        msSQL = msSQL & "AND ((invCategories.categoryID Not In (2,5,9,16,17,20,23,24,30,39,41)) "
        msSQL = msSQL & "OR (invCategories.categoryID=2 AND invGroups.groupID=711) "
        msSQL = msSQL & "OR (invCategories.categoryID=17 AND invGroups.groupID IN (333,528,530,716,732,733,734,735)) " '700's are storyline decryptors
        msSQL = msSQL & "OR (invTypes.typeID IN (3685, 3683, 3645, 9850, 41, 3773, 3687, 3727, 9826, 13267, 3721, 3699))) "
        ' Oxygen (3683), Water (3645), Garbage (41), Spirits (9850), Hydrogen Batteries (3685), Electronic Parts (3687), Hydrochloric Acid (3773)
        ' Plutonium (3727), Carbon (9826), Janitor (13267), Slaves (3721), Quafe (3699), 
        msSQL = msSQL & "OR invTypes.typeID IN (21815, 33195, 33539, 33577) " ' Spatial Attunement (33195), Shattered Villard Wheel (33539), 21815 - Elite Drone AI, (33577) Covert Research Tools
        msSQL = msSQL & "OR invTypes.typeID IN (3583,3584) " ' 3583	Badly Mangled Components, 3584	True Slave Decryption Node
        msSQL = msSQL & "OR invTypes.typeID IN (3812,3814) " ' 3812 Data Sheets, 3814 Reports
        msSQL = msSQL & "OR invTypes.typeID IN (27274,29203) " ' 27274 Villard Wheel, 29203 Minmatar DNA
        msSQL = msSQL & "OR invTypes.typeID IN (34575) " ' 34575 Antikythera Element

        Execute_msSQL(msSQL)

        ' See if the view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'PRICES_META'"
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_META"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE VIEW PRICES_META AS "
        msSQL = msSQL & "SELECT invTypes.typeID AS ITEM_ID, "
        msSQL = msSQL & "invTypes.typeName AS ITEM_NAME, "
        msSQL = msSQL & "1 AS TECH_LEVEL, "
        msSQL = msSQL & "0 AS PRICE, "
        msSQL = msSQL & "invCategories.categoryName AS ITEM_CATEGORY, "
        msSQL = msSQL & "invGroups.groupName AS ITEM_GROUP, "
        msSQL = msSQL & "0 AS MANUFACTURE, "
        msSQL = msSQL & "IsNull(dgmTypeAttributes.valueInt, dgmTypeAttributes.valueFloat)+20 AS ITEM_TYPE, "
        msSQL = msSQL & "'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM invCategories "
        msSQL = msSQL & "INNER JOIN ((invTypes "
        msSQL = msSQL & "INNER JOIN dgmTypeAttributes ON invTypes.typeID = dgmTypeAttributes.typeID) "
        msSQL = msSQL & "INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) ON invCategories.categoryID = invGroups.categoryID "
        msSQL = msSQL & "WHERE (IsNull(dgmTypeAttributes.valueInt,dgmTypeAttributes.valueFloat) + 20 >20) "
        msSQL = msSQL & "AND (IsNull(dgmTypeAttributes.valueInt,dgmTypeAttributes.valueFloat) + 20 < 25) "
        msSQL = msSQL & "AND dgmTypeAttributes.attributeID = 633 "
        msSQL = msSQL & "AND (invTypes.published<>0 AND invCategories.published<>0 AND invGroups.published<>0) "
        msSQL = msSQL & "AND invTypes.typeID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD) AND invTypes.typeID NOT IN (SELECT ITEM_ID FROM PRICES_NOBUILD)"

        Execute_msSQL(msSQL)

        ' See if the union view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'ITEM_PRICES_UNION'"
        msSQLQuery = New SqlCommand(SQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW ITEM_PRICES_UNION"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        SQL = "CREATE VIEW ITEM_PRICES_UNION AS SELECT * FROM PRICES_BUILD UNION SELECT * FROM PRICES_NOBUILD UNION SELECT * FROM PRICES_META"
        Execute_msSQL(SQL)

        ' Create SQLite table
        SQL = "CREATE TABLE ITEM_PRICES ("
        SQL = SQL & "ITEM_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "ITEM_NAME VARCHAR(" & GetLenSQLExpField("ITEM_NAME", "ITEM_PRICES_UNION") & ") NOT NULL,"
        SQL = SQL & "TECH_LEVEL INTEGER NOT NULL,"
        SQL = SQL & "PRICE REAL,"
        SQL = SQL & "ITEM_CATEGORY VARCHAR(" & GetLenSQLExpField("ITEM_CATEGORY", "ITEM_PRICES_UNION") & ") NOT NULL,"
        SQL = SQL & "ITEM_GROUP VARCHAR(" & GetLenSQLExpField("ITEM_GROUP", "ITEM_PRICES_UNION") & ") NOT NULL,"
        SQL = SQL & "MANUFACTURE INTEGER NOT NULL,"
        SQL = SQL & "ITEM_TYPE INTEGER NOT NULL,"
        SQL = SQL & "PRICE_TYPE VARCHAR(20) NOT NULL,"
        SQL = SQL & "ADJUSTED_PRICE FLOAT NOT NULL,"
        SQL = SQL & "AVERAGE_PRICE FLOAT NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ITEM_PRICES_UNION")

        ' Now select the final query of data
        msSQL = "SELECT * FROM ITEM_PRICES_UNION"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        ' Insert the data into the table
        While msSQLReader.Read
            SQL = "INSERT INTO ITEM_PRICES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ",0,0)" ' For Adjusted market price and Average market price from CREST

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

            i += 1
            pgMain.Value = i
        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_IP_GROUP ON ITEM_PRICES (ITEM_GROUP)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IP_TYPE ON ITEM_PRICES (ITEM_TYPE)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IP_CATEGORY ON ITEM_PRICES (ITEM_CATEGORY)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' ITEM_PRICES_CACHE
    Private Sub Build_ITEM_PRICES_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE ITEM_PRICES_CACHE ("
        SQL = SQL & "typeID INTEGER NOT NULL,"
        SQL = SQL & "allVolume REAL NOT NULL,"
        SQL = SQL & "allAvg REAL NOT NULL,"
        SQL = SQL & "allMax REAL NOT NULL,"
        SQL = SQL & "allMin REAL NOT NULL,"
        SQL = SQL & "allStdDev REAL NOT NULL,"
        SQL = SQL & "allMedian REAL NOT NULL,"
        SQL = SQL & "allPercentile REAL," ' make not null
        SQL = SQL & "buyVolume REAL NOT NULL,"
        SQL = SQL & "buyAvg REAL NOT NULL,"
        SQL = SQL & "buyMax REAL NOT NULL,"
        SQL = SQL & "buyMin REAL NOT NULL,"
        SQL = SQL & "buyStdDev REAL NOT NULL,"
        SQL = SQL & "buyMedian REAL NOT NULL,"
        SQL = SQL & "buyPercentile REAL," ' make not null
        SQL = SQL & "sellVolume REAL NOT NULL,"
        SQL = SQL & "sellAvg REAL NOT NULL,"
        SQL = SQL & "sellMax REAL NOT NULL,"
        SQL = SQL & "sellMin REAL NOT NULL,"
        SQL = SQL & "sellStdDev REAL NOT NULL,"
        SQL = SQL & "sellMedian REAL NOT NULL,"
        SQL = SQL & "sellPercentile REAL," ' make not null
        SQL = SQL & "RegionList VARCHAR(65535) NOT NULL," ' Memo is Up to 65,535 characters
        SQL = SQL & "UpdateDate VARCHAR(23) NOT NULL" ' Date
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IPC_TYPEID ON ITEM_PRICES_CACHE (typeID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IPC_ID_REGION ON ITEM_PRICES_CACHE (typeID, RegionList)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' EMD_ITEM_PRICE_HISTORY
    Private Sub Build_EMD_Item_Price_History()
        Dim SQL As String

        SQL = "CREATE TABLE EMD_ITEM_PRICE_HISTORY ("
        SQL = SQL & "TypeID INTEGER,"
        SQL = SQL & "RegionID INTEGER,"
        SQL = SQL & "PriceDate VARCHAR(23)," ' Date
        SQL = SQL & "LowPrice FLOAT,"
        SQL = SQL & "HighPrice FLOAT,"
        SQL = SQL & "AvgPrice FLOAT,"
        SQL = SQL & "Volume INTEGER,"
        SQL = SQL & "Orders INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE UNIQUE INDEX IDX_EMD_HISTORY ON EMD_ITEM_PRICE_HISTORY (TypeID, RegionID, PriceDate)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' EMD_UPDATE_HISTORY
    Private Sub Build_EMD_Update_History()
        Dim SQL As String

        SQL = "CREATE TABLE EMD_UPDATE_HISTORY ("
        SQL = SQL & "TypeID INTEGER,"
        SQL = SQL & "Days INTEGER,"
        SQL = SQL & "RegionID INTEGER,"
        SQL = SQL & "UpdateLastRan VARCHAR(23)" ' Date
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE UNIQUE INDEX IDX_EMD_U_HISTORY ON EMD_UPDATE_HISTORY (TypeID, Days, RegionID, UpdateLastRan)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' MARKET_HISTORY
    Private Sub Build_MARKET_HISTORY()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_HISTORY ("
        SQL = SQL & "TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL,"
        SQL = SQL & "VOLUME INTEGER NOT NULL,"
        SQL = SQL & "LOW_PRICE FLOAT NOT NULL,"
        SQL = SQL & "HIGH_PRICE FLOAT NOT NULL,"
        SQL = SQL & "AVERAGE_PRICE FLOAT NOT NULL,"
        SQL = SQL & "ORDER_COUNT INTEGER NOT NULL,"
        SQL = SQL & "DATE VARCHAR(23)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_MH_TID_RID ON MARKET_HISTORY (TYPE_ID, REGION_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' MARKET_HISTORY_UPDATE_CACHE
    Private Sub Build_MARKET_HISTORY_UPDATE_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_HISTORY_UPDATE_CACHE ("
        SQL = SQL & "TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL,"
        SQL = SQL & "CACHE_DATE VARCHAR(23)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_MHUC_TID_RID ON MARKET_HISTORY_UPDATE_CACHE (TYPE_ID, REGION_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

#End Region

#Region "CREST Tables"

    ' INDUSTRY_SPECIALTIES
    Private Sub Build_INDUSTRY_SPECIALTIES()
        Dim SQL As String

        ' Build two tables here
        SQL = "CREATE TABLE INDUSTRY_GROUP_SPECIALTIES ("
        SQL = SQL & "GROUP_ID INTEGER NOT NULL,"
        SQL = SQL & "SPECIALTY_GROUP_ID INTEGER NOT NULL,"
        SQL = SQL & "SPECIALTY_GROUP_NAME VARCHAR(100) NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_SGID_GID ON INDUSTRY_GROUP_SPECIALTIES (SPECIALTY_GROUP_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE TABLE INDUSTRY_CATEGORY_SPECIALTIES ("
        SQL = SQL & "SPECIALTY_CATEGORY_ID INTEGER NOT NULL,"
        SQL = SQL & "SPECIALTY_CATEGORY_NAME VARCHAR(100) NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_CAT_ID ON INDUSTRY_CATEGORY_SPECIALTIES (SPECIALTY_CATEGORY_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' INDUSTRY_TEAMS
    Private Sub Build_INDUSTRY_TEAMS()
        Dim SQL As String

        ' Create two tables for teams
        SQL = "CREATE TABLE INDUSTRY_TEAMS ("
        SQL = SQL & "TEAM_ID INTEGER PRIMARY_KEY,"
        SQL = SQL & "TEAM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "TEAM_ACTIVITY_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(10) NOT NULL,"
        SQL = SQL & "COST_MODIFIER FLOAT NOT NULL,"
        SQL = SQL & "CREATION_TIME VARCHAR(23) NOT NULL,"
        SQL = SQL & "EXPIRY_TIME VARCHAR(23) NOT NULL,"
        SQL = SQL & "SPECIALTY_CATEGORY_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_TEAMS_TEAM_ID ON INDUSTRY_TEAMS (TEAM_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_TEAMS_ACTIVITY_ID ON INDUSTRY_TEAMS (TEAM_ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_TEAMS_TEAM_NAME ON INDUSTRY_TEAMS (TEAM_NAME)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE TABLE INDUSTRY_TEAMS_BONUSES ("
        SQL = SQL & "TEAM_ID INTEGER NOT NULL,"
        SQL = SQL & "TEAM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "BONUS_ID INTEGER NOT NULL,"
        SQL = SQL & "BONUS_TYPE STRING NOT NULL,"
        SQL = SQL & "BONUS_VALUE FLOAT NOT NULL,"
        SQL = SQL & "SPECIALTY_GROUP_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_BONUSES_ID_GID ON INDUSTRY_TEAMS_BONUSES (TEAM_ID, SPECIALTY_GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_BONUSES_NAME_GID ON INDUSTRY_TEAMS_BONUSES (TEAM_NAME, SPECIALTY_GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' INDUSTRY_TEAMS_AUCTIONS
    Private Sub Build_INDUSTRY_TEAMS_AUCTIONS()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_TEAMS_AUCTIONS ("
        SQL = SQL & "TEAM_ID INTEGER PRIMARY_KEY,"
        SQL = SQL & "TEAM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "TEAM_ACTIVITY_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "COST_MODIFIER FLOAT NOT NULL,"
        SQL = SQL & "CREATION_TIME VARCHAR(23)," ' Date
        SQL = SQL & "EXPIRY_TIME VARCHAR(23)," ' Date
        SQL = SQL & "SPECIALTY_CATEGORY_ID INTEGER NOT NULL,"
        SQL = SQL & "AUCTION_ID INTEGER NOT NULL PRIMARY KEY"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AUCTIONS_TEAM_ID ON INDUSTRY_TEAMS_AUCTIONS (TEAM_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AUCTIONS_CTIVITY_ID ON INDUSTRY_TEAMS_AUCTIONS (TEAM_ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AUCTIONS_TEAM_NAME ON INDUSTRY_TEAMS_AUCTIONS (TEAM_NAME)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_AUCTIONS_AUCTION_ID ON INDUSTRY_TEAMS_AUCTIONS (AUCTION_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Make the Bids table too
        SQL = "CREATE TABLE INDUSTRY_TEAMS_AUCTIONS_BIDS ("
        SQL = SQL & "AUCTION_ID INTEGER NOT NULL," ' Links to auctions
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "BID_AMOUNT FLOAT NOT NULL,"
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "CHARACTER_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "IS_CHARACTER_NPC INTEGER NOT NULL,"
        SQL = SQL & "CHARACTER_BID FLOAT NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_BIDS_AUCTION_ID ON INDUSTRY_TEAMS_AUCTIONS_BIDS (AUCTION_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' INDUSTRY_SYSTEMS_COST_INDICIES
    Private Sub Build_INDUSTRY_SYSTEMS_COST_INDICIES()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_SYSTEMS_COST_INDICIES ("
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "ACTIVITY_ID INTEGER NOT NULL,"
        SQL = SQL & "ACTIVITY_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "COST_INDEX FLOAT NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_ISCI_SSID_AID ON INDUSTRY_SYSTEMS_COST_INDICIES (SOLAR_SYSTEM_ID, ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

    ' INDUSTRY_FACILITIES
    Private Sub Build_INDUSTRY_FACILITIES()
        Dim SQL As String

        SQL = "CREATE TABLE INDUSTRY_FACILITIES ("
        SQL = SQL & "FACILITY_ID INTEGER NOT NULL,"
        SQL = SQL & "FACILITY_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "FACILITY_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "FACILITY_TAX FLOAT NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL,"
        SQL = SQL & "OWNER_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IF_MAIN ON INDUSTRY_FACILITIES (FACILITY_TYPE_ID, REGION_ID, SOLAR_SYSTEM_ID, FACILITY_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IF_SSID ON INDUSTRY_FACILITIES (SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_IF_FTID ON INDUSTRY_FACILITIES (FACILITY_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

    End Sub

#End Region

#Region "PI Tables"

    ' PLANET_RESOURCES
    Private Sub Build_PLANET_RESOURCES()
        Dim SQL As String
        SQL = "CREATE TABLE PLANET_RESOURCES ("
        SQL = SQL & "PLANET_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "RESOURCE_TYPE_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2073)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2073)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2073)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2073)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2267)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2267)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2267)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2267)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2267)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2268)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2268)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2268)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2268)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2268)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2268)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2270)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2270)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2272)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2272)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2272)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2286)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2286)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2287)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2287)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2288)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2288)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2288)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2305)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2306)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2306)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2307)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2308)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2308)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2308)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2309)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2309)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2310)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2310)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2310)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2310)", SQLiteDB)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2311)", SQLiteDB)

        ' Indexes
        SQL = "CREATE INDEX IDX_PR_PTID_RTID ON PLANET_RESOURCES (PLANET_TYPE_ID, RESOURCE_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        SQL = "CREATE INDEX IDX_PR_PTID ON PLANET_RESOURCES (PLANET_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' PLANET_SCHEMATICS
    Private Sub Build_PLANET_SCHEMATICS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE PLANET_SCHEMATICS ("
        SQL = SQL & "schematicID INTEGER PRIMARY KEY,"
        SQL = SQL & "schematicName VARCHAR(50) NOT NULL,"
        SQL = SQL & "cycleTime INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM planetSchematics"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID ON PLANET_SCHEMATICS (schematicID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' PLANET_SCHEMATICS_TYPE_MAP
    Private Sub Build_PLANET_SCHEMATICS_TYPE_MAP()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE PLANET_SCHEMATICS_TYPE_MAP ("
        SQL = SQL & "schematicID INTEGER NOT NULL,"
        SQL = SQL & "typeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER NOT NULL,"
        SQL = SQL & "isInput INTEGER NOT NULL,"
        SQL = SQL & "PRIMARY KEY (schematicID, typeID)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM planetSchematicsTypeMap"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS_TYPE_MAP VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID_TMAP ON PLANET_SCHEMATICS_TYPE_MAP (schematicID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' PLANET_SCHEMATICS_PIN_MAP
    Private Sub Build_PLANET_SCHEMATICS_PIN_MAP()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE PLANET_SCHEMATICS_PIN_MAP ("
        SQL = SQL & "schematicID INTEGER NOT NULL,"
        SQL = SQL & "pintypeID INTEGER NOT NULL,"
        SQL = SQL & "PRIMARY KEY (schematicID, pintypeID)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM planetSchematicsPinMap"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call BeginSQLiteTransaction(SQLiteDB)

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS_PIN_MAP VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, SQLiteDB)

        End While

        Call CommitSQLiteTransaction(SQLiteDB)

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID_PIN_MAP ON PLANET_SCHEMATICS_PIN_MAP (schematicID)"
        Call Execute_SQLiteSQL(SQL, SQLiteDB)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

#End Region

    ' TO DO - Rebuild Adds/updates data for outpost items - these don't have BPs so I add them as BPs here (ie an egg has a bp but when deploying it doesn't but takes mats)
    'Private Sub AddOutpostData()
    '    Dim SQL As String

    '    ' Update some typeIDs stuff first so it will look up the type materials correctly
    '    '28183	Rank 1 Upgrade	27656	Foundation Upgrade Platform
    '    '28184	Rank 2 Upgrade	27658	Pedestal Upgrade Platform
    '    '28185	Rank 3 Upgrade	27660	Monument Upgrade Platform

    '    SQL = "UPDATE invTypeMaterials SET typeID = 27656 where typeID = 28183"
    '    Call Execute_msSQL(SQL)

    '    SQL = "UPDATE invTypeMaterials SET typeID = 27658 where typeID = 28184"
    '    Call Execute_msSQL(SQL)

    '    SQL = "UPDATE invTypeMaterials SET typeID = 27660 where typeID = 28185"
    '    Call Execute_msSQL(SQL)

    '    ' Update the published field for stations and customs offices in groups and categories
    '    SQL = "UPDATE invGroups SET published = -1 where groupID IN (15,1025)"
    '    Call Execute_msSQL(SQL)

    '    SQL = "UPDATE invCategories SET published = -1 where categoryID IN (3,46)"
    '    Call Execute_msSQL(SQL)

    '    SQL = "SELECT * FROM [OutpostCustoms invBlueprintTypes] "
    '    DBCommand = New OleDbCommand(SQL, DB_ADDL_DATA)
    '    readerDB = DBCommand.ExecuteReader

    '    While readerDB.Read
    '        Application.DoEvents()

    '        ' Delete the record first
    '        SQL = "DELETE FROM invBlueprintTypes WHERE blueprintTypeID = " & readerDB(0).ToString
    '        Call Execute_msSQL(SQL)

    '        ' Insert each value
    '        SQL = "INSERT INTO invBlueprintTypes VALUES ("
    '        SQL = SQL & BuildInsertFieldString(readerDB(0).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(1).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(2).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(3).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(4).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(5).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(6).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(7).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(8).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(9).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(10).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(11).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(12).ToString) & ")"

    '        Call Execute_msSQL(SQL)

    '        ' Ignore upgrade platforms
    '        If readerDB(0).ToString <> 27656 And readerDB(0).ToString <> 27658 And readerDB(0).ToString <> 27660 Then

    '            ' Update the published field for this "BP" in types
    '            SQL = "UPDATE invTypes SET published = -1 where typeID = " & readerDB(0).ToString
    '            Call Execute_msSQL(SQL)

    '            ' Update the typeName and the marketGroupID in invTypes
    '            SQL = "SELECT typeName, marketGroupID FROM [OutpostCustoms invTypes] WHERE typeID = " & readerDB(0).ToString
    '            DBCommand = New OleDbCommand(SQL, DB_ADDL_DATA)
    '            readerDB2 = DBCommand.ExecuteReader

    '            readerDB2.Read()

    '            SQL = "UPDATE invTypes SET typeName = '" & readerDB2.GetString(0) & "', marketGroupID = "
    '            SQL = SQL & readerDB2.GetValue(1) & " WHERE typeID = " & readerDB(0).ToString
    '            Call Execute_msSQL(SQL)

    '        End If

    '    End While

    '    ' Get all the requirements that I think we need extra to this "BP" (mostly skills for outpost deployment)
    '    SQL = "SELECT * FROM [OutpostCustoms ramTypeRequirements] "
    '    DBCommand = New OleDbCommand(SQL, DB_ADDL_DATA)
    '    readerDB = DBCommand.ExecuteReader

    '    While readerDB.Read
    '        Application.DoEvents()

    '        ' Delete the record first
    '        SQL = "DELETE FROM ramTypeRequirements WHERE typeID = " & readerDB(0).ToString & " AND requiredTypeID = " & readerDB(2).ToString
    '        Call Execute_msSQL(SQL)

    '        ' Insert each value
    '        SQL = "INSERT INTO ramTypeRequirements VALUES ("
    '        SQL = SQL & BuildInsertFieldString(readerDB(0).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(1).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(2).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(3).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(4).ToString) & ","
    '        SQL = SQL & BuildInsertFieldString(readerDB(5).ToString) & ")"

    '        Call Execute_msSQL(SQL)

    '    End While

    '    DB_ADDL_DATA.Close()

    'End Sub

#Region "Build SQL DB"

    ' Copies all the data from the universe DB into the MSSQL DB
    Private Sub btnBuildSQLServerDB_Click(sender As System.Object, e As System.EventArgs) Handles btnBuildSQLServerDB.Click

        ' Make sure we have a DB first
        If DatabaseName = "" Then
            MsgBox("Database Name not defined", vbExclamation, Application.ProductName)
            Call txtDBName.Focus()
            Exit Sub
        Else
            txtDBName.Text = DatabaseName
        End If

        Me.Cursor = Cursors.WaitCursor
        btnBuildDatabase.Enabled = False
        btnBuildSQLServerDB.Enabled = False
        btnImageCopy.Enabled = False

        If Not ConnectToDBs() Then
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnBuildSQLServerDB.Enabled = True
            btnImageCopy.Enabled = True
            Exit Sub
        End If

        ' First load the YMAL tables and data (need to add all ymal's to keep db updated to current)
        Call Load_YMAL_Blueprints()
        Call Load_YMAL_invTypes()
        Call Load_YMAL_invGroups()
        Call Load_YMAL_invCategories()

        'Call Load_YMAL_Icons() ' not currently using

        'Do all random updates here first
        Call RandomSDEUpdates()

        ' Build all the universe tables from SQLite 
        lblTableName.Text = "Building: mapCelestialStatistics"
        Call Build_mapCelestialStatistics()

        lblTableName.Text = "Building: mapConstellationJumps"
        Call Build_mapConstellationJumps()

        lblTableName.Text = "Building: mapConstellations"
        Call Build_mapConstellations()

        lblTableName.Text = "Building: mapDenormalize"
        Call Build_mapDenormalize()

        lblTableName.Text = "Building: mapJumps"
        Call Build_mapJumps()

        lblTableName.Text = "Building: mapLandmarks"
        Call Build_mapLandmarks()

        lblTableName.Text = "Building: mapLocationScenes"
        Call Build_mapLocationScenes()

        lblTableName.Text = "Building: mapLocationWormholeClasses"
        Call Build_mapLocationWormholeClasses()

        lblTableName.Text = "Building: mapRegionJumps"
        Call Build_mapRegionJumps()

        lblTableName.Text = "Building: mapRegions"
        Call Build_mapRegions()

        lblTableName.Text = "Building: mapSolarSystemJumps"
        Call Build_mapSolarSystemJumps()

        lblTableName.Text = "Building: mapSolarSystems"
        Call Build_mapSolarSystems()

        ' When rebuilding the DB, update the ramAssemblyLineTypeDetailPerCategory table
        ' so it is complete and not missing categories of blueprints for assembly lines in 
        ' the ramAssemblyLineTypes table - this will (should!) speed up updates for CREST facilities
        lblTableName.Text = "Updating ramAssemblyLineTypeDetailPerCategory"
        Call UpdateramAssemblyLineTypeDetailPerCategory()

        MsgBox("Build Complete")
        pgMain.Visible = False
        lblTableName.Text = ""

        Call CloseDBs()

        Me.Cursor = Cursors.Default
        Application.UseWaitCursor = False
        btnBuildDatabase.Enabled = True
        btnBuildSQLServerDB.Enabled = True
        btnImageCopy.Enabled = True

        Application.DoEvents()

    End Sub

#Region "YMAL"

    ' Determines length of spaces (indent) in string sent
    Private Function GetNumSpaces(inString As String) As Integer
        Dim SpaceCount As Integer = 0

        For Each c In inString
            If c <> " " Then
                Exit For
            End If
            SpaceCount += 1
        Next

        Return SpaceCount

    End Function

    ' Returns a boolean if there is a quote beginning the string sent - used for multi-line scalars
    Private Function StartQuotation(CheckString As String) As Boolean

        ' Find the first instance of the colon
        Dim firstColon As Integer = InStr(CheckString, COLON)

        ' If the first instance is at the end of the string, we can't search it
        If firstColon = Len(CheckString) Then
            Return False
        End If

        ' Get the first char after the colon and space of the header - will always be after the colon
        Dim firstChar As Char = CheckString.Substring(InStr(CheckString, COLON) + 1, 1)

        ' Look for single or double quotes
        If firstChar = Chr(ASCII_Quote_Code) Or firstChar = Chr(ASCII_DoubleQuote_Code) Then
            Return True
        Else
            Return False
        End If

    End Function

    ' Returns a boolean if there is a quote ending the string sent - used for multi-line scalars
    Private Function EndQuotation(CheckString As String) As Boolean

        ' Get the last char
        Dim lastChar As Char = CheckString.Substring(Len(CheckString) - 1, 1)

        ' Look for single or double quotes
        If lastChar = Chr(ASCII_Quote_Code) Or lastChar = Chr(ASCII_DoubleQuote_Code) Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function CompareIndentSizes(FirstLine As String, SecondLine As String) As Boolean
        Dim FirstIndentCount As Integer = 0
        Dim SecondIndentCount As Integer = 0

        For Each c In FirstLine
            If c = " " Then
                FirstIndentCount += 1
            Else
                Exit For
            End If
        Next

        For Each c In SecondLine
            If c = " " Then
                SecondIndentCount += 1
            Else
                Exit For
            End If
        Next

        If FirstIndentCount = SecondIndentCount Then
            Return True
        Else
            Return False
        End If

    End Function

    ' Takes a file and parses the file into a tree for searching - This is completely hacked. I hate YAML and I really don't give a shit if it doesn't work for all files
    Private Function ParseYMALFile(FileName As String) As TreeNode
        ' Use the tree node object with vb, Name is the node name, text is the value of the node - all we need to use here
        Dim CurrentNode As New TreeNode
        Dim BaseNode As New TreeNode
        Dim AnchorNode As New TreeNode

        Dim TreeLevel As Integer = 0 ' Records what tree level we are on
        Dim PreviousLevel As Integer = 0 ' Save the previous tree level
        Dim NodeName As String = ""
        Dim ActiveQuote As Boolean = False

        Dim k As Integer = 0 ' For tracking sequence items

        Dim Sequence As Boolean = False
        Dim SequenceName As String = ""

        TreeView.Nodes.Clear()
        AnchorNode = TreeView.Nodes.Add("Root Node")

        ' Load the yaml file into a string array for easier processing
        Dim Lines As String() = IO.File.ReadAllLines(FileName)

        ' Set the block and index values
        Dim SpaceCount As Integer = GetNumSpaces(Lines(1))
        INDENT = ""
        For i = 0 To SpaceCount - 1
            ' Build the indent variable
            INDENT = INDENT & " "
        Next

        ' Base loop to read the file
        For i = 0 To Lines.Count - 1
            Application.DoEvents()
            If Lines(i) <> "" Then

                ' Get the next lines till a value is found and add it to this item then clear
                If i <> Lines.Count - 1 Then
                    For j = i + 1 To Lines.Count - 1

                        ' See if this line(i) starts a multi-line quote if it ends it, then set it to false
                        If StartQuotation(Lines(i)) And Not EndQuotation(Lines(i)) Then
                            ActiveQuote = True
                        Else
                            ActiveQuote = False
                        End If

                        Dim LastChar As String = ""
                        If Lines(j) <> "" Then
                            LastChar = Lines(j).Substring(Len(Lines(j)) - 1, 1)
                        End If

                        ' Check indents, if not the same then potential multi-line
                        Dim SameIndentSize As Boolean = CompareIndentSizes(Lines(i), Lines(j))

                        ' If the next line is part of a quote, or doesn't have a colon space, or ends with a colon or contains a block sequence at the begining, then it's multi-line
                        If Not (Lines(j).Contains(COLON_SPACE) Or LastChar = COLON Or SameIndentSize) Or ActiveQuote Then

                            ' Reset the original
                            Dim Tempstring As String = Lines(i) & " " & Trim(Lines(j))
                            Lines(i) = Tempstring

                            ' Reset next
                            Lines(j) = ""
                        Else
                            Exit For
                        End If
                    Next
                End If

                ' Get the name of the node without the colon
                If TextContains(Trim(Lines(i)), BLOCK_4SEQUENCE) Then
                    ' Trim the dash from the front
                    NodeName = Trim(Lines(i).Substring(InStr(Lines(i), BLOCK_4SEQUENCE) + 1))
                    Sequence = True
                    k += 1 ' increment each sequence
                ElseIf TextContains(Trim(Lines(i)), BLOCK_2SEQUENCE) Then
                    ' Trim the dash from the front
                    NodeName = Trim(Lines(i).Substring(InStr(Lines(i), BLOCK_2SEQUENCE) + 1))
                    Sequence = True
                    k += 1 ' increment each sequence
                Else
                    NodeName = Trim(Lines(i))
                    Sequence = False
                End If

                ' Delete everything after the colon for the name
                If NodeName.Contains(COLON) Then
                    NodeName = Trim(NodeName.Substring(0, InStr(NodeName, COLON) - 1))
                End If

                ' See if we are at the beginning of a block
                If TextContains(Lines(i), INDENT) Then
                    ' We have an indent, so save the tree level of the indent (for assigning nodes to the correct tree)
                    TreeLevel = GetTreeLevel(Lines(i))

                    ' Anytime we read something that is in a column less than the current, then we need to find the right parent
                    If PreviousLevel > TreeLevel Then
                        ' Need to set the right node to the parent by iterating back to find the right level
                        For j = 1 To PreviousLevel - TreeLevel
                            If CurrentNode.Text.Contains(SequenceLabel) And Not Sequence Then
                                ' If we are on a sequence and it's reset, then we have to go back 2 nodes since we added sequence
                                CurrentNode = CurrentNode.Parent.Parent
                            Else
                                CurrentNode = CurrentNode.Parent
                            End If
                        Next
                    End If

                    ' If we start a sequence, then add a new node for this one item of the parent to add the values to
                    If Sequence Then
                        SequenceName = SequenceLabel & " " & CStr(k)
                        CurrentNode = CurrentNode.Nodes.Add(SequenceName)
                        CurrentNode.Name = SequenceName
                        ' We added a new level, so increment
                        TreeLevel += 1
                    End If

                    ' If the value has a colon at the end or a dash, then it is a tree node
                    ' if not, it is a key value pair, or the end node in that branch
                    If TextContains(Lines(i).Substring(Len(Lines(i)) - 1, 1), COLON) Then

                        ' Add a new node to this branch with this name
                        If TreeLevel = 1 Then
                            ' At the base level, need to add nodes to the base
                            CurrentNode = BaseNode.Nodes.Add(NodeName)
                        Else
                            ' Add to the current node, anything not mapped is a subnode of the current
                            CurrentNode = CurrentNode.Nodes.Add(NodeName)
                        End If

                        CurrentNode.Name = NodeName

                        ' Reset sequence numbers
                        k = 0

                    Else ' Must be a value here now
                        ' Add a node to the branch
                        If IsNothing(CurrentNode) Then
                            If Lines(i).Contains(COLON) Then
                                CurrentNode = BaseNode.Nodes.Add(Trim(Lines(i).Substring(InStr(Lines(i), COLON)))) ' Save the value
                            ElseIf Lines(i).Contains(BLOCK_2SEQUENCE) Then
                                CurrentNode = BaseNode.Nodes.Add(Trim(Lines(i).Substring(InStr(Lines(i), BLOCK_2SEQUENCE)))) ' Save the value
                            ElseIf Lines(i).Contains(BLOCK_4SEQUENCE) Then
                                CurrentNode = BaseNode.Nodes.Add(Trim(Lines(i).Substring(InStr(Lines(i), BLOCK_4SEQUENCE)))) ' Save the value
                            End If
                        Else
                            If Lines(i).Contains(COLON) Then
                                CurrentNode = CurrentNode.Nodes.Add(Trim(Lines(i).Substring(InStr(Lines(i), COLON)))) ' Save the value
                            ElseIf Lines(i).Contains(BLOCK_2SEQUENCE) Then
                                CurrentNode = CurrentNode.Nodes.Add(Trim(Lines(i).Substring(InStr(Lines(i), BLOCK_2SEQUENCE)))) ' Save the value
                            ElseIf Lines(i).Contains(BLOCK_4SEQUENCE) Then
                                CurrentNode = CurrentNode.Nodes.Add(Trim(Lines(i).Substring(InStr(Lines(i), BLOCK_4SEQUENCE)))) ' Save the value
                            End If
                        End If

                        CurrentNode.Name = NodeName

                        ' Always reset to the parent node so if we get another mapped value, we can add it correctly
                        CurrentNode = CurrentNode.Parent
                    End If

                    PreviousLevel = TreeLevel

                Else ' New block

                    ' Process the previous block if we have one
                    If BaseNode.Nodes.Count <> 0 Then
                        AnchorNode.Nodes.Add(CType(BaseNode, TreeNode))
                    End If

                    ' Add this as a base node
                    CurrentNode = Nothing ' Clean out
                    BaseNode = New TreeNode(NodeName)
                    BaseNode.Name = NodeName
                    PreviousLevel = 0

                End If
            End If
        Next

        ' Add the last node processed
        If BaseNode.Nodes.Count <> 0 Then
            AnchorNode.Nodes.Add(CType(BaseNode, TreeNode))
        End If

        ' For only one node in file
        If AnchorNode.Nodes.Count = 0 And BaseNode.Nodes.Count <> 0 Then
            AnchorNode.Nodes.Add(CType(BaseNode, TreeNode))
        End If

        Return AnchorNode

    End Function

    ' Gets the true or false of the text searched in the sent string
    Private Function TextContains(BaseText As String, SearchText As String) As Boolean

        If Len(BaseText) >= Len(SearchText) Then
            If BaseText.Substring(0, Len(SearchText)) = SearchText Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Private Function GetTreeLevel(InputLine As String) As Integer

        For i = 0 To Len(InputLine) - 1
            ' Find the first time a space does not occur
            If InputLine.Substring(i, 1) <> " " Then
                ' Record the position and exit for
                Return i / Len(INDENT)
            End If
        Next

        Return 0

    End Function

    ' Loads the blueprints table from the YMAL file
    Private Sub Load_YMAL_Blueprints()
        ' Use the tree node object with vb, Name is the node name, text is the value of the node - all we need to use here
        Dim BlueprintsTree As New TreeNode
        Dim BPNode As New TreeNode
        Dim ActivitiesNode As New TreeNode
        Dim ActivityNode As New TreeNode
        Dim MaterialNode() As TreeNode
        Dim SequenceNode() As TreeNode
        Dim ProductsNode() As TreeNode
        Dim SkillsNode() As TreeNode

        Dim i As Integer

        Dim blueprintTypeID As String
        Dim maxProductionLimit As String

        Dim activityID As String
        Dim activityName As String
        Dim time As String

        ' Use for materials and skills
        Dim materialID As String
        Dim materialQuantity As String
        Dim consume As String

        Dim productTypeID As String
        Dim probability As String

        Dim SQL As String
        Dim Count As Integer

        ' First set up our databases

        ' industryBlueprints
        Call ResetTable("industryBlueprints")
        ' Build table
        SQL = "CREATE TABLE industryBlueprints (blueprintTypeID bigint NOT NULL PRIMARY KEY, maxProductionLimit bigint NOT NULL)"
        Call Execute_msSQL(SQL)

        ' industryActivities
        Call ResetTable("industryActivities")
        ' Build table
        SQL = "CREATE TABLE industryActivities (blueprintTypeID bigint NOT NULL, activityID int NOT NULL, time int NOT NULL, "
        SQL = SQL & "PRIMARY KEY (blueprintTypeID, activityID))"
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_activityID ON industryActivities (activityID)"
        Call Execute_msSQL(SQL)

        ' industryActivityMaterials (mats and skills)
        Call ResetTable("industryActivityMaterials")
        ' Build table
        SQL = "CREATE TABLE industryActivityMaterials (blueprintTypeID bigint NOT NULL, activityID int NOT NULL, materialTypeID bigint NOT NULL, "
        SQL = SQL & "quantity bigint NOT NULL, consume tinyint NOT NULL)"
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_BPIDactivityID1 ON industryActivityMaterials (blueprintTypeID, activityID)"
        Call Execute_msSQL(SQL)

        ' industryActivityProducts 
        Call ResetTable("industryActivityProducts")
        ' Build table
        SQL = "CREATE TABLE industryActivityProducts (blueprintTypeID bigint NOT NULL, activityID int NOT NULL, productTypeID bigint NOT NULL, "
        SQL = SQL & "quantity bigint NOT NULL, probability float NOT NULL)"
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_BPIDactivityID2 ON industryActivityProducts (blueprintTypeID, activityID)"
        Call Execute_msSQL(SQL)

        ' Get the data from the YAML file
        BlueprintsTree = ParseYMALFile(DatabasePath & "\" & YAMLBlueprints)

        Count = 0
        pgMain.Minimum = Count
        pgMain.Maximum = BlueprintsTree.GetNodeCount(False)
        pgMain.Visible = True

        For Each BPNode In BlueprintsTree.Nodes

            ' Update form
            lblTableName.Text = "Saving BP: " & BPNode.Name
            pgMain.Value = Count
            Application.UseWaitCursor = True
            Application.DoEvents()

            ' blueprintTypeID = BPNode.Nodes.Find("blueprintTypeID", True)(0).Text
            blueprintTypeID = BPNode.Text
            maxProductionLimit = BPNode.Nodes.Find("maxProductionLimit", True)(0).Text

            ' Insert this industryBlueprints record
            Call Execute_msSQL("INSERT INTO industryBlueprints VALUES (" & blueprintTypeID & "," & maxProductionLimit & ")")

            ActivitiesNode = BPNode.Nodes.Find("activities", True)(0)

            For Each Activity In ActivitiesNode.Nodes
                time = Activity.Nodes.Find("time", True)(0).Text
                activityName = Activity.Name
                ' Set the activity
                Select Case activityName
                    Case "manufacturing"
                        activityID = "1"
                    Case "copying"
                        activityID = "5"
                    Case "invention"
                        activityID = "8"
                    Case "reverse_engineering"
                        activityID = "7"
                    Case "research_material"
                        activityID = "4"
                    Case "research_time"
                        activityID = "3"
                    Case Else
                        activityID = "0"
                End Select

                ' insert into industryActivities table
                Call Execute_msSQL("INSERT INTO industryActivities VALUES (" & blueprintTypeID & "," & activityID & "," & time & ")")

                ' Get the materials, products and skills for this activity
                MaterialNode = Activity.Nodes.Find("materials", True)

                If MaterialNode.Count <> 0 Then
                    ' Look at each sequence in the nodes
                    For i = 1 To MaterialNode(0).Nodes.Count
                        consume = 1 ' Always default to consumed if not given
                        materialID = "0"
                        materialQuantity = "0"

                        SequenceNode = MaterialNode(0).Nodes.Find(SequenceLabel & " " & CStr(i), True)

                        For Each Material In SequenceNode(0).Nodes
                            ' Values are stored in the text of the tree node
                            Select Case Material.name
                                Case "quantity"
                                    materialQuantity = Material.text
                                Case "typeID"
                                    materialID = Material.text
                                Case "consume"
                                    consume = Material.text
                            End Select
                        Next
                        ' Insert material record into industryActivityMaterials
                        SQL = "INSERT INTO industryActivityMaterials VALUES (" & blueprintTypeID & "," & activityID & ","
                        SQL = SQL & materialID & "," & materialQuantity & "," & consume & ")"
                        Call Execute_msSQL(SQL)
                    Next
                End If

                ' Get the skills for this activity
                SkillsNode = Activity.nodes.find("skills", True)

                If SkillsNode.Count <> 0 Then
                    ' Look up all the sequences
                    For i = 1 To SkillsNode(0).Nodes.Count
                        materialID = 0
                        materialQuantity = 0
                        consume = 0 ' Skills are never consumed

                        SequenceNode = SkillsNode(0).Nodes.Find(SequenceLabel & " " & CStr(i), True)

                        For Each Skill In SequenceNode(0).Nodes
                            ' Values are stored in the text of the tree node
                            Select Case Skill.name
                                Case "level"
                                    materialQuantity = Skill.text
                                Case "typeID"
                                    materialID = Skill.text
                            End Select
                        Next

                        ' Insert material record into industryActivityMaterials
                        SQL = "INSERT INTO industryActivityMaterials VALUES (" & blueprintTypeID & "," & activityID & ","
                        SQL = SQL & materialID & "," & materialQuantity & "," & consume & ")"
                        Call Execute_msSQL(SQL)
                    Next
                End If

                ' Get the products for this activity
                ProductsNode = Activity.nodes.find("products", True)

                If ProductsNode.Count <> 0 Then
                    ' Look up all the sequences
                    For i = 1 To ProductsNode(0).Nodes.Count
                        productTypeID = "0"
                        materialQuantity = "0"
                        probability = 1 ' Always default to 1 if not given - 100%

                        SequenceNode = ProductsNode(0).Nodes.Find(SequenceLabel & " " & CStr(i), True)

                        For Each Product In SequenceNode(0).Nodes
                            ' Values are stored in the text of the tree node
                            Select Case Product.name
                                Case "quantity"
                                    materialQuantity = Product.text
                                Case "typeID" ' productTypeID
                                    productTypeID = Product.text
                                Case "probability"
                                    probability = Product.text
                            End Select
                        Next

                        ' Insert material record into industryActivityProducts
                        SQL = "INSERT INTO industryActivityProducts VALUES (" & blueprintTypeID & "," & activityID & ","
                        SQL = SQL & productTypeID & "," & materialQuantity & "," & probability & ")"
                        Call Execute_msSQL(SQL)
                    Next
                End If

            Next
            Count += 1
        Next

        ' Now that this is all imported, check the industryActivityMaterials table for activites that aren't in industryActivityProducts and insert
        ' setting the productID = blueprintID. This is so we can get materials for ME/TE and copying - ie, skills are needed to do ME/TE and copying, no mats then no activity possible
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String
        Dim mySQLQuery2 As New SqlCommand
        Dim mySQLReader2 As SqlDataReader
        Dim msSQL2 As String

        ' Pull distinct bps and activities from materials
        msSQL = "SELECT distinct blueprintTypeID, activityID FROM industryActivityMaterials"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        While msSQLReader.Read
            ' Check each one and see if there is the bp with that activity, if not add the record
            msSQL2 = "SELECT 'X' FROM industryActivityProducts WHERE blueprintTypeID = " & msSQLReader.GetInt64(0) & " AND activityID = " & msSQLReader.GetInt32(1)
            mySQLQuery2 = New SqlCommand(msSQL2, SQLExpressConnection2)
            mySQLReader2 = mySQLQuery2.ExecuteReader()

            If Not mySQLReader2.Read Then
                ' Need to add this record - productTypeID is the blueprintTypeID
                SQL = "INSERT INTO industryActivityProducts VALUES (" & msSQLReader.GetInt64(0) & "," & msSQLReader.GetInt32(1) & ","
                SQL = SQL & msSQLReader.GetInt64(0) & ",1,1)"
                Call Execute_msSQL(SQL)
            End If

            mySQLReader2.Close()
            mySQLReader2 = Nothing
            mySQLQuery2 = Nothing
        End While

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        lblTableName.Text = ""
        pgMain.Visible = False
        Application.UseWaitCursor = True
        Application.DoEvents()

    End Sub

    ' Loads the eveIcons table from the YMAL file
    Private Sub Load_YMAL_Icons()
        ' Use the tree node object with vb, Name is the node name, text is the value of the node - all we need to use here
        Dim IconsTree As New TreeNode
        Dim BaseNode As New TreeNode
        Dim FindNodes() As TreeNode

        Dim iconID As String
        Dim description As String
        Dim iconFile As String

        Dim SQL As String
        Dim Count As Integer

        ' First set up the database

        ' industryActivities
        Call ResetTable("eveIcons")
        ' Build table
        SQL = "CREATE TABLE eveIcons (iconID int PRIMARY KEY NOT NULL, iconFile varchar(500) NOT NULL, description text) "
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_iconID ON eveIcons (iconID)"
        Call Execute_msSQL(SQL)

        ' Get the data from the YAML file
        IconsTree = ParseYMALFile(DatabasePath & "\" & YAMLiconIDs)

        Count = 0
        pgMain.Minimum = Count
        pgMain.Maximum = IconsTree.GetNodeCount(False)
        pgMain.Visible = True

        For Each BaseNode In IconsTree.Nodes

            ' Update form
            lblTableName.Text = "Saving Icon Data: " & BaseNode.Name
            pgMain.Value = Count
            Application.UseWaitCursor = True
            Application.DoEvents()

            iconID = BaseNode.Name
            ' Get the products for this activity
            FindNodes = BaseNode.Nodes.Find("description", True)
            iconFile = BaseNode.Nodes.Find("iconFile", True)(0).Text
            description = ""

            If FindNodes.Count <> 0 Then
                For Each TempNode In FindNodes(0).Nodes
                    description = TempNode.Nodes.Find("description", True)(0).Text
                Next
            End If

            If Not description.Contains("'") Then
                description = "'" & description & "'"
            End If

            If Not iconFile.Contains("'") Then
                iconFile = "'" & iconFile & "'"
            End If

            ' Insert this industryBlueprints record
            Call Execute_msSQL("INSERT INTO eveIcons VALUES (" & iconID & "," & description & "," & iconFile & ")")

            Count += 1
        Next

        lblTableName.Text = ""
        pgMain.Visible = False
        Application.UseWaitCursor = True
        Application.DoEvents()

    End Sub

    ' Loads the invTypes table (and invTypesMasteries, invTypesTraits) from the YMAL file
    Private Sub Load_YMAL_invTypes()
        ' Use the tree node object with vb, Name is the node name, text is the value of the node - all we need to use here
        Dim InventoryTypes As New TreeNode
        Dim BaseNode As New TreeNode
        Dim DescriptionNode As TreeNode
        Dim MasteryNode As TreeNode
        Dim TraitNode As TreeNode
        Dim NameNode As TreeNode
        Dim LanguageName As String
        Dim SQL As String
        Dim Count As Integer
        Dim InsertSQL As String
        Dim FoundValue As Boolean = False

        Dim CurrentTypeID As TypeIDRecord

        Application.UseWaitCursor = True
        Application.DoEvents()
        ' First set up the database

        ' inventoryTypes
        Call ResetTable("invTypes")
        ' Build table
        SQL = "CREATE TABLE [invTypes] ("
        SQL = SQL & "[typeID] [int] NOT NULL,"
        SQL = SQL & "[groupID] [int] NULL,"
        SQL = SQL & "[typeName] [nvarchar](100) NULL,"
        SQL = SQL & "[description] [nvarchar](4000) NULL,"
        SQL = SQL & "[mass] [float] NULL,"
        SQL = SQL & "[volume] [float] NULL,"
        SQL = SQL & "[capacity] [float] NULL,"
        SQL = SQL & "[portionSize] [int] NULL,"
        SQL = SQL & "[factionID] [int] NULL,"
        SQL = SQL & "[raceID] [tinyint] NULL,"
        SQL = SQL & "[basePrice] [money] NULL,"
        SQL = SQL & "[published] [bit] NULL,"
        SQL = SQL & "[marketGroupID] [int] NULL,"
        SQL = SQL & "[chanceOfDuplicating] [float] NULL,"
        SQL = SQL & "[graphicID] [int] NULL,"
        SQL = SQL & "[radius] [float] NULL,"
        SQL = SQL & "[iconID] [int] NULL,"
        SQL = SQL & "[soundID] [int] NULL,"
        SQL = SQL & "[sofFactionName] [nvarchar](100) NULL,"
        SQL = SQL & "[sofDnaAddition] [nvarchar](100) NULL,"
        SQL = SQL & "CONSTRAINT [invTypes_PK] PRIMARY KEY CLUSTERED ([typeID] ASC) "
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]"
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE NONCLUSTERED INDEX [invTypes_IX_Group] ON [dbo].[invTypes] ([groupID] ASC)"
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)"
        Call Execute_msSQL(SQL)

        Call ResetTable("invTypesTraits")
        ' Build table
        SQL = "CREATE TABLE [invTypesTraits] ("
        SQL = SQL & "[typeID] [int] NOT NULL,"
        SQL = SQL & "[skilltypeID] [int] NULL,"
        SQL = SQL & "[bonusID] [int] NULL,"
        SQL = SQL & "[bonus] [float] NULL,"
        SQL = SQL & "[bonusText] [nvarchar](4000) NULL,"
        SQL = SQL & "[unitID] [int] NULL)"
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE NONCLUSTERED INDEX [invTypesTraits_typeID] ON [dbo].[invTypesTraits] ([typeID] ASC)"
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)"
        Call Execute_msSQL(SQL)

        Call ResetTable("invTypesMasteries")
        ' Build table
        SQL = "CREATE TABLE [invTypesMasteries] ("
        SQL = SQL & "[typeID] [int] NOT NULL,"
        SQL = SQL & "[masteryLevel] [int] NULL,"
        SQL = SQL & "[masteryID] [int] NULL)"
        Call Execute_msSQL(SQL)
        ' Create index
        SQL = "CREATE NONCLUSTERED INDEX [invTypesMasteries_typeID] ON [dbo].[invTypesMasteries] ([typeID] ASC)"
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)"
        Call Execute_msSQL(SQL)

        ' Get the data from the YAML file
        InventoryTypes = ParseYMALFile(DatabasePath & "\" & YAMLtypeIDs)

        Count = 0
        pgMain.Minimum = Count
        pgMain.Maximum = InventoryTypes.GetNodeCount(False)
        pgMain.Visible = True

        For Each BaseNode In InventoryTypes.Nodes

            ' Update form
            lblTableName.Text = "Saving invTypes Data: " & BaseNode.Name
            pgMain.Value = Count
            Application.DoEvents()

            CurrentTypeID = New TypeIDRecord
            InsertSQL = ""

            With CurrentTypeID
                InsertSQL = "INSERT INTO invTypes VALUES ("

                .typeID = BaseNode.Name ' PK so will always be her
                InsertSQL = InsertSQL & .typeID & ","

                If BaseNode.Nodes.Find("groupID", True).Length <> 0 Then
                    .groupID = BaseNode.Nodes.Find("groupID", True)(0).Text
                    InsertSQL = InsertSQL & .groupID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                ' Get the name nodes to find the english description for typeName
                If BaseNode.Nodes.Find("name", True).Length <> 0 Then
                    NameNode = BaseNode.Nodes.Find("name", True)(0)
                    FoundValue = False
                    For Each Language In NameNode.Nodes
                        LanguageName = Language.Name
                        ' Set the activity
                        If LanguageName = "en" Then
                            .typeName = Language.Text
                            InsertSQL = InsertSQL & "'" & FormatDBString(.typeName) & "',"
                            FoundValue = True
                            Exit For
                        End If
                    Next
                    ' If it doesn't find the english version, mark as null
                    If Not FoundValue Then
                        InsertSQL = InsertSQL & "null,"
                    End If
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                ' Get the description nodes to find the english description
                If BaseNode.Nodes.Find("description", True).Length <> 0 Then
                    DescriptionNode = BaseNode.Nodes.Find("description", True)(0)
                    FoundValue = False
                    For Each Language In DescriptionNode.Nodes
                        LanguageName = Language.Name
                        ' Set the activity
                        If LanguageName = "en" Then
                            .description = Language.Text
                            ' Strip off quotes if they exist at beginning and end
                            If .description.Substring(0, 1) = Chr(ASCII_DoubleQuote_Code) Or .description.Substring(0, 1) = Chr(ASCII_Quote_Code) Then
                                .description = .description.Substring(1)
                            End If
                            If .description.Substring(Len(.description) - 1, 1) = Chr(ASCII_DoubleQuote_Code) Or .description.Substring(Len(.description) - 1, 1) = Chr(ASCII_Quote_Code) Then
                                .description = .description.Substring(0, Len(.description) - 1)
                            End If

                            InsertSQL = InsertSQL & "'" & FormatDBString(.description) & "',"
                            FoundValue = True
                            Exit For
                        End If
                    Next
                    ' If it doesn't find the english version, mark as null
                    If Not FoundValue Then
                        InsertSQL = InsertSQL & "null,"
                    End If
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("mass", True).Length <> 0 Then
                    .mass = BaseNode.Nodes.Find("mass", True)(0).Text
                    InsertSQL = InsertSQL & .mass & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("volume", True).Length <> 0 Then
                    .volume = BaseNode.Nodes.Find("volume", True)(0).Text
                    InsertSQL = InsertSQL & .volume & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("capacity", True).Length <> 0 Then
                    .capacity = BaseNode.Nodes.Find("capacity", True)(0).Text
                    InsertSQL = InsertSQL & .capacity & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("portionSize", True).Length <> 0 Then
                    .portionSize = BaseNode.Nodes.Find("portionSize", True)(0).Text
                    InsertSQL = InsertSQL & .portionSize & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("factionID", True).Length <> 0 Then
                    .factionID = BaseNode.Nodes.Find("factionID", True)(0).Text
                    InsertSQL = InsertSQL & .factionID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("raceID", True).Length <> 0 Then
                    .raceID = BaseNode.Nodes.Find("raceID", True)(0).Text
                    InsertSQL = InsertSQL & .raceID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("basePrice", True).Length <> 0 Then
                    .basePrice = BaseNode.Nodes.Find("basePrice", True)(0).Text
                    InsertSQL = InsertSQL & .basePrice & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("published", True).Length <> 0 Then
                    .published = CInt(CBool(BaseNode.Nodes.Find("published", True)(0).Text))
                    InsertSQL = InsertSQL & .published & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("marketGroupID", True).Length <> 0 Then
                    .marketGroupID = BaseNode.Nodes.Find("marketGroupID", True)(0).Text
                    InsertSQL = InsertSQL & .marketGroupID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("chanceOfDuplicating", True).Length <> 0 Then
                    .chanceOfDuplicating = BaseNode.Nodes.Find("chanceOfDuplicating", True)(0).Text
                    InsertSQL = InsertSQL & .chanceOfDuplicating & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("graphicID", True).Length <> 0 Then
                    .graphicID = BaseNode.Nodes.Find("graphicID", True)(0).Text
                    InsertSQL = InsertSQL & .graphicID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("radius", True).Length <> 0 Then
                    .radius = BaseNode.Nodes.Find("radius", True)(0).Text
                    InsertSQL = InsertSQL & .radius & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("iconID", True).Length <> 0 Then
                    .iconID = BaseNode.Nodes.Find("iconID", True)(0).Text
                    InsertSQL = InsertSQL & .iconID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("soundID", True).Length <> 0 Then
                    .soundID = BaseNode.Nodes.Find("soundID", True)(0).Text
                    InsertSQL = InsertSQL & .soundID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("sofFactionName", True).Length <> 0 Then
                    .sofFactionName = BaseNode.Nodes.Find("sofFactionName", True)(0).Text
                    InsertSQL = InsertSQL & "'" & FormatDBString(.sofFactionName) & "',"
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("sofDnaAddition", True).Length <> 0 Then
                    .sofDnaAddition = BaseNode.Nodes.Find("sofDnaAddition", True)(0).Text
                    InsertSQL = InsertSQL & "'" & FormatDBString(.sofDnaAddition) & "'"
                Else
                    InsertSQL = InsertSQL & "null"
                End If

                InsertSQL = InsertSQL & ")"

                ' Insert this invTypes record
                Call Execute_msSQL(InsertSQL)

                ' Insert the mastery
                If BaseNode.Nodes.Find("masteries", True).Length <> 0 Then
                    MasteryNode = BaseNode.Nodes.Find("masteries", True)(0)
                    Dim masteryLevel As Integer
                    Dim masteryID As Integer

                    For Each mastery In MasteryNode.Nodes
                        masteryLevel = mastery.Name

                        For Each sequence In mastery.nodes
                            masteryLevel = mastery.Name
                            For Each ID In sequence.nodes
                                masteryID = ID.name
                                ' Insert the record (typeID, masteryLevel, masteryID)
                                InsertSQL = "INSERT INTO invTypesMasteries VALUES (" & CStr(.typeID) & "," & CStr(masteryLevel) & "," & CStr(masteryID) & ")"
                                ' Insert this record
                                Call Execute_msSQL(InsertSQL)
                            Next
                        Next
                    Next
                End If

                ' Insert the Trait
                If BaseNode.Nodes.Find("traits", True).Length <> 0 Then
                    TraitNode = BaseNode.Nodes.Find("traits", True)(0)
                    Dim skillID As Integer = 0
                    Dim bonusID As Integer = 0
                    Dim bonus As Double = 0
                    Dim bonusText As String = Nothing
                    Dim unitID As Integer = 0

                    For Each trait In TraitNode.Nodes
                        skillID = trait.Name

                        For Each bonusIDLabel In trait.nodes
                            bonusID = bonusIDLabel.Name
                            For Each field In bonusIDLabel.nodes
                                If field.name = "bonus" Then
                                    bonus = CDbl(field.text)
                                End If
                                If field.name = "unitID" Then
                                    unitID = CInt(field.text)
                                End If
                                If field.Nodes.Find("en", True).Length <> 0 Then
                                    bonusText = field.nodes.find("en", True)(0).text
                                End If
                            Next
                            ' Form the SQL insert
                            InsertSQL = "INSERT INTO invTypesTraits VALUES (" & CStr(.typeID) & ","
                            If skillID = 0 Then
                                InsertSQL = InsertSQL & "null,"
                            Else
                                InsertSQL = InsertSQL & CStr(skillID) & ","
                            End If

                            If bonusID = 0 Then
                                InsertSQL = InsertSQL & "null,"
                            Else
                                InsertSQL = InsertSQL & CStr(bonusID) & ","
                            End If

                            If bonus = 0 Then
                                InsertSQL = InsertSQL & "null,"
                            Else
                                InsertSQL = InsertSQL & CStr(bonus) & ","
                            End If

                            If IsNothing(bonusText) Then
                                InsertSQL = InsertSQL & "null,"
                            Else
                                InsertSQL = InsertSQL & "'" & FormatDBString(bonusText) & "',"
                            End If

                            If unitID = 0 Then
                                InsertSQL = InsertSQL & "null"
                            Else
                                InsertSQL = InsertSQL & CStr(unitID)
                            End If

                            InsertSQL = InsertSQL & ")"

                            ' Insert this record
                            Call Execute_msSQL(InsertSQL)
                        Next
                    Next
                End If

            End With
            Count += 1
        Next

        lblTableName.Text = ""
        pgMain.Visible = False
        Application.UseWaitCursor = True
        Application.DoEvents()

    End Sub

    ' Loads the invGroups table from the YMAL file
    Private Sub Load_YMAL_invGroups()
        ' Use the tree node object with vb, Name is the node name, text is the value of the node - all we need to use here
        Dim InventoryGroups As New TreeNode
        Dim BaseNode As New TreeNode
        Dim NameNode As TreeNode
        Dim LanguageName As String
        Dim SQL As String
        Dim Count As Integer
        Dim InsertSQL As String
        Dim FoundValue As Boolean = False

        Dim CurrentGroupID As GroupIDRecord

        Application.UseWaitCursor = True
        Application.DoEvents()

        ' invGroups
        Call ResetTable("invGroups")
        ' Build table
        SQL = "CREATE TABLE [dbo].[invGroups](
        SQL = SQL & "[groupID] [int] NOT NULL,"
        SQL = SQL & "[categoryID] [int] NULL,"
        SQL = SQL & "[groupName] [nvarchar](100) NULL,"
        SQL = SQL & "[iconID] [int] NULL,"
        SQL = SQL & "[useBasePrice] [bit] NULL,"
        SQL = SQL & "[anchored] [bit] NULL,"
        SQL = SQL & "[anchorable] [bit] NULL,"
        SQL = SQL & "[fittableNonSingleton] [bit] NULL,"
        SQL = SQL & "[published] [bit] NULL,"
        SQL = SQL & "CONSTRAINT [invGroups_PK] PRIMARY KEY CLUSTERED ([groupID] ASC) "
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]"
        Call Execute_msSQL(SQL)

        ' Get the data from the YAML file
        InventoryGroups = ParseYMALFile(DatabasePath & "\" & YAMLgroupIDs)

        Count = 0
        pgMain.Minimum = Count
        pgMain.Maximum = InventoryGroups.GetNodeCount(False)
        pgMain.Visible = True

        For Each BaseNode In InventoryGroups.Nodes

            ' Update form
            lblTableName.Text = "Saving invGroups Data: " & BaseNode.Name
            pgMain.Value = Count
            Application.DoEvents()

            CurrentGroupID = New GroupIDRecord
            InsertSQL = ""

            With CurrentGroupID
                InsertSQL = "INSERT INTO invGroups VALUES ("

                .groupID = BaseNode.Name ' PK so will always be here
                InsertSQL = InsertSQL & .groupID & ","

                If BaseNode.Nodes.Find("categoryID", True).Length <> 0 Then
                    .categoryID = BaseNode.Nodes.Find("categoryID", True)(0).Text
                    InsertSQL = InsertSQL & .categoryID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                ' Get the name nodes to find the english description for typeName
                If BaseNode.Nodes.Find("name", True).Length <> 0 Then
                    NameNode = BaseNode.Nodes.Find("name", True)(0)
                    FoundValue = False
                    For Each Language In NameNode.Nodes
                        LanguageName = Language.Name
                        ' Set the activity
                        If LanguageName = "en" Then
                            .groupName = Language.Text
                            InsertSQL = InsertSQL & "'" & FormatDBString(.groupName) & "',"
                            FoundValue = True
                            Exit For
                        End If
                    Next
                    ' If it doesn't find the english version, mark as null
                    If Not FoundValue Then
                        InsertSQL = InsertSQL & "null,"
                    End If
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("iconID", True).Length <> 0 Then
                    .iconID = BaseNode.Nodes.Find("iconID", True)(0).Text
                    InsertSQL = InsertSQL & .iconID & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("useBasePrice", True).Length <> 0 Then
                    .useBasePrice = BaseNode.Nodes.Find("useBasePrice", True)(0).Text
                    InsertSQL = InsertSQL & CInt(CBool(.useBasePrice)) & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("anchored", True).Length <> 0 Then
                    .anchored = BaseNode.Nodes.Find("anchored", True)(0).Text
                    InsertSQL = InsertSQL & CInt(CBool(.anchored)) & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("anchorable", True).Length <> 0 Then
                    .anchorable = BaseNode.Nodes.Find("anchorable", True)(0).Text
                    InsertSQL = InsertSQL & CInt(CBool(.anchorable)) & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("fittableNonSingleton", True).Length <> 0 Then
                    .fittableNonSingleton = BaseNode.Nodes.Find("fittableNonSingleton", True)(0).Text
                    InsertSQL = InsertSQL & CInt(CBool(.fittableNonSingleton)) & ","
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("published", True).Length <> 0 Then
                    .published = BaseNode.Nodes.Find("published", True)(0).Text
                    InsertSQL = InsertSQL & CInt(CBool(.published))
                Else
                    InsertSQL = InsertSQL & "null"
                End If

                InsertSQL = InsertSQL & ")"

                ' Insert this invGroups record
                Call Execute_msSQL(InsertSQL)

            End With
            Count += 1
        Next

        lblTableName.Text = ""
        pgMain.Visible = False
        Application.UseWaitCursor = True
        Application.DoEvents()

    End Sub

    ' Loads the invCategories table from the YMAL file
    Private Sub Load_YMAL_invCategories()
        ' Use the tree node object with vb, Name is the node name, text is the value of the node - all we need to use here
        Dim InventoryCategories As New TreeNode
        Dim BaseNode As New TreeNode
        Dim NameNode As TreeNode
        Dim LanguageName As String
        Dim SQL As String
        Dim Count As Integer
        Dim InsertSQL As String
        Dim FoundValue As Boolean = False

        Dim CurrentCategoryID As CategoryIDRecord

        Application.UseWaitCursor = True
        Application.DoEvents()

        ' invCategories
        Call ResetTable("invCategories")
        ' Build table
        SQL = "CREATE TABLE [dbo].[invCategories]("
        SQL = SQL & "[categoryID] [int] NOT NULL,"
        SQL = SQL & "[categoryName] [nvarchar](100) NULL,"
        SQL = SQL & "[published] [bit] NULL,"
        SQL = SQL & "CONSTRAINT [invCategories_PK] PRIMARY KEY CLUSTERED ([categoryID] ASC) "
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]"
        Call Execute_msSQL(SQL)

        ' Get the data from the YAML file
        InventoryCategories = ParseYMALFile(DatabasePath & "\" & YAMLcategoryIDs)

        Count = 0
        pgMain.Minimum = Count
        pgMain.Maximum = InventoryCategories.GetNodeCount(False)
        pgMain.Visible = True

        For Each BaseNode In InventoryCategories.Nodes

            ' Update form
            lblTableName.Text = "Saving invCategories Data: " & BaseNode.Name
            pgMain.Value = Count
            Application.DoEvents()

            CurrentCategoryID = New CategoryIDRecord
            InsertSQL = ""

            With CurrentCategoryID
                InsertSQL = "INSERT INTO invCategories VALUES ("

                .categoryID = BaseNode.Name ' PK so will always be here
                InsertSQL = InsertSQL & .categoryID & ","

                ' Get the name nodes to find the english description for typeName
                If BaseNode.Nodes.Find("name", True).Length <> 0 Then
                    NameNode = BaseNode.Nodes.Find("name", True)(0)
                    FoundValue = False
                    For Each Language In NameNode.Nodes
                        LanguageName = Language.Name
                        ' Set the activity
                        If LanguageName = "en" Then
                            .categoryName = Language.Text
                            InsertSQL = InsertSQL & "'" & FormatDBString(.categoryName) & "',"
                            FoundValue = True
                            Exit For
                        End If
                    Next
                    ' If it doesn't find the english version, mark as null
                    If Not FoundValue Then
                        InsertSQL = InsertSQL & "null,"
                    End If
                Else
                    InsertSQL = InsertSQL & "null,"
                End If

                If BaseNode.Nodes.Find("published", True).Length <> 0 Then
                    .published = BaseNode.Nodes.Find("published", True)(0).Text
                    InsertSQL = InsertSQL & CInt(CBool(.published))
                Else
                    InsertSQL = InsertSQL & "null"
                End If

                InsertSQL = InsertSQL & ")"

                ' Insert this invCategories record
                Call Execute_msSQL(InsertSQL)

            End With
            Count += 1
        Next

        lblTableName.Text = ""
        pgMain.Visible = False
        Application.UseWaitCursor = True
        Application.DoEvents()

    End Sub

    Private Class TypeIDRecord
        Public typeID As Integer
        Public groupID As Integer
        Public typeName As String
        Public description As String
        Public mass As Double
        Public volume As Double
        Public capacity As Double
        Public portionSize As Integer
        Public factionID As Integer
        Public raceID As Integer
        Public basePrice As Double
        Public published As Integer
        Public marketGroupID As Integer
        Public chanceOfDuplicating As Double
        Public graphicID As Long
        Public radius As Double
        Public iconID As Long
        Public soundID As Long
        Public sofFactionName As String
        Public sofDnaAddition As String

        Public Sub New()
            typeID = 0
            groupID = 0
            typeName = ""
            description = ""
            mass = 0
            volume = 0
            capacity = 0
            portionSize = 0
            raceID = 0
            basePrice = 0
            published = 0
            marketGroupID = 0
            chanceOfDuplicating = 0
        End Sub

    End Class

    Private Class GroupIDRecord
        Public groupID As Integer
        Public groupName As String
        Public anchorable As Boolean
        Public anchored As Boolean
        Public categoryID As Integer
        Public fittableNonSingleton As Boolean
        Public iconID As Long
        Public published As Boolean
        Public useBasePrice As Boolean

        Public Sub New()
            groupID = 0
            groupName = ""
            anchorable = False
            anchored = False
            categoryID = 0
            fittableNonSingleton = False
            iconID = 0
            published = False
            useBasePrice = False
        End Sub
    End Class

    Private Class CategoryIDRecord
        Public categoryID As Integer
        Public categoryName As String
        Public published As Boolean

        Public Sub New()
            categoryID = 0
            categoryName = ""
            published = False
        End Sub
    End Class

#End Region

    ' Random updates to the main DB go here
    Private Sub RandomSDEUpdates()
        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Call Build_PACKAGED_SHIP_VOLUMES() ' Build this first
        Call Build_PACKAGED_CONTAINER_VOLUMES()

        ' Production volumes for ships need to be changed in inventory types before starting
        ' So, update the volumes for ships to their packaged values in Inventory types and all blueprint materials
        msSQL = "SELECT * FROM PACKAGED_SHIP_VOLUMES"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        While msSQLReader.Read
            Application.DoEvents()
            msSQL = "UPDATE invTypes SET volume = " & msSQLReader.GetValue(1) & " WHERE groupID = " & msSQLReader.GetValue(0)
            Call Execute_msSQL(msSQL)
        End While

        msSQLReader.Close()

        ' Do the same for containers
        msSQL = "SELECT * FROM PACKAGED_CONTAINER_VOLUMES"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()

        While msSQLReader.Read
            Application.DoEvents()
            msSQL = "UPDATE invTypes SET volume = " & msSQLReader.GetValue(1) & " WHERE typeID = " & msSQLReader.GetValue(0)
            Call Execute_msSQL(msSQL)
        End While

        msSQLReader.Close()

        ' Random updates

        ' Chinese named ships in invtypes for some reason
        msSQL = "DELETE FROM invTypes where typeID IN (34480,34478,34476,34474,34472,34470,34468,34466,34464,34462,34460,34458)"
        Call Execute_msSQL(msSQL)

        msSQLReader.Close()

    End Sub

    ' PACKAGED_CONTAINER_VOLUMES
    Private Sub Build_PACKAGED_CONTAINER_VOLUMES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'PACKAGED_CONTAINER_VOLUMES'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE PACKAGED_CONTAINER_VOLUMES"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Build the table in msSQL and use it later
        SQL = "CREATE TABLE PACKAGED_CONTAINER_VOLUMES ("
        SQL = SQL & "TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "PACKAGED_M3 INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_msSQL(SQL)

        ' Since this is all data from reddit post, just do inserts here
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (33003,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (24445,1200)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (11489,300)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (33005,5000)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (11488,150)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (17365,65)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (33007,1000)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (3465,65)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (3296,65)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (17364,33)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (33009,500)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (3466,33)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (3293,33)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (17363,10)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (33011,100)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (3467,10)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (3297,10)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (17366,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (17367,50000)")
        Execute_msSQL("INSERT INTO PACKAGED_CONTAINER_VOLUMES VALUES (17368,100000)")

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' PACKAGED_SHIP_VOLUMES 
    Private Sub Build_PACKAGED_SHIP_VOLUMES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'PACKAGED_SHIP_VOLUMES'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE PACKAGED_SHIP_VOLUMES"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Build the table in msSQL and use it later
        SQL = "CREATE TABLE PACKAGED_SHIP_VOLUMES ("
        SQL = SQL & "GROUP_ID INTEGER NOT NULL,"
        SQL = SQL & "PACKAGED_M3 INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_msSQL(SQL)

        ' Since this is all data from reddit post, just do inserts here
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (324,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (1201,15000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (419,15000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (27,50000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (898,50000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (1202,20000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (883,1300000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (29,500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (547,1300000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (906,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (540,15000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (830,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (26,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (420,5000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (485,1300000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (893,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (381,50000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (543,3750)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (1283,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (833,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (513,1300000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (25,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (358,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (894,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (28,20000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (941,500000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (831,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (541,5000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (902,1300000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (832,10000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (900,50000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (463,3750)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (1022,500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (237,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (31,500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (834,2500)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (963,5000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (659,1300000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (1305,5000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (30,10000000)")
        Execute_msSQL("INSERT INTO PACKAGED_SHIP_VOLUMES VALUES (380,20000)")

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapCelestialStatistics
    Private Sub Build_mapCelestialStatistics()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapCelestialStatistics'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapCelestialStatistics"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapCelestialStatistics "
        msSQL = msSQL & "(celestialID integer primary key,"
        msSQL = msSQL & "temperature real,"
        msSQL = msSQL & "spectralClass varchar(10),"
        msSQL = msSQL & "luminosity real,"
        msSQL = msSQL & "age real,"
        msSQL = msSQL & "life real,"
        msSQL = msSQL & "orbitRadius real,"
        msSQL = msSQL & "eccentricity real,"
        msSQL = msSQL & "massDust real,"
        msSQL = msSQL & "massGas real,"
        msSQL = msSQL & "fragmented int,"
        msSQL = msSQL & "density real,"
        msSQL = msSQL & "surfaceGravity real,"
        msSQL = msSQL & "escapeVelocity real,"
        msSQL = msSQL & "orbitPeriod real,"
        msSQL = msSQL & "rotationRate real,"
        msSQL = msSQL & "locked int,"
        msSQL = msSQL & "pressure real,"
        msSQL = msSQL & "radius real,"
        msSQL = msSQL & "mass real)"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapCelestialStatistics"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapCelestialStatistics"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        On Error Resume Next
        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapCelestialStatistics VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(6)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(7)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(8)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(9)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(10)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(11)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(12)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(13)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(14)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(15)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(16)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(17)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(18)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(19)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While
        On Error GoTo 0

        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapConstellationJumps
    Private Sub Build_mapConstellationJumps()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapConstellationJumps'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapConstellationJumps"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapConstellationJumps ( fromRegionID bigint,"
        msSQL = msSQL & "fromConstellationID bigint,"
        msSQL = msSQL & "toConstellationID bigint,"
        msSQL = msSQL & "toRegionID bigint,"
        msSQL = msSQL & "PRIMARY KEY(fromConstellationID,toConstellationID))"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapConstellationJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapConstellationJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapConstellationJumps VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        Application.DoEvents()
        pgMain.Visible = False

    End Sub

    ' mapConstellations
    Private Sub Build_mapConstellations()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapConstellations'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapConstellations"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapConstellations ( regionID integer,constellationID integer primary key,constellationName varchar(100),x real,y real,z real,xMin real,xMax real,yMin real,yMax real,zMin real,zMax real,factionID integer,radius real)"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapConstellations"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapConstellations"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapConstellations VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(6)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(7)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(8)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(9)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(10)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(11)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(12)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(13)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        Application.DoEvents()
        pgMain.Visible = False

    End Sub

    ' mapDenormalize
    Private Sub Build_mapDenormalize()
        Dim SQL As String
        Dim i As Long = 0

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapDenormalize'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapDenormalize"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapDenormalize ("
        msSQL = msSQL & "itemID integer,"
        msSQL = msSQL & "typeID integer,"
        msSQL = msSQL & "groupID integer,"
        msSQL = msSQL & "solarSystemID integer,"
        msSQL = msSQL & "constellationID integer,"
        msSQL = msSQL & "regionID integer,"
        msSQL = msSQL & " orbitID integer,"
        msSQL = msSQL & "x real,"
        msSQL = msSQL & "y real,"
        msSQL = msSQL & "z real,"
        msSQL = msSQL & "radius real,"
        msSQL = msSQL & "itemName varchar(100),"
        msSQL = msSQL & "security real,"
        msSQL = msSQL & "celestialIndex integer,"
        msSQL = msSQL & "orbitIndex integer"
        msSQL = msSQL & ")"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Pull new data and insert
        SQLUniverse = "SELECT COUNT(*) FROM mapDenormalize"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data and insert
        SQLUniverse = "SELECT * FROM mapDenormalize"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapDenormalize VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(6)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(7)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(8)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(9)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(10)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(11)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(12)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(13)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(14)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapJumps
    Private Sub Build_mapJumps()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapJumps'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapJumps"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapJumps ( stargateID bigint,destinationID bigint,PRIMARY KEY(stargateID))"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapJumps VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        Application.DoEvents()
        pgMain.Visible = False

    End Sub

    ' mapLandmarks
    Private Sub Build_mapLandmarks()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapLandmarks'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapLandmarks"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapLandmarks ( landmarkID bigint,landmarkName varchar(100),description varchar(7000),locationID bigint,x real,y real,z real,iconID bigint,PRIMARY KEY(landmarkID))"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapLandmarks"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapLandmarks"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapLandmarks VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(6)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(7)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapLocationScenes
    Private Sub Build_mapLocationScenes()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapLocationScenes'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapLocationScenes"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapLocationScenes ( locationID integer primary key,graphicID integer)"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapLocationScenes"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapLocationScenes"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapLocationScenes VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapLocationWormholeClasses
    Private Sub Build_mapLocationWormholeClasses()
        Dim SQL As String
        Dim i As Long = 0

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapLocationWormholeClasses'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapLocationWormholeClasses"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapLocationWormholeClasses ( locationID integer primary key,wormholeClassID integer)"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Pull new data and insert
        SQLUniverse = "SELECT COUNT(*) FROM mapLocationWormholeClasses"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data and insert
        SQLUniverse = "SELECT * FROM mapLocationWormholeClasses"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapLocationWormholeClasses VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapRegionJumps
    Private Sub Build_mapRegionJumps()
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQL As String

        Call ResetTable("mapRegionJumps")

        msSQL = "CREATE TABLE mapRegionJumps ( fromRegionID bigint,toRegionID bigint,PRIMARY KEY(fromRegionID,toRegionID))"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapRegionJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapRegionJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapRegionJumps VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapRegions
    Private Sub Build_mapRegions()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapRegions'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapRegions"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapRegions (regionID integer primary key,regionName varchar(100),x real,y real,z real,xMin real,xMax real,yMin real,yMax real,zMin real,zMax real,factionID integer,radius real)"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapRegions"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapRegions"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapRegions VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(6)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(7)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(8)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(9)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(10)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(11)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(12)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapSolarSystemJumps
    Private Sub Build_mapSolarSystemJumps()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapSolarSystemJumps'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapSolarSystemJumps"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapSolarSystemJumps ( fromRegionID bigint,fromConstellationID bigint,fromSolarSystemID bigint,toSolarSystemID bigint,toConstellationID bigint,toRegionID bigint,PRIMARY KEY(fromSolarSystemID,toSolarSystemID))"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapSolarSystemJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapSolarSystemJumps"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapSolarSystemJumps VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' mapSolarSystems
    Private Sub Build_mapSolarSystems()
        Dim SQL As String
        Dim i As Long

        ' SQLite variables
        Dim SQLiteDBCommand As New SQLiteCommand
        Dim SQLiteReader As SQLiteDataReader
        Dim SQLUniverse As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = 'mapSolarSystems'"
        msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP TABLE mapSolarSystems"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE TABLE mapSolarSystems ( regionID integer,constellationID integer,solarSystemID integer primary key,solarSystemName varchar(100),x real,y real,z real,xMin real,xMax real,yMin real,yMax real,zMin real,zMax real,luminosity real,border bit,fringe bit,corridor bit,hub bit,international bit,regional bit,constellation bit,security real,factionID integer,radius real,sunTypeID integer,securityClass varchar(2))"

        Call Execute_msSQL(msSQL) ' msSQL server table

        ' Get Count
        SQLUniverse = "SELECT COUNT(*) FROM mapSolarSystems"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        pgMain.Minimum = 0
        pgMain.Maximum = SQLiteReader.GetValue(0)
        pgMain.Visible = True

        ' Pull new data from UniverseDB (SQLite) and insert
        SQLUniverse = "SELECT * FROM mapSolarSystems"
        SQLiteDBCommand = New SQLiteCommand(SQLUniverse, UniverseDB)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        Call BeginSQLiteTransaction(UniverseDB)

        While SQLiteReader.Read
            Application.DoEvents()

            msSQL = "INSERT INTO mapSolarSystems VALUES ("
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(0)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(1)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(2)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(3)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(4)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(5)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(6)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(7)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(8)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(9)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(10)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(11)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(12)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(13)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(14)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(15)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(16)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(17)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(18)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(19)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(20)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(21)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(22)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(23)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(24)) & ","
            msSQL = msSQL & BuildInsertFieldString(SQLiteReader.GetValue(25)) & ")"

            Call Execute_msSQL(msSQL) ' add to msSQL Server

            i += 1
            pgMain.Value = i

        End While

        Call CommitSQLiteTransaction(UniverseDB)


        SQLiteReader.Close()
        SQLiteReader = Nothing
        SQLiteDBCommand = Nothing

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

#End Region

    Private Function CheckNull(ByVal inVariable As Object) As Object
        If IsNothing(inVariable) Then
            Return "null"
        ElseIf DBNull.Value.Equals(inVariable) Then
            Return "null"
        Else
            Return inVariable
        End If
    End Function

    Private Function FormatDBString(ByVal inStrVar As String) As String
        ' Anything with quote mark in name it won't correctly load - need to replace with double quotes
        If InStr(inStrVar, "'") Then
            inStrVar = Replace(inStrVar, "'", "''")
        End If
        Return inStrVar
    End Function

    ' Formats the value sent to what we want to insert inot the table field
    Private Function BuildInsertFieldString(ByVal inValue As Object) As String
        Dim CheckNullValue As Object
        Dim OutputString As String

        ' See if it is null first
        CheckNullValue = CheckNull(inValue)

        If CStr(CheckNullValue) <> "null" Then
            ' Not null, so format
            If TypeOf inValue Is Boolean Then
                ' Change these to numeric values
                If inValue = True Then
                    OutputString = "1"
                Else
                    OutputString = "0"
                End If
            ElseIf IsNumeric(inValue) Then
                OutputString = CStr(inValue)
            Else
                ' String, so check for appostrophes
                OutputString = "'" & FormatDBString(inValue) & "'"
            End If
        Else
            OutputString = "null"
        End If

        Return OutputString

    End Function

    Private Sub Execute_msSQL(ByVal SQL As String)
        Dim Command As SqlCommand

        Command = New SqlCommand(SQL, SQLExpressConnectionExecute)
        Command.ExecuteNonQuery()

        Command = Nothing

    End Sub

    Private Sub Execute_SQLiteSQL(ByVal SQL As String, ByRef DBRef As SQLiteConnection)
        Dim DBExecuteCmd As SQLiteCommand

        DBExecuteCmd = DBRef.CreateCommand
        DBExecuteCmd.CommandText = SQL
        DBExecuteCmd.ExecuteNonQuery()

        DBExecuteCmd.Dispose()

    End Sub

    Private Sub BeginSQLiteTransaction(ByRef DBRef As SQLiteConnection)
        Call Execute_SQLiteSQL("BEGIN;", DBRef)
    End Sub

    Private Sub CommitSQLiteTransaction(ByRef DBRef As SQLiteConnection)
        Call Execute_SQLiteSQL("END;", DBRef)
    End Sub

    Private Function GetLenSQLExpField(ByVal FieldName As String, ByVal TableName As String) As String
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

    Private Sub ResetTable(TableName As String)
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

#End Region

#Region "Deploy files"

    Private Sub btnCopyFilesBuildXML_Click(sender As System.Object, e As System.EventArgs) Handles btnCopyFilesBuildXML.Click
        Call CopyFilesBuildXML()
    End Sub

    ' Copies all the files from directories and then builds the xml file and saves it here for upload to github
    Private Sub CopyFilesBuildXML()
        Dim NewFilesAdded As Boolean = False

        On Error Resume Next
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()

        ' Copy the files over to the latest directory, overwrite if needed - check the hash first 
        If MD5CalcFile(RootDirectory & ZipForgeDLL) <> MD5CalcFile(MediaFireDirectory & ZipForgeDLL) Then
            File.Copy(RootDirectory & ZipForgeDLL, MediaFireDirectory & ZipForgeDLL, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & JSONDLL) <> MD5CalcFile(MediaFireDirectory & JSONDLL) Then
            File.Copy(RootDirectory & JSONDLL, MediaFireDirectory & JSONDLL, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & SQLiteDLL) <> MD5CalcFile(MediaFireDirectory & SQLiteDLL) Then
            File.Copy(RootDirectory & SQLiteDLL, MediaFireDirectory & SQLiteDLL, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & EVEIPHEXE) <> MD5CalcFile(MediaFireDirectory & EVEIPHEXE) Then
            File.Copy(RootDirectory & EVEIPHEXE, MediaFireDirectory & EVEIPHEXE, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & EVEIPHUpdater) <> MD5CalcFile(MediaFireDirectory & EVEIPHUpdater) Then
            File.Copy(RootDirectory & EVEIPHUpdater, MediaFireDirectory & EVEIPHUpdater, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(WorkingDirectory & EVEIPHDB) <> MD5CalcFile(MediaFireDirectory & EVEIPHDB) Then
            File.Copy(WorkingDirectory & EVEIPHDB, MediaFireDirectory & EVEIPHDB, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(WorkingDirectory & ImageZipFile) <> MD5CalcFile(MediaFireDirectory & ImageZipFile) Then
            File.Copy(WorkingDirectory & ImageZipFile, MediaFireDirectory & ImageZipFile, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & UpdaterManifest) <> MD5CalcFile(MediaFireDirectory & UpdaterManifest) Then
            File.Copy(RootDirectory & UpdaterManifest, MediaFireDirectory & UpdaterManifest, True)
            NewFilesAdded = True
        End If

        If MD5CalcFile(RootDirectory & EXEManifest) <> MD5CalcFile(MediaFireDirectory & EXEManifest) Then
            File.Copy(RootDirectory & EXEManifest, MediaFireDirectory & EXEManifest, True)
            NewFilesAdded = True
        End If

        On Error GoTo 0

        ' Output the Latest XML File if we have updates
        If NewFilesAdded Then
            Call WriteLatestXMLFile()

            ' Copy the new XML file into the root directory - so I don't get updates
            File.Copy(MediaFireDirectory & LatestVersionXML, RootDirectory & LatestVersionXML, True)

        End If

        ' Refresh the grid
        Call LoadFileGrid()

        Me.Cursor = Cursors.Default
        Application.DoEvents()

        MsgBox("Files Deployed", vbInformation, "Complete")

    End Sub

    ' Writes the sent settings to the sent file name
    Private Sub WriteLatestXMLFile()

        ' Create XmlWriterSettings.
        Dim XMLSettings As XmlWriterSettings = New XmlWriterSettings()
        XMLSettings.Indent = True

        ' Delete and make a fresh copy
        If File.Exists(LatestVersionXML) Then
            File.Delete(LatestVersionXML)
        End If

        ' Loop through the settings sent and output each name and value
        Using writer As XmlWriter = XmlWriter.Create(MediaFireDirectory & LatestVersionXML, XMLSettings)
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
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & EVEIPHEXE))
            writer.WriteAttributeString("URL", EVEIPHEXEURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", EVEIPHUpdater)
            writer.WriteAttributeString("Version", "2.0")
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & EVEIPHUpdater))
            writer.WriteAttributeString("URL", EVEIPHUpdaterURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", EVEIPHDB)
            writer.WriteAttributeString("Version", DatabaseName)
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & EVEIPHDB))
            writer.WriteAttributeString("URL", EVEIPHDBURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", ImageZipFile)
            writer.WriteAttributeString("Version", ImagesVersion)
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & ImageZipFile))
            writer.WriteAttributeString("URL", ImageZipFileURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", ZipForgeDLL)
            writer.WriteAttributeString("Version", "3.00")
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & ZipForgeDLL))
            writer.WriteAttributeString("URL", ZipForgeDLLURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", JSONDLL)
            writer.WriteAttributeString("Version", "6.03")
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & JSONDLL))
            writer.WriteAttributeString("URL", JSONDLLURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", SQLiteDLL)
            writer.WriteAttributeString("Version", "1.07.9.0")
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & SQLiteDLL))
            writer.WriteAttributeString("URL", SQLiteDLLURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", UpdaterManifest)
            writer.WriteAttributeString("Version", "1.0")
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & UpdaterManifest))
            writer.WriteAttributeString("URL", UpdaterManifestURL)
            writer.WriteEndElement()

            writer.WriteStartElement("row")
            writer.WriteAttributeString("Name", EXEManifest)
            writer.WriteAttributeString("Version", "1.0")
            writer.WriteAttributeString("MD5", MD5CalcFile(MediaFireDirectory & EXEManifest))
            writer.WriteAttributeString("URL", EXEManifestURL)
            writer.WriteEndElement()

            ' End document.
            writer.WriteEndDocument()
        End Using

    End Sub

#End Region

End Class
