using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float smoothingSpeed = 1.0f; // Daha düşük değer daha yavaş geçiş sağlar
    private Vector3 targetFollowOffset;
    private Vector3 currentVelocity; // SmoothDamp için gerekli değişken
    private bool hasReset = false;

    private void Start()
    {
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

        if (hasReset)
        {
            SmoothReset();
        }
    }


    private void SmoothReset()
    {
        if (virtualCamera != null)
        {
            CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
               // transposer.m_FollowOffset = Vector3.SmoothDamp(transposer.m_FollowOffset, targetFollowOffset, ref currentVelocity, smoothingSpeed);
                transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, new Vector3(0, 5, -22), .1f);

               
            }
        }
    }
}