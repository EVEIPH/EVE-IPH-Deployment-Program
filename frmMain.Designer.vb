<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtVersionNumber = New System.Windows.Forms.TextBox()
        Me.txtDBName = New System.Windows.Forms.TextBox()
        Me.lblMSIInstaller = New System.Windows.Forms.Label()
        Me.btnSelectTestFilePath = New System.Windows.Forms.Button()
        Me.lblMSIFolder = New System.Windows.Forms.Label()
        Me.lblVersionNumber = New System.Windows.Forms.Label()
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
        Me.btnRefreshList = New System.Windows.Forms.Button()
        Me.lblDBNameDisplay1 = New System.Windows.Forms.Label()
        Me.btnCopyFilesBuildXML = New System.Windows.Forms.Button()
        Me.btnBuildBinary = New System.Windows.Forms.Button()
        Me.lstFileInformation = New System.Windows.Forms.ListView()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.pgMain = New System.Windows.Forms.ProgressBar()
        Me.lblTableName = New System.Windows.Forms.Label()
        Me.btnBuildDatabase = New System.Windows.Forms.Button()
        Me.btnImageCopy = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtVersionNumber
        '
        Me.txtVersionNumber.Location = New System.Drawing.Point(292, 26)
        Me.txtVersionNumber.Name = "txtVersionNumber"
        Me.txtVersionNumber.Size = New System.Drawing.Size(38, 20)
        Me.txtVersionNumber.TabIndex = 48
        '
        'txtDBName
        '
        Me.txtDBName.Location = New System.Drawing.Point(79, 26)
        Me.txtDBName.Name = "txtDBName"
        Me.txtDBName.Size = New System.Drawing.Size(145, 20)
        Me.txtDBName.TabIndex = 46
        '
        'lblMSIInstaller
        '
        Me.lblMSIInstaller.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblMSIInstaller.Location = New System.Drawing.Point(351, 179)
        Me.lblMSIInstaller.Name = "lblMSIInstaller"
        Me.lblMSIInstaller.Size = New System.Drawing.Size(333, 40)
        Me.lblMSIInstaller.TabIndex = 53
        '
        'btnSelectTestFilePath
        '
        Me.btnSelectTestFilePath.Location = New System.Drawing.Point(352, 222)
        Me.btnSelectTestFilePath.Name = "btnSelectTestFilePath"
        Me.btnSelectTestFilePath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectTestFilePath.TabIndex = 54
        Me.btnSelectTestFilePath.Text = "Select"
        Me.btnSelectTestFilePath.UseVisualStyleBackColor = True
        '
        'lblMSIFolder
        '
        Me.lblMSIFolder.AutoSize = True
        Me.lblMSIFolder.Location = New System.Drawing.Point(351, 164)
        Me.lblMSIFolder.Name = "lblMSIFolder"
        Me.lblMSIFolder.Size = New System.Drawing.Size(61, 13)
        Me.lblMSIFolder.TabIndex = 52
        Me.lblMSIFolder.Text = "MSI Folder:"
        '
        'lblVersionNumber
        '
        Me.lblVersionNumber.Location = New System.Drawing.Point(232, 19)
        Me.lblVersionNumber.Name = "lblVersionNumber"
        Me.lblVersionNumber.Size = New System.Drawing.Size(54, 32)
        Me.lblVersionNumber.TabIndex = 47
        Me.lblVersionNumber.Text = "Version Number:"
        Me.lblVersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSaveFilePath
        '
        Me.btnSaveFilePath.Location = New System.Drawing.Point(352, 21)
        Me.btnSaveFilePath.Name = "btnSaveFilePath"
        Me.btnSaveFilePath.Size = New System.Drawing.Size(97, 28)
        Me.btnSaveFilePath.TabIndex = 61
        Me.btnSaveFilePath.Text = "Save Settings"
        Me.btnSaveFilePath.UseVisualStyleBackColor = True
        '
        'lblFilesPath
        '
        Me.lblFilesPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblFilesPath.Location = New System.Drawing.Point(351, 89)
        Me.lblFilesPath.Name = "lblFilesPath"
        Me.lblFilesPath.Size = New System.Drawing.Size(333, 40)
        Me.lblFilesPath.TabIndex = 50
        '
        'btnSelectFilePath
        '
        Me.btnSelectFilePath.Location = New System.Drawing.Point(352, 132)
        Me.btnSelectFilePath.Name = "btnSelectFilePath"
        Me.btnSelectFilePath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectFilePath.TabIndex = 51
        Me.btnSelectFilePath.Text = "Select"
        Me.btnSelectFilePath.UseVisualStyleBackColor = True
        '
        'lblFiles
        '
        Me.lblFiles.AutoSize = True
        Me.lblFiles.Location = New System.Drawing.Point(351, 73)
        Me.lblFiles.Name = "lblFiles"
        Me.lblFiles.Size = New System.Drawing.Size(98, 13)
        Me.lblFiles.TabIndex = 49
        Me.lblFiles.Text = "Deployment Folder:"
        '
        'lblRootDebugFolderPath
        '
        Me.lblRootDebugFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRootDebugFolderPath.Location = New System.Drawing.Point(349, 362)
        Me.lblRootDebugFolderPath.Name = "lblRootDebugFolderPath"
        Me.lblRootDebugFolderPath.Size = New System.Drawing.Size(333, 40)
        Me.lblRootDebugFolderPath.TabIndex = 59
        '
        'btnSelectRootDebugPath
        '
        Me.btnSelectRootDebugPath.Location = New System.Drawing.Point(352, 405)
        Me.btnSelectRootDebugPath.Name = "btnSelectRootDebugPath"
        Me.btnSelectRootDebugPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectRootDebugPath.TabIndex = 60
        Me.btnSelectRootDebugPath.Text = "Select"
        Me.btnSelectRootDebugPath.UseVisualStyleBackColor = True
        '
        'lblRootDebugFolder
        '
        Me.lblRootDebugFolder.AutoSize = True
        Me.lblRootDebugFolder.Location = New System.Drawing.Point(349, 346)
        Me.lblRootDebugFolder.Name = "lblRootDebugFolder"
        Me.lblRootDebugFolder.Size = New System.Drawing.Size(142, 13)
        Me.lblRootDebugFolder.TabIndex = 58
        Me.lblRootDebugFolder.Text = "EVEIPH Root Debug Folder:"
        '
        'lblWorkingFolderPath
        '
        Me.lblWorkingFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblWorkingFolderPath.Location = New System.Drawing.Point(351, 270)
        Me.lblWorkingFolderPath.Name = "lblWorkingFolderPath"
        Me.lblWorkingFolderPath.Size = New System.Drawing.Size(333, 40)
        Me.lblWorkingFolderPath.TabIndex = 56
        '
        'btnSelectWorkingPath
        '
        Me.btnSelectWorkingPath.Location = New System.Drawing.Point(352, 313)
        Me.btnSelectWorkingPath.Name = "btnSelectWorkingPath"
        Me.btnSelectWorkingPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectWorkingPath.TabIndex = 57
        Me.btnSelectWorkingPath.Text = "Select"
        Me.btnSelectWorkingPath.UseVisualStyleBackColor = True
        '
        'lblWorkingFolder
        '
        Me.lblWorkingFolder.AutoSize = True
        Me.lblWorkingFolder.Location = New System.Drawing.Point(351, 254)
        Me.lblWorkingFolder.Name = "lblWorkingFolder"
        Me.lblWorkingFolder.Size = New System.Drawing.Size(107, 13)
        Me.lblWorkingFolder.TabIndex = 55
        Me.lblWorkingFolder.Text = "SDE Working Folder:"
        '
        'btnRefreshList
        '
        Me.btnRefreshList.Location = New System.Drawing.Point(235, 151)
        Me.btnRefreshList.Name = "btnRefreshList"
        Me.btnRefreshList.Size = New System.Drawing.Size(97, 40)
        Me.btnRefreshList.TabIndex = 44
        Me.btnRefreshList.Text = "Refresh List"
        Me.btnRefreshList.UseVisualStyleBackColor = True
        '
        'lblDBNameDisplay1
        '
        Me.lblDBNameDisplay1.Location = New System.Drawing.Point(19, 19)
        Me.lblDBNameDisplay1.Name = "lblDBNameDisplay1"
        Me.lblDBNameDisplay1.Size = New System.Drawing.Size(54, 32)
        Me.lblDBNameDisplay1.TabIndex = 34
        Me.lblDBNameDisplay1.Text = "Database Name:"
        Me.lblDBNameDisplay1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnCopyFilesBuildXML
        '
        Me.btnCopyFilesBuildXML.Location = New System.Drawing.Point(19, 151)
        Me.btnCopyFilesBuildXML.Name = "btnCopyFilesBuildXML"
        Me.btnCopyFilesBuildXML.Size = New System.Drawing.Size(97, 40)
        Me.btnCopyFilesBuildXML.TabIndex = 41
        Me.btnCopyFilesBuildXML.Text = "Update Files for Export"
        Me.btnCopyFilesBuildXML.UseVisualStyleBackColor = True
        '
        'btnBuildBinary
        '
        Me.btnBuildBinary.Location = New System.Drawing.Point(127, 151)
        Me.btnBuildBinary.Name = "btnBuildBinary"
        Me.btnBuildBinary.Size = New System.Drawing.Size(97, 40)
        Me.btnBuildBinary.TabIndex = 42
        Me.btnBuildBinary.Text = "Build Binary"
        Me.btnBuildBinary.UseVisualStyleBackColor = True
        '
        'lstFileInformation
        '
        Me.lstFileInformation.FullRowSelect = True
        Me.lstFileInformation.GridLines = True
        Me.lstFileInformation.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstFileInformation.HideSelection = False
        Me.lstFileInformation.Location = New System.Drawing.Point(19, 195)
        Me.lstFileInformation.Name = "lstFileInformation"
        Me.lstFileInformation.Size = New System.Drawing.Size(313, 233)
        Me.lstFileInformation.TabIndex = 43
        Me.lstFileInformation.UseCompatibleStateImageBehavior = False
        Me.lstFileInformation.View = System.Windows.Forms.View.Details
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(235, 54)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(97, 40)
        Me.btnExit.TabIndex = 38
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'pgMain
        '
        Me.pgMain.Location = New System.Drawing.Point(19, 104)
        Me.pgMain.Name = "pgMain"
        Me.pgMain.Size = New System.Drawing.Size(313, 18)
        Me.pgMain.TabIndex = 39
        Me.pgMain.Visible = False
        '
        'lblTableName
        '
        Me.lblTableName.Location = New System.Drawing.Point(19, 128)
        Me.lblTableName.Name = "lblTableName"
        Me.lblTableName.Size = New System.Drawing.Size(313, 18)
        Me.lblTableName.TabIndex = 40
        Me.lblTableName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnBuildDatabase
        '
        Me.btnBuildDatabase.Location = New System.Drawing.Point(19, 54)
        Me.btnBuildDatabase.Name = "btnBuildDatabase"
        Me.btnBuildDatabase.Size = New System.Drawing.Size(97, 40)
        Me.btnBuildDatabase.TabIndex = 36
        Me.btnBuildDatabase.Text = "Build DB"
        Me.btnBuildDatabase.UseVisualStyleBackColor = True
        '
        'btnImageCopy
        '
        Me.btnImageCopy.Location = New System.Drawing.Point(127, 54)
        Me.btnImageCopy.Name = "btnImageCopy"
        Me.btnImageCopy.Size = New System.Drawing.Size(97, 40)
        Me.btnImageCopy.TabIndex = 37
        Me.btnImageCopy.Text = "Image Copy"
        Me.btnImageCopy.UseVisualStyleBackColor = True
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(703, 446)
        Me.Controls.Add(Me.txtVersionNumber)
        Me.Controls.Add(Me.txtDBName)
        Me.Controls.Add(Me.lblMSIInstaller)
        Me.Controls.Add(Me.btnSelectTestFilePath)
        Me.Controls.Add(Me.lblMSIFolder)
        Me.Controls.Add(Me.lblVersionNumber)
        Me.Controls.Add(Me.btnSaveFilePath)
        Me.Controls.Add(Me.lblFilesPath)
        Me.Controls.Add(Me.btnSelectFilePath)
        Me.Controls.Add(Me.lblFiles)
        Me.Controls.Add(Me.lblRootDebugFolderPath)
        Me.Controls.Add(Me.btnSelectRootDebugPath)
        Me.Controls.Add(Me.lblRootDebugFolder)
        Me.Controls.Add(Me.lblWorkingFolderPath)
        Me.Controls.Add(Me.btnSelectWorkingPath)
        Me.Controls.Add(Me.lblWorkingFolder)
        Me.Controls.Add(Me.btnRefreshList)
        Me.Controls.Add(Me.lblDBNameDisplay1)
        Me.Controls.Add(Me.btnCopyFilesBuildXML)
        Me.Controls.Add(Me.btnBuildBinary)
        Me.Controls.Add(Me.lstFileInformation)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.pgMain)
        Me.Controls.Add(Me.lblTableName)
        Me.Controls.Add(Me.btnBuildDatabase)
        Me.Controls.Add(Me.btnImageCopy)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EVE IPH Deployment Program"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents txtVersionNumber As TextBox
    Friend WithEvents txtDBName As TextBox
    Friend WithEvents lblMSIInstaller As Label
    Friend WithEvents btnSelectTestFilePath As Button
    Friend WithEvents lblMSIFolder As Label
    Friend WithEvents lblVersionNumber As Label
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
    Friend WithEvents btnRefreshList As Button
    Friend WithEvents lblDBNameDisplay1 As Label
    Friend WithEvents btnCopyFilesBuildXML As Button
    Friend WithEvents btnBuildBinary As Button
    Friend WithEvents lstFileInformation As ListView
    Friend WithEvents btnExit As Button
    Friend WithEvents pgMain As ProgressBar
    Friend WithEvents lblTableName As Label
    Friend WithEvents btnBuildDatabase As Button
    Friend WithEvents btnImageCopy As Button
End Class
