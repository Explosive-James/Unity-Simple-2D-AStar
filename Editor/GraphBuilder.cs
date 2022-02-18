using Simple2D_AStar;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class GraphBuilder : IProcessSceneWithReport
{
    #region Properties
    public int callbackOrder => 0;
    #endregion

    #region Public Functions
    public void OnProcessScene(Scene scene, BuildReport report)
    {
        foreach(GameObject obj in scene.GetRootGameObjects())
            foreach(AStarGraph graph in obj.GetComponentsInChildren<AStarGraph>()) {

                if (graph.generateOnBuild)
                    graph.UpdateGraph();
            }
    }
    #endregion
}
