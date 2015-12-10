using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellularAutomata : MonoBehaviour
{
    System.Random rand;
    private MeshGenerator meshGen;

    private int[,] map;
    private Texture2D texRandomFill;
    private GameObject quad;

    public int mapWidth, mapHeight, iterationNumCA, floorPercent, wallThreshold, floorThreshold,seed, minWallRegionSize, minFloorRegionSize;

    void Start ()
    {        
        quad = GameObject.Find("Quad");
        meshGen = GetComponent<MeshGenerator>();
        rand = new System.Random(seed);
    }
	
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            texRandomFill = new Texture2D(mapWidth, mapHeight, TextureFormat.RGB24, false);
            texRandomFill.name = "TextureRandomFill";
            texRandomFill.wrapMode = TextureWrapMode.Clamp;
            
            //calculate again
            int[,] randomMap = GenerateRandomMap();
            //GenerateTexture(randomMap);
            int [,] smoothMap = GenerateSmoothMap(randomMap);
            //GenerateTexture(smoothMap);
            map = smoothMap;

            SearchMapForRegions(minWallRegionSize, minFloorRegionSize);
            meshGen.GenerateMesh(smoothMap, 1);

        }
	}

    int [,] GenerateRandomMap()
    {
        int [,] map = new int[mapWidth, mapHeight];
        rand = new System.Random(seed);
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //check for border and create a wall (1)
                if (i == 0)
                    map[i, j] = 1;
                else if(j == 0)
                    map[i, j] = 1;
                else if(i == mapWidth - 1)
                    map[i, j] = 1;
                else if(j == mapHeight - 1)
                    map[i, j] = 1;
                //check if middle and create a wall (1) and create a floor or wall depending on a percentage
                else
                    map[i, j] = (j == mapHeight / 2) ? 0 : (floorPercent >= rand.Next(1, 100)) ? 1 : 0;
            }
        }
        return map;
    }

    int[,] GenerateSmoothMap(int [,] map)
    {
        for (int iteration = 0; iteration < iterationNumCA; iteration++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    //check number of walls around me. 
                    int wallCount = NumberOfAdjacentWalls(i, j, map);
                    //if I am wall        
                    if (map[i, j] == 1)
                    {
                        //If greater than wallThreshold, I am wall
                        if (wallCount >= wallThreshold)
                        {
                            map[i, j] = 1;
                        }
                        else
                            map[i, j] = 0;
                    }
                    //If not wall, check against floorThreshold. If greater than floorthreshold, I am wall
                    else if (wallCount >= floorThreshold)
                    {
                        map[i, j] = 1;
                    }
                    //else I am floor
                    else
                        map[i,j] = 0;
                }
            }
        }

        return map;
    }

    int NumberOfAdjacentWalls(int posX, int posY, int[,] map)
    {
        int wallCount = 0;

        for (int x = posX - 1; x <= posX + 1; x++)
        {
            for (int y = posY - 1; y <= posY + 1; y++)
            {
                if (!(x == posX && y == posY))
                {
                    //Out of bounds
                    if (!IsInsideMap(x,y))
                    {
                        wallCount++;
                        continue;
                    }                    
                    //If wall
                    if (map[x, y] == 1)
                        wallCount++;
                }
            }
        }
        return wallCount;
    }

    bool IsInsideMap(int posX, int posY)
    {
        return (posX >= 0 && posY >= 0 && posX < mapWidth && posY < mapHeight);
    }

    void GenerateTexture(int[,] values)
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //floor - white
                if (values[i, j] == 0)
                    texRandomFill.SetPixel(i, j, new Color(1, 1, 1, 1));
                //wall - black
                else if (values[i, j] == 1)
                    texRandomFill.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }
        texRandomFill.Apply(false);
        quad.GetComponent<Renderer>().material.SetTexture("_MainTex", texRandomFill);
    }
    
    void SearchMapForRegions(int minWallRegion, int minFloorRegion)
    {
        List<List<Coord>> wallRegions = GetAllRegions(1);
        foreach (List<Coord> region in wallRegions)
        {
            if(region.Count < minWallRegion)
            {
                foreach (Coord c in region)
                {
                    map[c.posX, c.posY] = 0;
                }
            }
        }

        List<List<Coord>> floorRegions = GetAllRegions(0);
        foreach (List<Coord> region in floorRegions)
        {
            if (region.Count < minFloorRegion)
            {
                foreach (Coord c in region)
                {
                    map[c.posX, c.posY] = 1;
                }
            }
        }
    }

    List<List<Coord>> GetAllRegions(int tileType)
    {
        List<List<Coord>> allRegions = new List<List<Coord>>();
        int[,] tileVisited = new int[mapWidth,mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if(map[i,j] == tileType && tileVisited[i,j] == 0)
                {
                    List<Coord> regionToAdd = GetSpecifiedRegion(i, j);
                    allRegions.Add(regionToAdd);

                    foreach (Coord c in regionToAdd)
                    {
                        tileVisited[c.posX, c.posY] = 1;
                    }
                }
            }
        }

        return allRegions;
    }

    List<Coord> GetSpecifiedRegion(int tilePosX, int tilePosY)
    {
        List<Coord> region = new List<Coord>();
        int[,] tileVisited = new int[mapWidth, mapHeight];
        int tileType = map[tilePosX,tilePosY];

        Queue<Coord> tileQueue = new Queue<Coord>();
        tileQueue.Enqueue(new Coord(tilePosX, tilePosY));
        tileVisited[tilePosX, tilePosY] = 1;

        while(tileQueue.Count > 0)
        {
            Coord tile = tileQueue.Dequeue();
            region.Add(tile);

            for (int i = tile.posX - 1; i <= tile.posX + 1; i++)
            {
                for (int j = tile.posY - 1; j <= tile.posY + 1; j++)
                {
                    if(IsInsideMap(i, j) && (i == tile.posX || j == tile.posY))
                    {
                        if(tileVisited[i,j] == 0 && map[i,j] == tileType)
                        {
                            tileVisited[i, j] = 1;
                            tileQueue.Enqueue(new Coord(i,j));
                        }
                    }
                }
            }
        }

        return region;
    }

    struct Coord
    {
        public int posX, posY;
        public Coord(int x, int y)
        {
            posX = x;
            posY = y;
        }
    }

}
