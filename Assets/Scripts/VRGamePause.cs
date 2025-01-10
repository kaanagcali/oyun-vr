using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class VRGamePause : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;
    private InputDevice device;

    public Transform originTransform;
    protected Vector3 stoppedPos;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand); // Use the desired controller
    }

    bool isPressing = false;

    void Update()
    {
        if (!GameManager.Instance) return;


        if (!GameManager.Instance.IsGameStarted) return;

        if (GameManager.Instance.IsGameEnd) return;

        if (device != null && device.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPressed) && menuButtonPressed)
        {
            if (!isPressing)
            {
                isPressing = true;
                TogglePause();
            }
        } else {
            if (isPressing) {
                isPressing = false;
            }
        }

        if (isPaused)
        {
            originTransform.position = stoppedPos;
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        isPaused = false;
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            stoppedPos = originTransform.position;
        }
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;
        if (isPaused) {
            InGameUIManager.Instance.OpenUI();
        } else {
            InGameUIManager.Instance.CloseUI();
        }
    }

    public void ContinueGame()
    {
        
    }
}
