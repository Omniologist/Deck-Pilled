using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class TKDungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] int dungeonRadius = 50;

    [SerializeField] int roomCount = 5;
    [SerializeField] int connectingRoomCount = 0;

    [SerializeField] int minRoomSize = 6;
    [SerializeField] int maxRoomSize = 12;

    [SerializeField] int minConnectingRoomSize = 2;
    [SerializeField] int maxConnectingRoomSize = 4;
    [SerializeField] float seperationTime = 10f;

    [Header("Physics Settings")]
    int layerMask = 8;

    [Header("Visualization")]
    [SerializeField] bool showRooms = true;
    [SerializeField] bool showTriangulation = true;
    [SerializeField] bool showMST = true;
    [SerializeField] bool showCorridors = true;
    [SerializeField] Color roomColor = Color.white;
    [SerializeField] Color traingulationColor = Color.yellow;
    [SerializeField] Color mstColor = Color.green;
    [SerializeField] Color corridorColor = Color.blue;

    private List<Room> _rooms = new List<Room>();
    private List<Room> _mainRooms = new List<Room>();
    private List<GameObject> _roomObjects = new List<GameObject>();

    class Room
    {
        public RectInt Bounds;
        public Vector2Int Center;
        public bool IsMainRoom;
        public GameObject gameObject;

        public Room(RectInt bounds, bool isMainRoom)
        {
            Bounds = bounds;
            Center = Vector2Int.RoundToInt(bounds.center);
            IsMainRoom = isMainRoom;
        }
    }

    public void GenerateDungeon()
    {
        ClearDungeon();
        GenerateRooms();
        SeperateRooms();
    }

    private void ClearDungeon()
    {
        _rooms.Clear();

        foreach (GameObject roomObj in _roomObjects)
        {
            Destroy(roomObj);
        }
        _roomObjects.Clear();
    }



    private void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GenerateRoom(true);
        }
        for (int i = 0; i < connectingRoomCount; i++)
        {
            GenerateRoom(false);
        }
    }

    private void GenerateRoom(bool isMainRoom) 
    {
        int sizeMin = isMainRoom ? minRoomSize : minConnectingRoomSize;
        int sizeMax = isMainRoom ? maxRoomSize : maxConnectingRoomSize;

        int width = Random.Range(sizeMin, sizeMax + 1);
        int height = Random.Range(sizeMin, sizeMax + 1);

        Vector2 randomPoint = Random.insideUnitCircle * dungeonRadius;
        int x = Mathf.RoundToInt(randomPoint.x) + dungeonRadius;
        int y = Mathf.RoundToInt(randomPoint.y) + dungeonRadius;

        RectInt bounds = new RectInt(x - width / 2, y - height / 2, width, height);
        Room newRoom = new Room(bounds, isMainRoom);
        _rooms.Add(newRoom);

        if (isMainRoom)
        {
            _mainRooms.Add(newRoom);
        }

        GameObject roomObj = new GameObject("Room");
        roomObj.transform.position = new Vector3(newRoom.Center.x, 0, newRoom.Center.y);
        roomObj.layer = LayerMask.NameToLayer("roomLayer");

        BoxCollider collider = roomObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(width, 1, height);
        collider.isTrigger = false;
        Rigidbody rb = roomObj.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        newRoom.gameObject = roomObj;
        _roomObjects.Add(roomObj);
    }

    private void SeperateRooms()
    {
        Physics.simulationMode = SimulationMode.Script;
        float timer = 0;
        while (timer < seperationTime)
        {
            Physics.Simulate(Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
        }
        Physics.simulationMode = SimulationMode.FixedUpdate;

        foreach (Room room in _rooms)
        {
            Vector3 newPos = room.gameObject.transform.position;
            room.Center = new Vector2Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.z));
            room.Bounds.x = room.Center.x - room.Bounds.width / 2;
            room.Bounds.y = room.Center.y - room.Bounds.height / 2;
            room.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (showRooms)
        {
            Gizmos.color = roomColor;
            foreach (Room room in _rooms)
            {
                Gizmos.DrawWireCube(new Vector3(room.Center.x, 0, room.Center.y), new Vector3(room.Bounds.width, 0.1f, room.Bounds.height));
            }

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
