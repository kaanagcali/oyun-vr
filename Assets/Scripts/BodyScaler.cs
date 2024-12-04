using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyScaler : MonoBehaviour
{
    public Camera head;
    public float DefaultHeight = 1.8f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        ScaleBody();
    }

    public void ScaleBody()
    {
        Debug.Log("height: " + head.transform.position.y);
        float headHeight = head.transform.position.y;
        float scale = headHeight / DefaultHeight;
        transform.localScale = Vector3.one * scale;
    }
}
