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
    public bool IsStickReleased = false;
    int a = 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        // Set up the initial state of the stick animation
        StickAnimation["Armature|Bend_Stick"].speed = 0;
        StickAnimation["Armature|Bend_Stick"].time = 0;
        StickAnimation.Play("Armature|Bend_Stick");
    }

    private void Update()
    {
        // Update the stick bending logic if the stick is not released and the game status is Stick
        if (!IsStickReleased &&  GameManager.instance.currentStatus == GameStatus.Stick)
        {
            HandleStickBending();
        }
    }

    public void HandleStickBending()
    {
        if (Input.touchCount > 0 )
        {            
            // Get the first touch
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Store the initial touch position
                    initialTouchPosition = touch.position.x;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    // Calculate the amount and direction of swiping
                    swipeDelta = touch.position.x - initialTouchPosition;
                    swerveAmount = swipeDirection.x * 0.01f;
                    swipeDirection.x = swipeDelta * SlideSpeed;
                    if (swipeDirection.x < 0)
                    {                        
                        // Adjust the stick animation frame based on the swipe amount
                        AdjustStickAnimationFrame(Mathf.Abs(swerveAmount) * StickAnimation["Armature|Bend_Stick"].length);
                    }
                    break;

                case TouchPhase.Ended:
                    // Adjust the stick animation frame
                    AdjustStickAnimationFrame(Mathf.Abs(swerveAmount) * StickAnimation["Armature|Bend_Stick"].length);
                    if (StickAnimation["Armature|Bend_Stick"].time > 0.037f)
                    {                       
                        // Calculate release force based on swerve amount
                        ReleaseForce = Mathf.Clamp(Mathf.Abs(swerveAmount*10), 1f, 10f);
                        Debug.Log("release 1 =" + ReleaseForce);

                        // Release the ball and play the release animation
                        FlightBall();
                        swipeDelta = 0;
                        StickAnimation["Armature|Bend_Stick"].speed = 1;
                        StickAnimation.Play("Armature|Release_Stick");
                        IsStickReleased = true;
                        a++;
                    }
                    break;
            }
        }
    }
    public void FlightBall()
    {    
        // Calculate the throw direction based on the swipe direction
        Vector3 throwDirection = -swipeDirection.normalized;
        if (ballRigidbody != null)
        {           
            // Apply the throw direction and forces to the ball
            ballRigidbody.velocity = throwDirection;
            ballRigidbody.isKinematic = false;

            Debug.Log("fırlatma"+"release force = "+ReleaseForce);
            GameManager.instance.ChangeGameStatus(GameStatus.Fly);
            ballRigidbody.gameObject.transform.parent = null;
            ballRigidbody.AddForce(transform.up* 10 * ReleaseForce, ForceMode.Impulse);
            ballRigidbody.AddForce(transform.forward * 10 * ReleaseForce, ForceMode.Impulse);
        }
    }

    // Adjust the stick animation frame and pause the animation
    public void AdjustStickAnimationFrame(float time)
    {
        StickAnimation["Armature|Bend_Stick"].time = time;
        StickAnimation["Armature|Bend_Stick"].speed = 0;
        StickAnimation.Play("Armature|Bend_Stick");
    }

    // Reset the stick animation to its initial state and set the game status to Stick
    public void ResetStick()
    {  
       StickAnimation["Armature|Bend_Stick"].speed = 0;
       StickAnimation["Armature|Bend_Stick"].time = 0;
       StickAnimation.Play("Armature|Bend_Stick");
       GameManager.instance.SetGameStatus(GameStatus.Stick);
    }
}
