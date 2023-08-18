using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; 
    private Vector3 targetFollowOffset; // The desired follow offset for smooth reset.
    private Vector3 startVelocity; // Initial follow offset for resetting.
    private bool hasReset = false; // Flag to track if the camera has been reset.
    CinemachineTransposer transposer; 

    private void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>(); 
        startVelocity = transposer.m_FollowOffset; // Store the initial follow offset for resetting.
        Debug.Log("start velocity:" + startVelocity);
        targetFollowOffset = new Vector3(0, 5, -22); // Define the desired follow offset for smooth reset.
    }

    private void Update()
    {
       
        if (GameManager.instance.currentStatus == GameStatus.Fly && !hasReset)
        {
            hasReset = true; 
            SmoothReset(); 
        }

        if (GameManager.instance.currentStatus == GameStatus.Stick)
        {
            Resetcam(); // Reset the camera offset for sticking behavior.
        }

        if (hasReset)
        {
            SmoothReset(); // Continue the smooth reset process if it has been initiated.
        }
    }

    public void Resetcam()
    {
        Debug.Log("reset cam");
        transposer.m_FollowOffset = startVelocity; // Reset the camera follow offset instantly.
    }

    private void SmoothReset()
    {
        if (virtualCamera != null)
        {
            if (transposer != null)
            {
                // Smoothly interpolate the camera's follow offset towards the target follow offset.
                transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetFollowOffset, 0.2f);
            }
        }
    }
}
