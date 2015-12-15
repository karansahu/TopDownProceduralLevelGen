using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[RequireComponent(typeof(RoomConnections))]
public class CellularAutomata : MonoBehaviour
{
    System.Random rand;
    
    private int[,] map;
    private RoomConnections roomConnection;

    public int mapWidth, mapHeight, iterationNumCA, floorPercent, wallThreshold, floorThreshold, seed;
    public bool useRandomSeed = false;

    void Start()
    {
        map = new int[mapWidth, mapHeight];
        roomConnection = GetComponent<RoomConnections>();
    }

    public void GenerateMap()
    {
        GenerateRandomMap();
        GenerateSmoothMap();
        roomConnection.InitilizeAndScan(mapWidth,mapHeight,map);            
    }
    
    void GenerateRandomMap()
    {
        if(!useRandomSeed)
            rand = new System.Random(seed);
        else
            rand = new System.Random();

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //check for border and create a wall (1)
                if (i == 0)
                    map[i, j] = 1;
                else if (j == 0)
                    map[i, j] = 1;
                else if (i == mapWidth - 1)
                    map[i, j] = 1;
                else if (j == mapHeight - 1)
                    map[i, j] = 1;
                //check if middle and create a wall (1) and create a floor or wall depending on a percentage
                else
                    map[i, j] = (j == mapHeight / 2) ? 0 : (floorPercent >= rand.Next(1, 100)) ? 1 : 0;
            }
        }
    }
        
    void GenerateSmoothMap()
    {
        for (int iteration = 0; iteration < iterationNumCA; iteration++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    //check number of walls around me. 
                    int wallCount = NumberOfAdjacentWalls(i, j);
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
                        map[i, j] = 0;
                }
            }
        }
    }

    int NumberOfAdjacentWalls(int posX, int posY)
    {
        int wallCount = 0;

        for (int x = posX - 1; x <= posX + 1; x++)
        {
            for (int y = posY - 1; y <= posY + 1; y++)
            {
                if (!(x == posX && y == posY))
                {
                    //Out of bounds
                    if (!IsInsideMap(x, y))
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
        return (posX >= 0 && posX < mapWidth && posY >= 0 && posY < mapHeight);
    }    
}
