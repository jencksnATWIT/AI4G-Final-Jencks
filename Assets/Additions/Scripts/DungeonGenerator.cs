using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System;

public class DungeonGenerator : MonoBehaviour
{
    /*
     * This is the main script. 
     * It orchestrates the entire level generation process. 
     * It currently lacks any corridor-creation logic.    
    */

    [Header("Dungeon Settings")]
    public int width = 50, height = 50;
    public int minRoomSize = 6;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject markerPrefab;
    public GameObject agentPrefab;

    [Header("NavMesh")]
    public Unity.AI.Navigation.NavMeshSurface navMeshSurface;
    public Transform player;

    public List<Room> allRooms = new();

    public static event Action OnDungeonGenerated;

    private List<EnvironmentMarker> _spawnedMarkers = new();

    void Start()
    {
        GenerateDungeon();
        // The NavMesh is baked here, on the generated floor tiles
        //navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
        // The agent is spawned after a short delay to allow the NavMesh to bake
        Invoke(nameof(SpawnAgent), 0.5f);
        //OnDungeonGenerated?.Invoke();
    }

    /// <summary>
    /// Generates the dungeon layout using a Binary Space Partitioning (BSP) algorithm.
    /// It creates rooms and connects them with corridors.
    /// Each room is tagged based on its aspect ratio to determine its type.
    /// The NavMesh is built after the dungeon generation.
    /// </summary>
    void GenerateDungeon()
    {
        BSPNode root = new BSPNode(new RectInt(0, 0, width, height));
        Split(root, 4); // The depth of the BSP tree
        CreateRooms(root);
        ConnectRooms(root);

        foreach (var room in allRooms)
        {
            CreateFloor(room.area);
            TagRoom(room);
        }
    }

    /// <summary>
    /// Spawns the agent at the center of the first room.
    /// This method checks if the agent prefab is assigned and if there are rooms generated.
    /// If conditions are met, it instantiates the agent at the center of the first room
    /// with a height of 1 unit.
    /// </summary>
    void SpawnAgent()
    {
        float numAgents = MathF.Floor(allRooms.Count / 2);
        if (agentPrefab == null || allRooms.Count == 0)
        {
            Debug.LogWarning("AgentPrefab not assigned or no rooms generated.");
            return;
        }

        for (int i = 0; i < numAgents; i++)
        {
            // Spawn the agent in the center of the first room.
            Vector2Int spawn = allRooms[2*1].Center;
            Vector3 spawnPosition = new Vector3(spawn.x, 1.5f, spawn.y);

            GameObject agent = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);

            var patrolAI = agent.GetComponent<PatrolAI>();
            if (patrolAI != null)
            {
                patrolAI.waypoints = _spawnedMarkers
                    .ConvertAll(marker => marker.transform)
                    .ToArray();

                Debug.Log($"Assigned {patrolAI.waypoints.Length} waypoints to PatrolAI.");
            }

            patrolAI.player = player;
            patrolAI.idlePosition = patrolAI.waypoints[0].transform;
        } /*
        // Spawn the agent in the center of the first room.
        Vector2Int spawn = allRooms[0].Center;
        Vector3 spawnPosition = new Vector3(spawn.x, 1.5f, spawn.y);

        GameObject agent = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);

        var patrolAI = agent.GetComponent<PatrolAI>();
        if (patrolAI != null)
        {
            patrolAI.waypoints = _spawnedMarkers
                .ConvertAll(marker => marker.transform)
                .ToArray();

            Debug.Log($"Assigned {patrolAI.waypoints.Length} waypoints to PatrolAI.");
        }

        patrolAI.player = player;
        patrolAI.idlePosition = patrolAI.waypoints[0].transform; */

