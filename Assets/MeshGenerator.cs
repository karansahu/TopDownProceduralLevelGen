using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<int> traingles;
    private SquareGrid squareGrid;
    public GameObject MeshContainer;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        traingles = new List<int>();

        for (int i = 0; i < squareGrid.squares.GetLength(0); i++)
        {
            for (int j = 0; j < squareGrid.squares.GetLength(1); j++)
            {
                TriangulateSquare(squareGrid.squares[i, j]);
            }
        }

        Mesh mesh = new Mesh();
        MeshContainer.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = traingles.ToArray();
        mesh.RecalculateNormals();
    }

    private void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 2:
                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBottom);
                break;
            case 4:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            TriangulateMesh(points[0],points[1],points[2]);
        if (points.Length >= 4)
            TriangulateMesh(points[0], points[2], points[3]);
        if (points.Length >= 5)
            TriangulateMesh(points[0], points[3], points[4]);
        if (points.Length >= 6)
            TriangulateMesh(points[0], points[4], points[5]);
    }

    private void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    private void TriangulateMesh(Node a, Node b, Node c)
    {
        traingles.Add(a.vertexIndex);
        traingles.Add(b.vertexIndex);
        traingles.Add(c.vertexIndex);
    }

    public class SquareGrid
    {
        public Square[,] squares;
        public SquareGrid(int [,] map, float squareSize)
        {
            int nodeCountX  = map.GetLength(0);
            int nodeCountY  = map.GetLength(1);
            float mapWidth  = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int i = 0; i < nodeCountX; i++)
            {
                for (int j = 0; j < nodeCountY; j++)
                {
                    controlNodes[i, j] = new ControlNode(new Vector3(-mapWidth / 2 + i * squareSize + squareSize / 2, 0, -mapHeight / 2 + j * squareSize + squareSize / 2), map[i, j] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int i = 0; i < nodeCountX - 1; i++)
            {
                for (int j = 0; j < nodeCountY - 1; j++)
                {
                    squares[i,j] = new Square(controlNodes[i, j + 1], controlNodes[i + 1, j + 1], controlNodes[i, j], controlNodes[i + 1, j]);
                }
            }
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomLeft, bottomRight;
        public Node centerLeft, centerRight, centerTop, centerBottom;
        public int configuration;
        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomLeft, ControlNode _bottomRight)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;

            centerLeft = bottomLeft.above;
            centerBottom = bottomLeft.right;
            centerRight = bottomRight.above;
            centerTop = topLeft.right;

            if (topLeft.wall)
                configuration += 8;
            if (topRight.wall)
                configuration += 4;
            if (bottomRight.wall)
                configuration += 2;
            if (bottomLeft.wall)
                configuration += 1;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;
        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }

    public class ControlNode : Node
    {
        public bool wall;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _wall, float squareSize) : base(_pos)
        {
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
            wall = _wall;
        }
    }
    /*
    void OnDrawGizmos()
    {
        if(squareGrid != null)
        {
            int lengthX = squareGrid.squares.GetLength(0);
            int lengthY = squareGrid.squares.GetLength(1);

            for (int i = 0; i < lengthX; i++)
            {
                for (int j = 0; j < lengthY; j++)
                {
                    Gizmos.color = (squareGrid.squares[i, j].topLeft.wall) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i, j].topLeft.position + transform.position, 0.5f * Vector3.one);

                    Gizmos.color = (squareGrid.squares[i, j].topRight.wall) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i, j].topRight.position + transform.position, 0.5f * Vector3.one);

                    Gizmos.color = (squareGrid.squares[i, j].bottomLeft.wall) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i, j].bottomLeft.position + transform.position, 0.5f * Vector3.one);

                    Gizmos.color = (squareGrid.squares[i, j].bottomRight.wall) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i, j].bottomRight.position + transform.position, 0.5f * Vector3.one);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[i, j].centerTop.position + transform.position, 0.15f * Vector3.one);
                    Gizmos.DrawCube(squareGrid.squares[i, j].centerBottom.position + transform.position, 0.15f * Vector3.one);
                    Gizmos.DrawCube(squareGrid.squares[i, j].centerLeft.position + transform.position, 0.15f * Vector3.one);
                    Gizmos.DrawCube(squareGrid.squares[i, j].centerRight.position + transform.position, 0.15f * Vector3.one);
                }
            }
        }
    }
    */
    
}
