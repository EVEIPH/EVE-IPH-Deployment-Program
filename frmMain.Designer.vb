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
        Me.lblRootDebugFolderPath = New System.Windows.Forms.Label()
        Me.btnSelectRootDebugPath = New System.Windows.Forms.Button()
        Me.lblRootDebugFolder = New System.Windows.Forms.Label()
        Me.lblFilePathText = New System.Windows.Forms.Label()
        Me.btnSelectFilePath = New System.Windows.Forms.Button()
        Me.lblDBName = New System.Windows.Forms.Label()
        Me.lblFilePath = New System.Windows.Forms.Label()
        Me.btnSaveFilePath = New System.Windows.Forms.Button()
        Me.pgMain = New System.Windows.Forms.ProgressBar()
        Me.txtDBName = New System.Windows.Forms.TextBox()
        Me.btnBuildSQLServerDB = New System.Windows.Forms.Button()
        Me.lblTableName = New System.Windows.Forms.Label()
        Me.btnBuildDatabase = New System.Windows.Forms.Button()
        Me.btnImageCopy = New System.Windows.Forms.Button()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TreeView = New System.Windows.Forms.TreeView()
        Me.btnExit = New System.Windows.Forms.Button()
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
        Me.TabControl1.Size = New System.Drawing.Size(471, 384)
        Me.TabControl1.TabIndex = 19
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.btnExit)
        Me.TabPage1.Controls.Add(Me.lblRootDebugFolderPath)
        Me.TabPage1.Controls.Add(Me.btnSelectRootDebugPath)
        Me.TabPage1.Controls.Add(Me.lblRootDebugFolder)
        Me.TabPage1.Controls.Add(Me.lblFilePathText)
        Me.TabPage1.Controls.Add(Me.btnSelectFilePath)
        Me.TabPage1.Controls.Add(Me.lblDBName)
        Me.TabPage1.Controls.Add(Me.lblFilePath)
        Me.TabPage1.Controls.Add(Me.btnSaveFilePath)
        Me.TabPage1.Controls.Add(Me.pgMain)
        Me.TabPage1.Controls.Add(Me.txtDBName)
        Me.TabPage1.Controls.Add(Me.btnBuildSQLServerDB)
        Me.TabPage1.Controls.Add(Me.lblTableName)
        Me.TabPage1.Controls.Add(Me.btnBuildDatabase)
        Me.TabPage1.Controls.Add(Me.btnImageCopy)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(463, 358)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "DB Updater"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'lblRootDebugFolderPath
        '
        Me.lblRootDebugFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRootDebugFolderPath.Location = New System.Drawing.Point(22, 297)
        Me.lblRootDebugFolderPath.Name = "lblRootDebugFolderPath"
        Me.lblRootDebugFolderPath.Size = New System.Drawing.Size(417, 20)
        Me.lblRootDebugFolderPath.TabIndex = 33
        Me.lblRootDebugFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectRootDebugPath
        '
        Me.btnSelectRootDebugPath.Location = New System.Drawing.Point(22, 323)
        Me.btnSelectRootDebugPath.Name = "btnSelectRootDebugPath"
        Me.btnSelectRootDebugPath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectRootDebugPath.TabIndex = 32
        Me.btnSelectRootDebugPath.Text = "Select"
        Me.btnSelectRootDebugPath.UseVisualStyleBackColor = True
        '
        'lblRootDebugFolder
        '
        Me.lblRootDebugFolder.AutoSize = True
        Me.lblRootDebugFolder.Location = New System.Drawing.Point(19, 283)
        Me.lblRootDebugFolder.Name = "lblRootDebugFolder"
        Me.lblRootDebugFolder.Size = New System.Drawing.Size(100, 13)
        Me.lblRootDebugFolder.TabIndex = 31
        Me.lblRootDebugFolder.Text = "Root Debug Folder:"
        '
        'lblFilePathText
        '
        Me.lblFilePathText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblFilePathText.Location = New System.Drawing.Point(22, 218)
        Me.lblFilePathText.Name = "lblFilePathText"
        Me.lblFilePathText.Size = New System.Drawing.Size(417, 20)
        Me.lblFilePathText.TabIndex = 30
        Me.lblFilePathText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectFilePath
        '
        Me.btnSelectFilePath.Location = New System.Drawing.Point(22, 244)
        Me.btnSelectFilePath.Name = "btnSelectFilePath"
        Me.btnSelectFilePath.Size = New System.Drawing.Size(55, 23)
        Me.btnSelectFilePath.TabIndex = 29
        Me.btnSelectFilePath.Text = "Select"
        Me.btnSelectFilePath.UseVisualStyleBackColor = True
        '
        'lblDBName
        '
        Me.lblDBName.AutoSize = True
        Me.lblDBName.Location = New System.Drawing.Point(118, 8)
        Me.lblDBName.Name = "lblDBName"
        Me.lblDBName.Size = New System.Drawing.Size(87, 13)
        Me.lblDBName.TabIndex = 28
        Me.lblDBName.Text = "Database Name:"
        '
        'lblFilePath
        '
        Me.lblFilePath.AutoSize = True
        Me.lblFilePath.Location = New System.Drawing.Point(19, 204)
        Me.lblFilePath.Name = "lblFilePath"
        Me.lblFilePath.Size = New System.Drawing.Size(82, 13)
        Me.lblFilePath.TabIndex = 27
        Me.lblFilePath.Text = "Working Folder:"
        '
        'btnSaveFilePath
        '
        Me.btnSaveFilePath.Location = New System.Drawing.Point(121, 96)
        Me.btnSaveFilePath.Name = "btnSaveFilePath"
        Me.btnSaveFilePath.Size = New System.Drawing.Size(104, 38)
        Me.btnSaveFilePath.TabIndex = 26
        Me.btnSaveFilePath.Text = "Save Settings"
        Me.btnSaveFilePath.UseVisualStyleBackColor = True
        '
        'pgMain
        '
        Me.pgMain.Location = New System.Drawing.Point(22, 169)
        Me.pgMain.Name = "pgMain"
        Me.pgMain.Size = New System.Drawing.Size(421, 18)
        Me.pgMain.TabIndex = 21
        Me.pgMain.Visible = False
        '
        'txtDBName
        '
        Me.txtDBName.Location = New System.Drawing.Point(121, 24)
        Me.txtDBName.Name = "txtDBName"
        Me.txtDBName.Size = New System.Drawing.Size(227, 20)
        Me.txtDBName.TabIndex = 25
        '
        'btnBuildSQLServerDB
        '
        Me.btnBuildSQLServerDB.Location = New System.Drawing.Point(300, 50)
        Me.btnBuildSQLServerDB.Name = "btnBuildSQLServerDB"
        Me.btnBuildSQLServerDB.Size = New System.Drawing.Size(104, 40)
        Me.btnBuildSQLServerDB.TabIndex = 23
        Me.btnBuildSQLServerDB.Text = "Update SQL Server DB"
        Me.btnBuildSQLServerDB.UseVisualStyleBackColor = True
        '
        'lblTableName
        '
        Me.lblTableName.Location = New System.Drawing.Point(22, 148)
        Me.lblTableName.Name = "lblTableName"
        Me.lblTableName.Size = New System.Drawing.Size(421, 18)
        Me.lblTableName.TabIndex = 22
        Me.lblTableName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnBuildDatabase
        '
        Me.btnBuildDatabase.Location = New System.Drawing.Point(54, 50)
        Me.btnBuildDatabase.Name = "btnBuildDatabase"
        Me.btnBuildDatabase.Size = New System.Drawing.Size(104, 40)
        Me.btnBuildDatabase.TabIndex = 20
        Me.btnBuildDatabase.Text = "Build DB"
        Me.btnBuildDatabase.UseVisualStyleBackColor = True
        '
        'btnImageCopy
        '
        Me.btnImageCopy.Location = New System.Drawing.Point(177, 50)
        Me.btnImageCopy.Name = "btnImageCopy"
        Me.btnImageCopy.Size = New System.Drawing.Size(104, 40)
        Me.btnImageCopy.TabIndex = 19
        Me.btnImageCopy.Text = "Image Copy"
        Me.btnImageCopy.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.TreeView)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(463, 358)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Deployment"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TreeView
        '
        Me.TreeView.Location = New System.Drawing.Point(6, 6)
        Me.TreeView.Name = "TreeView"
        Me.TreeView.Size = New System.Drawing.Size(451, 346)
        Me.TreeView.TabIndex = 25
        Me.TreeView.Visible = False
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(244, 96)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(104, 38)
        Me.btnExit.TabIndex = 34
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(471, 388)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EVE IPH Deployment Program"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents lblRootDebugFolderPath As System.Windows.Forms.Label
    Friend WithEvents btnSelectRootDebugPath As System.Windows.Forms.Button
    Friend WithEvents lblRootDebugFolder As System.Windows.Forms.Label
    Friend WithEvents lblFilePathText As System.Windows.Forms.Label
    Friend WithEvents btnSelectFilePath As System.Windows.Forms.Button
    Friend WithEvents lblDBName As System.Windows.Forms.Label
    Friend WithEvents lblFilePath As System.Windows.Forms.Label
    Friend WithEvents btnSaveFilePath As System.Windows.Forms.Button
    Friend WithEvents pgMain As System.Windows.Forms.ProgressBar
    Friend WithEvents txtDBName As System.Windows.Forms.TextBox
    Friend WithEvents btnBuildSQLServerDB As System.Windows.Forms.Button
    Friend WithEvents lblTableName As System.Windows.Forms.Label
    Friend WithEvents btnBuildDatabase As System.Windows.Forms.Button
    Friend WithEvents btnImageCopy As System.Windows.Forms.Button
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TreeView As System.Windows.Forms.TreeView
    Friend WithEvents btnExit As System.Windows.Forms.Button

End Class
