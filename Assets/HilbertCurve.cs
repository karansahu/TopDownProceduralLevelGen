using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HilbertCurve : MonoBehaviour
{
    public int dist0 = 512;
    public int dist;
    public int _level;
    private int x = 0, y = 0;
    public List<Vector3> curvePoints = new List<Vector3>();
    public float lineDisplayTime;

    public int colorIndex = 0;
    void Start()
    {
        dist = dist0;
        goToXY(x, y);
        Paint(_level);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            dist = dist0;
            goToXY(x, y);
            Paint(_level);
        }

    }

    public void goToXY(int x , int y)
    {
        this.x = x;
        this.y = y;
    }

    public void lineRel(int deltaX, int deltaY)
    {
        Vector3 p1 = new Vector3(x,0,y) + gameObject.transform.position;
        Vector3 p2 = new Vector3(x + deltaX,0, y + deltaY) + gameObject.transform.position;
        Debug.Log("p1" + p1 + "p2" + p2);
        Debug.DrawLine(p1,p2,Color.blue, lineDisplayTime);        
        x += deltaX;
        y += deltaY;
        if (!curvePoints.Contains(p1))
            curvePoints.Add(p1);
        if (!curvePoints.Contains(p2))
            curvePoints.Add(p2);
    }

    //void OnDrawGizmos()
    //{
    //    if(curvePoints != null)
    //    {
    //        foreach (Vector3 point in curvePoints)
    //        {
    //            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //            cube.name = curvePoints.IndexOf(point).ToString();
    //            cube.transform.localScale = Vector3.one;
    //            cube.transform.position = point;
    //        }
    //            //Gizmos.DrawCube(point, 0.8f * Vector3.one);
    //    }
    //}

    public void Paint(int L)
    {
        int level = L;
        dist = dist0;
        for (int i = level; i > 0; i--)
        {
            dist /= 2;
            goToXY(dist / 2, dist / 2);
        }
        HilbertA(L);
    }

    private void HilbertA(int level)
    {
        if (level > 0)
        {
            HilbertB(level - 1);
            lineRel(0, dist);

            HilbertA(level - 1);
            lineRel(dist, 0);

            HilbertA(level - 1);
            lineRel(0, -dist);

            HilbertC(level - 1);
        }
    }

    private void HilbertB(int level)
    {
        if (level > 0)
        {
            HilbertA(level - 1);
            lineRel(dist, 0);

            HilbertB(level - 1);
            lineRel(0, dist);

            HilbertB(level - 1);
            lineRel(-dist, 0);

            HilbertD(level - 1);
        }
    }

    private void HilbertC(int level)
    {
        if (level > 0)
        {
            HilbertD(level - 1);
            lineRel(-dist, 0);

            HilbertC(level - 1);
            lineRel(0, -dist);

            HilbertC(level - 1);
            lineRel(dist, 0);

            HilbertA(level - 1);
        }
    }

    private void HilbertD(int level)
    {
        if (level > 0)
        {
            HilbertC(level - 1);
            lineRel(0, -dist);

            HilbertD(level - 1);
            lineRel(-dist, 0);

            HilbertD(level - 1);
            lineRel(0, dist);

            HilbertB(level - 1);
        }
    }
}
