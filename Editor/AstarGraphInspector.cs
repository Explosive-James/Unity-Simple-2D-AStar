using Simple2D_AStar;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(AStarGraph))]
internal class AstarGraphInspector : Editor
{
    #region Data
    private AStarGraph graph;
    #endregion

    #region Unity Functions
    private void OnEnable() => graph = (AStarGraph)target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Separator();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Graph Settings", EditorStyles.boldLabel);

        GUIContent walkableTilesContent = new GUIContent("Walkable Tiles:", "Tiles that should be traversable on this grid.");
        Tilemap walkableTiles = (Tilemap)EditorGUILayout.ObjectField(walkableTilesContent, graph.walkableTiles, typeof(Tilemap), true);

        if (EditorGUI.EndChangeCheck()) {

            MakeDirty("Update Tiles");
            graph.walkableTiles = walkableTiles;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginDisabledGroup(graph.walkableTiles != null && graph.walkableTiles.layoutGrid.cellLayout == GridLayout.CellLayout.Hexagon);

        GUIContent directionTypeContent = new GUIContent("Direction Type:", "Which directions can the nodes connect to each other in.");
        DirectionType directionType = (DirectionType)EditorGUILayout.EnumPopup(directionTypeContent, graph.directionType);

        if (EditorGUI.EndChangeCheck()) {

            MakeDirty("Change Direction Type");
            graph.directionType = directionType;
        }

        EditorGUILayout.Separator();
        EditorGUI.BeginChangeCheck();

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

        GUIContent generateOnBuildContent = new GUIContent("Generate On Build:", "Should the graph automatically update when the project is built.");
        bool generateOnBuild = EditorGUILayout.Toggle(generateOnBuildContent, graph.generateOnBuild);

        if (EditorGUI.EndChangeCheck()) {

            MakeDirty("Change Generate On Build");
            graph.generateOnBuild = generateOnBuild;
        }

        if(GUILayout.Button("Update Navigation Graph")) {

            MakeDirty("Update Navigation Graph");
            graph.UpdateGraph();
        }
    }
    #endregion

    #region Private Functions
    private void MakeDirty(string msg)
    {
        Undo.RegisterCompleteObjectUndo(graph, msg);
        EditorUtility.SetDirty(graph);
    }
    #endregion
}
