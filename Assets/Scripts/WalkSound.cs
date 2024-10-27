using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    protected Vector3 lastPos;
    protected AudioSource audioSource;
    public float distToPlaySound = 0.01f;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        lastPos = transform.position;
    }

    private void Update() {
        if (Vector3.Distance(lastPos, transform.position) > distToPlaySound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }

        lastPos = transform.position;
    }
}
