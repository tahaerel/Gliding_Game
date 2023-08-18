using UnityEngine;
/// <summary>
/// Controls the behavior of the ball in the game.
/// </summary>
public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; } // Singleton instance of the BallController.

    public float swipeForce = 5f; 
    public float rotationForce = 2f; 
    public float rotationSmoothness = 10f; 
    public Animator wingsAnimator; // Animator component for wing animations.
    private Vector2 startPos; // Starting position of touch input.
    private Quaternion targetRotation; // Target rotation for smooth rotation.
    public static bool isMoving = false; // Is the ball currently moving?
    public static bool rotatingToZero = false; // Is the ball currently rotating back to zero rotation?
    private Rigidbody rb; 
    public TrailRenderer trailleft, trailright; 
    public static Vector3 ballstartpos; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component on start.
    }

    private void Update()
    {
        Debug.Log("moving=" + isMoving + "  " + "rotatingzero" + rotatingToZero);

        if (!isMoving && !rotatingToZero)
        {
            Debug.Log("ROTATE");
            rb.AddTorque(Vector3.right * rotationForce); // Apply rotational force when not moving.
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch input.

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position; // Store the starting touch position.
                    break;

                case TouchPhase.Moved:
                    Vector2 swipeDelta = touch.position - startPos; // Calculate swipe distance.
                    float swipeDirection = Mathf.Sign(swipeDelta.x); // Swipe direction (right: 1, left: -1)

                    // Calculate force based on swipe direction, only considering the x-axis.
                    Vector3 force = new Vector3(swipeDirection * swipeForce, 0f, 0.05f);

                    rb.AddForce(force, ForceMode.VelocityChange); // Apply force to move the ball.
                    isMoving = true;

                    // Trigger wing animation using the Animator.
                    TriggerWingAnimation(true);

                    break;

                case TouchPhase.Stationary:
                    // This case handles when the player simply touches the screen without moving.
                    // Perform the same actions as when the player swipes.
                    Vector3 stationaryForce = new Vector3(0f, 0f, 0.2f);
                    rb.AddForce(stationaryForce, ForceMode.VelocityChange);
                    isMoving = true;
                    TriggerWingAnimation(true);

                    break;

                case TouchPhase.Ended:
                    isMoving = false;

                    // Trigger wing animation to close wings.
                    TriggerWingAnimation(false);

                    rotatingToZero = false; 
                    break;
            }
        }
    }

    /// <summary>
    /// Triggers the wing animation based on the provided open parameter.
    /// </summary>
    /// <param name="open">Whether the wings should be open or closed.</param>
    public void TriggerWingAnimation(bool open)
    {
        wingsAnimator.SetBool("OpenWing", open); 
        wingsAnimator.SetBool("CloseWing", !open); 
        trailleft.enabled = open; 
        trailright.enabled = open; 
        rb.drag = open ? 1f : 0.2f; // Adjust the drag based on wing status.

        if (open)
        {
            rotatingToZero = true; 
            targetRotation = Quaternion.Euler(Vector3.zero); // Set the target rotation for smooth rotation.
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cylinder"))
        {
            // Handle collision with objects tagged as "Cylinder".
            // Apply upward and forward forces to simulate bouncing.
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 30f, ForceMode.Impulse);
            rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Cube"))
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 60f, ForceMode.Impulse);
            rb.AddForce(Vector3.forward * 20f, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Plane"))
        {
            GameManager.instance.ChangeGameStatus(GameStatus.Death);
        }
    }

    private void FixedUpdate()
    {
        if (rotatingToZero)
        {
            // Smoothly interpolate the rotation towards the target rotation.
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime));
        }
    }
}
