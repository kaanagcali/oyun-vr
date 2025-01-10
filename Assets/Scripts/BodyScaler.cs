using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyScaler : MonoBehaviour
{
    public Camera head;
    public float DefaultHeight = 1.8f;
    public float MinHeight = 1.2f;


    void Update()
    {
        ScaleBody();
    }

    public void ScaleBody()
    {
        Debug.Log("height: " + head.transform.position.y);
        float headHeight = head.transform.position.y;
        float t = headHeight - MinHeight;
        float d = DefaultHeight - MinHeight;
        float scale = t / d;
        transform.localScale = Vector3.one * scale;
    }
}
