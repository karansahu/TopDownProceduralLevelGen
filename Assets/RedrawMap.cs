using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RedrawMap : MonoBehaviour
{
    public Button redraw;

    CellularAutomata[] sections;

    void Start()
    {
        sections = GameObject.FindObjectsOfType<CellularAutomata>();
    }


    public void Redraw()
    {
        foreach (CellularAutomata c in sections)
        {
            c.GenerateMap();
        }
    }
}
