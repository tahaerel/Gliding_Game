using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    public float swipeForce = 5f;   // Hareket hızı
    public float rotationForce = 2f; // Dönme kuvveti
    public float rotationSmoothness = 10f; // Dönme yumuşaklığı
    public Animator wingsAnimator;   // Animator bileşeni (kanat animasyonları için)
    private Vector2 startPos;        // Dokunma başlangıç pozisyonu
    private Quaternion targetRotation; // Hedef rotasyon
    public static bool isMoving = false;   // Top hareket ediyor mu?
    public static bool rotatingToZero = false; // Sıfır rotasyonuna dönme sürecinde mi?
    private Rigidbody rb;
    public TrailRenderer trailleft, trailright;
    public static Vector3 ballstartpos;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.Log("moving="+isMoving +"  "+"rotatingzero" + rotatingToZero);

        if (!isMoving && !rotatingToZero)
        {
            Debug.Log("ROTATE");
            rb.AddTorque(Vector3.right * rotationForce);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 swipeDelta = touch.position - startPos;
                    float swipeDirection = Mathf.Sign(swipeDelta.x); // Hareket yönü (sağ: 1, sol: -1)

                    // Yalnızca x düzleminde hareket etmek için sadece x bileşenini kullan
                    Vector3 force = new Vector3(swipeDirection * swipeForce, 0f, 0.1f);

                    rb.AddForce(force, ForceMode.VelocityChange);
                    isMoving = true;

                    // Animator'ı kullanarak kanat animasyonunu tetikle
                    TriggerWingAnimation(true);

                    break;

                case TouchPhase.Stationary: // This case handles when the player simply touches the screen without moving
                                            // Perform the same actions as when the player swipes
                    Vector3 stationaryForce = new Vector3(0f, 0f, 0.2f);
                    rb.AddForce(stationaryForce, ForceMode.VelocityChange);
                    isMoving = true;
                    TriggerWingAnimation(true);

                    break;

                case TouchPhase.Ended:
                    isMoving = false;

                    // Animator'ı kullanarak kanat animasyonunu tetikle
                    TriggerWingAnimation(false);

                    rotatingToZero = false;
                    break;
            }
        }
    }

    public void TriggerWingAnimation(bool open)
    {
        wingsAnimator.SetBool("OpenWing", open);
        wingsAnimator.SetBool("CloseWing", !open);
        trailleft.enabled = open;
        trailright.enabled = open;
        rb.drag = open ? 0f : 0.5f;
        if (open)
        {
            rotatingToZero = true;
            targetRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cylinder"))
        {
            Debug.Log("cylinder Tag");
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 30f, ForceMode.Impulse);
            rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Cube"))
        {
            Debug.Log("Cube Tag");
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 60f, ForceMode.Impulse);
            rb.AddForce(Vector3.forward * 20f, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Plane"))
        {
            Debug.Log("Plane Tag");
            GameManager.instance.ChangeGameStatus(GameStatus.Death);
        }
    }
    private void FixedUpdate()
    {
        if (rotatingToZero)
        {
            // Dönme yumuşaklığı ile hedef rotasyona doğru dönmeyi sağlar
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime));
        }
    }
  
}
