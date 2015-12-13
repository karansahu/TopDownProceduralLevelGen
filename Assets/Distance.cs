using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Distance : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    private Text text;
    
    void Update()
    {
        if(text == null)
            text = pointA.GetComponent<Text>();
        if(pointA != null && pointB != null)
            text.text = Vector3.Distance(pointA.transform.position, pointB.transform.position).ToString();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pointA != null && pointB != null)
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}
