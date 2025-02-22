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

    class Room
    {
        public RectInt Bounds;
        public bool IsMainRoom;


        public Room(RectInt bounds, bool isMainRoom)
        {
            Bounds = bounds;
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
    }

    private void SeperateRooms()
    {
        bool anyOverlap = true;
        while (anyOverlap)
        {
            anyOverlap = false;
            for (int current = 0; current < _rooms.Count; current++)
            {
                for (int other = 0; other < _rooms.Count; other++)
                {
                    Room a = _rooms[current];
                    Room b = _rooms[other];

                    if (a == b) { continue; }


                    if (a.Bounds.Overlaps(b.Bounds))
                    {
                        anyOverlap = true;

                        Vector2 direction = (Vector2)(a.Bounds.center - b.Bounds.center).normalized;

                        Vector2Int aNewPos = Vector2Int.RoundToInt((Vector2)a.Bounds.center + direction);
                        Vector2Int bNewPos = Vector2Int.RoundToInt((Vector2)b.Bounds.center - direction);

                        a.Bounds.position = new Vector2Int(aNewPos.x - a.Bounds.width / 2, aNewPos.y - a.Bounds.height / 2);
                        b.Bounds.position = new Vector2Int(bNewPos.x - b.Bounds.width / 2, bNewPos.y - b.Bounds.height / 2);

                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {

        if (showRooms)
        {
            Gizmos.color = roomColor;
            foreach (Room room in _rooms)
            {
                Gizmos.DrawWireCube(new Vector3(room.Bounds.center.x, 0, room.Bounds.center.y), new Vector3(room.Bounds.width, 0.1f, room.Bounds.height));
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
