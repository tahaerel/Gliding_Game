using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private Vector3 targetFollowOffset;
    private Vector3 startVelocity; // SmoothDamp için gerekli değişken
    private bool hasReset = false;
    CinemachineTransposer transposer;

    private void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        startVelocity = transposer.m_FollowOffset;
        Debug.Log("start velocity:"+startVelocity);
        targetFollowOffset = new Vector3(0, 5, -22);
    }

    private void Update()
    {
        // Örnek olarak GameManager.instance.currentStatus kontrol ediliyor
        if (GameManager.instance.currentStatus == GameStatus.Fly && !hasReset)
        {
            hasReset = true;
            SmoothReset();
        }

        if (GameManager.instance.currentStatus == GameStatus.Stick)
        {
            Resetcam();
        }

        if (hasReset)
        {
            SmoothReset();
        }

    }

    public void Resetcam()
    {
        Debug.Log("reset cam");
        transposer.m_FollowOffset = startVelocity;
    }

    private void SmoothReset()
    {
        if (virtualCamera != null)
        {
           
            if (transposer != null)
            {
               // transposer.m_FollowOffset = Vector3.SmoothDamp(transposer.m_FollowOffset, targetFollowOffset, ref currentVelocity, smoothingSpeed);
                transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, new Vector3(0, 5, -22), .2f);

            }
        }
    }
}