
Imports System.Data.SqlClient ' For SQL Server Connection
Imports System.Data.SQLite
Imports System.IO
Imports System.Xml

Public Class frmMain
    Inherits Form

    Public EVEIPHSQLiteDB As SQLiteDBConnection

    Private Const SettingsFileName As String = "Settings.txt"

    Private VersionNumber As String = ""

    ' Directory files and paths
    Private RootDirectory As String ' For the debugging process, will copy images here as well
    Private WorkingDirectory As String ' Where the final DB and image zip is stored 
    Private MediaFireDirectory As String ' Where all the files we want to sync to the Media fire server for download are
    Private MediaFireTestDirectory As String

    ' DB
    Private DatabasePath As String ' Where we build the SQLite database
    Private FinalDBPath As String ' Final DB
    Private DatabaseName As String
    Private ImagesVersion As String ' Version of the images we have in the zip
    Private FinalDBName As String = "EVEIPH DB"
    Private SQLInstance As String ' how to log into the SQL server on the host computer

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

    Private JSONDLL As String = "Newtonsoft.Json.dll"
    Private SQLiteDLL As String = "System.Data.SQLite.dll"
    Private EVEIPHEXE As String = "EVE Isk per Hour.exe"
    Private EVEIPHUpdater As String = "EVEIPH Updater.exe"
    Private EVEIPHDB As String = "EVEIPH DB.s3db"
    Private UpdaterManifest As String = "EVEIPH Updater.exe.manifest"
    Private EXEManifest As String = "EVE Isk per Hour.exe.manifest"
    'Private IonicZipFile As String = "Ionic.Zip.dll"
    Private ImageZipFile As String = "EVEIPH Images.zip"
    Private MoreLinqDLL As String = "MoreLinq.Portable.dll"
    Private LatestVersionXML As String
    Private LatestTestVersionXML As String

    ' File URLs
    'Private JSONDLLURL As String = "http://www.mediafire.com/download/7a6ml9gwu14616d/Newtonsoft.Json.dll"
    'Private SQLiteDLLURL As String = "http://www.mediafire.com/download/b0px46xwaa8jgx4/System.Data.SQLite.dll"
    'Private EVEIPHEXEURL As String = "http://www.mediafire.com/download/2ckpd2th8xlpysv/EVE_Isk_per_Hour.exe"
    'Private EVEIPHUpdaterURL As String = "http://www.mediafire.com/download/r9innfrf287mnd7/EVEIPH_Updater.exe"
    'Private EVEIPHDBURL As String = "http://www.mediafire.com/download/cfylxmlq6v8i26c/EVEIPH_DB.s3db"
    'Private UpdaterManifestURL As String = "http://www.mediafire.com/download/c149x7vcf1gab2p/EVEIPH_Updater.exe.manifest"
    'Private IconicZipFileURL As String = "http://www.mediafire.com/download/6ucs9it1bxjiccv/Ionic.Zip.dll"
    'Private EXEManifestURL As String = "http://www.mediafire.com/download/sdlrk28t18gv8z0/EVE_Isk_per_Hour.exe.manifest"
    'Private ImageZipFileURL As String = "http://www.mediafire.com/download/duq6nw4d0p59rci/EVEIPH_Images.zip"
    'Private MoreLinqDLLURL As String = "http://www.mediafire.com/download/mckqc8cxljc5p86/MoreLinq.Portable.dll"

    'Private TestJSONDLLURL As String = "http://www.mediafire.com/download/wmgmu7qu6ha4qag/Newtonsoft.Json.dll"
    'Private TestSQLiteDLLURL As String = "http://www.mediafire.com/download/q1bbs5hgp4e18gh/System.Data.SQLite.dll"
    'Private TestEVEIPHEXEURL As String = "http://www.mediafire.com/download/8cp2ffnb50y0or5/EVE_Isk_per_Hour.exe"
    'Private TestEVEIPHUpdaterURL As String = "http://www.mediafire.com/download/2d0dsgfap2gq299/EVEIPH_Updater.exe"
    'Private TestEVEIPHDBURL As String = "http://www.mediafire.com/download/w69bgqr9bo7awt4/EVEIPH_DB.s3db"
    'Private TestUpdaterManifestURL As String = "http://www.mediafire.com/download/9my8r2x78bbym9k/EVEIPH_Updater.exe.manifest"
    'Private TestIconicZipFileURL As String = "http://www.mediafire.com/download/w3xwdhl8vnxw0q7/Ionic.Zip.dll"
    'Private TestEXEManifestURL As String = "http://www.mediafire.com/download/9snq0e79zbesfuq/EVE_Isk_per_Hour.exe.manifest"
    'Private TestImageZipFileURL As String = "http://www.mediafire.com/download/eox20bz6ddey1g3/EVEIPH_Images.zip"

    Private JSONDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/Newtonsoft.Json.dll"
    Private SQLiteDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/System.Data.SQLite.dll"
    Private EVEIPHEXEURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe"
    Private EVEIPHUpdaterURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe"
    Private EVEIPHDBURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20DB.s3db"
    Private UpdaterManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe.manifest"
    Private IconicZipFileURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/Ionic.Zip.dll"
    Private EXEManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe.manifest"
    Private ImageZipFileURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Images.zip"
    Private MoreLinqDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/MoreLinq.Portable.dll"

    Private TestJSONDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/Newtonsoft.Json.dll"
    Private TestSQLiteDLLURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/System.Data.SQLite.dll"
    Private TestEVEIPHEXEURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVE%20Isk%20per%20Hour.exe"
    Private TestEVEIPHUpdaterURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe"
    Private TestEVEIPHDBURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20DB.s3db"
    Private TestUpdaterManifestURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/EVEIPH%20Updater.exe.manifest"
    Private TestIconicZipFileURL As String = "https://raw.githubusercontent.com/EVEIPH/LatestFiles/master/Ionic.Zip.dll"
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
        lstFileInformation.Columns.Add("File Name", 250, HorizontalAlignment.Left)
        lstFileInformation.Columns.Add("File Date/Time", 150, HorizontalAlignment.Left)

        Call LoadFileGrid()

    End Sub

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
            MediaFireDirectory = BPStream.ReadLine
            If Not Directory.Exists(MediaFireDirectory) Then
                MediaFireDirectory = ""
            End If
            MediaFireTestDirectory = BPStream.ReadLine
            If Not Directory.Exists(MediaFireTestDirectory) Then
                MediaFireTestDirectory = ""
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
            MediaFireDirectory = ""
            MediaFireTestDirectory = ""
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

        If MediaFireDirectory <> "" Then
            If MediaFireDirectory.Substring(Len(MediaFireDirectory) - 1) <> "\" Then
                MediaFireDirectory = MediaFireDirectory & "\"
            End If
        End If

        If MediaFireTestDirectory <> "" Then
            If MediaFireTestDirectory.Substring(Len(MediaFireTestDirectory) - 1) <> "\" Then
                MediaFireTestDirectory = MediaFireTestDirectory & "\"
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

        If MediaFireDirectory <> "\" Then
            lblFilesPath.Text = MediaFireDirectory
        End If

        If MediaFireTestDirectory <> "\" Then
            lblTestPath.Text = MediaFireTestDirectory
        End If

        If RootDirectory <> "\" Then
            lblRootDebugFolderPath.Text = RootDirectory
        End If

        LatestVersionXML = "LatestVersionIPH.xml"
        LatestTestVersionXML = "LatestVersionIPH Test.xml"

        ' When updating the image files to build the zip, update the root directory images as well so we have the updated images for running in debug mode
        WorkingImageFolder = RootDirectory & "EVEIPH Images"

        IECFOlder = WorkingDirectory & "Types"
        EVEIPHImageFolder = WorkingDirectory & "EVEIPH Images"
        MissingImagesFilePath = WorkingDirectory & " Missing Images.txt"

    End Sub

    Private Sub SetProgressBarValues(ByVal TableName As String)

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim mySQLReader1 As SqlDataReader
        Dim msSQL As String

        Dim DBRef As New SqlConnection
        DBRef = DBConnectionRef()

        Dim i As Integer

        ' Now select the count of the final query of data
        msSQL = "SELECT COUNT(*) FROM " & TableName
        msSQLQuery = New SqlCommand(msSQL, DBRef)
        mySQLReader1 = msSQLQuery.ExecuteReader()
        mySQLReader1.Read()
        pgMain.Maximum = mySQLReader1.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader1.Close()
        msSQLQuery = Nothing

    End Sub

    Private Sub btnSelectMediaFirePath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectFilePath.Click
        If MediaFireDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = MediaFireDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblFilesPath.Text = FolderBrowserDialog.SelectedPath
                MediaFireDirectory = FolderBrowserDialog.SelectedPath
                Call SetFilePaths()
            Catch ex As Exception
                MsgBox(Err.Description, vbExclamation, Application.ProductName)
            End Try
        End If
    End Sub

    Private Sub btnSelectMediaFireTestPath_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectTestFilePath.Click
        If MediaFireTestDirectory <> "" Then
            FolderBrowserDialog.SelectedPath = MediaFireTestDirectory
        End If

        If FolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Try
                lblTestPath.Text = FolderBrowserDialog.SelectedPath
                MediaFireTestDirectory = FolderBrowserDialog.SelectedPath
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
        MediaFireDirectory = lblFilesPath.Text
        MediaFireTestDirectory = lblTestPath.Text

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

        If MediaFireDirectory <> "" Then
            If chkCreateTest.Checked Then
                di = New DirectoryInfo(MediaFireTestDirectory)
            Else
                di = New DirectoryInfo(MediaFireDirectory)
            End If

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
        Call EnableButtons(False)

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
        File.Delete(MissingImagesFilePath)

        Dim OutputFile As New StreamWriter(MissingImagesFilePath)
        OutputFile.WriteLine("Blueprint ID - Blueprint Name")
        MissingImages = False

        ' Get the count first
        SQL = "SELECT COUNT(*) FROM ALL_BLUEPRINTS"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBREf)
        readerBPs = DBCommand.ExecuteReader
        readerBPs.Read()
        ReaderCount = readerBPs.GetValue(0)
        readerBPs.Close()

        ' Get all the BP ID numbers we use in the program and copy those files to the directory
        SQL = "SELECT BLUEPRINT_ID, BLUEPRINT_NAME FROM ALL_BLUEPRINTS"
        DBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBREf)
        readerBPs = DBCommand.ExecuteReader

        pgMain.Value = 0
        pgMain.Maximum = ReaderCount
        pgMain.Visible = True

        Application.DoEvents()

        ' Loop through and copy all the images to the new folder - use absolute value for stuff I use negative typeIDs for (outpost stuff)
        While readerBPs.Read
            Application.DoEvents()
            Try
                ' For zip use
                File.Copy(IECFOlder & "\" & CStr(Math.Abs(readerBPs.GetValue(0))) & "_64.png", EVEIPHImageFolder & "\" & CStr(Math.Abs(readerBPs.GetValue(0))) & "_64.png")
                ' To root Working Directory
                File.Copy(IECFOlder & "\" & CStr(Math.Abs(readerBPs.GetValue(0))) & "_64.png", WorkingImageFolder & "\" & CStr(Math.Abs(readerBPs.GetValue(0))) & "_64.png")
            Catch
                ' Build a file with the BP ID's and Names that do not have a image
                OutputFile.WriteLine(readerBPs(0).ToString & " - " & readerBPs(1).ToString)
                MissingImages = True
            End Try

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

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
        File.Copy(IECFOlder & "\37135_64.png", EVEIPHImageFolder & "\37135_64.png")    ' Endurance
        File.Copy(IECFOlder & "\37135_64.png", WorkingImageFolder & "\37135_64.png")   ' Endurance

        ' Delete the file if it already exists
        File.Delete(WorkingDirectory & "EVEIPH Images.zip")
        ' Compress the images
        Call ZipFile.CreateFromDirectory(EVEIPHImageFolder, WorkingDirectory & "EVEIPH Images.zip", CompressionLevel.Optimal, False)

        pgMain.Visible = False

        ' If we didn't output any missing images, delete the output fille
        If Not MissingImages Then
            OutputFile.Close()
            File.Delete(MissingImagesFilePath)
        End If

        ' Leave working folder for use with binary builder
        Call CloseDBs()

        Me.Cursor = Cursors.Default
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

        ' Copy all these files from the media file directory (should be most up to date) to the working directory to make the zip
        File.Copy(MediaFireDirectory & JSONDLL, FinalBinaryFolderPath & JSONDLL)
        File.Copy(MediaFireDirectory & SQLiteDLL, FinalBinaryFolderPath & SQLiteDLL)
        File.Copy(MediaFireDirectory & EVEIPHEXE, FinalBinaryFolderPath & EVEIPHEXE)
        File.Copy(MediaFireDirectory & EVEIPHUpdater, FinalBinaryFolderPath & EVEIPHUpdater)
        File.Copy(MediaFireDirectory & UpdaterManifest, FinalBinaryFolderPath & UpdaterManifest)
        File.Copy(MediaFireDirectory & EXEManifest, FinalBinaryFolderPath & EXEManifest)
        File.Copy(MediaFireDirectory & LatestVersionXML, FinalBinaryFolderPath & LatestVersionXML)
        'File.Copy(MediaFireDirectory & LatestVersionXML, FinalBinaryFolderPath & IonicZipFile)
        File.Copy(MediaFireDirectory & MoreLinqDLL, FinalBinaryFolderPath & MoreLinqDLL)

        ' DB
        File.Copy(WorkingDirectory & EVEIPHDB, FinalBinaryFolderPath & EVEIPHDB)

        ' IPH images
        My.Computer.FileSystem.CopyDirectory(WorkingDirectory & ImageFolder, FinalBinaryFolderPath & ImageFolder, True)

        ' Delete the file if it already exists
        File.Delete(FinalBinaryZipPath)
        ' Compress the whole file for download
        Call ZipFile.CreateFromDirectory(FinalBinaryFolderPath, FinalBinaryZipPath, CompressionLevel.Optimal, False)

        File.Delete(MediaFireDirectory & FinalBinaryZip)

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
        btnBuildSQLServerDB.Enabled = EnableValue
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
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim ColumnLength As Integer

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "SELECT MAX(Datalength(" & FieldName & ")) FROM " & TableName
        msSQLQuery = New SqlCommand(SQL, DBRef1)
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

    Public Sub Execute_msSQL(ByVal SQL As String)
        Dim Command As SqlCommand
        Dim DBRef As New SqlConnection
        DBRef = DBConnectionRef()

        Command = New SqlCommand(SQL, DBRef)
        Command.ExecuteNonQuery()

        Command = Nothing

        DBRef.Close()
        DBRef.Dispose()

    End Sub

    Public Sub ResetTable(TableName As String)
        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim SQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        ' See if the table exists and drop if it does
        msSQL = "SELECT COUNT(*) FROM sys.tables WHERE name = '" & TableName & "'"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
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

#Region "Database Update"

    ' Create a new database, build tables and indexes, then populate it with the different updated tables
    Private Sub btnBuildDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuildDatabase.Click

        ' Make sure we have a DB first
        If DatabaseName = "" Then
            MsgBox("Database Name not defined", vbExclamation, Application.ProductName)
            Call txtDBName.Focus()
            Exit Sub
        End If

        Call EnableButtons(False)

        ' Build DB's and open connections
        Call BuildDB(FinalDBPath)

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

        Call EnableButtons(True)

        Call CloseDBs()

        Application.DoEvents()
        Call MsgBox("Database Created", vbInformation, "Complete")

    End Sub

    Private Sub BuildDB(DBPathandName As String)

        ' Check for SQLite DB
        If File.Exists(DBPathandName & ".s3db") Then
            Try
                EVEIPHSQLiteDB.CloseDB()
            Catch
                ' Nothing
            End Try
            ' Delete old one
            File.Delete(DBPathandName & ".s3db")
        End If

        ' Create new SQLite DB
        SQLiteConnection.CreateFile(DBPathandName & ".s3db")

    End Sub

    Private Function ConnectToDBs() As Boolean
        Application.DoEvents()
        Me.Cursor = Cursors.WaitCursor
        Dim SQLInstanceName = ""

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        If (txtSqlInstanceName.Text = "") Then
            MessageBox.Show("SQL Server Instance Name not supplied", "Error", MessageBoxButtons.OK)
            Return False
        End If

        SQLInstanceName = txtSqlInstanceName.Text

        Try
            ' Open connections
            DBRef1 = New SqlConnection(String.Format("Server={0}\{1};Database={2};Trusted_Connection=True; Initial Catalog=master; Integrated Security=True;Connection Timeout=60;",
                                             Environment.MachineName, SQLInstanceName, DatabaseName))
            DBRef1.Open()

            ' SQL Lite DB
            If File.Exists(FinalDBPath & ".s3db") Then
                EVEIPHSQLiteDB = New SQLiteDBConnection(FinalDBPath & ".s3db")
                ' Set pragma to make this faster
                Call Execute_SQLiteSQL("PRAGMA synchronous = OFF", EVEIPHSQLiteDB.DBREf)
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
        EVEIPHSQLiteDB.CloseDB()
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

        ' Set the version value
        SQL = "CREATE TABLE DB_VERSION ("
        SQL = SQL & "VERSION_NUMBER VARCHAR(50)"
        SQL = SQL & ")"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Insert the database name for the version
        Call Execute_SQLiteSQL("INSERT INTO DB_VERSION VALUES ('" & DatabaseName & "')", EVEIPHSQLiteDB.DBREf)

        ' Need a view for industryMaterials
        Call Execute_msSQL("IF EXISTS(select * FROM sys.views where name = 'MY_INDUSTRY_MATERIALS') DROP VIEW MY_INDUSTRY_MATERIALS")

        SQL = "CREATE VIEW MY_INDUSTRY_MATERIALS AS "
        SQL = SQL & "SELECT blueprintTypeID, activityID, materialTypeID, quantity, 1 AS consume FROM industryActivityMaterials "
        SQL = SQL & "UNION "
        SQL = SQL & "SELECT blueprintTypeID, activityID, skillID AS materialTypeID, level as quantity, 0 AS consume FROM industryActivitySkills"
        Call Execute_msSQL(SQL)

        lblTableName.Text = "Building: OreRefine"
        Call BuildOreRefine()

        lblTableName.Text = "Building: LP_OFFER_REQUIREMENTS"
        Call Build_LP_OFFER_REQUIREMENTS()

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

        lblTableName.Text = "Building: LP_STORE"
        Call Build_LP_STORE()

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

        lblTableName.Text = "Building: CHARACTER_SHEET"
        Call Build_CHARACTER_SHEET()

        lblTableName.Text = "Building: CHARACTER_SKILLS"
        Call Build_CHARACTER_SKILLS()

        lblTableName.Text = "Building: CHARACTER_IMPLANTS"
        Call Build_CHARACTER_IMPLANTS()

        lblTableName.Text = "Building: CHARACTER_JUMP_CLONES"
        Call Build_CHARACTER_JUMP_CLONES()

        lblTableName.Text = "Building: CHARACTER_CORP_ROLES"
        Call Build_CHARACTER_CORP_ROLES()

        lblTableName.Text = "Building: CHARACTER_CORP_TITLES"
        Call Build_CHARACTER_CORP_TITLES()

        lblTableName.Text = "Building: CHARACTER_API"
        Call Build_API()

        lblTableName.Text = "Building: PRICE_PROFILES"
        Call Build_PRICE_PROFILES()

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

        lblTableName.Text = "Building: REPROCESSING"
        Call Build_Reprocessing()

        lblTableName.Text = "Building: ORES"
        Call Build_ORES()

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

        lblTableName.Text = "Building: INVENTORY_TRAITS"
        Call Build_INVENTORY_TRAITS()

        ' After we are done with everything, use the following tables to update the RACE ID value in the ALL_BLUEPRINTS table
        lblTableName.Text = "Updating the Race ID's"

        ' Set null to zero
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 0 WHERE RACE_ID IS NULL "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' 1 = Caldari
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 1 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Caldari Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Caldari Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Caldari' OR BLUEPRINT_GROUP IN ('Missile Blueprint','Missile Launcher Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Caldari%'  OR BLUEPRINT_NAME LIKE 'Caldari%' "
        SQL = SQL & "AND RACE_ID = 0 "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' 2 = Minmatar
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 2 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Minmatar Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Minmatar Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Minmatar' OR BLUEPRINT_GROUP IN ('Projectile Ammo Blueprint','Projectile Weapon Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Republic%'  OR BLUEPRINT_NAME LIKE 'Minmatar%' "
        SQL = SQL & "AND RACE_ID = 0 "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' 4 = Amarr
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 4 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Amarr Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Amarr Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Amarr' OR BLUEPRINT_GROUP IN ('Energy Weapon Blueprint','Frequency Crystal Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Ammatar%' OR BLUEPRINT_NAME LIKE 'Imperial Navy%' OR BLUEPRINT_NAME LIKE 'Khanid Navy%' OR BLUEPRINT_NAME LIKE 'Amarr%' "
        SQL = SQL & "AND RACE_ID = 0"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' 8 = Gallente
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 8 "
        SQL = SQL & "WHERE "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Gallente Encryption Methods') OR "
        SQL = SQL & "BLUEPRINT_ID IN (SELECT DISTINCT productTypeID FROM INDUSTRY_ACTIVITY_PRODUCTS WHERE blueprintTypeID IN "
        SQL = SQL & "(SELECT DISTINCT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL = 'Gallente Encryption Methods')) "
        SQL = SQL & "OR MARKET_GROUP ='Gallente' OR BLUEPRINT_GROUP IN ('Hybrid Charge Blueprint','Hybrid Weapon Blueprint', 'Capacitor Booster Charge Blueprint', 'Bomb Blueprint') "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Federation%' OR BLUEPRINT_NAME LIKE 'Gallente%' "
        SQL = SQL & "AND RACE_ID = 0"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 15 WHERE MARKET_GROUP = 'Pirate Faction' OR RACE_ID > 15"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 15 WHERE BLUEPRINT_NAME LIKE 'Serpentis%' OR BLUEPRINT_NAME LIKE 'Angel%' OR BLUEPRINT_NAME LIKE 'Blood%'"
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'Domination%' OR BLUEPRINT_NAME LIKE 'Dread Guristas%' OR BLUEPRINT_NAME LIKE 'Guristas%' "
        SQL = SQL & "OR BLUEPRINT_NAME LIKE 'True Sansha%' OR BLUEPRINT_NAME LIKE 'Sansha%' OR BLUEPRINT_NAME LIKE 'Shadow%' OR BLUEPRINT_NAME LIKE 'Dark Blood%'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Set all the structures now that are zero
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 1 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Missile Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 2 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Projectile Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 4 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Laser Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 8 WHERE RACE_ID <> 15 AND ITEM_CATEGORY = 'Structure' "
        SQL = SQL & "AND ITEM_GROUP IN ('Mobile Hybrid Sentry')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Update any remaining by blueprint group
        SQL = "SELECT DISTINCT BLUEPRINT_GROUP, RACE_ID FROM ALL_BLUEPRINTS WHERE RACE_ID <> 0 "
        SQLiteDBCommand = New SQLiteCommand(SQL, EVEIPHSQLiteDB.DBREf)
        SQLiteReader = SQLiteDBCommand.ExecuteReader

        While SQLiteReader.Read
            SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = " & SQLiteReader.GetInt32(1) & " "
            SQL = SQL & "WHERE BLUEPRINT_GROUP = '" & SQLiteReader.GetString(0) & "' "
            SQL = SQL & "AND RACE_ID = 0 AND ITEM_CATEGORY IN ('Module', 'Drone')"
            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        End While

        ' Station Parts should be 'Other'
        SQL = "UPDATE ALL_BLUEPRINTS SET RACE_ID = 0 WHERE ITEM_GROUP_ID = 536 "
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Fix for Pheobe SDE issues - These were removed from game but haven't been deleted from SDE
        SQL = "DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE MATERIAL_CATEGORY = 'Decryptors'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE BLUEPRINT_NAME LIKE '%Data Interface Blueprint'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "DELETE FROM ALL_BLUEPRINTS WHERE ITEM_GROUP = 'Data Interfaces'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        lblTableName.Text = "Finalizing..."
        Application.DoEvents()

        ' Run a vacuum on the new SQL DB
        Call Execute_SQLiteSQL("VACUUM;", EVEIPHSQLiteDB.DBREf)

    End Sub

    ' ALL_BLUEPRINTS
    Private Sub Build_ALL_BLUEPRINTS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        ' See if the table exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.tables where name = 'ALL_BLUEPRINTS'"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
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
        SQL = SQL & "industryActivityProducts.quantity AS PORTION_SIZE, "
        SQL = SQL & "industryActivities.time AS BASE_PRODUCTION_TIME, "
        SQL = SQL & "IA1.time AS BASE_RESEARCH_TL_TIME, "
        SQL = SQL & "IA2.time AS BASE_RESEARCH_ML_TIME, "
        SQL = SQL & "IA3.time AS BASE_COPY_TIME, "
        SQL = SQL & "IA4.time AS BASE_INVENTION_TIME, "
        SQL = SQL & "industryBlueprints.maxProductionLimit AS MAX_PRODUCTION_LIMIT, "
        SQL = SQL & "isNULL(dgmTypeAttributes.valueInt,dgmTypeAttributes.valueFloat) AS ITEM_TYPE, "
        SQL = SQL & "invTypes.raceID AS RACE_ID, "
        SQL = SQL & "invMetaTypes.metaGroupID AS META_GROUP, "
        SQL = SQL & "'XX' AS SIZE_GROUP, "
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
        SQL = SQL & "AND (invTypes1.published <> 0 AND invTypes.published <> 0 AND invGroups1.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0" ' -- 2830 bps
        SQL = SQL & "OR industryBlueprints.blueprintTypeID < 0)" ' For all outpost "blueprints"

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
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 15 WHERE META_GROUP = 4 AND ITEM_CATEGORY IN ('Structure', 'Starbase')"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Module'"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = 16 WHERE META_GROUP = 4 AND ITEM_CATEGORY = 'Drone'" ' Augmented and Integrated drones
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET ITEM_TYPE = TECH_LEVEL WHERE ITEM_TYPE = 0 OR ITEM_TYPE IS NULL"
        Execute_msSQL(SQL)
        SQL = "UPDATE ALL_BLUEPRINTS SET TECH_LEVEL = 1, ITEM_TYPE = 15 WHERE  BLUEPRINT_GROUP = 'Combat Drone Blueprint' AND ITEM_TYPE = 16" ' Aug/Integrated drones
        Execute_msSQL(SQL)

        ' Add the S/M/L/XL tag to these here

        ' Drones are light, missiles are rockets and light
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'S' WHERE SIZE_GROUP = 'XX' AND ("
        SQL = SQL & "ITEM_NAME LIKE '% S' OR ITEM_NAME Like '%Small%' "
        SQL = SQL & "OR (ITEM_NAME Like '%Micro%' AND ITEM_GROUP <> 'Propulsion Module' AND ITEM_NAME NOT LIKE 'Microwave%') "
        SQL = SQL & "OR ITEM_NAME Like '%Defender%' "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Implant') "
        SQL = SQL & "OR ITEM_NAME IN ('Cap Booster 25','Cap Booster 50') "
        SQL = SQL & "OR MARKET_GROUP IN ('Interdiction Probes', 'Mining Crystals', 'Nanite Repair Paste', 'Scan Probes', 'Survey Probes', 'Scripts') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID from invTypes where packagedVolume = 5)) "
        SQL = SQL & "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '1MN%') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID from invTypes where marketGroupID IN (561,564,567,570,574,577,1671,1672,1037)))  "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND (ITEM_NAME Like '%Rocket%' OR ITEM_NAME Like '%Light Missile%') AND ITEM_GROUP NOT IN ('Propulsion Module', 'Rig Launcher'))  "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE packagedVolume < 10000) AND ITEM_GROUP_ID <> 963) OR ITEM_GROUP_ID = 1527)"

        Execute_msSQL(SQL)

        ' Drones are medium, missiles are heavys and hams
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'M' WHERE SIZE_GROUP = 'XX' AND ("
        SQL = SQL & "ITEM_NAME LIKE '% M' OR ITEM_NAME Like '%Medium%' OR ITEM_NAME IN ('Cap Booster 75','Cap Booster 100') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE packagedVolume = 10)) "
        SQL = SQL & "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '10MN%') "
        SQL = SQL & "OR (ITEM_GROUP IN ('Gang Coordinator')) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Subsystem') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (562,565,568,572,575,578,1673,1674))) "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND ITEM_NAME Like '%Heavy%' AND ITEM_NAME Not Like '%Jolt%')  "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE (packagedVolume >= 10000 AND packagedVolume < 50000) "
        SQL = SQL & "OR ITEM_GROUP_ID = 963 OR ITEM_GROUP_ID = 1534))) "
        Execute_msSQL(SQL)

        ' Drones are Heavy, missiles are cruise/torp, towers are regular towers (Caldari Control Tower)
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'L' "
        SQL = SQL & "WHERE SIZE_GROUP = 'XX' AND (ITEM_NAME LIKE '% L' "
        SQL = SQL & "OR (ITEM_NAME Like '%Large%' AND ITEM_NAME NOT Like '%X-Large%') "
        SQL = SQL & "OR ITEM_NAME IN ('Cap Booster 150','Cap Booster 200')"
        SQL = SQL & "OR (ITEM_CATEGORY = 'Drone' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE packagedVolume >= 25 and packagedVolume <=50)) "
        SQL = SQL & "OR (ITEM_GROUP = 'Propulsion Module' AND ITEM_NAME Like '100MN%') "
        SQL = SQL & "OR (ITEM_NAME Like ('%Control Tower')) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Deployable' AND ITEM_GROUP <> 'Mobile Warp Disruptor') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Structure' AND ITEM_GROUP <> 'Control Tower')"
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_NAME Like '%Heavy%' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID NOT IN (563,566,569,573,576,579,1675,1676))) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (563,566,569,573,576,579,1675,1676))) "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND (ITEM_NAME Like '%Cruise%' OR ITEM_NAME Like '%Torpedo%')) "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE (packagedVolume >= 50000 AND packagedVolume < 500000))))"
        Execute_msSQL(SQL)

        ' Drones are fighters, missiles are citadel
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'XL' "
        SQL = SQL & "WHERE SIZE_GROUP = 'XX' AND (ITEM_NAME LIKE '% XL' "
        SQL = SQL & "OR ITEM_NAME Like '%Capital%' "
        SQL = SQL & "OR ITEM_NAME Like '%Huge%'"
        SQL = SQL & "OR ITEM_NAME Like '%X-Large%' "
        SQL = SQL & "OR ITEM_NAME Like '%Giant%' "
        SQL = SQL & "OR ITEM_CATEGORY IN ('Infrastructure Upgrades','Sovereignty Structures','Orbitals') "
        SQL = SQL & "OR ITEM_GROUP IN ('Station Components', 'Remote ECM Burst', 'Super Weapon', 'Siege Module')"
        SQL = SQL & "OR ITEM_NAME IN ('Cap Booster 400','Cap Booster 800') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Fighter') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Module' AND (ITEM_ID IN (SELECT typeID FROM invTypes WHERE marketGroupID IN (771,772,773,774,775,776,1642,1941)))) "
        SQL = SQL & "OR (ITEM_GROUP IN ('Jump Drive Economizer','Drone Control Unit') OR ITEM_NAME LIKE 'Jump Portal%') "
        SQL = SQL & "OR (ITEM_CATEGORY IN ('Charge','Module') AND ITEM_NAME Like '%Citadel%') "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Celestial' AND (ITEM_NAME Like 'Station%' OR ITEM_NAME LIKE '%Outpost%' OR ITEM_NAME LIKE '%Freight%')) "
        SQL = SQL & "OR ITEM_GROUP LIKE 'Bomb%' "
        SQL = SQL & "OR (ITEM_CATEGORY = 'Ship' AND ITEM_ID IN (SELECT typeID FROM invTypes WHERE packagedVolume >= 500000)))"
        Execute_msSQL(SQL)

        ' Anything left update to small (may need to revisit later)
        SQL = "UPDATE ALL_BLUEPRINTS SET SIZE_GROUP = 'S' WHERE SIZE_GROUP = 'XX'"
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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ALL_BLUEPRINTS")

        ' Now select the final query of data
        msSQL = "SELECT * FROM ALL_BLUEPRINTS"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_AB_ITEM_ID ON ALL_BLUEPRINTS (ITEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AB_BP_NAME ON ALL_BLUEPRINTS (BLUEPRINT_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AB_CAT_ITEM ON ALL_BLUEPRINTS (ITEM_CATEGORY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AB_GROUP_ITEM ON ALL_BLUEPRINTS (ITEM_GROUP,ITEM_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        Application.DoEvents()

        ' See if the table exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.tables where name = 'ALL_BLUEPRINT_MATERIALS'"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
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
        SQL = SQL & "MY_INDUSTRY_MATERIALS.materialTypeID AS MATERIAL_ID, matTypes.typeName AS MATERIAL, matGroups.groupName AS MATERIAL_GROUP,  "
        SQL = SQL & "matCategories.categoryName AS MATERIAL_CATEGORY, matTypes.packagedVolume AS MATERIAL_VOLUME, MY_INDUSTRY_MATERIALS.quantity AS QUANTITY, "
        SQL = SQL & "MY_INDUSTRY_MATERIALS.activityID AS ACTIVITY, MY_INDUSTRY_MATERIALS.consume AS CONSUME "
        SQL = SQL & "INTO ALL_BLUEPRINT_MATERIALS "
        SQL = SQL & "FROM industryBlueprints, invTypes, industryActivityProducts, MY_INDUSTRY_MATERIALS, invGroups, invCategories, "
        SQL = SQL & "invTypes AS matTypes, invGroups AS matGroups, invCategories AS matCategories "
        SQL = SQL & "WHERE industryBlueprints.blueprintTypeID = invTypes.typeID "
        SQL = SQL & "AND industryBlueprints.blueprintTypeID = industryActivityProducts.blueprintTypeID "
        SQL = SQL & "AND industryBlueprints.blueprintTypeID = MY_INDUSTRY_MATERIALS.blueprintTypeID "
        SQL = SQL & "AND industryActivityProducts.activityID = MY_INDUSTRY_MATERIALS.activityID "
        SQL = SQL & "AND matTypes.typeID = MY_INDUSTRY_MATERIALS.materialTypeID "
        SQL = SQL & "AND matGroups.groupID = matTypes.groupID "
        SQL = SQL & "AND matGroups.categoryID = matCategories.categoryID "
        SQL = SQL & "AND invTypes.groupID = invGroups.groupID "
        SQL = SQL & "AND invGroups.categoryID = invCategories.categoryID "
        SQL = SQL & "AND (invTypes.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0 "
        SQL = SQL & "OR industryBlueprints.blueprintTypeID < 0) " ' For all outpost "blueprints"
        SQL = SQL & "ORDER BY BLUEPRINT_ID, PRODUCT_ID "

        Call Execute_msSQL(SQL)

        ' Update all the materials that are blueprints - add copy to the name to reduce confusion, only materials required are BPCs
        Call Execute_msSQL("UPDATE ALL_BLUEPRINT_MATERIALS SET MATERIAL = MATERIAL + ' Copy' WHERE MATERIAL_CATEGORY = 'Blueprint'")

        ' Also, find any bp that has the product the same as a material id, this will cause an infinite loop
        SQL = "SELECT BLUEPRINT_ID FROM ALL_BLUEPRINT_MATERIALS where PRODUCT_ID = MATERIAL_ID"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        ' Delete these BPs from the materials and all_blueprints tables before building the final in SQLite
        While msSQLReader.Read
            Call Execute_msSQL("DELETE FROM ALL_BLUEPRINT_MATERIALS WHERE BLUEPRINT_ID = " & CStr(msSQLReader.GetInt32(0)))
            ' This table is already built in sqlite, so delete from there
            Call Execute_SQLiteSQL("DELETE FROM ALL_BLUEPRINTS WHERE BLUEPRINT_ID = " & CStr(msSQLReader.GetInt32(0)), EVEIPHSQLiteDB.DBREf)
        End While

        msSQLReader.Close()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ALL_BLUEPRINT_MATERIALS")

        ' Now select the final query of data
        msSQL = "SELECT * FROM ALL_BLUEPRINT_MATERIALS"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_ABM_BP_ID_ACTIVITY ON ALL_BLUEPRINT_MATERIALS (BLUEPRINT_ID,ACTIVITY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ABM_PRODUCT_ID_ACTIVITY ON ALL_BLUEPRINT_MATERIALS (PRODUCT_ID, ACTIVITY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ABM_REQCOMP_ID_PRODUCT ON ALL_BLUEPRINT_MATERIALS (MATERIAL_ID, PRODUCT_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' ASSEMBLY_ARRAYS
    Private Sub Build_ASSEMBLY_ARRAYS()
        Dim SQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call SetProgressBarValues(" (" & msSQL & ") AS X ")

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        msSQLReader.Close()

        ' Finally, index
        SQL = "CREATE INDEX IDX_AA_AID_CID_GID ON ASSEMBLY_ARRAYS (ACTIVITY_ID, CATEGORY_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AA_AN_AID_CID_GID ON ASSEMBLY_ARRAYS (ARRAY_NAME, ACTIVITY_ID, CATEGORY_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    End Sub

    ' STATION_FACILITIES - Temp table, update with CREST
    Private Sub Build_STATION_FACILITIES()
        Dim SQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call SetProgressBarValues(" (" & msSQL & ") AS X ")

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        msSQLReader.Close()
        Application.DoEvents()

        ' Finally do indexes
        SQL = "CREATE INDEX IDX_SF_FN_AID ON STATION_FACILITIES (FACILITY_NAME, ACTIVITY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SF_FID_AID_GID_CID ON STATION_FACILITIES (FACILITY_ID, ACTIVITY_ID, GROUP_ID, CATEGORY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SF_OP_FN_AID_CID ON STATION_FACILITIES (OUTPOST, FACILITY_NAME, ACTIVITY_ID, CATEGORY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SF_OP_FN_AID_GID ON STATION_FACILITIES (OUTPOST, FACILITY_NAME, ACTIVITY_ID, GROUP_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SF_SSID_AID ON STATION_FACILITIES (SOLAR_SYSTEM_ID, ACTIVITY_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SF_OP_AID_GID_CID_RN_SSN ON STATION_FACILITIES (OUTPOST, ACTIVITY_ID, GROUP_ID, CATEGORY_ID, REGION_NAME, SOLAR_SYSTEM_NAME);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()
        Dim DBRef2 As New SqlConnection
        DBRef2 = DBConnectionRef()
        Dim DBRef3 As New SqlConnection
        DBRef3 = DBConnectionRef()

        ' Figure out what lines are not in the categories table so that we can add the missing line and categoryID
        msSQL = "SELECT ramAssemblyLineTypes.assemblyLineTypeID, activityID "
        msSQL = msSQL & "FROM ramAssemblyLineTypes, ramInstallationTypeContents, invTypes "
        msSQL = msSQL & "WHERE ramAssemblyLineTypes.assemblyLineTypeID NOT IN (SELECT assemblyLineTypeID FROM ramAssemblyLineTypeDetailPerCategory) "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID NOT IN (SELECT assemblyLineTypeID FROM ramAssemblyLineTypeDetailPerGroup) "
        msSQL = msSQL & "AND ramAssemblyLineTypes.assemblyLineTypeID = ramInstallationTypeContents.assemblyLineTypeID "
        msSQL = msSQL & "AND ramInstallationTypeContents.installationTypeID = invTypes.typeID "
        msSQL = msSQL & "GROUP BY ramAssemblyLineTypes.assemblyLineTypeID, activityID "
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
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

            msSQLQuery2 = New SqlCommand(msSQL, DBRef2)
            msSQLReader2 = msSQLQuery2.ExecuteReader()

            While msSQLReader2.Read
                ' Now insert the data into the ramAssemblyLineTypeDetailPerCategory table if not there
                msSQL = "SELECT 'X' FROM ramAssemblyLineTypeDetailPerCategory "
                msSQL = msSQL & "WHERE assemblyLineTypeID = " & msSQLReader.GetValue(0) & " "
                msSQL = msSQL & "AND categoryID = " & msSQLReader2.GetValue(0) & " "
                msSQL = msSQL & "AND timeMultiplier = 1 AND materialMultiplier = 1 AND costMultiplier = 1"

                msSQLQuery3 = New SqlCommand(msSQL, DBRef3)
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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        SQL = "CREATE TABLE STATIONS ("
        SQL = SQL & "STATION_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "STATION_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "STATION_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER,"
        SQL = SQL & "SOLAR_SYSTEM_SECURITY FLOAT NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL,"
        SQL = SQL & "REPROCESSING_EFFICIENCY FLOAT NOT NULL,"
        SQL = SQL & "REPROCESSING_TAX_RATE FLOAT NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("staStations")

        ' Pull new data and insert
        msSQL = "SELECT stationID, stationName, stationTypeID, solarSystemID, security, regionID, reprocessingEfficiency, reprocessingStationsTake FROM staStations"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO STATIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQL = "CREATE INDEX IDX_S_FID ON STATIONS (STATION_ID);"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        msSQLReader.Close()

    End Sub

    ' RACE_IDS
    Private Sub Build_RACE_IDS()
        Dim SQL As String

        SQL = "CREATE TABLE RACE_IDS (ID INTEGER PRIMARY KEY, RACE VARCHAR(8) NOT NULL)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "INSERT INTO RACE_IDS VALUES (1, 'Caldari')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "INSERT INTO RACE_IDS VALUES (2, 'Minmatar')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "INSERT INTO RACE_IDS VALUES (4, 'Amarr')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "INSERT INTO RACE_IDS VALUES (8, 'Gallente')"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' FW_SYSTEM_UPGRADES
    Private Sub Build_FW_SYSTEM_UPGRADES()
        Dim SQL As String

        SQL = "CREATE TABLE FW_SYSTEM_UPGRADES (SOLAR_SYSTEM_ID INTEGER PRIMARY KEY, UPGRADE_LEVEL INTEGER NOT NULL)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CSKILLS_CHARACTER_ID ON CHARACTER_SKILLS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CSKILLS_SKILL_TYPE_ID ON CHARACTER_SKILLS (SKILL_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' CHARACTER_SHEET
    Private Sub Build_CHARACTER_SHEET()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_SHEET ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "CHARACTER_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "HOME_STATION_ID INTEGER NOT NULL,"
        SQL = SQL & "DOB VARCHAR(20) NOT NULL,"
        SQL = SQL & "RACE VARCHAR(10) NOT NULL,"
        SQL = SQL & "BLOOD_LINE_ID INTEGER NOT NULL,"
        SQL = SQL & "BLOOD_LINE VARCHAR(20) NOT NULL,"
        SQL = SQL & "ANCESTRY_LINE_ID INTEGER NOT NULL,"
        SQL = SQL & "ANCESTRY_LINE VARCHAR(20) NOT NULL,"
        SQL = SQL & "GENDER VARCHAR(6) NOT NULL,"
        SQL = SQL & "CORPORATION_NAME VARCHAR(200),"
        SQL = SQL & "CORPORATION_ID INTEGER NOT NULL,"
        SQL = SQL & "ALLIANCE_NAME VARCHAR(200),"
        SQL = SQL & "ALLIANCE_ID INTEGER NOT NULL,"
        SQL = SQL & "FACTION_NAME VARCHAR(200) NOT NULL,"
        SQL = SQL & "FACTION_ID INTEGER NOT NULL,"
        SQL = SQL & "FREE_SKILL_POINTS INTEGER NOT NULL,"
        SQL = SQL & "FREE_RESPECS INTEGER NOT NULL,"
        SQL = SQL & "CLONE_JUMP_DATE VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "LAST_RESPEC_DATE VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "LAST_TIMED_RESPEC VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "REMOTE_STATION_DATE VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "JUMP_ACTIVATION VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "JUMP_FATIGUE VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "JUMP_LAST_UPDATE VARCHAR(23) NOT NULL," ' Date
        SQL = SQL & "BALANCE FLOAT NOT NULL," ' Date
        SQL = SQL & "INTELLIGENCE INTEGER NOT NULL,"
        SQL = SQL & "MEMORY INTEGER NOT NULL,"
        SQL = SQL & "WILLPOWER INTEGER NOT NULL,"
        SQL = SQL & "PERCEPTION INTEGER NOT NULL,"
        SQL = SQL & "CHARISMA INTEGER NOT NULL"
        SQL = SQL & ")"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CSHEET_CHARACTER_ID ON CHARACTER_SHEET (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' CHARACTER_IMPLANTS
    Private Sub Build_CHARACTER_IMPLANTS()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_IMPLANTS ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "JUMP_CLONE_ID INTEGER NOT NULL,"
        SQL = SQL & "IMPLANT_ID INTEGER NOT NULL,"
        SQL = SQL & "IMPLANT_NAME VARCHAR(100) NOT NULL)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CI_CHARACTER_ID ON CHARACTER_IMPLANTS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' CHARACTER_JUMP_CLONES
    Private Sub Build_CHARACTER_JUMP_CLONES()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_JUMP_CLONES ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "JUMP_CLONE_ID INTEGER NOT NULL,"
        SQL = SQL & "LOCATION_ID INTEGER NOT NULL,"
        SQL = SQL & "TYPE_ID INTEGER,"
        SQL = SQL & "CLONE_NAME VARCHAR(100)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CJD_CHARACTER_ID ON CHARACTER_JUMP_CLONES (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' CHARACTER_CORP_ROLES
    Private Sub Build_CHARACTER_CORP_ROLES()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_CORP_ROLES ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "ROLE_TYPE VARCHAR(5) NOT NULL," ' Main, HQ, Base, Other
        SQL = SQL & "ROLE_ID INTEGER NOT NULL,"
        SQL = SQL & "ROLE_NAME VARCHAR(100)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CCR_CID_RT ON CHARACTER_CORP_ROLES (CHARACTER_ID, ROLE_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' CHARACTER_CORP_TITLES
    Private Sub Build_CHARACTER_CORP_TITLES()
        Dim SQL As String

        SQL = "CREATE TABLE CHARACTER_CORP_TITLES ("
        SQL = SQL & "CHARACTER_ID INTEGER NOT NULL,"
        SQL = SQL & "TITLE_ID INTEGER NOT NULL,"
        SQL = SQL & "TITLE_NAME VARCHAR(100)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CCT_CID ON CHARACTER_CORP_TITLES (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Since this is all data I created, just do inserts here 
        ' Data from: https://forums.eveonline.com/default.aspx?g=posts&t=418719
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Arkonor',4,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Crimson Arkonor',4,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60800,'Colossal','Prime Arkonor',4,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Bistot',7,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Triclinic Bistot',7,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (114300,'Colossal','Monoclinic Bistot',7,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Crokite',9,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Sharp Crokite',9,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (225200,'Colossal','Crystalline Crokite',9,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Dark Ochre',6,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Onyx Ochre',6,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (115000,'Colossal','Obsidian Ochre',6,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Gneiss',13,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Iridescent Gneiss',13,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (630000,'Colossal','Prismatic Gneiss',13,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Spodumain',14,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Bright Spodumain',14,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (736200,'Colossal','Gleaming Spodumain',14,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Mercoxit',5,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Magma Mercoxit',5,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (7000,'Colossal','Vitreous Mercoxit',5,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (1,'Testing','Vitreous Mercoxit',5,10)", EVEIPHSQLiteDB.DBREf)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Arkonor',4,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Crimson Arkonor',4,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (58000,'Enormous','Prime Arkonor',4,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Bistot',5,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Monoclinic Bistot',5,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (86000,'Enormous','Triclinic Bistot',5,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Crokite',7,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Sharp Crokite',7,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (169000,'Enormous','Crystalline Crokite',7,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Dark Ochre',10,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Obsidian Ochre',10,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (500000,'Enormous','Onyx Ochre',10,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Gneiss',10,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Prismatic Gneiss',10,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (540000,'Enormous','Iridescent Gneiss',10,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (578000,'Enormous','Spodumain',10,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (578000,'Enormous','Gleaming Spodumain',10,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (578000,'Enormous','Bright Spodumain',10,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Mercoxit',4,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Magma Mercoxit',4,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (5200,'Enormous','Vitreous Mercoxit',4,10)", EVEIPHSQLiteDB.DBREf)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Arkonor',3,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Crimson Arkonor',3,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (29900,'Large','Prime Arkonor',3,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Bistot',5,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Monoclinic Bistot',5,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (57000,'Large','Triclinic Bistot',5,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Crokite',6,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Sharp Crokite',6,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (124000,'Large','Crystalline Crokite',6,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Dark Ochre',4,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Obsidian Ochre',4,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (60000,'Large','Onyx Ochre',4,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Gneiss',9,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Prismatic Gneiss',9,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (313500,'Large','Iridescent Gneiss',9,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Spodumain',9,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Gleaming Spodumain',9,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (368100,'Large','Bright Spodumain',9,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Mercoxit',3,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Magma Mercoxit',3,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (3500,'Large','Vitreous Mercoxit',3,10)", EVEIPHSQLiteDB.DBREf)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Medium','Arkonor',3,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Medium','Crimson Arkonor',3,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (28000,'Medium','Prime Arkonor',3,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Medium','Bistot',4,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Medium','Monoclinic Bistot',4,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (38700,'Medium','Triclinic Bistot',4,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Medium','Crokite',5,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Medium','Sharp Crokite',5,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (84700,'Medium','Crystalline Crokite',5,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Medium','Dark Ochre',3,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Medium','Obsidian Ochre',3,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (31000,'Medium','Onyx Ochre',3,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Medium','Gneiss',8,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Medium','Prismatic Gneiss',8,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (340000,'Medium','Iridescent Gneiss',8,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Medium','Spodumain',8,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Medium','Gleaming Spodumain',8,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (270000,'Medium','Bright Spodumain',8,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Medium','Mercoxit',2,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Medium','Magma Mercoxit',2,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (2600,'Medium','Vitreous Mercoxit',2,10)", EVEIPHSQLiteDB.DBREf)

        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9700,'Small','Arkonor',3,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9700,'Small','Crimson Arkonor',3,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (9700,'Small','Prime Arkonor',3,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Bistot',3,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Monoclinic Bistot',3,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (12800,'Small','Triclinic Bistot',3,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crokite',5,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Sharp Crokite',5,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (30000,'Small','Crystalline Crokite',5,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Dark Ochre',4,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Obsidian Ochre',4,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (16000,'Small','Onyx Ochre',4,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Gneiss',6,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Prismatic Gneiss',6,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (170000,'Small','Iridescent Gneiss',6,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Spodumain',7,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Gleaming Spodumain',7,10)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (300000,'Small','Bright Spodumain',7,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Mercoxit',0,0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Magma Mercoxit',0,5)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO INDUSTRY_UPGRADE_BELTS VALUES (0,'Small','Vitreous Mercoxit',0,10)", EVEIPHSQLiteDB.DBREf)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQL = "CREATE INDEX IDX_BP_ID_BELT_NAME ON INDUSTRY_UPGRADE_BELTS (BELT_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CAPI_CHARACTER_ID ON API (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CAPI_KEY_ID ON API (KEY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' PRICE_PROFILES
    Private Sub Build_PRICE_PROFILES()
        Dim SQL As String

        SQL = "CREATE TABLE PRICE_PROFILES ("
        SQL = SQL & "ID INTEGER NOT NULL,"
        SQL = SQL & "GROUP_NAME VARCHAR(50) NOT NULL,"
        SQL = SQL & "PRICE_TYPE VARCHAR(25) NOT NULL,"
        SQL = SQL & "REGION_NAME VARCHAR(50) NOT NULL,"
        SQL = SQL & "SOLAR_SYSTEM_NAME VARCHAR(50) NOT NULL,"
        SQL = SQL & "PRICE_MODIFIER FLOAT NOT NULL,"
        SQL = SQL & "RAW_MATERIAL INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Insert the base data, this will be default until they change it and it's copied in the updater - start with raw, in Jita
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Minerals','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ice Products','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Gas','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Datacores','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Decryptors','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Planetary','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Asteroids','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Salvage','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ancient Salvage','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ancient Relics','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Hybrid Polymers','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Misc.','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Raw Moon Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Processed Moon Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Advanced Moon Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Materials & Compounds','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Rogue Drone Components','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Booster Materials','Min Sell', 'The Forge','Jita',0,1)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Ships','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Charges','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Modules','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Drones','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Rigs','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Deployables','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Subsystems','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Boosters','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Structures','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Structure Modules','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Celestials','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Station Parts','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Adv. Capital Construction Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Capital Construction Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Construction Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Hybrid Tech Components','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Tools','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Fuel Blocks','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "INSERT INTO PRICE_PROFILES VALUES (0,'Implants','Min Sell', 'The Forge','Jita',0,0)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_PP_ID ON PRICE_PROFILES (ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' industryBlueprints
    Private Sub Build_industryBlueprints()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE industryBlueprints ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL PRIMARY KEY,"
        SQL = SQL & "maxProductionLimit INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, maxProductionLimit FROM industryBlueprints"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO industryBlueprints VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_blueprintTypeID ON industryBlueprints (blueprintTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE industryActivities ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL,"
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "time INTEGER NOT NULL,"
        SQL = SQL & "PRIMARY KEY (blueprintTypeID, activityID)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, activityID, time FROM industryActivities"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO industryActivities VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_activityID ON industryActivities (activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        ' Build table
        SQL = "CREATE TABLE industryActivityMaterials ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL,"
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "materialTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER NOT NULL,"
        SQL = SQL & "consume INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM MY_INDUSTRY_MATERIALS "
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO industryActivityMaterials VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_BPIDactivityID1 ON industryActivityMaterials (blueprintTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        ' Build table
        SQL = "CREATE TABLE INDUSTRY_ACTIVITY_PRODUCTS ("
        SQL = SQL & "blueprintTypeID INTEGER NOT NULL,"
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "productTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER NOT NULL,"
        SQL = SQL & "probability FLOAT NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT blueprintTypeID, activityID, productTypeID, quantity, probability FROM industryActivityProducts"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INDUSTRY_ACTIVITY_PRODUCTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_IAP_BTID_AID ON INDUSTRY_ACTIVITY_PRODUCTS (blueprintTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE RAM_ACTIVITIES ("
        SQL = SQL & "activityID INTEGER NOT NULL,"
        SQL = SQL & "activityName VARCHAR(100),"
        SQL = SQL & "iconNo VARCHAR(5),"
        SQL = SQL & "description VARCHAR(1000),"
        SQL = SQL & "published INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("ramActivities")

        ' Pull new data and insert
        msSQL = "SELECT activityID, activityName, iconNo, description, published FROM ramActivities"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ACTIVITIES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetString(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetString(3)) & ","
            SQL = SQL & BuildInsertFieldString(CInt(msSQLReader.GetBoolean(4))) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_ACTIVITY_ID ON RAM_ACTIVITIES (activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' RAM_ASSEMBLY_LINE_STATIONS
    Private Sub Build_RAM_ASSEMBLY_LINE_STATIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_STATIONS ("
        SQL = SQL & "stationID INTEGER NOT NULL,"
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER,"
        SQL = SQL & "stationTypeID INTEGER, "
        SQL = SQL & "ownerID INTEGER,"
        SQL = SQL & "solarSystemID INTEGER,"
        SQL = SQL & "regionID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("ramAssemblyLineStations")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineStations"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_RALS_SID ON RAM_ASSEMBLY_LINE_STATIONS (stationID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_RALS_SSID ON RAM_ASSEMBLY_LINE_STATIONS (solarSystemID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_RALS_ALTID ON RAM_ASSEMBLY_LINE_STATIONS (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY ("
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "categoryID INTEGER NOT NULL,"
        SQL = SQL & "timeMultiplier FLOAT,"
        SQL = SQL & "materialMultiplier FLOAT, "
        SQL = SQL & "costMultiplier FLOAT"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("ramAssemblyLineTypeDetailPerCategory")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineTypeDetailPerCategory"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALC_ALTID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ALC_CID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_CATEGORY (categoryID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP ("
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "groupID INTEGER NOT NULL,"
        SQL = SQL & "timeMultiplier FLOAT,"
        SQL = SQL & "materialMultiplier FLOAT, "
        SQL = SQL & "costMultiplier FLOAT"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("ramAssemblyLineTypeDetailPerGroup")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineTypeDetailPerGroup"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALG_ALTID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ALG_GID ON RAM_ASSEMBLY_LINE_TYPE_DETAIL_PER_GROUP (groupID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' RAM_ASSEMBLY_LINE_TYPES
    Private Sub Build_RAM_ASSEMBLY_LINE_TYPES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("ramAssemblyLineTypes")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramAssemblyLineTypes"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_ALT_ALTID_AID ON RAM_ASSEMBLY_LINE_TYPES (assemblyLineTypeID, activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ALT_AID ON RAM_ASSEMBLY_LINE_TYPES (activityID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' RAM_INSTALLATION_TYPE_CONTENTS
    Private Sub Build_RAM_INSTALLATION_TYPE_CONTENTS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE RAM_INSTALLATION_TYPE_CONTENTS ("
        SQL = SQL & "installationTypeID INTEGER NOT NULL,"
        SQL = SQL & "assemblyLineTypeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("ramInstallationTypeContents")

        ' Pull new data and insert
        msSQL = "SELECT * FROM ramInstallationTypeContents"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO RAM_INSTALLATION_TYPE_CONTENTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Indexes
        SQL = "CREATE INDEX IDX_RITC_ITID_ALTID ON RAM_INSTALLATION_TYPE_CONTENTS (installationTypeID, assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_RITC_ALTID ON RAM_INSTALLATION_TYPE_CONTENTS (assemblyLineTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CS_CHARACTER_ID ON CHARACTER_STANDINGS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CS_NPC_TYPE_ID ON CHARACTER_STANDINGS (NPC_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Indexes
        SQL = "CREATE INDEX IDX_OBP_USER_ID ON OWNED_BLUEPRINTS (USER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' FACTIONS
    Private Sub Build_FACTIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE FACTIONS ("
        SQL = SQL & "factionID INTEGER PRIMARY KEY,"
        SQL = SQL & "factionName VARCHAR(" & GetLenSQLExpField("factionName", "chrFactions") & ") NOT NULL,"
        SQL = SQL & "raceID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("chrFactions")

        Application.DoEvents()

        ' Pull new data and insert
        msSQL = "SELECT factionID, factionName, raceIDs FROM chrFactions"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()
            SQL = "INSERT INTO FACTIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_F_FACTION_NAME ON FACTIONS (factionName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    ' INVENTORY_TRAITS
    Private Sub Build_INVENTORY_TRAITS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE INVENTORY_TRAITS ("
        SQL = SQL & "bonusID INTEGER,"
        SQL = SQL & "typeID INTEGER,"
        SQL = SQL & "skilltypeID INTEGER,"
        SQL = SQL & "bonus FLOAT,"
        SQL = SQL & "bonusText TEXT,"
        SQL = SQL & "importance INTEGER,"
        SQL = SQL & "nameID INTEGER,"
        SQL = SQL & "unitID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("invTraits")

        Application.DoEvents()

        ' Pull new data and insert
        msSQL = "SELECT * FROM invTraits"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()
            SQL = "INSERT INTO INVENTORY_TRAITS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_INVENTORY_TRAITS_BID ON INVENTORY_TRAITS (bonusID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_INVENTORY_TRAITS_TID ON INVENTORY_TRAITS (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE META_TYPES ("
        SQL = SQL & "typeID INTEGER PRIMARY KEY,"
        SQL = SQL & "parentTypeID INTEGER,"
        SQL = SQL & "metaGroupID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("invMetaTypes")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invMetaTypes"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO META_TYPES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE CONTROL_TOWER_RESOURCES ("
        SQL = SQL & "controlTowerTypeID INTEGER NOT NULL,"
        SQL = SQL & "resourceTypeID INTEGER NOT NULL,"
        SQL = SQL & "purpose INTEGER,"
        SQL = SQL & "quantity INTEGER,"
        SQL = SQL & "minSecurityLevel FLOAT,"
        SQL = SQL & "factionID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("invControlTowerResources")

        ' Pull new data and insert
        msSQL = "SELECT controlTowerTypeID, resourceTypeID, purpose, quantity, minSecurityLevel, factionID FROM invControlTowerResources"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_CT_TYPE_ID ON CONTROL_TOWER_RESOURCES (controlTowerTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_RESOURCE_TYPE_ID ON CONTROL_TOWER_RESOURCES (resourceTypeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' ATTRIBUTE_TYPES
    Private Sub Build_Attribute_Types()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE ATTRIBUTE_TYPES ("
        SQL = SQL & "attributeID INTEGER PRIMARY KEY,"
        SQL = SQL & "attributeName VARCHAR(" & GetLenSQLExpField("attributeName", "dgmAttributeTypes") & "),"
        SQL = SQL & "displayName VARCHAR(" & GetLenSQLExpField("displayName", "dgmAttributeTypes") & ")"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("dgmAttributeTypes")

        ' Pull new data and insert
        msSQL = "SELECT attributeID, attributeName, displayName FROM dgmAttributeTypes"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO ATTRIBUTE_TYPES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE TYPE_ATTRIBUTES ("
        SQL = SQL & "typeID INTEGER NOT NULL,"
        SQL = SQL & "attributeID INTEGER,"
        SQL = SQL & "valueInt INTEGER,"
        SQL = SQL & "valueFloat REAL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("dgmTypeAttributes")

        ' Pull new data and insert
        msSQL = "SELECT typeID, attributeID, valueInt, valueFloat FROM dgmTypeAttributes"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO TYPE_ATTRIBUTES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_TA_ATTRIBUTE_ID ON TYPE_ATTRIBUTES (attributeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_TA_TYPE_ID ON TYPE_ATTRIBUTES (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False

    End Sub

    ' OreRefine
    Private Sub BuildOreRefine()
        'Dim SQL As String = ""

        'Application.DoEvents()

        '' Build table
        'SQL = "CREATE TABLE ORE_REFINE ("
        'SQL = SQL & "OreID INTEGER PRIMARY KEY,"
        'SQL = SQL & "MineralID INTEGER,"
        'SQL = SQL & "MineralQuality INTEGER)"

        'Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        '' Add Data
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (18,35,213)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES  (18,34,107)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (18,36,107)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,34,56000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,35,12050)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,36,2100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (19,37,450)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (20,34,134)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (20,36,267)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (20,37,134)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,35,1000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,37,200)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,38,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (21,39,19)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (22,34,22000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (22,36,2500)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (22,40,320)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1223,35,12000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1223,39,450)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1223,40,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,34,351)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,35,25)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,36,50)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1224,38,5)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1225,34,21000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1225,38,760)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1225,39,135)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1226,36,350)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1226,38,75)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1226,39,8)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1227,34,800)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1227,35,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1227,37,85)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1228,34,346)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1228,35,173)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1229,35,2200)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1229,36,2400)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1229,37,300)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1230,34,415)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,34,2200)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,37,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,38,120)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1231,39,15)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1232,34,10000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1232,37,1600)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (1232,38,120)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (11396,11399,300)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28422,35,213)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28422,34,107)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28422,36,107)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,34,56000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,35,12050)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,36,2100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28420,37,450)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28410,34,134)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28410,36,267)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28410,37,134)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,35,1000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,37,200)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,38,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28401,39,19)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28367,34,22000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28367,36,2500)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28367,40,320)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28388,35,12000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28388,39,450)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28388,40,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,34,351)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,35,25)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,36,50)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28424,38,5)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28391,34,21000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28391,38,760)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28391,39,135)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28406,36,350)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28406,38,75)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28406,39,8)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28416,34,800)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28416,35,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28416,37,85)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28429,34,346)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28429,35,173)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28397,35,2200)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28397,36,2400)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28397,37,300)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28432,34,415)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,34,2200)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,37,100)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,38,120)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28403,39,15)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28394,34,10000)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28394,37,1600)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28394,38,120)", EVEIPHSQLiteDB.DBREf)
        'Call Execute_SQLiteSQL("INSERT INTO ORE_REFINE (OreID,MineralID,MineralQuantity) VALUES (28413,11399,300)", EVEIPHSQLiteDB.DBREf)

        Call Execute_SQLiteSQL(File.OpenText(WorkingDirectory & "\OreRefine.sql").ReadToEnd(), EVEIPHSQLiteDB.DBREf)

    End Sub

    ' ORES
    Private Sub Build_ORES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        Application.DoEvents()

        SQL = "CREATE TABLE ORES ("
        SQL = SQL & "ORE_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "ORE_NAME VARCHAR(50),"
        SQL = SQL & "ORE_VOLUME REAL,"
        SQL = SQL & "UNITS_TO_REFINE INTEGER,"
        SQL = SQL & "BELT_TYPE VARCHAR(3),"
        SQL = SQL & "HIGH_YIELD_ORE INTEGER)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Pull new data and insert
        msSQL = "SELECT invTypes.typeID, invTypes.typeName, invTypes.packagedVolume, invTypes.portionSize, "
        msSQL = msSQL & "CASE WHEN invTypes.groupID = 465 THEN 'Ice' WHEN invTypes.groupID = 711 THEN 'Gas' ELSE 'Ore' END, "
        msSQL = msSQL & "CASE WHEN invTypes.typeName IN ('Arkonor','Bistot','Crokite','Dark Ochre','Gneiss','Hedbergite',  "
        msSQL = msSQL & "'Hemorphite','Jaspet','Kernite','Mercoxit','Omber','Plagioclase','Pyroxeres','Scordite','Spodumain','Veldspar') THEN 0 "
        msSQL = msSQL & "WHEN invTypes.groupID = 465 THEN -1 WHEN invTypes.groupID = 711 THEN -2 ELSE 1 END "
        msSQL = msSQL & "FROM invTypes, invGroups "
        msSQL = msSQL & "WHERE invTypes.groupID = invGroups.groupID "
        msSQL = msSQL & "AND (invGroups.categoryID = 25 OR invGroups.groupID = 711) " ' Clouds and Ores
        msSQL = msSQL & "AND invTypes.marketGroupID <> 0 "
        msSQL = msSQL & "GROUP BY invTypes.typeID, invTypes.typeName, invTypes.packagedVolume, invTypes.portionSize, "
        msSQL = msSQL & "CASE WHEN invTypes.groupID = 465 THEN 'Ice' WHEN invTypes.groupID = 711 THEN 'Gas' ELSE 'Ore' END, "
        msSQL = msSQL & "CASE WHEN invTypes.typeName IN ('Arkonor','Bistot','Crokite','Dark Ochre','Gneiss','Hedbergite', "
        msSQL = msSQL & "'Hemorphite','Jaspet','Kernite','Mercoxit','Omber','Plagioclase','Pyroxeres','Scordite','Spodumain','Veldspar') THEN 0 "
        msSQL = msSQL & "WHEN invTypes.groupID = 465 THEN -1 WHEN invTypes.groupID = 711 THEN -2 ELSE 1 END "

        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLQuery.CommandTimeout = 300
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Now set the 5%/10% flag
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Crimson Arkonor'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Triclinic Bistot'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Sharp Crokite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Onyx Ochre'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Vitric Hedbergite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Vivid Hemorphite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Pure Jaspet'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Luminous Kernite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Azure Plagioclase'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Solid Pyroxeres'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Condensed Scordite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Bright Spodumain'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Concentrated Veldspar'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Iridescent Gneiss'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Magma Mercoxit'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 1 WHERE ORE_NAME LIKE '%Silvery Omber'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Prime Arkonor'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Monoclinic Bistot'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Crystalline Crokite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Obsidian Ochre'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Glazed Hedbergite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Radiant Hemorphite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Pristine Jaspet'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Fiery Kernite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Rich Plagioclase'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Viscous Pyroxeres'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Massive Scordite'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Gleaming Spodumain'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Dense Veldspar'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Prismatic Gneiss'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Vitreous Mercoxit'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        SQL = "UPDATE ORES SET HIGH_YIELD_ORE = 2 WHERE ORE_NAME LIKE '%Golden Omber'"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ORES_ORE_ID ON ORES (ORE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        ' Now open the saved table and insert all the values into this new table

        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (18,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (19,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (20,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (21,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (22,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1223,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1224,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1225,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1226,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1227,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1228,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1229,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'High Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1230,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Low Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Low Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1231,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C1','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C2','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (1232,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Amarr',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Caldari',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Minmatar',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'Null Sec','Gallente',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C5','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C6','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C3','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (11396,'C4','WH',0)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'High Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Low Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16262,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'High Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16263,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'High Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'Low Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'High Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16264,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'High Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16265,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16266,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Low Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16267,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16268,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (16269,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17425,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17426,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17428,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17429,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17432,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17433,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17436,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17437,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17440,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17441,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17444,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17445,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17448,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17449,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17452,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17453,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17455,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17456,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17459,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17460,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17463,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17464,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17466,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17467,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17470,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17471,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17865,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17866,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17867,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'High Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Low Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'High Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Low Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17868,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17869,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Amarr',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Caldari',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Minmatar',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17870,'Null Sec','Gallente',1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17975,'Null Sec','Gallente',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17976,'Null Sec','Caldari',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17977,'Null Sec','Minmatar',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (17978,'Null Sec','Amarr',-1)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25268,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25273,'Null Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25274,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25274,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Null Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25275,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25276,'Low Sec','Amarr',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25276,'Null Sec','Amarr',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25277,'Null Sec','Amarr',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25277,'Low Sec','Amarr',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Null Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Null Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25278,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (25279,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28694,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28694,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28695,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28695,'Low Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28696,'High Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28696,'High Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28697,'Low Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28697,'High Sec','Caldari',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28698,'Low Sec','Amarr',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28699,'High Sec','Amarr',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28700,'High Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28700,'High Sec','Minmatar',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28701,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (28701,'Low Sec','Gallente',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C1','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C2','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30370,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C1','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C2','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30371,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C1','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C2','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30372,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C1','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C2','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30373,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C1','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C2','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30374,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30375,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C3','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C4','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30376,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30377,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30377,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30378,'C5','WH',-2)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO ORE_LOCATIONS VALUES (30378,'C6','WH',-2)", EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ORE_LOCS_ORE_ID ON ORE_LOCATIONS (ORE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        msSQL = "SELECT invTypes.typeID, invTypes.typeName, invTypes.packagedVolume, "
        msSQL = msSQL & "invTypes.portionSize, invTypes_1.typeID, invTypes_1.typeName, "
        msSQL = msSQL & "invGroups_1.groupName, invTypes_1.packagedVolume, "
        msSQL = msSQL & "invTypeMaterials.quantity "
        msSQL2 = "FROM ((((invTypes INNER JOIN invTypeMaterials ON invTypes.typeID = invTypeMaterials.typeID) "
        msSQL2 = msSQL2 & "INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) INNER JOIN invCategories ON invGroups.categoryID = invCategories.categoryID) "
        msSQL2 = msSQL2 & "INNER JOIN invTypes AS invTypes_1 ON invTypeMaterials.materialTypeID = invTypes_1.typeID) INNER JOIN (invGroups AS invGroups_1 "
        msSQL2 = msSQL2 & "INNER JOIN invCategories AS invCategories_1 ON invGroups_1.categoryID = invCategories_1.categoryID) ON invTypes_1.groupID = invGroups_1.groupID "

        ' Get the count
        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, DBRef1)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_REPRO_ITEM_MAT_ID ON REPROCESSING (ITEM_ID, REFINED_MATERIAL_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Pull new data and insert
        msSQL = "SELECT invTypeReactions.reactionTypeID, "
        msSQL = msSQL & "invTypes.typeName, "
        msSQL = msSQL & "invGroups.groupName, "
        msSQL = msSQL & "CASE WHEN invTypeReactions.input = 0 THEN 'Output' ELSE 'Input' END, "
        msSQL = msSQL & "invTypeReactions.typeID, invTypes_1.typeName, "
        msSQL = msSQL & "invGroups_1.groupName, "
        msSQL = msSQL & "invCategories.categoryName, "
        msSQL = msSQL & "CASE WHEN COALESCE(valueInt, valueFloat) IS NULL THEN invTypeReactions.quantity ELSE COALESCE(valueInt, valueFloat) * invTypeReactions.quantity END AS Quantity, "
        msSQL = msSQL & "invTypes_1.packagedVolume "

        msSQL2 = "FROM invTypeReactions LEFT JOIN dgmTypeAttributes ON invTypeReactions.typeID = dgmTypeAttributes.typeID AND dgmTypeAttributes.attributeID =726,"
        msSQL2 = msSQL2 & "invTypes, invTypes AS invTypes_1, invGroups, invGroups AS invGroups_1, invCategories "
        msSQL2 = msSQL2 & "WHERE invTypeReactions.reactionTypeID = invTypes.typeID "
        msSQL2 = msSQL2 & "AND invTypeReactions.typeID = invTypes_1.typeID "
        msSQL2 = msSQL2 & "AND invTypes.groupID = invGroups.groupID "
        msSQL2 = msSQL2 & "AND invTypes_1.groupID = invGroups_1.groupID "
        msSQL2 = msSQL2 & "AND invGroups_1.categoryID = invCategories.categoryID "
        msSQL2 = msSQL2 & "AND invTypes.published <> 0 AND invTypes_1.published <> 0 AND invCategories.published <> 0 AND invGroups.published <> 0 AND invGroups_1.published <> 0 "

        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, DBRef1)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_REACTION_MAT_TYPE_ID ON REACTIONS (MATERIAL_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_REACTION_MAT_GROUP ON REACTIONS (MATERIAL_GROUP)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CRA_AGENTID ON CURRENT_RESEARCH_AGENTS (AGENT_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CRA_CHARID ON CURRENT_RESEARCH_AGENTS (CHARACTER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        Dim i As Integer

        SQL = "CREATE TABLE SKILLS ("
        SQL = SQL & "SKILL_TYPE_ID INTEGER PRIMARY KEY,"
        SQL = SQL & "SKILL_NAME VARCHAR(100),"
        SQL = SQL & "SKILL_GROUP VARCHAR(100)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Pull new data and insert
        msSQL = "SELECT invTypes.typeID, invTypes.typeName, invGroups.groupName "
        msSQL2 = "FROM (invTypes INNER JOIN invGroups ON invTypes.groupID = invGroups.groupID) INNER JOIN invCategories ON invGroups.categoryID = invCategories.categoryID "
        msSQL2 = msSQL2 & "WHERE invCategories.categoryName='Skill' AND invTypes.published<>0 AND invGroups.published<>0 AND invCategories.published<>0"

        ' Get the count
        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, DBRef1)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO SKILLS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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
        msSQLQuery = New SqlCommand("SELECT COUNT(*) " & msSQL2, DBRef1)
        mySQLReader2 = msSQLQuery.ExecuteReader()
        mySQLReader2.Read()
        pgMain.Maximum = mySQLReader2.GetValue(0)
        pgMain.Value = 0
        i = 0
        pgMain.Visible = True
        mySQLReader2.Close()

        msSQLQuery = New SqlCommand(msSQL & msSQL2, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_RA_TYPE_CORP_ID ON RESEARCH_AGENTS (RESEARCH_TYPE, CORPORATION_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_RA_REGION_ID ON RESEARCH_AGENTS (REGION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC ON ASSETS (LocationID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ITEM_TYPEID_ID ON ASSETS (TypeID, ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC_TYPE_ACCID ON ASSET_LOCATIONS (EnumAssetType, ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ITEM_ASSET_LOC_ACCOUNT_ID ON ASSET_LOCATIONS (ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' REGIONS
    Private Sub Build_REGIONS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE REGIONS ("
        SQL = SQL & "regionID INTEGER PRIMARY KEY,"
        SQL = SQL & "regionName VARCHAR(20) NOT NULL,"
        SQL = SQL & "factionID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf) ' SQLite table

        Application.DoEvents()

        msSQL = "SELECT regionID, regionName, factionID FROM mapRegions ORDER BY regionName"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            If msSQLReader.GetValue(0) = 11000001 Then
                Application.DoEvents()
            End If
            SQL = "INSERT INTO REGIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)
        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_R_REGION_NAME ON REGIONS (regionName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_R_REGION_ID ON REGIONS (regionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_R_FID ON REGIONS (factionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE CONSTELLATIONS ("
        SQL = SQL & "regionID INTEGER NOT NULL,"
        SQL = SQL & "constellationID INTEGER PRIMARY KEY,"
        SQL = SQL & "constellationName VARCHAR(20) NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Pull new data and insert
        msSQL = "SELECT regionID, constellationID, constellationName FROM mapConstellations"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO CONSTELLATIONS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()


        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        SQL = "CREATE INDEX IDX_C_REGION_ID ON CONSTELLATIONS (regionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' SOLAR_SYSTEMS
    Private Sub Build_SOLAR_SYSTEMS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE SOLAR_SYSTEMS ("
        SQL = SQL & "regionID INTEGER NOT NULL,"
        SQL = SQL & "constellationID INTEGER NOT NULL,"
        SQL = SQL & "solarSystemID INTEGER PRIMARY KEY,"
        SQL = SQL & "solarSystemName VARCHAR(17) NOT NULL,"
        SQL = SQL & "security REAL NOT NULL,"
        SQL = SQL & "factionWarzone INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Pull new data and insert
        msSQL = "SELECT regionID, constellationID, solarSystemID, solarSystemName, security, "
        msSQL = msSQL & "CASE WHEN solarSystemID in (SELECT solarSystemID FROM warCombatZoneSystems) THEN 1 ELSE 0 END as fwsystem "
        msSQL = msSQL & "FROM mapSolarSystems "
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO SOLAR_SYSTEMS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SS_REGION_ID ON SOLAR_SYSTEMS (regionID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SS_SS_ID ON SOLAR_SYSTEMS (solarSystemID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SS_CONSTELLATION_ID ON SOLAR_SYSTEMS (constellationID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SS_SYSTEM_NAME ON SOLAR_SYSTEMS (solarSystemName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE INVENTORY_TYPES ("
        SQL = SQL & "typeID INTEGER PRIMARY KEY,"
        SQL = SQL & "groupID INTEGER,"
        SQL = SQL & "typeName VARCHAR(" & GetLenSQLExpField("typeName", "invTypes") & "),"
        SQL = SQL & "description VARCHAR(" & GetLenSQLExpField("description", "invTypes") & "),"
        SQL = SQL & "mass REAL,"
        SQL = SQL & "volume REAL,"
        SQL = SQL & "packagedVolume REAL,"
        SQL = SQL & "capacity REAL,"
        SQL = SQL & "portionSize INTEGER,"
        SQL = SQL & "factionID INTEGER,"
        SQL = SQL & "raceID INTEGER,"
        SQL = SQL & "basePrice REAL,"
        SQL = SQL & "published INTEGER,"
        SQL = SQL & "marketGroupID INTEGER,"
        SQL = SQL & "graphicID INTEGER,"
        SQL = SQL & "radius REAL,"
        SQL = SQL & "iconID INTEGER,"
        SQL = SQL & "soundID INTEGER,"
        SQL = SQL & "sofFactionName INTEGER,"
        SQL = SQL & "sofMaterialSetID INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("invTypes")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invTypes "
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & "," ' Packaged Volume
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & "," ' Capacity
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & "," ' PortionSize
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(9)) & "," ' FactionID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(10)) & "," ' RaceID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(11)) & "," ' BasePrice
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(12)) & "," ' published
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(13)) & "," ' marketGroupID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(14)) & "," ' graphicID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(15)) & "," ' radius
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(16)) & "," ' iconID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(17)) & "," ' soundID
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(18)) & "," ' sofFactionName
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(19)) & ")" ' sofMaterialSetID

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_IT_GROUP_ID ON INVENTORY_TYPES (groupID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IT_TYPE_NAME_ID ON INVENTORY_TYPES (typeName)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IT_TYPE_ID ON INVENTORY_TYPES (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False

    End Sub

    ' INVENTORY_GROUPS
    Private Sub Build_INVENTORY_GROUPS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("invGroups")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invGroups"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_IG_GROUP_ID ON INVENTORY_GROUPS (groupID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IG_CATEGORY_ID ON INVENTORY_GROUPS (categoryID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False

    End Sub

    ' INVENTORY_CATEGORIES
    Public Sub Build_INVENTORY_CATEGORIES()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE INVENTORY_CATEGORIES ("
        SQL = SQL & "categoryID INTEGER PRIMARY KEY,"
        SQL = SQL & "categoryName VARCHAR(" & GetLenSQLExpField("categoryName", "invCategories") & "),"
        SQL = SQL & "published INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("invCategories")

        ' Pull new data and insert
        msSQL = "SELECT categoryID, categoryName, published FROM invCategories"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        ' Add to Access table
        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO INVENTORY_CATEGORIES VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(CInt(msSQLReader.GetValue(2))) & ")" ' A bit value, but reads as a boolean for some reason

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        SQL = "CREATE INDEX IDX_IC_CATEGORY_ID ON INVENTORY_GROUPS (categoryID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE INVENTORY_FLAGS ("
        SQL = SQL & "FlagID INTEGER NOT NULL,"
        SQL = SQL & "FlagName VARCHAR(200) NOT NULL,"
        SQL = SQL & "FlagText VARCHAR(100) NOT NULL,"
        SQL = SQL & "OrderID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call SetProgressBarValues("invFlags")

        ' Pull new data and insert
        msSQL = "SELECT * FROM invFlags"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        ' Add a final flag for space
        SQL = "INSERT INTO INVENTORY_FLAGS VALUES (" & CStr(SpaceFlagCode) & ",'Space','Space',0)"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        SQL = "CREATE INDEX IDX_ITEM_FLAG_ID ON INVENTORY_FLAGS (FlagID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        Application.DoEvents()

        ' See if the view exists and drop if it does
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'PRICES_BUILD'"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
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
        msSQLQuery = New SqlCommand(SQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_NOBUILD"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        msSQL = "CREATE VIEW PRICES_NOBUILD AS SELECT * FROM ("
        ' Get all the materials used to build stuff
        msSQL = msSQL & "SELECT DISTINCT MATERIAL_ID, MATERIAL, 0 AS TECH_LEVEL, 0 AS PRICE, MATERIAL_CATEGORY, MATERIAL_GROUP, 0 AS MANUFACTURE, "
        msSQL = msSQL & "0 AS ITEM_TYPE, 'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM ALL_BLUEPRINT_MATERIALS "
        msSQL = msSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM ALL_BLUEPRINTS) "
        msSQL = msSQL & "AND MATERIAL_CATEGORY <> 'Skill' "
        msSQL = msSQL & "UNION "
        ' Get specific materials for later use or other areas in IPH (ie asteroids) - include items for LP Store
        msSQL = msSQL & "SELECT DISTINCT typeID AS MATERIAL_ID, typeName AS MATERIAL, 0 AS TECH_LEVEL, 0 AS PRICE, categoryName AS MATERIAL_CATEGORY, "
        msSQL = msSQL & "groupName AS MATERIAL_GROUP, 0 AS MANUFACTURE, 0 AS ITEM_TYPE, 'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM invTypes, invGroups, invCategories "
        msSQL = msSQL & "WHERE invTypes.groupID = invGroups.groupID "
        msSQL = msSQL & "AND invGroups.categoryID = invCategories.categoryID "
        msSQL = msSQL & "AND invTypes.published <> 0 AND invGroups.published <> 0 AND invCategories.published <> 0 "
        msSQL = msSQL & "AND invTypes.marketGroupID IS NOT NULL "
        msSQL = msSQL & "AND (categoryName IN ('Asteroid','Decryptors','Planetary Commodities','Planetary Resources') "
        msSQL = msSQL & "OR groupName in ('Moon Materials','Ice Product','Harvestable Cloud','Intermediate Materials') "
        ' The last IDs are random items that come out of booster production or needed in station building (Quafe), or for the LP Store
        msSQL = msSQL & "OR typeID in (41, 3699, 3773, 9850, 33195))) AS X " ' Garbage, Quafe, Hydrochloric Acid, Spirits, Spatial Attunment
        msSQL = msSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD)"
        Execute_msSQL(msSQL)

        ' See if the view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'PRICES_LP_OFFERS'"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_LP_OFFERS"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Build a view for items that are LP offers and requirements for those items
        msSQL = "CREATE VIEW PRICES_LP_OFFERS AS SELECT * FROM ("
        msSQL = msSQL & "SELECT DISTINCT lpOfferRequirements.typeID AS MATERIAL_ID, typeName AS MATERIAL, "
        ' If we can build this group, then mark the item as a tech 1 (lp stores don't have tech 2 items) and manufacture as 1 too
        msSQL = msSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS TECH_LEVEL, "
        msSQL = msSQL & "0 AS PRICE, categoryName AS MATERIAL_CATEGORY, groupName AS MATERIAL_GROUP, "
        msSQL = msSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS MANUFACTURE, "
        msSQL = msSQL & "CASE WHEN (typeName LIKE '%Navy%' OR typeName LIKE '%Fleet%' OR typeName LIKE '%Sisters%' OR typeName LIKE '%ORE%' OR typeName LIKE '%Thukker%') THEN 15 ELSE 16 END as ITEM_TYPE, 'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM invTypes, invGroups, invCategories, lpOfferRequirements "
        msSQL = msSQL & "WHERE lpOfferRequirements.typeID = invTypes.typeID "
        msSQL = msSQL & "AND invTypes.groupID = invGroups.groupID "
        msSQL = msSQL & "AND invGroups.categoryID = invCategories.categoryID "
        msSQL = msSQL & "UNION "
        msSQL = msSQL & "SELECT DISTINCT lpOffers.typeID AS MATERIAL_ID, typeName AS MATERIAL, "
        msSQL = msSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS TECH_LEVEL, "
        msSQL = msSQL & "0 AS PRICE, categoryName AS MATERIAL_CATEGORY, groupName AS MATERIAL_GROUP, "
        msSQL = msSQL & "CASE WHEN groupName IN (SELECT ITEM_GROUP FROM ALL_BLUEPRINTS GROUP BY ITEM_GROUP) AND groupName NOT IN ('Cyberimplant','Miscellaneous') THEN 1 ELSE 0 END AS MANUFACTURE, "
        msSQL = msSQL & "CASE WHEN (typeName LIKE '%Navy%' OR typeName LIKE '%Fleet%' OR typeName LIKE '%Sisters%' OR typeName LIKE '%ORE%' OR typeName LIKE '%Thukker%') THEN 15 ELSE 16 END as ITEM_TYPE, 'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM invTypes, invGroups, invCategories, lpOffers "
        msSQL = msSQL & "WHERE lpOffers.typeID = invTypes.typeID "
        msSQL = msSQL & "AND invTypes.groupID = invGroups.groupID "
        msSQL = msSQL & "AND invGroups.categoryID = invCategories.categoryID) AS X "
        msSQL = msSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD) AND MATERIAL_ID NOT IN (SELECT MATERIAL_ID FROM PRICES_NOBUILD)"
        Execute_msSQL(msSQL)

        ' See if the view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'PRICES_LP_BP_ITEMS'"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW PRICES_LP_BP_ITEMS"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        ' Finally make sure all the items that we can build from BP's are included
        msSQL = "CREATE VIEW PRICES_LP_BP_ITEMS AS SELECT * FROM ("
        msSQL = msSQL & "SELECT DISTINCT industryActivityProducts.productTypeID AS MATERIAL_ID, typeName AS MATERIAL, 1 AS TECH_LEVEL, 0 AS PRICE, "
        msSQL = msSQL & "categoryName AS MATERIAL_CATEGORY, groupName AS MATERIAL_GROUP, 1 AS MANUFACTURE, 0 AS ITEM_TYPE, 'None' AS PRICE_TYPE "
        msSQL = msSQL & "FROM lpOffers, industryActivityProducts, invTypes, invGroups, invCategories "
        msSQL = msSQL & "WHERE lpOffers.typeID = industryActivityProducts.blueprintTypeID "
        msSQL = msSQL & "AND industryActivityProducts.productTypeID = invTypes.typeID "
        msSQL = msSQL & "AND invTypes.groupID = invGroups.groupID "
        msSQL = msSQL & "AND invGroups.categoryID = invCategories.categoryID "
        msSQL = msSQL & "AND activityID = 1) AS X "
        msSQL = msSQL & "WHERE MATERIAL_ID NOT IN (SELECT ITEM_ID FROM PRICES_BUILD) AND MATERIAL_ID NOT IN (SELECT MATERIAL_ID FROM PRICES_NOBUILD) "
        msSQL = msSQL & "AND MATERIAL_ID NOT IN (SELECT MATERIAL_ID FROM PRICES_LP_OFFERS)"
        Execute_msSQL(msSQL)

        ' See if the union view exists and delete if so
        SQL = "SELECT COUNT(*) FROM sys.all_views where name = 'ITEM_PRICES_UNION'"
        msSQLQuery = New SqlCommand(SQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()
        msSQLReader.Read()

        If CInt(msSQLReader.GetValue(0)) = 1 Then
            SQL = "DROP VIEW ITEM_PRICES_UNION"
            msSQLReader.Close()
            Execute_msSQL(SQL)
        Else
            msSQLReader.Close()
        End If

        SQL = "CREATE VIEW ITEM_PRICES_UNION AS SELECT * FROM PRICES_BUILD UNION SELECT * FROM PRICES_NOBUILD "
        SQL = SQL & "UNION SELECT * FROM PRICES_LP_OFFERS UNION SELECT * FROM PRICES_LP_BP_ITEMS"
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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data
        Call SetProgressBarValues("ITEM_PRICES_UNION")

        ' Now select the final query of data
        msSQL = "SELECT ITEM_ID, ITEM_NAME, TECH_LEVEL, PRICE, ITEM_CATEGORY, ITEM_GROUP, MANUFACTURE, ITEM_TYPE, PRICE_TYPE FROM ITEM_PRICES_UNION "
        msSQL = msSQL & "GROUP BY ITEM_ID, ITEM_NAME, TECH_LEVEL, PRICE, ITEM_CATEGORY, ITEM_GROUP, MANUFACTURE, ITEM_TYPE, PRICE_TYPE"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

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

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

            ' For each record, update the progress bar
            Call IncrementProgressBar(pgMain)
            Application.DoEvents()

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()

        ' Update the item types fields to make sure they are all able to be found
        SQL = "UPDATE ITEM_PRICES SET ITEM_TYPE = 1 WHERE ITEM_TYPE = 0 AND TECH_LEVEL = 1"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Build SQL Lite indexes
        SQL = "CREATE INDEX IDX_IP_GROUP ON ITEM_PRICES (ITEM_GROUP)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IP_TYPE ON ITEM_PRICES (ITEM_TYPE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IP_CATEGORY ON ITEM_PRICES (ITEM_CATEGORY)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IPC_TYPEID ON ITEM_PRICES_CACHE (typeID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IPC_ID_REGION ON ITEM_PRICES_CACHE (typeID, RegionList)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' EMD_ITEM_PRICE_HISTORY
    Private Sub Build_EMD_Item_Price_History()
        Dim SQL As String

        SQL = "CREATE TABLE EMD_ITEM_PRICE_HISTORY ("
        SQL = SQL & "TYPE_ID INTEGER,"
        SQL = SQL & "REGION_ID INTEGER,"
        SQL = SQL & "PRICE_HISTORY_DATE VARCHAR(23)," ' Date
        SQL = SQL & "LOW_PRICE FLOAT,"
        SQL = SQL & "HIGH_PRICE FLOAT,"
        SQL = SQL & "AVG_PRICE FLOAT,"
        SQL = SQL & "TOTAL_ORDERS_FILLED INTEGER,"
        SQL = SQL & "TOTAL_VOLUME_FILLED INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE UNIQUE INDEX IDX_EMD_HISTORY ON EMD_ITEM_PRICE_HISTORY (TYPE_ID, REGION_ID, PRICE_HISTORY_DATE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' EMD_UPDATE_HISTORY
    Private Sub Build_EMD_Update_History()
        Dim SQL As String

        SQL = "CREATE TABLE EMD_UPDATE_HISTORY ("
        SQL = SQL & "TYPE_ID INTEGER,"
        SQL = SQL & "DAYS INTEGER,"
        SQL = SQL & "REGION_ID INTEGER,"
        SQL = SQL & "UPDATE_LAST_RAN VARCHAR(23)" ' Date
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE UNIQUE INDEX IDX_EMD_U_HISTORY ON EMD_UPDATE_HISTORY (TYPE_ID, DAYS, REGION_ID, UPDATE_LAST_RAN)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' MARKET_HISTORY
    Private Sub Build_MARKET_HISTORY()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_HISTORY ("
        SQL = SQL & "TYPE_ID INTEGER,"
        SQL = SQL & "REGION_ID INTEGER,"
        SQL = SQL & "PRICE_HISTORY_DATE VARCHAR(23)," ' Date
        SQL = SQL & "LOW_PRICE FLOAT,"
        SQL = SQL & "HIGH_PRICE FLOAT,"
        SQL = SQL & "AVG_PRICE FLOAT,"
        SQL = SQL & "TOTAL_ORDERS_FILLED INTEGER,"
        SQL = SQL & "TOTAL_VOLUME_FILLED INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE UNIQUE INDEX IDX_MH_TID_RID ON MARKET_HISTORY (TYPE_ID, REGION_ID, PRICE_HISTORY_DATE)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' MARKET_HISTORY_UPDATE_CACHE
    Private Sub Build_MARKET_HISTORY_UPDATE_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_HISTORY_UPDATE_CACHE ("
        SQL = SQL & "TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL,"
        SQL = SQL & "CACHE_DATE VARCHAR(23)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE UNIQUE INDEX IDX_MHUC_TID_RID ON MARKET_HISTORY_UPDATE_CACHE (TYPE_ID, REGION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' MARKET_ORDERS
    Private Sub Build_MARKET_ORDERS()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_ORDERS ("
        SQL = SQL & "TYPE_ID INTEGER,"
        SQL = SQL & "REGION_ID INTEGER,"
        SQL = SQL & "SOLAR_SYSTEM_ID INTEGER,"
        SQL = SQL & "ORDER_ISSUED VARCHAR(23)," ' Date
        SQL = SQL & "DURATION INTEGER,"
        SQL = SQL & "ORDER_TYPE VARCHAR(4)," ' buy or sell
        SQL = SQL & "PRICE FLOAT,"
        SQL = SQL & "VOLUME_ENTERED INTEGER,"
        SQL = SQL & "MINIMUM_VOLUME INTEGER,"
        SQL = SQL & "VOLUME_REMAINING INTEGER"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_MO_TID_RID_SID ON MARKET_ORDERS (TYPE_ID, REGION_ID, SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    End Sub

    ' MARKET_ORDERS_UPDATE_CACHE
    Private Sub Build_MARKET_ORDERS_UPDATE_CACHE()
        Dim SQL As String

        SQL = "CREATE TABLE MARKET_ORDERS_UPDATE_CACHE ("
        SQL = SQL & "TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "REGION_ID INTEGER NOT NULL,"
        SQL = SQL & "CACHE_DATE VARCHAR(23)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE UNIQUE INDEX IDX_MOUC_TID_RID ON MARKET_ORDERS_UPDATE_CACHE (TYPE_ID, REGION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_SGID_GID ON INDUSTRY_GROUP_SPECIALTIES (SPECIALTY_GROUP_ID, GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE TABLE INDUSTRY_CATEGORY_SPECIALTIES ("
        SQL = SQL & "SPECIALTY_CATEGORY_ID INTEGER NOT NULL,"
        SQL = SQL & "SPECIALTY_CATEGORY_NAME VARCHAR(100) NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_CAT_ID ON INDUSTRY_CATEGORY_SPECIALTIES (SPECIALTY_CATEGORY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_TEAMS_TEAM_ID ON INDUSTRY_TEAMS (TEAM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_TEAMS_ACTIVITY_ID ON INDUSTRY_TEAMS (TEAM_ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_TEAMS_TEAM_NAME ON INDUSTRY_TEAMS (TEAM_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE TABLE INDUSTRY_TEAMS_BONUSES ("
        SQL = SQL & "TEAM_ID INTEGER NOT NULL,"
        SQL = SQL & "TEAM_NAME VARCHAR(100) NOT NULL,"
        SQL = SQL & "BONUS_ID INTEGER NOT NULL,"
        SQL = SQL & "BONUS_TYPE STRING NOT NULL,"
        SQL = SQL & "BONUS_VALUE FLOAT NOT NULL,"
        SQL = SQL & "SPECIALTY_GROUP_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_BONUSES_ID_GID ON INDUSTRY_TEAMS_BONUSES (TEAM_ID, SPECIALTY_GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_BONUSES_NAME_GID ON INDUSTRY_TEAMS_BONUSES (TEAM_NAME, SPECIALTY_GROUP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AUCTIONS_TEAM_ID ON INDUSTRY_TEAMS_AUCTIONS (TEAM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AUCTIONS_CTIVITY_ID ON INDUSTRY_TEAMS_AUCTIONS (TEAM_ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AUCTIONS_TEAM_NAME ON INDUSTRY_TEAMS_AUCTIONS (TEAM_NAME)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_AUCTIONS_AUCTION_ID ON INDUSTRY_TEAMS_AUCTIONS (AUCTION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_BIDS_AUCTION_ID ON INDUSTRY_TEAMS_AUCTIONS_BIDS (AUCTION_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_ISCI_SSID_AID ON INDUSTRY_SYSTEMS_COST_INDICIES (SOLAR_SYSTEM_ID, ACTIVITY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IF_MAIN ON INDUSTRY_FACILITIES (FACILITY_TYPE_ID, REGION_ID, SOLAR_SYSTEM_ID, FACILITY_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IF_SSID ON INDUSTRY_FACILITIES (SOLAR_SYSTEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_IF_FTID ON INDUSTRY_FACILITIES (FACILITY_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2073)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2073)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2073)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2073)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2267)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2267)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2267)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2267)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2267)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2268)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2268)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2268)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2268)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2268)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2268)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2270)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2270)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2272)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2272)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2272)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2286)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2286)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2287)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2287)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2288)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2288)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2014,2288)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (11,2305)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2306)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2306)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2307)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2015,2308)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2308)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2063,2308)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2309)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2309)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2016,2310)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2310)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (2017,2310)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (12,2310)", EVEIPHSQLiteDB.DBREf)
        Execute_SQLiteSQL("INSERT INTO PLANET_RESOURCES VALUES (13,2311)", EVEIPHSQLiteDB.DBREf)

        ' Indexes
        SQL = "CREATE INDEX IDX_PR_PTID_RTID ON PLANET_RESOURCES (PLANET_TYPE_ID, RESOURCE_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_PR_PTID ON PLANET_RESOURCES (PLANET_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE PLANET_SCHEMATICS ("
        SQL = SQL & "schematicID INTEGER PRIMARY KEY,"
        SQL = SQL & "schematicName VARCHAR(50) NOT NULL,"
        SQL = SQL & "cycleTime INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM planetSchematics"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID ON PLANET_SCHEMATICS (schematicID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE PLANET_SCHEMATICS_TYPE_MAP ("
        SQL = SQL & "schematicID INTEGER NOT NULL,"
        SQL = SQL & "typeID INTEGER NOT NULL,"
        SQL = SQL & "quantity INTEGER NOT NULL,"
        SQL = SQL & "isInput INTEGER NOT NULL,"
        SQL = SQL & "PRIMARY KEY (schematicID, typeID)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM planetSchematicsTypeMap"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS_TYPE_MAP VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID_TMAP ON PLANET_SCHEMATICS_TYPE_MAP (schematicID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

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

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE PLANET_SCHEMATICS_PIN_MAP ("
        SQL = SQL & "schematicID INTEGER NOT NULL,"
        SQL = SQL & "pintypeID INTEGER NOT NULL,"
        SQL = SQL & "PRIMARY KEY (schematicID, pintypeID)"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM planetSchematicsPinMap"
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO PLANET_SCHEMATICS_PIN_MAP VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table

        SQL = "CREATE INDEX IDX_SCHEMATIC_ID_PIN_MAP ON PLANET_SCHEMATICS_PIN_MAP (schematicID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

#End Region

#Region "LP Store Tables"

    ' LP_OFFER_REQUIREMENTS
    Private Sub Build_LP_OFFER_REQUIREMENTS()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE LP_OFFER_REQUIREMENTS ("
        SQL = SQL & "OFFER_ID INTEGER NOT NULL,"
        SQL = SQL & "REQ_TYPE_ID INTEGER NOT NULL,"
        SQL = SQL & "REQ_QUANTITY INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT * FROM lpOfferRequirements WHERE offerID <> 1201" '1201 seems to be extra
        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO LP_OFFER_REQUIREMENTS VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table
        SQL = "CREATE INDEX IDX_LO_OID_RTID ON LP_OFFER_REQUIREMENTS (OFFER_ID, REQ_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_LO_RTID ON LP_OFFER_REQUIREMENTS (REQ_TYPE_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    '' LP_OFFERS
    'Private Sub Build_LP_OFFERS()
    '    Dim SQL As String

    '    ' MS SQL variables
    '    Dim msSQLQuery As New SqlCommand
    '    Dim msSQLReader As SqlDataReader
    '    Dim msSQL As String

    '    SQL = "CREATE TABLE LP_OFFERS ("
    '    SQL = SQL & "offerID INTEGER PRIMARY KEY,"
    '    SQL = SQL & "typeID INTEGER NOT NULL,"
    '    SQL = SQL & "quantity INTEGER NOT NULL,"
    '    SQL = SQL & "lpCost FLOAT NOT NULL,"
    '    SQL = SQL & "iskCost FLOAT NOT NULL"
    '    SQL = SQL & ")"

    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    ' Now select the count of the final query of data

    '    ' Pull new data and insert
    '    msSQL = "SELECT * FROM lpOffers"
    '    msSQLQuery = New SqlCommand(msSQL, DBRef1)
    '    msSQLReader = msSQLQuery.ExecuteReader()

    '    Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

    '    While msSQLReader.Read
    '        Application.DoEvents()

    '        SQL = "INSERT INTO LP_OFFERS VALUES ("
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ")"

    '        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    End While

    '    Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    '    msSQLReader.Close()
    '    msSQLReader = Nothing
    '    msSQLQuery = Nothing

    '    ' Now index and PK the table
    '    SQL = "CREATE INDEX IDX_LO_OID ON LP_OFFERS (offerID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    pgMain.Visible = False
    '    Application.DoEvents()

    'End Sub

    ' LP_STORE
    Private Sub Build_LP_STORE()
        Dim SQL As String

        ' MS SQL variables
        Dim msSQLQuery As New SqlCommand
        Dim msSQLReader As SqlDataReader
        Dim msSQL As String

        Dim DBRef1 As New SqlConnection
        DBRef1 = DBConnectionRef()

        SQL = "CREATE TABLE LP_STORE ("
        SQL = SQL & "OFFER_ID INTEGER NOT NULL,"
        SQL = SQL & "CORP_ID INTEGER NOT NULL,"
        SQL = SQL & "CORP_NAME VARCHAR(200) NOT NULL,"
        SQL = SQL & "ITEM_ID INTEGER NOT NULL,"
        SQL = SQL & "ITEM VARCHAR (100) NOT NULL,"
        SQL = SQL & "ITEM_QUANTITY INTEGER NOT NULL,"
        SQL = SQL & "LP_COST INTEGER NOT NULL,"
        SQL = SQL & "ISK_COST INTEGER NOT NULL,"
        SQL = SQL & "RACE_ID INTEGER NOT NULL"
        SQL = SQL & ")"

        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        ' Now select the count of the final query of data

        ' Pull new data and insert
        msSQL = "SELECT lpOffers.offerID AS OFFER_ID, lpStore.corporationID AS CORP_ID, invNames.itemName AS CORP_NAME, "
        msSQL = msSQL & "invTypes.typeID AS ITEM_ID, invTypes.typeName AS ITEM, lpOffers.quantity AS ITEM_QUANTITY, "
        msSQL = msSQL & "lpCost AS LP_COST, iskCost AS ISK_COST, chrFactions.raceIDs AS RACE_ID "
        msSQL = msSQL & "FROM invTypes, lpOffers, lpStore, invNames, crpNPCCorporations, chrFactions "
        msSQL = msSQL & "WHERE lpOffers.typeID = invTypes.typeID "
        msSQL = msSQL & "AND lpStore.corporationID = invNames.itemID "
        msSQL = msSQL & "AND lpOffers.offerID = lpStore.offerID "
        msSQL = msSQL & "AND lpStore.corporationID = crpNPCCorporations.corporationID "
        msSQL = msSQL & "AND crpNPCCorporations.factionID = chrFactions.factionID "

        msSQLQuery = New SqlCommand(msSQL, DBRef1)
        msSQLReader = msSQLQuery.ExecuteReader()

        Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

        While msSQLReader.Read
            Application.DoEvents()

            SQL = "INSERT INTO LP_STORE VALUES ("
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(3)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(4)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(5)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(6)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(7)) & ","
            SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(8)) & ")"

            Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        End While

        Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

        msSQLReader.Close()
        msSQLReader = Nothing
        msSQLQuery = Nothing

        ' Now index and PK the table
        SQL = "CREATE INDEX IDX_LS_OID ON LP_STORE (OFFER_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_LS_CID ON LP_STORE (CORP_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        SQL = "CREATE INDEX IDX_LS_IID ON LP_STORE (ITEM_ID)"
        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

        pgMain.Visible = False
        Application.DoEvents()

    End Sub

    '' LP_VERIFIED
    'Private Sub Build_LP_VERIFIED()
    '    Dim SQL As String

    '    ' MS SQL variables
    '    Dim msSQLQuery As New SqlCommand
    '    Dim msSQLReader As SqlDataReader
    '    Dim msSQL As String

    '    SQL = "CREATE TABLE LP_VERIFIED ("
    '    SQL = SQL & "corporationID INTEGER PRIMARY KEY,"
    '    SQL = SQL & "verified INTEGER NOT NULL,"
    '    SQL = SQL & "verifiedWith INTEGER"
    '    SQL = SQL & ")"

    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    ' Now select the count of the final query of data

    '    ' Pull new data and insert
    '    msSQL = "SELECT * FROM lpVerified"
    '    msSQLQuery = New SqlCommand(msSQL, DBRef1)
    '    msSQLReader = msSQLQuery.ExecuteReader()

    '    Call EVEIPHSQLiteDB.BeginSQLiteTransaction()

    '    While msSQLReader.Read
    '        Application.DoEvents()

    '        SQL = "INSERT INTO LP_VERIFIED VALUES ("
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(0)) & ","
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(1)) & ","
    '        SQL = SQL & BuildInsertFieldString(msSQLReader.GetValue(2)) & ")"

    '        Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    End While

    '    Call EVEIPHSQLiteDB.CommitSQLiteTransaction()

    '    msSQLReader.Close()
    '    msSQLReader = Nothing
    '    msSQLQuery = Nothing

    '    ' Now index and PK the table
    '    SQL = "CREATE INDEX IDX_LO_COID ON LP_VERIFIED (corporationID)"
    '    Call Execute_SQLiteSQL(SQL, EVEIPHSQLiteDB.DBREf)

    '    pgMain.Visible = False
    '    Application.DoEvents()

    'End Sub

#End Region

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

        Call EnableButtons(False)

        If Not ConnectToDBs() Then
            Me.Cursor = Cursors.Default
            btnBuildDatabase.Enabled = True
            btnBuildSQLServerDB.Enabled = True
            btnImageCopy.Enabled = True
            Exit Sub
        End If

        'Do all random updates here first

        ' Chinese named ships in invtypes for some reason
        Call Execute_msSQL("DELETE FROM invTypes where typeID IN (34480,34478,34476,34474,34472,34470,34468,34466,34464,34462,34460,34458)")

        ' Insert all the data for outpost upgrades, and eggs here
        Call UploadOutpostItems()

        ' When rebuilding the DB, update the ramAssemblyLineTypeDetailPerCategory table
        ' so it is complete and not missing categories of blueprints for assembly lines in 
        ' the ramAssemblyLineTypes table - this will (should!) speed up updates for CREST facilities
        lblTableName.Text = "Updating ramAssemblyLineTypeDetailPerCategory"
        Call UpdateramAssemblyLineTypeDetailPerCategory()

        Try
            ' Add the LP Store tables from script - TODO convert to CREST Look ups
            Call Execute_msSQL(File.OpenText(WorkingDirectory & "\lpDatabase_v0.11\lpDatabase_v0.11.sql").ReadToEnd())
        Catch
            MsgBox(Err.Description)
        End Try

        MsgBox("Updates Complete")
        pgMain.Visible = False
        lblTableName.Text = ""

        Call CloseDBs()

        Me.Cursor = Cursors.Default
        Application.UseWaitCursor = False
        Call EnableButtons(True)

        Application.DoEvents()

    End Sub

    ' Inserts all the items for building outpost upgrades and platforms
    Private Sub UploadOutpostItems()
        Dim msSQL As String = ""

        ' Delete records first if they exist
        Execute_msSQL("DELETE FROM industryBlueprints WHERE blueprintTypeID < 0")
        Execute_msSQL("DELETE FROM industryActivities WHERE blueprintTypeID < 0")
        Execute_msSQL("DELETE FROM industryActivityProducts WHERE blueprintTypeID < 0")
        Execute_msSQL("DELETE FROM industryActivityMaterials WHERE blueprintTypeID < 0")
        Execute_msSQL("DELETE FROM industryActivitySkills WHERE blueprintTypeID < 0")
        Execute_msSQL("DELETE FROM invTypes WHERE typeID < 0")
        Execute_msSQL("DELETE FROM invTypes WHERE typeID > 100000000")

        ' Insert all the "blueprints" into the industry tables manually - these are just the typeID's for the items but negate them and add records for invTypes

        ' Add Upgrade Platforms (Three Tiers)
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27656,1)") ' Foundation - Tier 1
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27656,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27656,1,100027656,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,34,86767990)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,35,7230666)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,36,1355750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,37,271150)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,38,56490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,39,12105)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,40,2648)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,44,275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3683,5975)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3685,5621)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3687,4245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3689,3968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3691,3468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3697,4539)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3727,213)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,3828,32200)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,9826,1964)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,9828,3368)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,9832,2561)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,9838,262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,9842,1718)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,9848,506)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27656,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27656,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27656)

        ' Special insert for the product, which will be 1000 added to the current type id
        msSQL = "INSERT INTO invTypes SELECT 100027656 AS typeID, groupID, 'Foundation Upgrade', description, mass, volume, packagedVolume, capacity, portionSize, factionID, raceID, "
        msSQL = msSQL & "basePrice, published, marketGroupID, graphicID, radius, iconID, soundID, sofFactionName, sofMaterialSetID "
        msSQL = msSQL & "FROM invTypes WHERE typeID = 27656"
        Call Execute_msSQL(msSQL)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27658,1)") ' Pedestal - Tier 2
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27658,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27658,1,100027658,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27658,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27658,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27658)

        ' Special insert for the product, which will be 1000 added to the current type id
        msSQL = "INSERT INTO invTypes SELECT 100027658 AS typeID, groupID, 'Pedestal Upgrade', description, mass, volume, packagedVolume, capacity, portionSize, factionID, raceID, "
        msSQL = msSQL & "basePrice, published, marketGroupID, graphicID, radius, iconID, soundID, sofFactionName, sofMaterialSetID "
        msSQL = msSQL & "FROM invTypes WHERE typeID = 27658"
        Call Execute_msSQL(msSQL)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27660,1)") ' Monument - Tier 3
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27660,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27660,1,100027660,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,34,173535980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,35,14461332)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,36,2711500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,37,542300)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,38,112979)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,39,24210)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,40,5296)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,44,550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3683,11949)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3685,11242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3687,8490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3689,7936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3691,6935)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3697,9078)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3727,426)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,3828,64399)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,9826,3928)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,9828,6735)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,9832,5121)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,9838,524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,9842,3436)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,9848,1012)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27660,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27660,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27660)

        ' Special insert for the product, which will be 1000 added to the current type id
        msSQL = "INSERT INTO invTypes SELECT 100027660 AS typeID, groupID, 'Monument Upgrade', description, mass, volume, packagedVolume, capacity, portionSize, factionID, raceID, "
        msSQL = msSQL & "basePrice, published, marketGroupID, graphicID, radius, iconID, soundID, sofFactionName, sofMaterialSetID "
        msSQL = msSQL & "FROM invTypes WHERE typeID = 27660"
        Call Execute_msSQL(msSQL)

        ' Add the stations

        ' Amarr Factory Outpost
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-21644,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-21644,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-21644,1,21644,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3721,7500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,9848,2024)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21644,1,10260,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-21644,1,3400,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(21644)

        ' Caldari Research Outpost
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-21642,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-21642,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-21642,1,21642,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,34,293190294)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,35,24432524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,36,4581098)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,37,916219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,38,190879)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,39,40902)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,40,8947)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,44,5404)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3683,29897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3685,35489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3687,38724)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3689,19546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3691,11654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3697,38465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3727,3549)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,3828,74897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,9826,17984)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,9828,18465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,9832,8465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,9838,12111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,9842,12441)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,9848,25987)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,13267,50)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21642,1,19758,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-21642,1,3400,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(21642)

        ' Gallente Administrative Outpost
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-21645,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-21645,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-21645,1,21645,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,34,257702131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,35,21475177)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,36,4026595)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,37,805319)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,38,167774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,39,35951)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,40,7864)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3683,55489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3685,31546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3687,66849)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3689,17654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3691,5449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3697,26546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3699,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,3828,89846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,9826,9875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,9828,15555)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,9832,15465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,9838,6874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,9842,9874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,9848,14419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,13267,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21645,1,10257,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-21645,1,3400,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(21645)

        ' Minmatar Service Outpost
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-21646,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-21646,1,3600)") ' Build time is anchoring time
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-21646,1,21646,1,1)")
        ' Add all the mats we put in the egg after it's anchored to build the outpost item
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,34,387522911)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,35,32293575)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,36,6055045)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,37,1211009)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,38,252293)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,39,54062)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,40,11826)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,44,3511)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3683,25468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3685,23574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3687,19871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3689,16876)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3691,17874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3697,8846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3727,1844)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,3828,155649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,9826,5587)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,9828,5489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,9832,12489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,9838,897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,9842,7465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,9848,12499)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-21646,1,10258,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-21646,1,3400,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(21646)

        ' Add the Improvement Platforms

        ' Amarr - Tier 1
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27662,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27662,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27662,1,28081,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,34,86767990)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,35,7230666)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,36,1355750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,37,271150)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,38,56490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,39,12105)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,40,2648)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,44,275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3683,5975)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3685,5621)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3687,4245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3689,3968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3691,3468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3697,4539)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3721,1875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3727,213)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,3828,32200)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,9826,1964)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,9828,3368)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,9832,2561)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,9838,262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,9842,1718)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,9848,506)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,27662,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27662,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27662,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27662)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27961,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27961,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27961,1,28082,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,34,86767990)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,35,7230666)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,36,1355750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,37,271150)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,38,56490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,39,12105)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,40,2648)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,44,275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3683,5975)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3685,5621)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3687,4245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3689,3968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3691,3468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3697,4539)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3721,1875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3727,213)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,3828,32200)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,9826,1964)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,9828,3368)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,9832,2561)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,9838,262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,9842,1718)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,9848,506)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,27961,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27961,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27961,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27961)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27987,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27987,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27987,1,28083,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,34,86767990)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,35,7230666)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,36,1355750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,37,271150)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,38,56490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,39,12105)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,40,2648)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,44,275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3683,5975)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3685,5621)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3687,4245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3689,3968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3691,3468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3697,4539)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3721,1875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3727,213)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,3828,32200)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,9826,1964)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,9828,3368)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,9832,2561)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,9838,262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,9842,1718)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,9848,506)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,27987,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27987,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27987,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27987)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28017,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28017,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28017,1,28084,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,34,86767990)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,35,7230666)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,36,1355750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,37,271150)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,38,56490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,39,12105)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,40,2648)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,44,275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3683,5975)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3685,5621)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3687,4245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3689,3968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3691,3468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3697,4539)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3721,1875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3727,213)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,3828,32200)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,9826,1964)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,9828,3368)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,9832,2561)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,9838,262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,9842,1718)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,9848,506)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,28017,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28017,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28017,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28017)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28041,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28041,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28041,1,28085,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,34,86767990)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,35,7230666)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,36,1355750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,37,271150)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,38,56490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,39,12105)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,40,2648)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,44,275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3683,5975)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3685,5621)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3687,4245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3689,3968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3691,3468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3697,4539)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3721,1875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3727,213)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,3828,32200)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,9826,1964)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,9828,3368)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,9832,2561)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,9838,262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,9842,1718)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,9848,506)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,28041,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28041,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28041,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28041)

        ' Amarr - Tier 2
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27666,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27666,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27666,1,28086,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,34,173535980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,35,14461332)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,36,2711500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,37,542300)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,38,112979)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,39,24210)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,40,5296)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,44,550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3683,11949)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3685,11242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3687,8490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3689,7936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3691,6935)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3697,9078)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3721,3750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3727,426)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,3828,64399)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,9826,3928)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,9828,6735)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,9832,5121)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,9838,524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,9842,3436)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,9848,1012)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,27666,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27666,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27666,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27666)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27963,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27963,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27963,1,28087,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,34,173535980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,35,14461332)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,36,2711500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,37,542300)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,38,112979)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,39,24210)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,40,5296)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,44,550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3683,11949)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3685,11242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3687,8490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3689,7936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3691,6935)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3697,9078)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3721,3750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3727,426)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,3828,64399)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,9826,3928)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,9828,6735)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,9832,5121)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,9838,524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,9842,3436)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,9848,1012)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,27963,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27963,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27963,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27963)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27989,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27989,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27989,1,28088,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,34,173535980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,35,14461332)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,36,2711500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,37,542300)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,38,112979)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,39,24210)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,40,5296)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,44,550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3683,11949)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3685,11242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3687,8490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3689,7936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3691,6935)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3697,9078)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3721,3750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3727,426)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,3828,64399)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,9826,3928)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,9828,6735)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,9832,5121)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,9838,524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,9842,3436)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,9848,1012)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,27989,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27989,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27989,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27989)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28019,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28019,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28019,1,28089,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,34,173535980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,35,14461332)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,36,2711500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,37,542300)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,38,112979)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,39,24210)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,40,5296)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,44,550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3683,11949)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3685,11242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3687,8490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3689,7936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3691,6935)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3697,9078)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3721,3750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3727,426)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,3828,64399)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,9826,3928)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,9828,6735)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,9832,5121)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,9838,524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,9842,3436)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,9848,1012)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,28019,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28019,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28019,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28019)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28043,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28043,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28043,1,28090,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,34,173535980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,35,14461332)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,36,2711500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,37,542300)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,38,112979)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,39,24210)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,40,5296)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,44,550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3683,11949)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3685,11242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3687,8490)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3689,7936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3691,6935)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3697,9078)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3721,3750)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3727,426)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,3828,64399)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,9826,3928)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,9828,6735)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,9832,5121)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,9838,524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,9842,3436)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,9848,1012)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,28043,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28043,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28043,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28043)

        ' Amarr - Tier 3
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27664,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27664,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27664,1,28076,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3721,7500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,9848,2024)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,27664,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27664,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27664,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27664)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27965,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27965,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27965,1,28077,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3721,7500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,9848,2024)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,27965,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27965,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27965,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27965)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27991,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27991,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27991,1,28078,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3721,7500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,9848,2024)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,27991,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27991,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27991,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27991)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28021,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28021,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28021,1,28079,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3721,7500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,9848,2024)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,28021,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28021,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28021,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28021)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28045,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28045,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28045,1,28080,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,34,347071960)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,35,28922663)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,36,5422999)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,37,1084599)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,38,225958)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,39,48419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,40,10591)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,44,1099)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3683,23897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3685,22484)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3687,16980)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3689,15872)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3691,13870)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3697,18156)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3721,7500)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3727,851)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,3828,128798)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,9826,7855)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,9828,13469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,9832,10242)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,9838,1048)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,9842,6871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,9848,2024)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,13267,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,28045,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28045,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28045,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28045)

        ' Caldari - Tier 1
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27937,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27937,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27937,1,28096,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,34,73297574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,35,6108131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,36,1145275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,37,229055)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,38,47720)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,39,10226)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,40,2237)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,44,1351)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3683,7475)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3685,8873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3687,9681)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3689,4887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3691,2914)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3697,9617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3727,888)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,3828,18725)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,9826,4496)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,9828,4617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,9832,2117)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,9838,3028)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,9842,3111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,9848,6497)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,13267,13)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,27937,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27937,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27937,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27937)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27993,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27993,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27993,1,28097,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,34,73297574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,35,6108131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,36,1145275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,37,229055)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,38,47720)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,39,10226)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,40,2237)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,44,1351)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3683,7475)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3685,8873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3687,9681)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3689,4887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3691,2914)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3697,9617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3727,888)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,3828,18725)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,9826,4496)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,9828,4617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,9832,2117)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,9838,3028)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,9842,3111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,9848,6497)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,13267,13)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,27993,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27993,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27993,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27993)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27999,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27999,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27999,1,28098,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,34,73297574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,35,6108131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,36,1145275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,37,229055)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,38,47720)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,39,10226)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,40,2237)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,44,1351)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3683,7475)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3685,8873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3687,9681)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3689,4887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3691,2914)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3697,9617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3727,888)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,3828,18725)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,9826,4496)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,9828,4617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,9832,2117)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,9838,3028)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,9842,3111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,9848,6497)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,13267,13)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,27999,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27999,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27999,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27999)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28023,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28023,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28023,1,28099,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,34,73297574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,35,6108131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,36,1145275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,37,229055)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,38,47720)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,39,10226)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,40,2237)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,44,1351)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3683,7475)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3685,8873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3687,9681)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3689,4887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3691,2914)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3697,9617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3727,888)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,3828,18725)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,9826,4496)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,9828,4617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,9832,2117)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,9838,3028)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,9842,3111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,9848,6497)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,13267,13)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,28023,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28023,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28023,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28023)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28047,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28047,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28047,1,28100,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,34,73297574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,35,6108131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,36,1145275)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,37,229055)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,38,47720)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,39,10226)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,40,2237)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,44,1351)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3683,7475)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3685,8873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3687,9681)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3689,4887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3691,2914)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3697,9617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3727,888)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,3828,18725)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,9826,4496)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,9828,4617)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,9832,2117)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,9838,3028)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,9842,3111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,9848,6497)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,13267,13)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,28047,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28047,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28047,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(20847)

        ' Caldari Tier 2
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27957,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27957,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27957,1,28101,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,34,146595148)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,35,12216262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,36,2290550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,37,458110)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,38,95440)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,39,20452)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,40,4474)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,44,2702)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3683,14950)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3685,17746)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3687,19362)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3689,9774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3691,5828)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3697,19234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3727,1776)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,3828,37450)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,9826,8992)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,9828,9234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,9832,4234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,9838,6056)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,9842,6222)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,9848,12994)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,13267,26)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,27957,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27957,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27957,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27957)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27995,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27995,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27995,1,28102,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,34,146595148)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,35,12216262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,36,2290550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,37,458110)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,38,95440)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,39,20452)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,40,4474)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,44,2702)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3683,14950)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3685,17746)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3687,19362)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3689,9774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3691,5828)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3697,19234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3727,1776)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,3828,37450)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,9826,8992)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,9828,9234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,9832,4234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,9838,6056)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,9842,6222)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,9848,12994)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,13267,26)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,27995,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27995,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27995,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27995)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28001,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28001,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28001,1,28103,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,34,146595148)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,35,12216262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,36,2290550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,37,458110)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,38,95440)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,39,20452)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,40,4474)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,44,2702)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3683,14950)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3685,17746)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3687,19362)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3689,9774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3691,5828)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3697,19234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3727,1776)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,3828,37450)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,9826,8992)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,9828,9234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,9832,4234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,9838,6056)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,9842,6222)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,9848,12994)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,13267,26)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,28001,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28001,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28001,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28001)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28025,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28025,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28025,1,28104,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,34,146595148)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,35,12216262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,36,2290550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,37,458110)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,38,95440)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,39,20452)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,40,4474)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,44,2702)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3683,14950)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3685,17746)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3687,19362)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3689,9774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3691,5828)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3697,19234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3727,1776)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,3828,37450)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,9826,8992)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,9828,9234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,9832,4234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,9838,6056)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,9842,6222)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,9848,12994)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,13267,26)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,28025,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28025,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28025,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28025)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28049,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28049,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28049,1,28105,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,34,146595148)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,35,12216262)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,36,2290550)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,37,458110)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,38,95440)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,39,20452)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,40,4474)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,44,2702)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3683,14950)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3685,17746)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3687,19362)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3689,9774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3691,5828)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3697,19234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3727,1776)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,3828,37450)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,9826,8992)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,9828,9234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,9832,4234)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,9838,6056)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,9842,6222)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,9848,12994)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,13267,26)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,28049,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28049,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28049,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28049)

        ' Caldari - Tier 3
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27959,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27959,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27959,1,28091,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,34,293190294)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,35,24432524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,36,4581098)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,37,916219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,38,190879)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,39,40902)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,40,8947)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,44,5404)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3683,29897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3685,35489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3687,38724)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3689,19546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3691,11654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3697,38465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3727,3549)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,3828,74897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,9826,17984)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,9828,18465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,9832,8465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,9838,12111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,9842,12441)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,9848,25987)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,13267,50)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,27959,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27959,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27959,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27959)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27997,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27997,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27997,1,28092,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,34,293190294)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,35,24432524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,36,4581098)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,37,916219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,38,190879)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,39,40902)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,40,8947)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,44,5404)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3683,29897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3685,35489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3687,38724)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3689,19546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3691,11654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3697,38465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3727,3549)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,3828,74897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,9826,17984)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,9828,18465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,9832,8465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,9838,12111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,9842,12441)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,9848,25987)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,13267,50)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,27997,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27997,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27997,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27997)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28003,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28003,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28003,1,28093,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,34,293190294)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,35,24432524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,36,4581098)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,37,916219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,38,190879)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,39,40902)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,40,8947)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,44,5404)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3683,29897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3685,35489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3687,38724)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3689,19546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3691,11654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3697,38465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3727,3549)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,3828,74897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,9826,17984)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,9828,18465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,9832,8465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,9838,12111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,9842,12441)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,9848,25987)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,13267,50)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,28003,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28003,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28003,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28003)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28027,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28027,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28027,1,28094,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,34,293190294)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,35,24432524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,36,4581098)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,37,916219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,38,190879)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,39,40902)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,40,8947)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,44,5404)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3683,29897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3685,35489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3687,38724)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3689,19546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3691,11654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3697,38465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3727,3549)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,3828,74897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,9826,17984)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,9828,18465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,9832,8465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,9838,12111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,9842,12441)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,9848,25987)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,13267,50)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,28027,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28027,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28027,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28027)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28051,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28051,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28051,1,28095,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,34,293190294)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,35,24432524)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,36,4581098)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,37,916219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,38,190879)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,39,40902)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,40,8947)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,44,5404)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3683,29897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3685,35489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3687,38724)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3689,19546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3691,11654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3697,38465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3727,3549)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,3828,74897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,9826,17984)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,9828,18465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,9832,8465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,9838,12111)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,9842,12441)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,9848,25987)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,13267,50)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,28051,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28051,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28051,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28051)

        ' Gallente - Tier 1
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27939,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27939,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27939,1,28111,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,27939,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27939,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27939,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27939)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27983,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27983,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27983,1,28112,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,27983,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27983,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27983,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27983)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28005,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28005,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28005,1,28113,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,28005,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28005,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28005,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28005)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28029,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28029,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28029,1,28114,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,28029,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28029,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28029,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28029)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28053,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28053,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28053,1,28115,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,28053,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28053,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28053,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28053)

        ' Gallente - Tier 2
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27967,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27967,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27967,1,28116,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,27967,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27967,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27967,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27967)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27975,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27975,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27975,1,28117,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,27975,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27975,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27975,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27975)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28007,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28007,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28007,1,28118,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,28007,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28007,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28007,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28007)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28031,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28031,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28031,1,28119,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,28031,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28031,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28031,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28031)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28055,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28055,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28055,1,28120,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,34,64425533)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,35,5368795)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,36,1006649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,37,201330)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,38,41944)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,39,8988)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,40,1966)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3683,13873)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3685,7887)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3687,16713)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3689,4414)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3691,1363)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3697,6637)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3699,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,3828,22462)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,9826,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,9828,3889)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,9832,3867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,9838,1719)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,9842,2469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,9848,3605)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,13267,25)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,28055,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28055,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28055,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28055)

        ' Gallente - Tier 3
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27969,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27969,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27969,1,28106,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,34,257702131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,35,21475177)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,36,4026595)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,37,805319)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,38,167774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,39,35951)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,40,7864)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3683,55489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3685,31546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3687,66849)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3689,17654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3691,5449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3697,26546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3699,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,3828,89846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,9826,9875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,9828,15555)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,9832,15465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,9838,6874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,9842,9874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,9848,14419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,13267,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,27969,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27969,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27969,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27969)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27977,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27977,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27977,1,28107,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,34,257702131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,35,21475177)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,36,4026595)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,37,805319)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,38,167774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,39,35951)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,40,7864)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3683,55489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3685,31546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3687,66849)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3689,17654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3691,5449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3697,26546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3699,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,3828,89846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,9826,9875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,9828,15555)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,9832,15465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,9838,6874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,9842,9874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,9848,14419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,13267,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,27977,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27977,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27977,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27977)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28009,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28009,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28009,1,28108,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,34,257702131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,35,21475177)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,36,4026595)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,37,805319)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,38,167774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,39,35951)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,40,7864)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3683,55489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3685,31546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3687,66849)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3689,17654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3691,5449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3697,26546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3699,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,3828,89846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,9826,9875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,9828,15555)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,9832,15465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,9838,6874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,9842,9874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,9848,14419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,13267,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,28009,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28009,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28009,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28009)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28033,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28033,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28033,1,28109,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,34,257702131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,35,21475177)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,36,4026595)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,37,805319)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,38,167774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,39,35951)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,40,7864)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3683,55489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3685,31546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3687,66849)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3689,17654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3691,5449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3697,26546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3699,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,3828,89846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,9826,9875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,9828,15555)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,9832,15465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,9838,6874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,9842,9874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,9848,14419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,13267,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,28033,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28033,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28033,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28033)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28057,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28057,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28057,1,28110,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,34,257702131)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,35,21475177)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,36,4026595)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,37,805319)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,38,167774)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,39,35951)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,40,7864)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3683,55489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3685,31546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3687,66849)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3689,17654)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3691,5449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3697,26546)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3699,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,3828,89846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,9826,9875)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,9828,15555)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,9832,15465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,9838,6874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,9842,9874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,9848,14419)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,13267,100)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,28057,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28057,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28057,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28057)

        ' Minmatar - Tier 1
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27941,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27941,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27941,1,28126,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,34,96880728)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,35,8073394)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,36,1513762)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,37,302753)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,38,63074)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,39,13516)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,40,2957)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,44,878)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3683,6367)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3685,5894)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3687,4968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3689,4219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3691,4469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3697,2212)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3727,461)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,3828,38913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,9826,1397)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,9828,1373)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,9832,3123)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,9838,225)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,9842,1867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,9848,3125)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,27941,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27941,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27941,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27941)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27985,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27985,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27985,1,28127,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,34,96880728)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,35,8073394)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,36,1513762)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,37,302753)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,38,63074)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,39,13516)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,40,2957)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,44,878)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3683,6367)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3685,5894)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3687,4968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3689,4219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3691,4469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3697,2212)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3727,461)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,3828,38913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,9826,1397)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,9828,1373)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,9832,3123)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,9838,225)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,9842,1867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,9848,3125)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,27985,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27985,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27985,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27985)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28011,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28011,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28011,1,28128,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,34,96880728)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,35,8073394)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,36,1513762)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,37,302753)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,38,63074)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,39,13516)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,40,2957)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,44,878)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3683,6367)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3685,5894)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3687,4968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3689,4219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3691,4469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3697,2212)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3727,461)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,3828,38913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,9826,1397)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,9828,1373)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,9832,3123)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,9838,225)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,9842,1867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,9848,3125)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,28011,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28011,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28011,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28011)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28035,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28035,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28035,1,28129,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,34,96880728)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,35,8073394)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,36,1513762)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,37,302753)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,38,63074)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,39,13516)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,40,2957)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,44,878)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3683,6367)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3685,5894)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3687,4968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3689,4219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3691,4469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3697,2212)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3727,461)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,3828,38913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,9826,1397)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,9828,1373)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,9832,3123)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,9838,225)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,9842,1867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,9848,3125)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,28035,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28035,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28035,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28035)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28059,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28059,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28059,1,28130,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,34,96880728)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,35,8073394)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,36,1513762)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,37,302753)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,38,63074)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,39,13516)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,40,2957)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,44,878)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3683,6367)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3685,5894)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3687,4968)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3689,4219)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3691,4469)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3697,2212)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3727,461)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,3828,38913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,9826,1397)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,9828,1373)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,9832,3123)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,9838,225)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,9842,1867)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,9848,3125)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,28059,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28059,1,3400,1)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28059,1,27656,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28059)

        ' Minmatar - Tier 2
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27971,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27971,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27971,1,28131,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,34,193761456)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,35,16146788)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,36,3027523)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,37,605505)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,38,126147)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,39,27031)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,40,5913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,44,1756)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3683,12734)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3685,11787)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3687,9936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3689,8438)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3691,8937)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3697,4423)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3727,922)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,3828,77825)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,9826,2794)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,9828,2745)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,9832,6245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,9838,449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,9842,3733)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,9848,6250)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,27971,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27971,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27971,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27971)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27979,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27979,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27979,1,28132,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,34,193761456)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,35,16146788)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,36,3027523)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,37,605505)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,38,126147)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,39,27031)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,40,5913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,44,1756)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3683,12734)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3685,11787)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3687,9936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3689,8438)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3691,8937)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3697,4423)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3727,922)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,3828,77825)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,9826,2794)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,9828,2745)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,9832,6245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,9838,449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,9842,3733)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,9848,6250)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,27979,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27979,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27979,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27979)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28013,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28013,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28013,1,28133,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,34,193761456)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,35,16146788)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,36,3027523)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,37,605505)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,38,126147)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,39,27031)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,40,5913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,44,1756)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3683,12734)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3685,11787)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3687,9936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3689,8438)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3691,8937)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3697,4423)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3727,922)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,3828,77825)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,9826,2794)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,9828,2745)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,9832,6245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,9838,449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,9842,3733)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,9848,6250)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,28013,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28013,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28013,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28013)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28037,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28037,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28037,1,28134,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,34,193761456)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,35,16146788)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,36,3027523)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,37,605505)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,38,126147)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,39,27031)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,40,5913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,44,1756)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3683,12734)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3685,11787)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3687,9936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3689,8438)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3691,8937)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3697,4423)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3727,922)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,3828,77825)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,9826,2794)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,9828,2745)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,9832,6245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,9838,449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,9842,3733)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,9848,6250)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,28037,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28037,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28037,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28037)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28061,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28061,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28061,1,28135,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,34,193761456)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,35,16146788)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,36,3027523)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,37,605505)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,38,126147)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,39,27031)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,40,5913)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,44,1756)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3683,12734)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3685,11787)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3687,9936)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3689,8438)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3691,8937)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3697,4423)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3727,922)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,3828,77825)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,9826,2794)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,9828,2745)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,9832,6245)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,9838,449)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,9842,3733)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,9848,6250)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,28061,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28061,1,3400,3)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28061,1,27660,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28061)

        ' Minmatar - Tier 3
        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27973,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27973,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27973,1,28121,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,34,387522911)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,35,32293575)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,36,6055045)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,37,1211009)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,38,252293)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,39,54062)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,40,11826)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,44,3511)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3683,25468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3685,23574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3687,19871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3689,16876)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3691,17874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3697,8846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3727,1844)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,3828,155649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,9826,5587)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,9828,5489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,9832,12489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,9838,897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,9842,7465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,9848,12499)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,27973,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27973,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27973,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27973)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-27981,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-27981,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-27981,1,28122,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,34,387522911)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,35,32293575)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,36,6055045)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,37,1211009)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,38,252293)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,39,54062)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,40,11826)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,44,3511)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3683,25468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3685,23574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3687,19871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3689,16876)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3691,17874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3697,8846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3727,1844)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,3828,155649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,9826,5587)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,9828,5489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,9832,12489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,9838,897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,9842,7465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,9848,12499)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,27981,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-27981,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-27981,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(27981)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28015,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28015,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28015,1,28123,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,34,387522911)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,35,32293575)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,36,6055045)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,37,1211009)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,38,252293)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,39,54062)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,40,11826)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,44,3511)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3683,25468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3685,23574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3687,19871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3689,16876)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3691,17874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3697,8846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3727,1844)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,3828,155649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,9826,5587)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,9828,5489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,9832,12489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,9838,897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,9842,7465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,9848,12499)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,28015,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28015,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28015,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28015)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28039,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28039,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28039,1,28124,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,34,387522911)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,35,32293575)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,36,6055045)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,37,1211009)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,38,252293)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,39,54062)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,40,11826)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,44,3511)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3683,25468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3685,23574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3687,19871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3689,16876)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3691,17874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3697,8846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3727,1844)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,3828,155649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,9826,5587)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,9828,5489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,9832,12489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,9838,897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,9842,7465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,9848,12499)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,28039,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28039,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28039,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28039)

        Execute_msSQL("INSERT INTO industryBlueprints VALUES (-28063,1)")
        Execute_msSQL("INSERT INTO industryActivities VALUES (-28063,1,3600)")
        Execute_msSQL("INSERT INTO industryActivityProducts VALUES (-28063,1,28125,1,1)")

        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,34,387522911)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,35,32293575)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,36,6055045)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,37,1211009)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,38,252293)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,39,54062)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,40,11826)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,44,3511)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3683,25468)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3685,23574)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3687,19871)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3689,16876)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3691,17874)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3697,8846)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3727,1844)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,3828,155649)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,9826,5587)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,9828,5489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,9832,12489)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,9838,897)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,9842,7465)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,9848,12499)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,28063,1)")
        Execute_msSQL("INSERT INTO industryActivitySkills VALUES (-28063,1,3400,5)")
        Execute_msSQL("INSERT INTO industryActivityMaterials VALUES (-28063,1,27658,1)")

        ' Insert the "blueprint" into inventory types
        Call InsertNegativeBPTypeIDRecord(28063)

    End Sub

    ' InsertNegativeBPTypeIDRecord
    Private Sub InsertNegativeBPTypeIDRecord(BPID As Long)
        Dim msSQL As String

        ' Look up the current record and add the negative type id so we don't get into a recursive loop for outposts
        msSQL = "INSERT INTO invTypes SELECT typeID * -1 AS typeID, groupID, typeName, description, mass, volume, packagedVolume, capacity, portionSize, factionID, raceID, "
        msSQL = msSQL & "basePrice, published, marketGroupID, graphicID, radius, iconID, soundID, sofFactionName, sofMaterialSetID "
        msSQL = msSQL & "FROM invTypes WHERE typeID = " & BPID

        Call Execute_msSQL(msSQL)

    End Sub

    ' Returns a connection reference for internal use
    Private Function DBConnectionRef() As SqlConnection

        ' Open the connection for reference
        Dim DBRef As New SqlConnection(String.Format("Server={0}\{1};Database={2};Trusted_Connection=True;Connection Timeout=60;",
                                             Environment.MachineName, SQLInstance, DatabaseName))
        DBRef.Open()

        Return DBRef

    End Function

    ' PACKAGED_CONTAINER_VOLUMES
    Private Sub Build_PACKAGED_CONTAINER_VOLUMES()
        Dim SQL As String

        ' Build the table in msSQL and use it later
        SQL = String.Format("If OBJECT_ID('{0}') IS NOT NULL " +
                            "   DROP TABLE {0} " +
                            "   CREATE TABLE {0} (TYPE_ID INTEGER NOT NULL,PACKAGED_M3 INTEGER NOT NULL)", "PACKAGED_CONTAINER_VOLUMES")

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

        ' Build the table in msSQL and use it later
        SQL = String.Format("If OBJECT_ID('{0}') IS NOT NULL " +
                            "   DROP TABLE {0} " +
                            "   CREATE TABLE {0} (GROUP_ID INTEGER NOT NULL,PACKAGED_M3 INTEGER NOT NULL)", "PACKAGED_SHIP_VOLUMES")

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
            FileDirectory = MediaFireTestDirectory
        Else
            FileDirectory = MediaFireDirectory
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

        ' Output the Latest XML File if we have updates
        If NewFilesAdded Then
            Call WriteLatestXMLFile()
        End If

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
            FileDirectory = MediaFireTestDirectory

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
                writer.WriteAttributeString("Version", "6.03")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & JSONDLL))
                writer.WriteAttributeString("URL", TestJSONDLLURL)
                writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", SQLiteDLL)
                writer.WriteAttributeString("Version", "1.07.9.0")
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

                'writer.WriteStartElement("row")
                'writer.WriteAttributeString("Name", IonicZipFile)
                'writer.WriteAttributeString("Version", "1.0")
                'writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & IonicZipFile))
                'writer.WriteAttributeString("URL", TestIconicZipFileURL)
                'writer.WriteEndElement()

                writer.WriteStartElement("row")
                writer.WriteAttributeString("Name", MoreLinqDLL)
                writer.WriteAttributeString("Version", "1.4")
                writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & MoreLinqDLL))
                writer.WriteAttributeString("URL", MoreLinqDLLURL)
                writer.WriteEndElement()

                ' End document.
                writer.WriteEndDocument()
            End Using
        Else
            File.Delete(LatestVersionXML)
            VersionXMLFileName = LatestVersionXML
            FileDirectory = MediaFireDirectory

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

                'writer.WriteStartElement("row")
                'writer.WriteAttributeString("Name", IonicZipFile)
                'writer.WriteAttributeString("Version", "1.0")
                'writer.WriteAttributeString("MD5", MD5CalcFile(FileDirectory & IonicZipFile))
                'writer.WriteAttributeString("URL", IconicZipFileURL)
                'writer.WriteEndElement()

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
