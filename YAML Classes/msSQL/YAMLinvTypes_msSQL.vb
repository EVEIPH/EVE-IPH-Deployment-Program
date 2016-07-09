
Imports YamlDotNet.RepresentationModel

Public Class YAMLinvTypes_msSQL

    Private UpdateDB As msSQLDBConnection ' Reference of the DB we want to update - opened within the class
    Private TRN As YAMLTranslations_msSQL
    Private YAMLTypeIDs As YAMLDocument

    Public Sub New(ByRef DBRef As String, Optional InstanceName As String = "")
        UpdateDB = New msSQLDBConnection(DBRef, InstanceName)
        TRN = New YAMLTranslations_msSQL(DBRef, InstanceName)
    End Sub

    Protected Overrides Sub Finalize()
        UpdateDB.CloseDB()
        UpdateDB = Nothing
        MyBase.Finalize()
    End Sub

    Public Sub ImportFile(FilePath As String, ByRef LabelRef As Label, ByRef PGRef As ProgressBar)
        Dim SQL As String = ""
        Dim Count As Long = 0

        LabelRef.Text = "Reading typeIDs.yaml"
        Application.UseWaitCursor = True
        Application.DoEvents()

        ' Build tables
        Call BuildImportTables()

        ' Get the YAML file - typeIDs is a mapping node root
        YAMLTypeIDs = New YAMLDocument(FilePath)

        ' This is a mapped node file
        Dim TypeIDData As YamlMappingNode = YAMLTypeIDs.GetFileMappingNode
        Dim TypeID As String = "" ' Save so we can pass to other functions

        Dim CheckString As String = ""

        ' Update form
        PGRef.Value = 0
        PGRef.Maximum = TypeIDData.Count
        PGRef.Visible = True

        For Each DataNode In TypeIDData.Children
            ' Get the main TypeID first
            TypeID = DataNode.Key.ToString

            SQL = "INSERT INTO invTypes VALUES ("
            SQL = SQL & TypeID & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("groupID", DataNode) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(TRN.InsertTranslationData(TypeID, YAMLTypeIDs.GetNamedMappingNode("name", DataNode), "name", "invTypes", "en")) & "," ' Load translation data and get english typeName
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(TRN.InsertTranslationData(TypeID, YAMLTypeIDs.GetNamedMappingNode("description", DataNode), "description", "invTypes", "en")) & "," ' Load translation data and get english description
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("mass", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("volume", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("capacity", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("portionSize", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("factionID", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("raceID", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("baseprice", DataNode) & ","
            SQL = SQL & CInt(CBool(YAMLTypeIDs.GetSQLMappingScalarValue("published", DataNode))) & "," ' convert to a number
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("marketGroupID", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("chanceOfDuplicating", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("graphicID", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("radius", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("iconID", DataNode) & ","
            SQL = SQL & YAMLTypeIDs.GetSQLMappingScalarValue("soundID", DataNode) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTypeIDs.GetSQLMappingScalarValue("sofFactionName", DataNode)) & ","
            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(YAMLTypeIDs.GetSQLMappingScalarValue("sofDnaAddition", DataNode)) & ")"

            ' Load the traits data
            Call LoadTypeTraits(TypeID, YAMLTypeIDs.GetNamedMappingNode("traits", DataNode))

            ' Load the mastery data
            Call LoadTypeMasteries(TypeID, YAMLTypeIDs.GetNamedMappingNode("masteries", DataNode))

            UpdateDB.ExecuteNonQuerySQL(SQL)

            Count += 1
            PGRef.Value = Count
            LabelRef.Text = "Saving typeID:  " & TypeID
            Application.DoEvents()

        Next

        Application.UseWaitCursor = False
        Application.DoEvents()

    End Sub

    ' Loads the traits associated with this type ID from YAML record
    Private Sub LoadTypeTraits(TypeID As String, TraitTypesNode As YamlMappingNode)
        Dim SQL As String = ""
        Dim Traits As YamlSequenceNode
        Dim SkillTraits As YamlMappingNode
        Dim DataNode As YamlMappingNode
        Dim TraitName As String = ""

        If Not IsNothing(TraitTypesNode) Then
            ' First node is skill type
            For Each Trait In TraitTypesNode
                TraitName = Trait.Key.ToString

                If TraitName = "types" Then
                    ' Skill TypeIDs - reach each skill
                    SkillTraits = TraitTypesNode.Children(Trait.Key)
                    For Each SkillTypeID In SkillTraits
                        Traits = SkillTraits.Children(SkillTypeID.Key)
                        ' read through each bonus
                        For Each DataNode In Traits
                            SQL = "INSERT INTO invTraits VALUES (" & TypeID & "," & SkillTypeID.Key.ToString & ","
                            SQL = SQL & YAMLTypeIDs.GetSQLScalarValueFromMapping("bonus", DataNode) & ","
                            SQL = SQL & UpdateDB.BuildSQLInsertStringValue(TRN.InsertTranslationData(TypeID, YAMLTypeIDs.GetNamedMappingNode("bonusText", DataNode),
                                                                           "bonusText", "invTraits", "en")) & "," ' Load translation data and get english typeName
                            SQL = SQL & YAMLTypeIDs.GetSQLScalarValueFromMapping("importance", DataNode) & ","
                            SQL = SQL & YAMLTypeIDs.GetSQLScalarValueFromMapping("unitID", DataNode) & ")"

                            Call UpdateDB.ExecuteNonQuerySQL(SQL)
                        Next
                    Next
                Else
                    Dim SkillTypeID As String
                    ' For the skilltypeID use -1 for role bonuses
                    If TraitName = "roleBonuses" Then
                        SkillTypeID = "-1"
                    Else
                        SkillTypeID = "-2"
                    End If

                    ' Read in all the traits for these bonuses
                    Traits = TraitTypesNode.Children(Trait.Key)
                    ' read through each bonus
                    For Each DataNode In Traits
                        SQL = "INSERT INTO invTraits VALUES (" & TypeID & "," & SkillTypeID & ","
                        SQL = SQL & YAMLTypeIDs.GetSQLScalarValueFromMapping("bonus", DataNode) & ","
                        SQL = SQL & UpdateDB.BuildSQLInsertStringValue(TRN.InsertTranslationData(TypeID, YAMLTypeIDs.GetNamedMappingNode("bonusText", DataNode),
                                                                           "bonusText", "invTraits", "en")) & "," ' Load translation data and get english typeName
                        SQL = SQL & YAMLTypeIDs.GetSQLScalarValueFromMapping("importance", DataNode) & ","
                        SQL = SQL & YAMLTypeIDs.GetSQLScalarValueFromMapping("unitID", DataNode) & ")"

                        Call UpdateDB.ExecuteNonQuerySQL(SQL)
                    Next
                End If
            Next
        End If

        Application.DoEvents()
    End Sub

    ' Loads the mastery levels and certifications for this type ID
    Private Sub LoadTypeMasteries(TypeID As String, MasteryNode As YamlMappingNode)
        Dim SQLBase As String = ""
        Dim SQL As String = ""
        Dim Masteries As YamlSequenceNode
        Dim Mastery As YamlScalarNode

        If Not IsNothing(MasteryNode) Then
            ' Basically two loops, one for the level and the other for cert ids
            For Each MasteryLevel In MasteryNode
                SQLBase = "INSERT INTO certMasteries VALUES (" & TypeID & "," & MasteryLevel.Key.ToString & ","
                ' Read the masteries for each level
                Masteries = MasteryNode.Children(MasteryLevel.Key)

                ' Load the masteries for this level
                For Each Mastery In Masteries.Children
                    SQL = SQLBase & Mastery.Value & ")"
                    ' Insert the total record here
                    UpdateDB.ExecuteNonQuerySQL(SQL)
                Next
            Next
        End If

    End Sub

    ' Builds all the tables used by this class for inserts
    Private Sub BuildImportTables()
        Dim SQL As String

        ' inventoryTypes
        Call UpdateDB.DropTable("invTypes")
        ' Build table
        SQL = "CREATE TABLE [invTypes] ("
        SQL = SQL & "[typeID] [int] Not NULL,"
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
        Call UpdateDB.ExecuteNonQuerySQL(SQL)
        ' Create index
        SQL = "CREATE NONCLUSTERED INDEX [invTypes_IX_Group] ON [dbo].[invTypes] ([groupID] ASC)"
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.DropTable("invTraits")
        ' Build table
        SQL = "CREATE TABLE [invTraits] ("
        SQL = SQL & "[typeID] [int] NOT NULL,"
        SQL = SQL & "[skilltypeID] [int] NULL,"
        SQL = SQL & "[bonus] [float] NULL,"
        SQL = SQL & "[bonusText] [nvarchar](4000) NULL,"
        SQL = SQL & "[importance] [int] NULL,"
        SQL = SQL & "[unitID] [int] NULL)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)
        ' Create index
        SQL = "CREATE NONCLUSTERED INDEX [invTraits] ON [dbo].[invTraits] ([typeID] ASC)"
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)

        Call UpdateDB.DropTable("certMasteries")
        ' Build table
        SQL = "CREATE TABLE [certMasteries] ("
        SQL = SQL & "[typeID] [int] NOT NULL,"
        SQL = SQL & "[masteryLevel] [int] NULL,"
        SQL = SQL & "[masteryID] [int] NULL)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)
        ' Create index
        SQL = "CREATE NONCLUSTERED INDEX [certMasteries] ON [dbo].[certMasteries] ([typeID] ASC)"
        SQL = SQL & "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)"
        Call UpdateDB.ExecuteNonQuerySQL(SQL)

    End Sub

End Class