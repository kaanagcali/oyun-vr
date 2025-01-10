using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ReturnToPos : MonoBehaviour
{
    protected Vector3 startPos;
    protected Vector3 euler;


    private void Awake()
    {
        startPos = transform.position;
        euler = transform.eulerAngles;
    }
    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        if (leftHandDevices.Count > 0)
        {
            leftController = leftHandDevices[0];
        }

        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
        {
            rightController = rightHandDevices[0];
        }
    }

    void Update()
    {
        // Check if a button (like the primary button on the right controller) is pressed
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
        {
            /* transform.position = startPos;
            transform.eulerAngles = euler;
        GetComponent<Rigidbody>().velocity = Vector3.zero; */
        }

    }
}
