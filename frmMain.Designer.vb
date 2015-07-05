<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.lblDBNameDisplay1 = New System.Windows.Forms.Label()
        Me.lblDBNameDisplay = New System.Windows.Forms.Label()
        Me.btnCopyFilesBuildXML = New System.Windows.Forms.Button()
        Me.btnBuildBinary = New System.Windows.Forms.Button()
        Me.lstFileInformation = New System.Windows.Forms.ListView()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.pgMain = New System.Windows.Forms.ProgressBar()
        Me.btnBuildSQLServerDB = New System.Windows.Forms.Button()
        Me.lblTableName = New System.Windows.Forms.Label()
        Me.btnBuildDatabase = New System.Windows.Forms.Button()
        Me.btnImageCopy = New System.Windows.Forms.Button()
        Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.lblImageVersion = New System.Windows.Forms.Label()
        Me.txtImageVersion = New System.Windows.Forms.TextBox()
        Me.TreeView = New System.Windows.Forms.TreeView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtVersionNumber = New System.Windows.Forms.TextBox()
        Me.lblDBName = New System.Windows.Forms.Label()
        Me.txtDBName = New System.Windows.Forms.TextBox()
        Me.btnSaveFilePath = New System.Windows.Forms.Button()
        Me.lblMediaFirePath = New System.Windows.Forms.Label()
        Me.btnSelectInstallerBinaryPath = New System.Windows.Forms.Button()
        Me.lblMediaFire = New System.Windows.Forms.Label()
        Me.lblRootDebugFolderPath = New System.Windows.Forms.Label()
        Me.btnSelectRootDebugPath = New System.Windows.Forms.Button()
        Me.lblRootDebugFolder = New System.Windows.Forms.Label()
        Me.lblWorkingFolderPath = New System.Windows.Forms.Label()
        Me.btnSelectDBImagesPath = New System.Windows.Forms.Button()
        Me.lblWorkingFolder = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(1, 2)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(471, 378)
        Me.TabControl1.TabIndex = 19
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.lblDBNameDisplay1)
        Me.TabPage1.Controls.Add(Me.lblDBNameDisplay)
        Me.TabPage1.Controls.Add(Me.btnCopyFilesBuildXML)
        Me.TabPage1.Controls.Add(Me.btnBuildBinary)
        Me.TabPage1.Controls.Add(Me.lstFileInformation)
        Me.TabPage1.Controls.Add(Me.btnExit)
        Me.TabPage1.Controls.Add(Me.pgMain)
        Me.TabPage1.Controls.Add(Me.btnBuildSQLServerDB)
        Me.TabPage1.Controls.Add(Me.lblTableName)
        Me.TabPage1.Controls.Add(Me.btnBuildDatabase)
        Me.TabPage1.Controls.Add(Me.btnImageCopy)
        Me.TabPage1.Controls.Add(Me.ShapeContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(463, 352)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "DB Updater & Deployment"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'lblDBNameDisplay1
        '
        Me.lblDBNameDisplay1.AutoSize = True
        Me.lblDBNameDisplay1.Location = New System.Drawing.Point(76, 19)
        Me.lblDBNameDisplay1.Name = "lblDBNameDisplay1"
        Me.lblDBNameDisplay1.Size = New System.Drawing.Size(87, 13)
        Me.lblDBNameDisplay1.TabIndex = 1
        Me.lblDBNameDisplay1.Text = "Database Name:"
        '
        'lblDBNameDisplay
        '
        Me.lblDBNameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblDBNameDisplay.Location = New System.Drawing.Point(169, 15)
        Me.lblDBNameDisplay.Name = "lblDBNameDisplay"
        Me.lblDBNameDisplay.Size = New System.Drawing.Size(217, 20)
        Me.lblDBNameDisplay.TabIndex = 2
        Me.lblDBNameDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnCopyFilesBuildXML
        '
        Me.btnCopyFilesBuildXML.Location = New System.Drawing.Point(20, 157)
        Me.btnCopyFilesBuildXML.Name = "btnCopyFilesBuildXML"
        Me.btnCopyFilesBuildXML.Size = New System.Drawing.Size(97, 40)
        Me.btnCopyFilesBuildXML.TabIndex = 9
        Me.btnCopyFilesBuildXML.Text = "Update Files for Export"
        Me.btnCopyFilesBuildXML.UseVisualStyleBackColor = True
        '
        'btnBuildBinary
        '
        Me.btnBuildBinary.Location = New System.Drawing.Point(128, 157)
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
        Me.lstFileInformation.Location = New System.Drawing.Point(20, 205)
        Me.lstFileInformation.Name = "lstFileInformation"
        Me.lstFileInformation.Size = New System.Drawing.Size(421, 130)
        Me.lstFileInformation.TabIndex = 11
        Me.lstFileInformation.UseCompatibleStateImageBehavior = False
        Me.lstFileInformation.View = System.Windows.Forms.View.Details
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(344, 43)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(97, 40)
        Me.btnExit.TabIndex = 6
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'pgMain
        '
        Me.pgMain.Location = New System.Drawing.Point(20, 93)
        Me.pgMain.Name = "pgMain"
        Me.pgMain.Size = New System.Drawing.Size(421, 18)
        Me.pgMain.TabIndex = 7
        Me.pgMain.Visible = False
        '
        'btnBuildSQLServerDB
        '
        Me.btnBuildSQLServerDB.Location = New System.Drawing.Point(236, 43)
        Me.btnBuildSQLServerDB.Name = "btnBuildSQLServerDB"
        Me.btnBuildSQLServerDB.Size = New System.Drawing.Size(97, 40)
        Me.btnBuildSQLServerDB.TabIndex = 5
        Me.btnBuildSQLServerDB.Text = "Update SQL Server DB"
        Me.btnBuildSQLServerDB.UseVisualStyleBackColor = True
        '
        'lblTableName
        '
        Me.lblTableName.Location = New System.Drawing.Point(20, 117)
        Me.lblTableName.Name = "lblTableName"
        Me.lblTableName.Size = New System.Drawing.Size(421, 18)
        Me.lblTableName.TabIndex = 8
        Me.lblTableName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnBuildDatabase
        '
        Me.btnBuildDatabase.Location = New System.Drawing.Point(20, 43)
        Me.btnBuildDatabase.Name = "btnBuildDatabase"
        Me.btnBuildDatabase.Size = New System.Drawing.Size(97, 40)
        Me.btnBuildDatabase.TabIndex = 3
        Me.btnBuildDatabase.Text = "Build DB"
        Me.btnBuildDatabase.UseVisualStyleBackColor = True
        '
        'btnImageCopy
        '
        Me.btnImageCopy.Location = New System.Drawing.Point(128, 43)
        Me.btnImageCopy.Name = "btnImageCopy"
        Me.btnImageCopy.Size = New System.Drawing.Size(97, 40)
        Me.btnImageCopy.TabIndex = 4
        Me.btnImageCopy.Text = "Image Copy"
        Me.btnImageCopy.UseVisualStyleBackColor = True
        '
        'ShapeContainer1
        '
        Me.ShapeContainer1.Location = New System.Drawing.Point(3, 3)
        Me.ShapeContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer1.Name = "ShapeContainer1"
        Me.ShapeContainer1.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape1})
        Me.ShapeContainer1.Size = New System.Drawing.Size(457, 346)
        Me.ShapeContainer1.TabIndex = 0
        Me.ShapeContainer1.TabStop = False
        '
        'LineShape1
        '
        Me.LineShape1.Name = "LineShape1"
        Me.LineShape1.X1 = 21
        Me.LineShape1.X2 = 435
        Me.LineShape1.Y1 = 146
        Me.LineShape1.Y2 = 146
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.lblImageVersion)
        Me.TabPage2.Controls.Add(Me.txtImageVersion)
        Me.TabPage2.Controls.Add(Me.TreeView)
        Me.TabPage2.Controls.Add(Me.Label1)
        Me.TabPage2.Controls.Add(Me.txtVersionNumber)
        Me.TabPage2.Controls.Add(Me.lblDBName)
        Me.TabPage2.Controls.Add(Me.txtDBName)
        Me.TabPage2.Controls.Add(Me.btnSaveFilePath)
        Me.TabPage2.Controls.Add(Me.lblMediaFirePath)
        Me.TabPage2.Controls.Add(Me.btnSelectInstallerBinaryPath)
        Me.TabPage2.Controls.Add(Me.lblMediaFire)
        Me.TabPage2.Controls.Add(Me.lblRootDebugFolderPath)
        Me.TabPage2.Controls.Add(Me.btnSelectRootDebugPath)
        Me.TabPage2.Controls.Add(Me.lblRootDebugFolder)
        Me.TabPage2.Controls.Add(Me.lblWorkingFolderPath)
        Me.TabPage2.Controls.Add(Me.btnSelectDBImagesPath)
        Me.TabPage2.Controls.Add(Me.lblWorkingFolder)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(463, 352)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "File Path Settings"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'lblImageVersion
        '
        Me.lblImageVersion.AutoSize = True
        Me.lblImageVersion.Location = New System.Drawing.Point(230, 20)
        Me.lblImageVersion.Name = "lblImageVersion"
        Me.lblImageVersion.Size = New System.Drawing.Size(77, 13)
        Me.lblImageVersion.TabIndex = 2
        Me.lblImageVersion.Text = "Image Version:"
        '
        'txtImageVersion
        '
        Me.txtImageVersion.Location = New System.Drawing.Point(233, 36)
        Me.txtImageVersion.Name = "txtImageVersion"
        Me.txtImageVersion.Size = New System.Drawing.Size(122, 20)
        Me.txtImageVersion.TabIndex = 3
        '
        'TreeView
        '
        Me.TreeView.Location = New System.Drawing.Point(428, 320)
        Me.TreeView.Name = "TreeView"
        Me.TreeView.Size = New System.Drawing.Size(29, 25)
        Me.TreeView.TabIndex = 16
        Me.TreeView.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(358, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Version Number:"
        '
        'txtVersionNumber
        '
        Me.txtVersionNumber.Location = New System.Drawing.Point(361, 36)
        Me.txtVersionNumber.Name = "txtVersionNumber"
        Me.txtVersionNumber.Size = New System.Drawing.Size(82, 20)
        Me.txtVersionNumber.TabIndex = 5
        '
        'lblDBName
        '
        Me.lblDBName.AutoSize = True
        Me.lblDBName.Location = New System.Drawing.Point(19, 20)
        Me.lblDBName.Name = "lblDBName"
        Me.lblDBName.Size = New System.Drawing.Size(87, 13)
        Me.lblDBName.TabIndex = 0
        Me.lblDBName.Text = "Database Name:"
        '
        'txtDBName
        '
        Me.txtDBName.Location = New System.Drawing.Point(22, 36)
        Me.txtDBName.Name = "txtDBName"
        Me.txtDBName.Size = New System.Drawing.Size(205, 20)
        Me.txtDBName.TabIndex = 1
        '
        'btnSaveFilePath
        '
        Me.btnSaveFilePath.Location = New System.Drawing.Point(183, 282)
        Me.btnSaveFilePath.Name = "btnSaveFilePath"
        Me.btnSaveFilePath.Size = New System.Drawing.Size(97, 28)
        Me.btnSaveFilePath.TabIndex = 15
        Me.btnSaveFilePath.Text = "Save Settings"
        Me.btnSaveFilePath.UseVisualStyleBackColor = True
        '
        'lblMediaFirePath
        '
        Me.lblMediaFirePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblMediaFirePath.Location = New System.Drawing.Point(22, 86)
        Me.lblMediaFirePath.Name = "lblMediaFirePath"
        Me.lblMediaFirePath.Size = New System.Drawing.Size(421, 20)
        Me.lblMediaFirePath.TabIndex = 7
        Me.lblMediaFirePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectInstallerBinaryPath
        '
        Me.btnSelectInstallerBinaryPath.Location = New System.Drawing.Point(22, 109)
        Me.btnSelectInstallerBinaryPath.Name = "btnSelectInstallerBinaryPath"
        Me.btnSelectInstallerBinaryPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectInstallerBinaryPath.TabIndex = 8
        Me.btnSelectInstallerBinaryPath.Text = "Select"
        Me.btnSelectInstallerBinaryPath.UseVisualStyleBackColor = True
        '
        'lblMediaFire
        '
        Me.lblMediaFire.AutoSize = True
        Me.lblMediaFire.Location = New System.Drawing.Point(19, 70)
        Me.lblMediaFire.Name = "lblMediaFire"
        Me.lblMediaFire.Size = New System.Drawing.Size(147, 13)
        Me.lblMediaFire.TabIndex = 6
        Me.lblMediaFire.Text = "MediaFire Deployment Folder:"
        '
        'lblRootDebugFolderPath
        '
        Me.lblRootDebugFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRootDebugFolderPath.Location = New System.Drawing.Point(22, 216)
        Me.lblRootDebugFolderPath.Name = "lblRootDebugFolderPath"
        Me.lblRootDebugFolderPath.Size = New System.Drawing.Size(421, 20)
        Me.lblRootDebugFolderPath.TabIndex = 13
        Me.lblRootDebugFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectRootDebugPath
        '
        Me.btnSelectRootDebugPath.Location = New System.Drawing.Point(22, 239)
        Me.btnSelectRootDebugPath.Name = "btnSelectRootDebugPath"
        Me.btnSelectRootDebugPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectRootDebugPath.TabIndex = 14
        Me.btnSelectRootDebugPath.Text = "Select"
        Me.btnSelectRootDebugPath.UseVisualStyleBackColor = True
        '
        'lblRootDebugFolder
        '
        Me.lblRootDebugFolder.AutoSize = True
        Me.lblRootDebugFolder.Location = New System.Drawing.Point(19, 200)
        Me.lblRootDebugFolder.Name = "lblRootDebugFolder"
        Me.lblRootDebugFolder.Size = New System.Drawing.Size(100, 13)
        Me.lblRootDebugFolder.TabIndex = 12
        Me.lblRootDebugFolder.Text = "Root Debug Folder:"
        '
        'lblWorkingFolderPath
        '
        Me.lblWorkingFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblWorkingFolderPath.Location = New System.Drawing.Point(22, 151)
        Me.lblWorkingFolderPath.Name = "lblWorkingFolderPath"
        Me.lblWorkingFolderPath.Size = New System.Drawing.Size(421, 20)
        Me.lblWorkingFolderPath.TabIndex = 10
        Me.lblWorkingFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectDBImagesPath
        '
        Me.btnSelectDBImagesPath.Location = New System.Drawing.Point(22, 174)
        Me.btnSelectDBImagesPath.Name = "btnSelectDBImagesPath"
        Me.btnSelectDBImagesPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectDBImagesPath.TabIndex = 11
        Me.btnSelectDBImagesPath.Text = "Select"
        Me.btnSelectDBImagesPath.UseVisualStyleBackColor = True
        '
        'lblWorkingFolder
        '
        Me.lblWorkingFolder.AutoSize = True
        Me.lblWorkingFolder.Location = New System.Drawing.Point(19, 135)
        Me.lblWorkingFolder.Name = "lblWorkingFolder"
        Me.lblWorkingFolder.Size = New System.Drawing.Size(82, 13)
        Me.lblWorkingFolder.TabIndex = 9
        Me.lblWorkingFolder.Text = "Working Folder:"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(471, 381)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EVE IPH Deployment Program"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents pgMain As System.Windows.Forms.ProgressBar
    Friend WithEvents btnBuildSQLServerDB As System.Windows.Forms.Button
    Friend WithEvents lblTableName As System.Windows.Forms.Label
    Friend WithEvents btnBuildDatabase As System.Windows.Forms.Button
    Friend WithEvents btnImageCopy As System.Windows.Forms.Button
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TreeView As System.Windows.Forms.TreeView
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents lblRootDebugFolderPath As System.Windows.Forms.Label
    Friend WithEvents btnSelectRootDebugPath As System.Windows.Forms.Button
    Friend WithEvents lblRootDebugFolder As System.Windows.Forms.Label
    Friend WithEvents lblWorkingFolderPath As System.Windows.Forms.Label
    Friend WithEvents btnSelectDBImagesPath As System.Windows.Forms.Button
    Friend WithEvents lblWorkingFolder As System.Windows.Forms.Label
    Friend WithEvents lblMediaFirePath As System.Windows.Forms.Label
    Friend WithEvents btnSelectInstallerBinaryPath As System.Windows.Forms.Button
    Friend WithEvents lblMediaFire As System.Windows.Forms.Label
    Friend WithEvents btnCopyFilesBuildXML As System.Windows.Forms.Button
    Friend WithEvents btnBuildBinary As System.Windows.Forms.Button
    Friend WithEvents lstFileInformation As System.Windows.Forms.ListView
    Friend WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Friend WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents btnSaveFilePath As System.Windows.Forms.Button
    Friend WithEvents lblDBNameDisplay1 As System.Windows.Forms.Label
    Friend WithEvents lblDBNameDisplay As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtVersionNumber As System.Windows.Forms.TextBox
    Friend WithEvents lblDBName As System.Windows.Forms.Label
    Friend WithEvents txtDBName As System.Windows.Forms.TextBox
    Friend WithEvents lblImageVersion As System.Windows.Forms.Label
    Friend WithEvents txtImageVersion As System.Windows.Forms.TextBox

End Class
