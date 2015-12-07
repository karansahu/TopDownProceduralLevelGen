using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapHandler : MonoBehaviour
{
    System.Random rand = new System.Random();

    public int[,] Map;

    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public int PercentAreWalls { get; set; }

    private Texture2D texRandom;
    private Texture2D texCavern;

    private GameObject randomGO;
    private GameObject cavernGO;

    public int _MapWidth,_MapHeight, cavernIterations, _percentAreWalls, wallLimit,floorLimit;
    //public string randomImageName, cavernImageName;

    void Start()
    {
        MapWidth = _MapWidth;
        MapHeight = _MapHeight;
        PercentAreWalls = _percentAreWalls;

        randomGO = GameObject.Find("Random");
        cavernGO = GameObject.Find("Cavern");

        texRandom = new Texture2D(MapWidth, MapHeight, TextureFormat.RGB24, true);
        texRandom.name = "TextureRandom";
        texRandom.wrapMode = TextureWrapMode.Clamp;

        texCavern = new Texture2D(MapWidth, MapHeight, TextureFormat.RGB24, true);
        texCavern.name = "TextureCavern";
        texCavern.wrapMode = TextureWrapMode.Clamp;

        RandomFillMap();
        CreateTextureRandom();
        MakeCaverns();
        CreateTextureCavern();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MapWidth = _MapWidth;
            MapHeight = _MapHeight;
            PercentAreWalls = _percentAreWalls;

            texRandom = new Texture2D(MapWidth, MapHeight, TextureFormat.RGB24, true);
            texRandom.name = "TextureRandom";
            texRandom.wrapMode = TextureWrapMode.Clamp;

            texCavern = new Texture2D(MapWidth, MapHeight, TextureFormat.RGB24, true);
            texCavern.name = "TextureCavern";
            texCavern.wrapMode = TextureWrapMode.Clamp;

            RandomFillMap();
            CreateTextureRandom();
            MakeCaverns();
            CreateTextureCavern();
        }
    }
    private void CreateTextureRandom()
    {
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Map[i, j] == 0)
                    texRandom.SetPixel(i, j, new Color(1, 1, 1, 1));
                else if (Map[i, j] == 1)
                    texRandom.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }
        texRandom.Apply(false);
        randomGO.GetComponent<Renderer>().material.SetTexture("_MainTex", texRandom);

        //save image
        //byte[] data = texRandom.EncodeToPNG();
        //System.IO.File.WriteAllBytes(Application.dataPath + "/../randomImageName.png", data);
    }

    private void CreateTextureCavern()
    {
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if(Map[i,j] == 0)
                    texCavern.SetPixel(i, j, new Color(1, 1, 1, 1));
                else if (Map[i, j] == 1)
                    texCavern.SetPixel(i, j, new Color(0,0,0,0));
            }
        }
        texCavern.Apply(false);
        cavernGO.GetComponent<Renderer>().material.SetTexture("_MainTex", texCavern);
        //Save Image
        string imageName = string.Format("Cavern W{0} H{1} I{2} %{3} WL{4} FL{5}", _MapWidth,_MapHeight,cavernIterations,_percentAreWalls,wallLimit,floorLimit);
        byte[] data = texCavern.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/../" + imageName + ".png", data);
    }

    public void MakeCaverns()
    {
        //WHY
        // By initilizing column in the outter loop, its only created ONCE  
        for (int k = 0; k < cavernIterations; k++)
        {
            for (int column = 0, row = 0; row <= MapHeight - 1; row++)
            {
                for (column = 0; column <= MapWidth - 1; column++)
                {
                    Map[column, row] = PlaceWallLogic(column, row);
                }
            }
        }
    }

    public int PlaceWallLogic(int x, int y)
    {
        int numWalls = GetAdjacentWalls(x, y, 1, 1);


        if (Map[x, y] == 1)
        {
            if (numWalls >= wallLimit)
            {
                return 1;
            }
            //if (numWalls < 2)
            //{
            //    return 0;
            //}
            return 0;
        }
        else
        {
            //limit for surrounding blocks. If they are walls and im not
            if (numWalls >= floorLimit)
            {
                return 1;
            }
        }
        return 0;
    }

    public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
    {
        int startX = x - scopeX;
        int startY = y - scopeY;
        int endX = x + scopeX;
        int endY = y + scopeY;

        int iX = startX;
        int iY = startY;

        int wallCounter = 0;

        for (iY = startY; iY <= endY; iY++)
        {
            for (iX = startX; iX <= endX; iX++)
            {
                if (!(iX == x && iY == y))
                {
                    if (IsWall(iX, iY))
                    {
                        wallCounter += 1;
                    }
                }
            }
        }
        return wallCounter;
    }

    bool IsWall(int x, int y)
    {
        // Consider out-of-bound a wall
        if (IsOutOfBounds(x, y))
        {
            return true;
        }

        if (Map[x, y] == 1)
        {
            return true;
        }

        if (Map[x, y] == 0)
        {
            return false;
        }
        return false;
    }

    bool IsOutOfBounds(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return true;
        }
        else if (x > MapWidth - 1 || y > MapHeight - 1)
        {
            return true;
        }
        return false;
    }

    //public void PrintMap()
    //{
    //    Console.Clear();
    //    Console.Write(MapToString());
    //}

    string MapToString()
    {
        string[] temp = new string[4];

        temp[0] = "Width:" + MapWidth.ToString();
        temp[1] = "\tHeight:" + MapHeight.ToString();
        temp[2] = "\t% Walls:" + PercentAreWalls.ToString();
        temp[3] = "\n";
                     
        string returnString = string.Join(" ", temp);

        List<string> mapSymbols = new List<string>();
        mapSymbols.Add(".");
        mapSymbols.Add("#");
        mapSymbols.Add("+");

        for (int column = 0, row = 0; row < MapHeight; row++)
        {
            for (column = 0; column < MapWidth; column++)
            {
                returnString += mapSymbols[Map[column, row]];
            }
            returnString += "\n";
        }
        return returnString;
    }

    public void BlankMap()
    {
        for (int column = 0, row = 0; row < MapHeight; row++)
        {
            for (column = 0; column < MapWidth; column++)
            {
                Map[column, row] = 0;
            }
        }
    }

    public void RandomFillMap()
    {
        // New, empty map
        Map = new int[MapWidth, MapHeight];

        int mapMiddle = 0; // Temp variable
        for (int column = 0, row = 0; row < MapHeight; row++)
        {
            for (column = 0; column < MapWidth; column++)
            {
                // If coordinants lie on the the edge of the map (creates a border)
                if (column == 0)
                {
                    Map[column, row] = 1;
                }
                else if (row == 0)
                {
                    Map[column, row] = 1;
                }
                else if (column == MapWidth - 1)
                {
                    Map[column, row] = 1;
                }
                else if (row == MapHeight - 1)
                {
                    Map[column, row] = 1;
                }
                // Else, fill with a wall a random percent of the time
                else
                {
                    mapMiddle = (MapHeight / 2);

                    if (row == mapMiddle)
                    {
                        Map[column, row] = 0;
                    }
                    else
                    {
                        Map[column, row] = RandomPercent(PercentAreWalls);
                    }
                }
            }
        }
    }

    int RandomPercent(int percent)
    {
        if (percent >= rand.Next(0, 100))
        {
            return 1;
        }
        return 0;
    }

    //public MapHandler(int mapWidth, int mapHeight, int[,] map, int percentWalls = 40)
    //{
    //    this.MapWidth = mapWidth;
    //    this.MapHeight = mapHeight;
    //    this.PercentAreWalls = percentWalls;
    //    this.Map = new int[this.MapWidth, this.MapHeight];
    //    this.Map = map;
    //}
}