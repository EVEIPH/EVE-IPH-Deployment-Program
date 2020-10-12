<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.txtVersionNumber = New System.Windows.Forms.TextBox()
        Me.txtDBName = New System.Windows.Forms.TextBox()
        Me.lblTestPath = New System.Windows.Forms.Label()
        Me.btnSelectTestFilePath = New System.Windows.Forms.Button()
        Me.lblTest = New System.Windows.Forms.Label()
        Me.lblVersionNumber = New System.Windows.Forms.Label()
        Me.lblDBName = New System.Windows.Forms.Label()
        Me.btnSaveFilePath = New System.Windows.Forms.Button()
        Me.lblFilesPath = New System.Windows.Forms.Label()
        Me.btnSelectFilePath = New System.Windows.Forms.Button()
        Me.lblFiles = New System.Windows.Forms.Label()
        Me.lblRootDebugFolderPath = New System.Windows.Forms.Label()
        Me.btnSelectRootDebugPath = New System.Windows.Forms.Button()
        Me.lblRootDebugFolder = New System.Windows.Forms.Label()
        Me.lblWorkingFolderPath = New System.Windows.Forms.Label()
        Me.btnSelectWorkingPath = New System.Windows.Forms.Button()
        Me.lblWorkingFolder = New System.Windows.Forms.Label()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.chkCreateTest = New System.Windows.Forms.CheckBox()
        Me.btnRefreshList = New System.Windows.Forms.Button()
        Me.lblDBNameDisplay1 = New System.Windows.Forms.Label()
        Me.lblDBNameDisplay = New System.Windows.Forms.Label()
        Me.btnCopyFilesBuildXML = New System.Windows.Forms.Button()
        Me.btnBuildBinary = New System.Windows.Forms.Button()
        Me.lstFileInformation = New System.Windows.Forms.ListView()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.pgMain = New System.Windows.Forms.ProgressBar()
        Me.lblTableName = New System.Windows.Forms.Label()
        Me.btnBuildDatabase = New System.Windows.Forms.Button()
        Me.btnImageCopy = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage2.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.txtVersionNumber)
        Me.TabPage2.Controls.Add(Me.txtDBName)
        Me.TabPage2.Controls.Add(Me.lblTestPath)
        Me.TabPage2.Controls.Add(Me.btnSelectTestFilePath)
        Me.TabPage2.Controls.Add(Me.lblTest)
        Me.TabPage2.Controls.Add(Me.lblVersionNumber)
        Me.TabPage2.Controls.Add(Me.lblDBName)
        Me.TabPage2.Controls.Add(Me.btnSaveFilePath)
        Me.TabPage2.Controls.Add(Me.lblFilesPath)
        Me.TabPage2.Controls.Add(Me.btnSelectFilePath)
        Me.TabPage2.Controls.Add(Me.lblFiles)
        Me.TabPage2.Controls.Add(Me.lblRootDebugFolderPath)
        Me.TabPage2.Controls.Add(Me.btnSelectRootDebugPath)
        Me.TabPage2.Controls.Add(Me.lblRootDebugFolder)
        Me.TabPage2.Controls.Add(Me.lblWorkingFolderPath)
        Me.TabPage2.Controls.Add(Me.btnSelectWorkingPath)
        Me.TabPage2.Controls.Add(Me.lblWorkingFolder)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(366, 486)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "File Path Settings"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'txtVersionNumber
        '
        Me.txtVersionNumber.Location = New System.Drawing.Point(313, 23)
        Me.txtVersionNumber.Name = "txtVersionNumber"
        Me.txtVersionNumber.Size = New System.Drawing.Size(38, 20)
        Me.txtVersionNumber.TabIndex = 5
        '
        'txtDBName
        '
        Me.txtDBName.Location = New System.Drawing.Point(18, 23)
        Me.txtDBName.Name = "txtDBName"
        Me.txtDBName.Size = New System.Drawing.Size(205, 20)
        Me.txtDBName.TabIndex = 1
        '
        'lblTestPath
        '
        Me.lblTestPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblTestPath.Location = New System.Drawing.Point(18, 151)
        Me.lblTestPath.Name = "lblTestPath"
        Me.lblTestPath.Size = New System.Drawing.Size(333, 20)
        Me.lblTestPath.TabIndex = 17
        Me.lblTestPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectTestFilePath
        '
        Me.btnSelectTestFilePath.Location = New System.Drawing.Point(18, 174)
        Me.btnSelectTestFilePath.Name = "btnSelectTestFilePath"
        Me.btnSelectTestFilePath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectTestFilePath.TabIndex = 18
        Me.btnSelectTestFilePath.Text = "Select"
        Me.btnSelectTestFilePath.UseVisualStyleBackColor = True
        '
        'lblTest
        '
        Me.lblTest.AutoSize = True
        Me.lblTest.Location = New System.Drawing.Point(15, 135)
        Me.lblTest.Name = "lblTest"
        Me.lblTest.Size = New System.Drawing.Size(122, 13)
        Me.lblTest.TabIndex = 16
        Me.lblTest.Text = "Test Deployment Folder:"
        '
        'lblVersionNumber
        '
        Me.lblVersionNumber.AutoSize = True
        Me.lblVersionNumber.Location = New System.Drawing.Point(229, 26)
        Me.lblVersionNumber.Name = "lblVersionNumber"
        Me.lblVersionNumber.Size = New System.Drawing.Size(85, 13)
        Me.lblVersionNumber.TabIndex = 4
        Me.lblVersionNumber.Text = "Version Number:"
        '
        'lblDBName
        '
        Me.lblDBName.AutoSize = True
        Me.lblDBName.Location = New System.Drawing.Point(15, 7)
        Me.lblDBName.Name = "lblDBName"
        Me.lblDBName.Size = New System.Drawing.Size(87, 13)
        Me.lblDBName.TabIndex = 0
        Me.lblDBName.Text = "Database Name:"
        '
        'btnSaveFilePath
        '
        Me.btnSaveFilePath.Location = New System.Drawing.Point(127, 343)
        Me.btnSaveFilePath.Name = "btnSaveFilePath"
        Me.btnSaveFilePath.Size = New System.Drawing.Size(97, 28)
        Me.btnSaveFilePath.TabIndex = 15
        Me.btnSaveFilePath.Text = "Save Settings"
        Me.btnSaveFilePath.UseVisualStyleBackColor = True
        '
        'lblFilesPath
        '
        Me.lblFilesPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblFilesPath.Location = New System.Drawing.Point(18, 86)
        Me.lblFilesPath.Name = "lblFilesPath"
        Me.lblFilesPath.Size = New System.Drawing.Size(333, 20)
        Me.lblFilesPath.TabIndex = 7
        Me.lblFilesPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectFilePath
        '
        Me.btnSelectFilePath.Location = New System.Drawing.Point(18, 109)
        Me.btnSelectFilePath.Name = "btnSelectFilePath"
        Me.btnSelectFilePath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectFilePath.TabIndex = 8
        Me.btnSelectFilePath.Text = "Select"
        Me.btnSelectFilePath.UseVisualStyleBackColor = True
        '
        'lblFiles
        '
        Me.lblFiles.AutoSize = True
        Me.lblFiles.Location = New System.Drawing.Point(15, 70)
        Me.lblFiles.Name = "lblFiles"
        Me.lblFiles.Size = New System.Drawing.Size(98, 13)
        Me.lblFiles.TabIndex = 6
        Me.lblFiles.Text = "Deployment Folder:"
        '
        'lblRootDebugFolderPath
        '
        Me.lblRootDebugFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRootDebugFolderPath.Location = New System.Drawing.Point(18, 285)
        Me.lblRootDebugFolderPath.Name = "lblRootDebugFolderPath"
        Me.lblRootDebugFolderPath.Size = New System.Drawing.Size(333, 20)
        Me.lblRootDebugFolderPath.TabIndex = 13
        Me.lblRootDebugFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectRootDebugPath
        '
        Me.btnSelectRootDebugPath.Location = New System.Drawing.Point(18, 308)
        Me.btnSelectRootDebugPath.Name = "btnSelectRootDebugPath"
        Me.btnSelectRootDebugPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectRootDebugPath.TabIndex = 14
        Me.btnSelectRootDebugPath.Text = "Select"
        Me.btnSelectRootDebugPath.UseVisualStyleBackColor = True
        '
        'lblRootDebugFolder
        '
        Me.lblRootDebugFolder.AutoSize = True
        Me.lblRootDebugFolder.Location = New System.Drawing.Point(15, 269)
        Me.lblRootDebugFolder.Name = "lblRootDebugFolder"
        Me.lblRootDebugFolder.Size = New System.Drawing.Size(142, 13)
        Me.lblRootDebugFolder.TabIndex = 12
        Me.lblRootDebugFolder.Text = "EVEIPH Root Debug Folder:"
        '
        'lblWorkingFolderPath
        '
        Me.lblWorkingFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblWorkingFolderPath.Location = New System.Drawing.Point(18, 220)
        Me.lblWorkingFolderPath.Name = "lblWorkingFolderPath"
        Me.lblWorkingFolderPath.Size = New System.Drawing.Size(333, 20)
        Me.lblWorkingFolderPath.TabIndex = 10
        Me.lblWorkingFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectWorkingPath
        '
        Me.btnSelectWorkingPath.Location = New System.Drawing.Point(18, 243)
        Me.btnSelectWorkingPath.Name = "btnSelectWorkingPath"
        Me.btnSelectWorkingPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectWorkingPath.TabIndex = 11
        Me.btnSelectWorkingPath.Text = "Select"
        Me.btnSelectWorkingPath.UseVisualStyleBackColor = True
        '
        'lblWorkingFolder
        '
        Me.lblWorkingFolder.AutoSize = True
        Me.lblWorkingFolder.Location = New System.Drawing.Point(15, 204)
        Me.lblWorkingFolder.Name = "lblWorkingFolder"
        Me.lblWorkingFolder.Size = New System.Drawing.Size(107, 13)
        Me.lblWorkingFolder.TabIndex = 9
        Me.lblWorkingFolder.Text = "SDE Working Folder:"
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.chkCreateTest)
        Me.TabPage1.Controls.Add(Me.btnRefreshList)
        Me.TabPage1.Controls.Add(Me.lblDBNameDisplay1)
        Me.TabPage1.Controls.Add(Me.lblDBNameDisplay)
        Me.TabPage1.Controls.Add(Me.btnCopyFilesBuildXML)
        Me.TabPage1.Controls.Add(Me.btnBuildBinary)
        Me.TabPage1.Controls.Add(Me.lstFileInformation)
        Me.TabPage1.Controls.Add(Me.btnExit)
        Me.TabPage1.Controls.Add(Me.pgMain)
        Me.TabPage1.Controls.Add(Me.lblTableName)
        Me.TabPage1.Controls.Add(Me.btnBuildDatabase)
        Me.TabPage1.Controls.Add(Me.btnImageCopy)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(366, 486)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "DB Updater & Deployment"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'chkCreateTest
        '
        Me.chkCreateTest.Location = New System.Drawing.Point(123, 462)
        Me.chkCreateTest.Name = "chkCreateTest"
        Me.chkCreateTest.Size = New System.Drawing.Size(124, 21)
        Me.chkCreateTest.TabIndex = 13
        Me.chkCreateTest.Text = "Create Test Version"
        Me.chkCreateTest.UseVisualStyleBackColor = True
        '
        'btnRefreshList
        '
        Me.btnRefreshList.Location = New System.Drawing.Point(243, 157)
        Me.btnRefreshList.Name = "btnRefreshList"
        Me.btnRefreshList.Size = New System.Drawing.Size(97, 40)
        Me.btnRefreshList.TabIndex = 12
        Me.btnRefreshList.Text = "Refresh List"
        Me.btnRefreshList.UseVisualStyleBackColor = True
        '
        'lblDBNameDisplay1
        '
        Me.lblDBNameDisplay1.AutoSize = True
        Me.lblDBNameDisplay1.Location = New System.Drawing.Point(30, 19)
        Me.lblDBNameDisplay1.Name = "lblDBNameDisplay1"
        Me.lblDBNameDisplay1.Size = New System.Drawing.Size(87, 13)
        Me.lblDBNameDisplay1.TabIndex = 1
        Me.lblDBNameDisplay1.Text = "Database Name:"
        '
        'lblDBNameDisplay
        '
        Me.lblDBNameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDBNameDisplay.Location = New System.Drawing.Point(123, 15)
        Me.lblDBNameDisplay.Name = "lblDBNameDisplay"
        Me.lblDBNameDisplay.Size = New System.Drawing.Size(217, 20)
        Me.lblDBNameDisplay.TabIndex = 2
        Me.lblDBNameDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnCopyFilesBuildXML
        '
        Me.btnCopyFilesBuildXML.Location = New System.Drawing.Point(27, 157)
        Me.btnCopyFilesBuildXML.Name = "btnCopyFilesBuildXML"
        Me.btnCopyFilesBuildXML.Size = New System.Drawing.Size(97, 40)
        Me.btnCopyFilesBuildXML.TabIndex = 9
        Me.btnCopyFilesBuildXML.Text = "Update Files for Export"
        Me.btnCopyFilesBuildXML.UseVisualStyleBackColor = True
        '
        'btnBuildBinary
        '
        Me.btnBuildBinary.Location = New System.Drawing.Point(135, 157)
        Me.btnBuildBinary.Name = "btnBuildBinary"
        Me.btnBuildBinary.Size = New System.Drawing.Size(97, 40)
        Me.btnBuildBinary.TabIndex = 10
        Me.btnBuildBinary.Text = "Build Binary"
        Me.btnBuildBinary.UseVisualStyleBackColor = True
        '
        'lstFileInformation
        '
        Me.lstFileInformation.FullRowSelect = True
        Me.lstFileInformation.GridLines = True
        Me.lstFileInformation.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstFileInformation.Location = New System.Drawing.Point(27, 205)
        Me.lstFileInformation.Name = "lstFileInformation"
        Me.lstFileInformation.Size = New System.Drawing.Size(313, 251)
        Me.lstFileInformation.TabIndex = 11
        Me.lstFileInformation.UseCompatibleStateImageBehavior = False
        Me.lstFileInformation.View = System.Windows.Forms.View.Details
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(243, 43)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(97, 40)
        Me.btnExit.TabIndex = 6
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'pgMain
        '
        Me.pgMain.Location = New System.Drawing.Point(27, 93)
        Me.pgMain.Name = "pgMain"
        Me.pgMain.Size = New System.Drawing.Size(313, 18)
        Me.pgMain.TabIndex = 7
        Me.pgMain.Visible = False
        '
        'lblTableName
        '
        Me.lblTableName.Location = New System.Drawing.Point(27, 117)
        Me.lblTableName.Name = "lblTableName"
        Me.lblTableName.Size = New System.Drawing.Size(313, 18)
        Me.lblTableName.TabIndex = 8
        Me.lblTableName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnBuildDatabase
        '
        Me.btnBuildDatabase.Location = New System.Drawing.Point(27, 43)
        Me.btnBuildDatabase.Name = "btnBuildDatabase"
        Me.btnBuildDatabase.Size = New System.Drawing.Size(97, 40)
        Me.btnBuildDatabase.TabIndex = 3
        Me.btnBuildDatabase.Text = "Build DB"
        Me.btnBuildDatabase.UseVisualStyleBackColor = True
        '
        'btnImageCopy
        '
        Me.btnImageCopy.Location = New System.Drawing.Point(135, 43)
        Me.btnImageCopy.Name = "btnImageCopy"
        Me.btnImageCopy.Size = New System.Drawing.Size(97, 40)
        Me.btnImageCopy.TabIndex = 4
        Me.btnImageCopy.Text = "Image Copy"
        Me.btnImageCopy.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(1, 2)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(374, 512)
        Me.TabControl1.TabIndex = 19
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(374, 512)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EVE IPH Deployment Program"
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents txtVersionNumber As TextBox
    Friend WithEvents txtDBName As TextBox
    Friend WithEvents lblTestPath As Label
    Friend WithEvents btnSelectTestFilePath As Button
    Friend WithEvents lblTest As Label
    Friend WithEvents lblVersionNumber As Label
    Friend WithEvents lblDBName As Label
    Friend WithEvents btnSaveFilePath As Button
    Friend WithEvents lblFilesPath As Label
    Friend WithEvents btnSelectFilePath As Button
    Friend WithEvents lblFiles As Label
    Friend WithEvents lblRootDebugFolderPath As Label
    Friend WithEvents btnSelectRootDebugPath As Button
    Friend WithEvents lblRootDebugFolder As Label
    Friend WithEvents lblWorkingFolderPath As Label
    Friend WithEvents btnSelectWorkingPath As Button
    Friend WithEvents lblWorkingFolder As Label
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents chkCreateTest As CheckBox
    Friend WithEvents btnRefreshList As Button
    Friend WithEvents lblDBNameDisplay1 As Label
    Friend WithEvents lblDBNameDisplay As Label
    Friend WithEvents btnCopyFilesBuildXML As Button
    Friend WithEvents btnBuildBinary As Button
    Friend WithEvents lstFileInformation As ListView
    Friend WithEvents btnExit As Button
    Friend WithEvents pgMain As ProgressBar
    Friend WithEvents lblTableName As Label
    Friend WithEvents btnBuildDatabase As Button
    Friend WithEvents btnImageCopy As Button
    Friend WithEvents TabControl1 As TabControl
End Class
