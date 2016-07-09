
Imports System.IO
Imports YamlDotNet.RepresentationModel

Public Class YAMLDocument

    Private yStream As New YamlStream() ' For loading the inital data
    Private yMappingNode As YamlMappingNode ' for referencing the main mapping node
    Private ySequenceNode As YamlSequenceNode ' for referencing the main sequence node

    Public Sub New(FilePath As String)

        If FilePath <> "" Then
            ' Open the new file stream
            Dim tReader As New StreamReader(FilePath)

            ' Load the file into the stream
            Call yStream.Load(tReader)
        End If
    End Sub

    ' Try's to load a mapping node from the file and returns nothing if it fails
    Public Function GetFileMappingNode() As YamlMappingNode

        Try
            ' This is a mapped node file
            Dim ReturnNode As YamlMappingNode = yStream.Documents.First().RootNode
            yMappingNode = ReturnNode
            Return ReturnNode
        Catch ex As Exception
            ' return nothing
            Return Nothing
        End Try

    End Function

    ' Try's to load a mapping node from the file and returns nothing if it fails
    Public Function GetFileSequenceNode() As YamlSequenceNode

        Try
            ' This is a mapped node file
            Dim ReturnNode As YamlSequenceNode = yStream.Documents.First().RootNode
            ySequenceNode = ReturnNode
            Return ReturnNode
        Catch ex As Exception
            ' return nothing
            Return Nothing
        End Try

    End Function

    ' Returns the SQL value we want to use to build the SQL insert string or 'null' if not found from a mapping node
    Public Function GetSQLMappingScalarValue(ScalarName As String, DataKey As KeyValuePair(Of YamlNode, YamlNode)) As String
        Dim DataEntry As YamlMappingNode

        DataEntry = yMappingNode.Children(DataKey.Key)

        Try
            If DataEntry.Children.ContainsKey(New YamlScalarNode(ScalarName)) Then
                Dim y As YamlScalarNode = DataEntry.Children(New YamlScalarNode(ScalarName))
                Return y.Value
            Else
                Return "null"
            End If
        Catch ex As Exception
            ' Some error
            Return Nothing
        End Try

    End Function

    ' Returns the mapping node for a named mapping in the yaml file record or nothing if not found
    Public Function GetNamedMappingNode(MappingName As String, DataKey As KeyValuePair(Of YamlNode, YamlNode)) As YamlMappingNode
        Dim DataEntry As YamlMappingNode

        Try
            DataEntry = yMappingNode.Children(DataKey.Key)

            If DataEntry.Children.ContainsKey(New YamlScalarNode(MappingName)) Then
                Return DataEntry.Children(New YamlScalarNode(MappingName))
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    ' Returns the mapping node for a named mapping in the yaml file record or nothing if not found
    Public Function GetNamedMappingNode(MappingName As String, DataKey As YamlMappingNode) As YamlMappingNode
        Try

            If DataKey.Children.ContainsKey(New YamlScalarNode(MappingName)) Then
                Return DataKey.Children(New YamlScalarNode(MappingName))
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    ' Returns the scalar value for the name sent in a mapping node
    Public Function GetSQLScalarValueFromMapping(ScalarName As String, MappingNode As YamlMappingNode) As String

        If MappingNode.Children.ContainsKey(New YamlScalarNode(ScalarName)) Then
            Return MappingNode.Children(New YamlScalarNode(ScalarName)).ToString
        Else
            Return "null"
        End If

    End Function

End Class
