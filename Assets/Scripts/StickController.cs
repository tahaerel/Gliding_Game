using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{
    public static StickController Instance { get; private set; }

    public Animation StickAnimation;
    public float SlideSpeed = 0.1f;
    public float ReleaseForce = 0f;

    private Vector3 swipeDirection;
    private float initialTouchPosition;
    private float swipeDelta;
    private float swerveAmount = 0.0f;
    public Rigidbody ballRigidbody;
    public bool IsStickReleased { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       

        Application.targetFrameRate = 60;

        StickAnimation["Armature|Bend_Stick"].speed = 0;
        StickAnimation["Armature|Bend_Stick"].time = 0;
        StickAnimation.Play("Armature|Bend_Stick");
    }

    private void Update()
    {
        if (GameManager.instance.currentStatus == GameStatus.Stick) // Sadece Stick durumunda çalış
        {
            if (!IsStickReleased)
            {
                HandleStickBending();
            }
        
        }
    }

    public void HandleStickBending()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    initialTouchPosition = touch.position.x;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    swipeDelta = touch.position.x - initialTouchPosition;
                    swerveAmount = swipeDirection.x * 0.01f;
                    swipeDirection.x = swipeDelta * SlideSpeed;
                    if (swipeDirection.x < 0)
                    {
                        AdjustStickAnimationFrame(Mathf.Abs(swerveAmount) * StickAnimation["Armature|Bend_Stick"].length);
                    }
                    break;

                case TouchPhase.Ended:
                  
                    AdjustStickAnimationFrame(Mathf.Abs(swerveAmount) * StickAnimation["Armature|Bend_Stick"].length);
                    if (StickAnimation["Armature|Bend_Stick"].time > 0.037f)
                    {
                        ReleaseForce = Mathf.Clamp(Mathf.Abs(swerveAmount*10), 1f, 10f);
                        Debug.Log("release 1 =" + ReleaseForce);
                        FlightBall();
                        swipeDelta = 0;
                        StickAnimation["Armature|Bend_Stick"].speed = 1;
                        StickAnimation.Play("Armature|Release_Stick");
                        IsStickReleased = true;
                    }
                    break;
            }
        }
    }
    public void FlightBall()
    {

        Vector3 throwDirection = -swipeDirection.normalized;
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = throwDirection;
            ballRigidbody.isKinematic = false;

            Debug.Log("fırlatma"+"release force = "+ReleaseForce);
            GameManager.instance.ChangeGameStatus(GameStatus.Fly);
            ballRigidbody.gameObject.transform.parent = null;
            ballRigidbody.AddForce(transform.up* 10 * ReleaseForce, ForceMode.Impulse);
            ballRigidbody.AddForce(transform.forward * 10 * ReleaseForce, ForceMode.Impulse);



        }
    }
    public void AdjustStickAnimationFrame(float time)
    {
        StickAnimation["Armature|Bend_Stick"].time = time;
        StickAnimation["Armature|Bend_Stick"].speed = 0;
        StickAnimation.Play("Armature|Bend_Stick");
    }

    public void ResetStick()
    {
        IsStickReleased = false;
        StickAnimation["Armature|Bend_Stick"].speed = 0;
        StickAnimation["Armature|Bend_Stick"].time = 0;
        StickAnimation.Play("Armature|Bend_Stick");
    }
}
