using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScaler : MonoBehaviour
{
    public float delta = 1;
    void Update()
    {
        transform.localPosition = transform.localPosition + new Vector3(0, delta * Time.deltaTime, 0);  
    }
}
