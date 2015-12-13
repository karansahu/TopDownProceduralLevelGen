using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoomConnections : MonoBehaviour
{
    private MeshGenerator meshGen;
    private int[,] map;
    private int _mapWidth, _mapHeight;
    public int minWallRegionSize, minFloorRegionSize;
    public List<Room> roomRegions = new List<Room>();
    public GameObject[] sectionConnections;
    [NonSerialized]
    public Vector3 offset;
    public bool connectedToSection = false;
    public List<GameObject> connectedSections = new List<GameObject>();

    void Awake()
    {
        meshGen = GetComponent<MeshGenerator>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            FindConnectionsBetweenSections(roomRegions);
    }

    public void InitilizeAndScan(int width, int height, int[,] smoothMap)
    {
        _mapWidth = width;
        _mapHeight = height;
        map = smoothMap;
        offset = transform.position - new Vector3(_mapWidth / 2 - 0.5f, 0, _mapHeight / 2 - 0.5f);
        ScanMapForRegions();
        meshGen.GenerateMesh(map, 1);
    }

    public class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;

        public bool mainRoom;
        public bool isConnectedToMainRoom;
        public Room() { }

        public Room(List<Coord> _tiles, int[,] map)
        {
            tiles = _tiles;
            roomSize = tiles.Count;
            edgeTiles = new List<Coord>();
            connectedRooms = new List<Room>();

            foreach (Coord c in tiles)
            {
                for (int x = c.posX - 1; x <= c.posX + 1; x++)
                {
                    for (int y = c.posY - 1; y <= c.posY + 1; y++)
                    {
                        if (map[x, y] == 1 && (x == c.posX || y == c.posY))
                        {
                            edgeTiles.Add(c);
                        }
                    }
                }
            }
        }

        public static void ConnectRooms(Room firstRoom, Room secondRoom)
        {
            if (firstRoom.isConnectedToMainRoom)
                secondRoom.ConnectToMainRoom();
            else if (secondRoom.isConnectedToMainRoom)
                firstRoom.ConnectToMainRoom();

            firstRoom.connectedRooms.Add(secondRoom);
            secondRoom.connectedRooms.Add(firstRoom);
        }

        public bool IsConnectedTo(Room room)
        {
            return (connectedRooms.Contains(room));
        }

        public int CompareTo(Room room)
        {
            return (room.roomSize.CompareTo(roomSize));
        }

        public void ConnectToMainRoom()
        {
            if (!isConnectedToMainRoom)
            {
                isConnectedToMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.ConnectToMainRoom();
                }
            }
        }
    }

    public struct Coord
    {
        public int posX, posY;
        public Coord(int x, int y)
        {
            posX = x;
            posY = y;
        }
    }

    void ScanMapForRegions()
    {
        List<List<Coord>> wallRegions = GetAllRegions(1);
        foreach (List<Coord> region in wallRegions)
        {
            if (region.Count < minWallRegionSize)
            {
                foreach (Coord c in region)
                {
                    map[c.posX, c.posY] = 0;
                }
            }
        }

        List<List<Coord>> floorRegions = GetAllRegions(0);
        List<Room> roomsLeftAfterRegionCrop = new List<Room>();
        foreach (List<Coord> region in floorRegions)
        {
            if (region.Count < minFloorRegionSize)
            {
                foreach (Coord c in region)
                {
                    map[c.posX, c.posY] = 1;
                }
            }
            else
                roomsLeftAfterRegionCrop.Add(new Room(region, map));
        }
        roomsLeftAfterRegionCrop.Sort();
        roomsLeftAfterRegionCrop[0].mainRoom = true;
        roomsLeftAfterRegionCrop[0].isConnectedToMainRoom = true;
        roomRegions = roomsLeftAfterRegionCrop;
        FindConnectionsBetweenRooms(roomsLeftAfterRegionCrop);
    }

    List<List<Coord>> GetAllRegions(int tileType)
    {
        List<List<Coord>> allRegionsOfTileType = new List<List<Coord>>();
        int[,] tileVisited = new int[_mapWidth, _mapHeight];

        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                if (map[x, y] == tileType && tileVisited[x, y] == 0)
                {
                    List<Coord> regionToAdd = GetSpecifiedRegion(x, y);
                    allRegionsOfTileType.Add(regionToAdd);

                    foreach (Coord c in regionToAdd)
                    {
                        tileVisited[c.posX, c.posY] = 1;
                    }
                }
            }
        }

        return allRegionsOfTileType;
    }

    List<Coord> GetSpecifiedRegion(int posX, int posY)
    {
        List<Coord> region = new List<Coord>();
        int[,] tileVisisted = new int[_mapWidth, _mapHeight];
        int tileType = map[posX, posY];

        Queue<Coord> regionTiles = new Queue<Coord>();
        regionTiles.Enqueue(new Coord(posX, posY));
        tileVisisted[posX, posY] = 1;

        while (regionTiles.Count > 0)
        {
            Coord tile = regionTiles.Dequeue();
            region.Add(tile);

            for (int x = tile.posX - 1; x <= tile.posX + 1; x++)
            {
                for (int y = tile.posY - 1; y <= tile.posY + 1; y++)
                {
                    if (IsInsideMap(x, y) && (x == tile.posX || y == tile.posY))
                    {
                        if (map[x, y] == tileType && tileVisisted[x, y] == 0)
                        {
                            tileVisisted[x, y] = 1;
                            regionTiles.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return region;
    }

    void FindConnectionsBetweenRooms(List<Room> roomsToConnect, bool forceMainRoomConnection = false)
    {
        int shortestDistance = 0;
        bool connectionFound = false;

        Room roomA_ConnectFrom = new Room();
        Room roomB_ConnectTo = new Room();

        Coord tileA_ConnectFrom = new Coord();
        Coord tileB_ConnectTo = new Coord();

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceMainRoomConnection)
        {
            //if rooms are connected to Main Room, add to ListB
            foreach (Room r in roomsToConnect)
            {
                if (r.isConnectedToMainRoom)
                    roomListB.Add(r);
                //else add to ListA
                else
                    roomListA.Add(r);
            }
        }
        else
            roomListA = roomListB = roomsToConnect;

        foreach (Room roomA in roomListA)
        {
            if (!forceMainRoomConnection)
            {
                connectionFound = false;
                //This room has more than one connection, break
                if (roomA.connectedRooms.Count > 0)
                    break;
            }
            foreach (Room roomB in roomListB)
            {
                //If the rooms are the same or if they are already connected, skip to the next room.
                if (roomA == roomB || roomA.IsConnectedTo(roomB))
                    continue;

                for (int i = 0; i < roomA.edgeTiles.Count; i++)
                {
                    for (int j = 0; j < roomB.edgeTiles.Count; j++)
                    {

                        Coord tileRoomA = roomA.edgeTiles[i];
                        Coord tileRoomB = roomB.edgeTiles[j];

                        int distance = (int)(Mathf.Pow((tileRoomA.posX - tileRoomB.posX), 2) + Mathf.Pow((tileRoomA.posY - tileRoomB.posY), 2));
                        if (distance < shortestDistance || !connectionFound)
                        {
                            shortestDistance = distance;
                            connectionFound = true;
                            roomA_ConnectFrom = roomA;
                            roomB_ConnectTo = roomB;
                            tileA_ConnectFrom = tileRoomA;
                            tileB_ConnectTo = tileRoomB;
                        }
                    }
                }
            }
            if (connectionFound && !forceMainRoomConnection)
                CreatePassage(roomA_ConnectFrom, roomB_ConnectTo, tileA_ConnectFrom, tileB_ConnectTo);
        }
        if (forceMainRoomConnection && connectionFound)
        {
            //Calling the create passage outside both loops to ensure the shortest connection to the main room via rooms
            CreatePassage(roomA_ConnectFrom, roomB_ConnectTo, tileA_ConnectFrom, tileB_ConnectTo);
            //Calling the method again to check for any more connections
            FindConnectionsBetweenRooms(roomsToConnect, true);
        }

        if (!forceMainRoomConnection)
            FindConnectionsBetweenRooms(roomsToConnect, true);

    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorld(tileA), CoordToWorld(tileB), Color.green, 100);
    }

    bool IsInsideMap(int posX, int posY)
    {
        return (posX >= 0 && posX < _mapWidth && posY >= 0 && posY < _mapHeight);
    }

    Vector3 CoordToWorld(Coord A)
    {
        return (meshGen.MeshContainer.transform.position + (new Vector3(-_mapWidth / 2 + 0.5f + A.posX, 1, -_mapHeight / 2 + 0.5f + A.posY)));
    }

    public static void ConnectSections(GameObject firstSection, GameObject secondConnection)
    {
        firstSection.GetComponent<RoomConnections>().connectedSections.Add(secondConnection);
    }

    public bool SectionIsConnectedTo(GameObject section)
    {
        return (connectedSections.Contains(section));
    }

    void CreateSectionConnection(GameObject sectionA, GameObject sectionB, Coord tileA, Coord tileB)
    {
        ConnectSections(sectionA, sectionB);
        Vector3 start = new Vector3(tileA.posX,0,tileA.posY) + offset;
        Vector3 end = new Vector3(tileB.posX + sectionB.GetComponent<RoomConnections>().offset.x, 0, tileB.posY + sectionB.GetComponent<RoomConnections>().offset.z);
        Debug.DrawLine(start, end, Color.blue, 100);
    }

    void FindConnectionsBetweenSections(List<Room> myRoomList)
    {
        int shortestDistance = 0;

        Coord tileA_ConnectFrom = new Coord();
        Coord tileB_ConnectTo = new Coord();

        List<List<Room>> allSectionsRooms = new List<List<Room>>();

        foreach (GameObject connectionGameObj in sectionConnections)
        {
            if (SectionIsConnectedTo(connectionGameObj))
                break;
            
            allSectionsRooms.Add(connectionGameObj.GetComponent<RoomConnections>().roomRegions);
            foreach (List<Room> sectionRoomList in allSectionsRooms)
            {
                foreach (Room roomA in myRoomList)
                {
                    foreach (Room roomB in sectionRoomList)
                    {
                        for (int i = 0; i < roomA.edgeTiles.Count; i++)
                        {
                            for (int j = 0; j < roomB.edgeTiles.Count; j++)
                            {
                                Coord tileRoomA = roomA.edgeTiles[i];
                                Coord tileRoomB = roomB.edgeTiles[j];


                                int distance = (int)(Mathf.Pow( (tileRoomA.posX + offset.x) - (tileRoomB.posX + connectionGameObj.GetComponent<RoomConnections>().offset.x) , 2) + 
                                                     Mathf.Pow( (tileRoomA.posY + offset.z) - (tileRoomB.posY + connectionGameObj.GetComponent<RoomConnections>().offset.z) , 2));

                                if (distance < shortestDistance || !connectedToSection)
                                {
                                    shortestDistance = distance;
                                    connectedToSection = true;
                                    tileA_ConnectFrom = tileRoomA;
                                    tileB_ConnectTo = tileRoomB;
                                }
                            }
                        }
                    }
                }
            }
            allSectionsRooms.Clear();
            if (!connectedSections.Contains(connectionGameObj))
                CreateSectionConnection(this.gameObject, connectionGameObj, tileA_ConnectFrom, tileB_ConnectTo);
            connectedToSection = false;
        }        
    }

    void OnDrawGizmos()
    {
        List<List<Room>> sectionConnectionRooms = new List<List<Room>>();        
        foreach (Room room in roomRegions)
        {
            for (int i = 0; i < room.edgeTiles.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(room.edgeTiles[i].posX + offset.x, 0, room.edgeTiles[i].posY + offset.z),Vector3.one);
            }

        }
    }
}