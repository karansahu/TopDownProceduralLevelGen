using UnityEngine;
using System.Collections;

public class CellularAutomata : MonoBehaviour
{
    System.Random rand = new System.Random();

    //private int[,] map;
    private Texture2D texRandomFill;
    private GameObject quad;

    public int mapWidth, mapHeight, iterationNumCA, floorPercent, wallThreshold, floorThreshold;

    void Start ()
    {        
        quad = GameObject.Find("Quad");
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
            GenerateTexture(smoothMap);
        }
	}

    int [,] GenerateRandomMap()
    {
        int [,] map = new int[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                //check for border and create a wall (1)
                if (i == 0 || j == 0 || i == mapHeight - 1 || j == mapWidth - 1)
                    map[i, j] = 1;
                //check if middle and create a wall (1) and create a floor or wall depending on a percentage
                else
                    map[i, j] = (j == mapHeight/2) ? 0 : (floorPercent >= rand.Next(1, 100)) ? 1 : 0;                
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
                    if (x < 0 || y < 0 || x > mapWidth - 1 || y > mapHeight - 1)
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
    
}
