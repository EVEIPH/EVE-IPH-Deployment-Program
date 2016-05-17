
Imports System.Data.SQLite
Imports System.IO
Imports YamlDotNet.Serialization


Public Class YAMLBlueprints

    Private Const ActivityManufacturing As Integer = 1
    Private Const ActivityResearchTime As Integer = 3
    Private Const ActivityResearchMaterial As Integer = 4
    Private Const ActivityCopying As Integer = 5
    Private Const ActivityReverseEngineering As Integer = 7
    Private Const ActivityInvention As Integer = 8

    Private UpdateDB As New SQLiteConnection

    Public Sub New(ByRef DBRef As SQLiteConnection)
        UpdateDB = DBRef
    End Sub

    Public Sub ImportFile(FilePath As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0

        Dim Deserializer = New Deserializer() ' Don't use a naming convention
        ' Read the file into a string reader (file returns a string)
        Dim InputText = New StringReader(File.ReadAllText(FilePath))
        ' Now parse the input text
        Dim Blueprints = Deserializer.Deserialize(Of Dictionary(Of Integer, Blueprint))(InputText)

        ' Update form
        PGRef.Value = 0
        PGRef.Maximum = Blueprints.Count
        PGRef.Visible = True

        ' Build the tables we will insert first
        Call BuildIndustryTables()

        ' Now loop through the data imported and insert records into the database
        For Each BP In Blueprints

            With BP.Value
                ' Insert this industryBlueprints record
                Call ExecuteSQLiteSQL("INSERT INTO industryBlueprints VALUES (" & .blueprintTypeID & "," & .maxProductionLimit & ")")

                ' Now upload activities
                If Not IsNothing(.activities.copying) Then
                    Call ExecuteSQLiteSQL("INSERT INTO industryActivities VALUES (" & .blueprintTypeID & "," & ActivityCopying & "," & .activities.copying.time & ")")
                    Call InsertMaterials(.blueprintTypeID, .activities.copying.materials, ActivityCopying)
                    Call InsertSkills(.blueprintTypeID, .activities.copying.skills, ActivityCopying)
                    Call InsertProducts(.blueprintTypeID, .activities.copying.products, ActivityCopying)
                End If

                If Not IsNothing(.activities.invention) Then
                    Call ExecuteSQLiteSQL("INSERT INTO industryActivities VALUES (" & .blueprintTypeID & "," & ActivityInvention & "," & .activities.invention.time & ")")
                    Call InsertMaterials(.blueprintTypeID, .activities.invention.materials, ActivityInvention)
                    Call InsertSkills(.blueprintTypeID, .activities.invention.skills, ActivityInvention)
                    Call InsertProducts(.blueprintTypeID, .activities.invention.products, ActivityInvention)
                End If

                If Not IsNothing(.activities.manufacturing) Then
                    Call ExecuteSQLiteSQL("INSERT INTO industryActivities VALUES (" & .blueprintTypeID & "," & ActivityManufacturing & "," & .activities.manufacturing.time & ")")
                    Call InsertMaterials(.blueprintTypeID, .activities.manufacturing.materials, ActivityManufacturing)
                    Call InsertSkills(.blueprintTypeID, .activities.manufacturing.skills, ActivityManufacturing)
                    Call InsertProducts(.blueprintTypeID, .activities.manufacturing.products, ActivityManufacturing)
                End If

                If Not IsNothing(.activities.research_material) Then
                    Call ExecuteSQLiteSQL("INSERT INTO industryActivities VALUES (" & .blueprintTypeID & "," & ActivityResearchMaterial & "," & .activities.research_material.time & ")")
                    Call InsertMaterials(.blueprintTypeID, .activities.research_material.materials, ActivityResearchMaterial)
                    Call InsertSkills(.blueprintTypeID, .activities.research_material.skills, ActivityResearchMaterial)
                    Call InsertProducts(.blueprintTypeID, .activities.research_material.products, ActivityResearchMaterial)
                End If

                If Not IsNothing(.activities.research_time) Then
                    Call ExecuteSQLiteSQL("INSERT INTO industryActivities VALUES (" & .blueprintTypeID & "," & ActivityResearchTime & "," & .activities.research_time.time & ")")
                    Call InsertMaterials(.blueprintTypeID, .activities.research_time.materials, ActivityResearchTime)
                    Call InsertSkills(.blueprintTypeID, .activities.research_time.skills, ActivityResearchTime)
                    Call InsertProducts(.blueprintTypeID, .activities.research_time.products, ActivityResearchTime)
                End If

                Count += 1
                PGRef.Value = Count
                LabelRef.Text = "Saving BP:  " & CStr(.blueprintTypeID)
                Application.DoEvents()
            End With
        Next

        '' Now that this is all imported, check the industryActivityMaterials table for activites that aren't in industryActivityProducts and insert
        '' setting the productID = blueprintID. This is so we can get materials for ME/TE and copying - ie, skills are needed to do ME/TE and copying, no mats then no activity possible

        '' Pull distinct bps and activities from materials
        'msSQL = "SELECT distinct blueprintTypeID, activityID FROM industryActivityMaterials"
        'msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        'msSQLReader = msSQLQuery.ExecuteReader()

        'While msSQLReader.Read
        '    ' Check each one and see if there is the bp with that activity, if not add the record
        '    msSQL2 = "SELECT 'X' FROM industryActivityProducts WHERE blueprintTypeID = " & msSQLReader.GetInt64(0) & " AND activityID = " & msSQLReader.GetInt32(1)
        '    mySQLQuery2 = New SqlCommand(msSQL2, SQLExpressConnection2)
        '    mySQLReader2 = mySQLQuery2.ExecuteReader()

        '    If Not mySQLReader2.Read Then
        '        ' Need to add this record - productTypeID is the blueprintTypeID
        '        SQL = "INSERT INTO industryActivityProducts VALUES (" & msSQLReader.GetInt64(0) & "," & msSQLReader.GetInt32(1) & ","
        '        SQL = SQL & msSQLReader.GetInt64(0) & ",1,1)"
        '        Call ExecuteSQLiteSQL(SQL)
        '    End If

        '    mySQLReader2.Close()
        '    mySQLReader2 = Nothing
        '    mySQLQuery2 = Nothing
        'End While


        '' For all invention jobs, we need a BPC or Relic BPC to invent the blueprint so add the BPC to the list of materials
        'msSQL = "SELECT blueprintTypeID FROM industryActivityMaterials WHERE activityID = 8 GROUP BY blueprintTypeID"
        'msSQLQuery = New SqlCommand(msSQL, SQLExpressConnection)
        'msSQLReader = msSQLQuery.ExecuteReader()

        'While msSQLReader.Read
        '    ' Insert each record as a consumable with the same material type id as the bptypeid
        '    SQL = "INSERT INTO industryActivityMaterials VALUES (" & msSQLReader.GetInt64(0) & ",8,"
        '    SQL = SQL & msSQLReader.GetInt64(0) & ",1,1)"
        '    Call ExecuteSQLiteSQL(SQL)
        'End While


        '' Put all the funky data fixes here

        '' Fix a few YAML data issues
        'Call ExecuteSQLiteSQL("UPDATE industryActivityMaterials SET materialTypeID = 11467 WHERE blueprintTypeID = 12613 AND activityID = 5 AND materialTypeID = 11879")


        '' Temp fixes for Citadel data 
        '' Capital Capacitor Battery - assigns correct productTypeID for invention
        'Call ExecuteSQLiteSQL("UPDATE industryActivityProducts SET productTypeID = 41641 where blueprintTypeID = 41639 and activityID = 8")
        '' Capital Capacitor Booster II Blueprint - assigns correct productTyepID (was Heavy capacitor booster II)
        'Call ExecuteSQLiteSQL("UPDATE industryActivityProducts SET productTypeID = 41493 WHERE blueprintTypeID = 41646 AND activityID = 1")

        '' Fix for Citadel data - Cap Emergency Energizer using capital fernite armor plates when they should be regular
        'Call ExecuteSQLiteSQL("UPDATE industryActivityMaterials SET materialTypeID = 11542 WHERE blueprintTypeID = 40722 AND materialTypeID = 29049")
        '' Fix for Capital Shield Booster II - this record ends up deleting the entire item in all_blueprint_materials
        'Call ExecuteSQLiteSQL("DELETE FROM industryActivityMaterials WHERE blueprintTypeID = 41634 AND materialTypeID = 41507")

        LabelRef.Text = ""
        PGRef.Visible = False

    End Sub

    ' Builds all the tables to insert blueprint data into. Includes the following tables:
    ' - industryBlueprints
    ' - industryActivities
    ' - industryActivityMaterials
    ' - industryActivityProducts

    Private Sub BuildIndustryTables()
        Dim SQL As String = ""

        ' industryBlueprints
        Call ResetTable("industryBlueprints")
        ' Build table
        SQL = "CREATE TABLE industryBlueprints (blueprintTypeID bigint NOT NULL PRIMARY KEY, maxProductionLimit bigint NOT NULL)"
        Call ExecuteSQLiteSQL(SQL)

        ' industryActivities
        Call ResetTable("industryActivities")
        ' Build table
        SQL = "CREATE TABLE industryActivities (blueprintTypeID bigint NOT NULL, activityID int NOT NULL, time int NOT NULL, "
        SQL = SQL & "PRIMARY KEY (blueprintTypeID, activityID))"
        Call ExecuteSQLiteSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_activityID ON industryActivities (activityID)"
        Call ExecuteSQLiteSQL(SQL)

        ' industryActivityMaterials (mats and skills)
        Call ResetTable("industryActivityMaterials")
        ' Build table
        SQL = "CREATE TABLE industryActivityMaterials (blueprintTypeID bigint NOT NULL, activityID int NOT NULL, materialTypeID bigint NOT NULL, "
        SQL = SQL & "quantity bigint NOT NULL, consume tinyint NOT NULL)"
        Call ExecuteSQLiteSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_BPIDactivityID1 ON industryActivityMaterials (blueprintTypeID, activityID)"
        Call ExecuteSQLiteSQL(SQL)

        ' industryActivityProducts 
        Call ResetTable("industryActivityProducts")
        ' Build table
        SQL = "CREATE TABLE industryActivityProducts (blueprintTypeID bigint NOT NULL, activityID int NOT NULL, productTypeID bigint NOT NULL, "
        SQL = SQL & "quantity bigint NOT NULL, probability float NOT NULL)"
        Call ExecuteSQLiteSQL(SQL)
        ' Create index
        SQL = "CREATE INDEX IDX_BPIDactivityID2 ON industryActivityProducts (blueprintTypeID, activityID)"
        Call ExecuteSQLiteSQL(SQL)

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

    Private Sub InsertMaterials(ByVal BPID As Long, ByVal Materials As List(Of Blueprint.Material), ByVal ActivityID As Integer)
        Dim SQL As String

        If Not IsNothing(Materials) Then
            For Each mat In Materials
                ' Insert material/skill record into industryActivityMaterials
                SQL = "INSERT INTO industryActivityMaterials VALUES (" & CStr(BPID) & "," & CStr(ActivityID) & ","
                SQL = SQL & CStr(mat.typeID) & "," & CStr(mat.quantity) & ",1)" ' Mats are always consumed
                Call ExecuteSQLiteSQL(SQL)
            Next
        End If

    End Sub

    Private Sub InsertSkills(ByVal BPID As Long, ByVal Skills As List(Of Blueprint.Skill), ByVal ActivityID As Integer)
        Dim SQL As String

        If Not IsNothing(Skills) Then
            For Each skill In Skills
                ' Insert material/skill record into industryActivityMaterials
                SQL = "INSERT INTO industryActivityMaterials VALUES (" & CStr(BPID) & "," & CStr(ActivityID) & ","
                SQL = SQL & CStr(skill.typeID) & "," & CStr(skill.level) & ",0)" ' Skills aren't consumed
                Call ExecuteSQLiteSQL(SQL)
            Next
        End If

    End Sub

    Private Sub InsertProducts(ByVal BPID As Long, ByVal Products As List(Of Blueprint.Product), ByVal ActivityID As Integer)
        Dim SQL As String

        If Not IsNothing(Products) Then
            For Each prod In Products
                ' Insert material record into industryActivityProducts
                SQL = "INSERT INTO industryActivityProducts VALUES (" & CStr(BPID) & "," & CStr(ActivityID) & ","
                SQL = SQL & CStr(prod.typeID) & "," & CStr(prod.quantity) & "," & CStr(prod.probability) & ")"
                Call ExecuteSQLiteSQL(SQL)
            Next
        End If

    End Sub

    Public Sub ExecuteSQLiteSQL(ByVal SQL As String)
        Dim DBExecuteCmd As SQLiteCommand

        DBExecuteCmd = UpdateDB.CreateCommand
        DBExecuteCmd.CommandText = SQL
        DBExecuteCmd.ExecuteNonQuery()

        DBExecuteCmd.Dispose()

    End Sub

End Class

' Class to parse the blueprints.yaml file
' I assume the property names need to be exactly the same as the names in the YAML file
Public Class Blueprint

    Public Property activities As BlueprintActivities
    Public Property blueprintTypeID As Long
    Public Property maxProductionLimit As Long

    Public Class BlueprintActivities
        Public Property copying As CopyingActivity
        Public Property invention As InventionActivity
        Public Property manufacturing As ManufacturingActivity
        Public Property research_material As ResearchMaterialActivity
        Public Property research_time As ResearchTimeActivity
    End Class

    Public Class CopyingActivity
        Public Property materials As List(Of Material)
        Public Property products As List(Of Product)
        Public Property skills As List(Of Skill)
        Public Property time As Long
    End Class

    Public Class InventionActivity
        Public Property materials As List(Of Material)
        Public Property products As List(Of Product)
        Public Property skills As List(Of Skill)
        Public Property time As Long
    End Class

    Public Class ManufacturingActivity
        Public Property materials As List(Of Material)
        Public Property products As List(Of Product)
        Public Property skills As List(Of Skill)
        Public Property time As Long
    End Class

    Public Class ResearchMaterialActivity
        Public Property materials As List(Of Material)
        Public Property products As List(Of Product)
        Public Property skills As List(Of Skill)
        Public Property time As Long
    End Class

    Public Class ResearchTimeActivity
        Public Property materials As List(Of Material)
        Public Property products As List(Of Product)
        Public Property skills As List(Of Skill)
        Public Property time As Long
    End Class

    Public Class Material
        Public Property quantity As Long
        Public Property typeID As Long
    End Class

    Public Class Skill
        Public Property level As Integer
        Public Property typeID As Long
    End Class

    Public Class Product
        Public Property probability As Double
        Public Property quantity As Long
        Public Property typeID As Long
    End Class

End Class
