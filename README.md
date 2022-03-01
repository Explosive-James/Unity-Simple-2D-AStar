# Unity Simple 2D A-Star Package

## Summary

The simple 2D A-Star package is an easy to use pathfinding solution for tilemaps that allows you to quickly implement pathfinding for your game. It supports 'hexagon', 'rectangle' and 'isometric' grid layouts but doesn't fully support tilemaps that utilise the z-position of the grid yet, like 'isometric z as y'.<br><br>
**Features Include:**
* **Advanced Neighbour Selection**: You have the option to change how graph nodes are connected to one another such as only moving diagonally or orthogonally and even preventing diagonal movement if it means traversing over empty tiles.
* **Automatic Graph Building**: Which when left enabled will automatically update the A-Star graph every time you enter play mode or when you build the project.
* **Runtime Graph Generation**: The graph is able to update during runtime which allows you to manually update the graph after you've adjusted the tilemap or if you're generating the tilemap procedurally at runtime.

## How to Install

#### Through Git:

To do this you must first have [Git](https://git-scm.com/) installed or other version control software using git. Simply copy the git URL under Code >> HTTPS then inside of the Unity Editor paste the code into the package manager under Window >> Package Manager >> + >> Add package from git URL.

#### Through Zip:

Download the zip under Code >> Downlod ZIP and extract the files, then inside of the Unity Editor open up the package manager under Window >> Package Manager and click on + >> Add package from disk and locate the **package.json** file extracted from the ZIP.<br>
*Note: The files won't be moved into your project directory and deleting the files after importing them will break the package, you should put them in a safe directory before importing them into Unity.*

## Basic User Guide:

#### Adding an A-Star Graph to a Tilemap:

The A-Star Graph only needs a reference to the tilemap it's graphing so it doesn't *need* to be on the same game object as the tilemap but it is generally recommended that it is. To add the component, select the game object you want the component on and in the inspector hit 'Add Component' and search for 'A-Star Graph', once added you only need to drag and drop the tilemap you want to graph into the 'Walkable Tiles' field or hit the small button on the right of the 'Walkable Tiles' slot to select a tilemap.<br><br>
Each tile of the 'Walkable Tiles' tilemap is assigned a 'node' that an agent, i.e any game object using the 'A-Star Pathfinder' component, can move to. And neighbouring nodes can connect to each other so long as it follows the rules defined by the direction type.

#### Understanding the Direction Types:

* **Omnidirectional**: makes the nodes connect in any direction, as long as the tiles are next to one another.
* **Orthogonal Only**: makes the nodes connect to north, south, east and west directions.
* **Diagonal Only**: makes the nodes connect in northeast, northwest, southeast and southwest.
* **Smart Orthogonal**: makes the nodes only connect orthogonally when both the neighbouring diagonal tiles exist.
* **Smart Diagonal**: makes the nodes only connect diagonally when both the neighbouring orthogonal tiles exist.

#### Finding a Path in the A-Star Graph:

For a game object to find a path on the A-Star graph they must have an 'A-Star Pathfinder' component on them, by using the 'Add Component' button in the inspector, and assigning the 'Graph' field to the graph you want to find a path on.<br>
Then inside another script, such as a player contoller or AI controller, you need to give it a reference to the 'A-Star Pathfinder' by including the 'Simple2D_AStar' namespace and giving the class a public AStarPathfinder field and assigning it in the inspector or using GetComponent inside of the 'Awake' function.<br>
    
    using UnityEngine;
    using Simple2D_AStar;

    public class MyController : MonoBehaviour {
        
        private AStarPathfinder pathfinder;

        private void Awake() {
            pathfinder = GetComponent<AStarPathfinder>();
        }
    }

Then to find a path, access the pathfinder variable and call the 'FindPath' function and give it the start and end position in world position and it will return a Vector3[] of node positions in world space to follow to get from the start position to the end position. If the pathfinder cannot find a path it will return an empty Vector3[] with the length of 0, it will not return null.

    private AStarPathfinder pathfinder;

    public Vector3 targetPosition;
    private Vector3[] path;
    
    private void Awake() {
        pathfinder = GetComponent<AStarPathfinder>();
        path = pathfinder.FindPath(transform.position, targetPosition);
    }