        //Instantiate(agentPrefab, spawnPosition, Quaternion.identity);
    }

    /// <summary>
    /// Splits the BSP tree recursively.
    /// This method divides the node's area into two sub-areas based on a random split.
    /// It continues to split until the specified depth is reached or the area is too small.
    /// </summary>
    void Split(BSPNode node, int depth)
    {
        if (depth <= 0) return;
        if (node.Split(minRoomSize))
        {
            Split(node.Left, depth - 1);
            Split(node.Right, depth - 1);
        }
    }

    /// <summary>
    /// Creates rooms in the BSP tree.
    /// This method generates random room sizes and positions within the node's area.
    /// Each room is stored in the allRooms list for later processing.
    /// </summary>
    void CreateRooms(BSPNode node)
    {
        if (node.IsLeaf)
        {
            int w = UnityEngine.Random.Range(minRoomSize, node.Area.width - 1);
            int h = UnityEngine.Random.Range(minRoomSize, node.Area.height - 1);
            int x = node.Area.x + UnityEngine.Random.Range(1, node.Area.width - w);
            int y = node.Area.y + UnityEngine.Random.Range(1, node.Area.height - h);

            RectInt roomRect = new(x, y, w, h);
            node.Room = roomRect;
            allRooms.Add(new Room { area = roomRect });
        }
        else
        {
            CreateRooms(node.Left);
            CreateRooms(node.Right);
        }
    }

    /// <summary>
    /// Creates a floor in the specified area.
    /// This method instantiates floor tiles in the given rectangular area.
    /// Each tile is marked as static for NavMesh baking.
    /// </summary>
    void CreateFloor(RectInt area)
    {
        for (int x = area.xMin; x < area.xMax; x++)
        {
            for (int y = area.yMin; y < area.yMax; y++)
            {
                GameObject tile = Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);
                tile.isStatic = true; // Mark as static for NavMesh baking
            }
        }
    }

    /// <summary>
    /// Creates a corridor between two points.
    /// This method generates a path of floor tiles from the start point to the end point.
    /// It moves horizontally first, then vertically, ensuring a straight corridor.
    /// </summary>
    void CreateCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;
        while (current.x != end.x)
        {
            CreateFloorTile(current);
            CreateFloorTile(current + Vector2Int.up);
            current.x += (int)Mathf.Sign(end.x - current.x);
        }
        while (current.y != end.y)
        {
            CreateFloorTile(current);
            CreateFloorTile(current + Vector2Int.right);
            current.y += (int)Mathf.Sign(end.y - current.y);
        }
        CreateFloorTile(end);
    }

    /// <summary>
    /// Creates a floor tile at the specified position.
    /// This method instantiates a floor tile GameObject at the given position.
    /// </summary>
    void CreateFloorTile(Vector2Int pos)
    {
        GameObject tile = Instantiate(floorPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        tile.isStatic = true;
    }

    /// <summary>
    /// Connects rooms in the BSP tree.
    /// This method traverses the BSP tree and connects rooms by creating corridors between their centers.
    /// It currently does not create corridors but can be expanded in the future.
    /// </summary>
    void ConnectRooms(BSPNode node)
    {
        if (node.IsLeaf) return;

        Room leftRoom = GetRoomFromNode(node.Left);
        Room rightRoom = GetRoomFromNode(node.Right);

        if (leftRoom != null && rightRoom != null)
        {
            Vector2Int leftCenter = leftRoom.Center;
            Vector2Int rightCenter = rightRoom.Center;
            CreateCorridor(leftCenter, rightCenter);
        }

        // This function is a placeholder for future corridor creation logic.
        // Currently, it does not connect rooms, but it can be expanded later.
        ConnectRooms(node.Left);
        ConnectRooms(node.Right);
    }

    /// <summary>
    /// Tags a room based on its aspect ratio.
    /// This method determines the type of room based on its width-to-height ratio.
    /// It creates a marker GameObject at the room's center with the appropriate tag.
    /// </summary>
    void TagRoom(Room room)
    {
        float aspect = (float)room.area.width / room.area.height;
        var tag = (aspect < 0.5f || aspect > 2.0f) ? EnvironmentTagType.Chokepoint : EnvironmentTagType.SafeZone;

        GameObject marker = Instantiate(markerPrefab, new Vector3(room.Center.x, 0.5f, room.Center.y), Quaternion.identity);
        var envMarker = marker.AddComponent<EnvironmentMarker>();
        envMarker.tagType = tag;
        _spawnedMarkers.Add(envMarker);
        //marker.AddComponent<EnvironmentMarker>().tagType = tag;

        //Debug.Log($"Spawned EnvironmentMarker at {marker.transform.position}");
    }

    /// <summary>
    /// Retrieves a room from the BSP node.
    /// This method checks if the node is a leaf and returns the corresponding room.
    /// If the node is not a leaf, it randomly selects one of the child nodes to continue searching.
    /// </summary>
    Room GetRoomFromNode(BSPNode node)
    {
        if (node.IsLeaf)
        {
            return allRooms.Find(r => r.area.Equals(node.Room));
        }

        return GetRoomFromNode(UnityEngine.Random.value > 0.5f ? node.Left : node.Right);
    }

    public class Room
    {
        public RectInt area;
        public Vector2Int Center => new(area.x + area.width / 2, area.y + area.height / 2);
    }

    public class BSPNode
    {
        public RectInt Area;
        public BSPNode Left, Right;
        public RectInt Room;
        public bool IsLeaf => Left == null && Right == null;

        public BSPNode(RectInt area) { Area = area; }

        public bool Split(int minSize)
        {
            if (!IsLeaf) return false;

            bool horizontal = UnityEngine.Random.value > 0.5f;
            int max = (horizontal ? Area.height : Area.width) - minSize;
            if (max <= minSize) return false;

            int split = UnityEngine.Random.Range(minSize, max);
            if (horizontal)
            {
                Left = new BSPNode(new RectInt(Area.x, Area.y, Area.width, split));
                Right = new BSPNode(new RectInt(Area.x, Area.y + split, Area.width, Area.height - split));
            }
            else
            {
                Left = new BSPNode(new RectInt(Area.x, Area.y, split, Area.height));
                Right = new BSPNode(new RectInt(Area.x + split, Area.y, Area.width - split, Area.height));
            }
            return true;
        }
    }
}
