using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    public float swipeForce = 5f;   // Hareket hızı
    public float rotationForce = 2f; // Dönme kuvveti
    public float rotationSmoothness = 5f; // Dönme yumuşaklığı
    public Animator wingsAnimator;   // Animator bileşeni (kanat animasyonları için)

    private Vector2 startPos;        // Dokunma başlangıç pozisyonu
    private bool isMoving = false;   // Top hareket ediyor mu?
    private Quaternion targetRotation; // Hedef rotasyon
    private bool rotatingToZero = false; // Sıfır rotasyonuna dönme sürecinde mi?
    private Rigidbody rb;
    public TrailRenderer trailleft, trailright;
    public static Vector3 ballstartpos;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        if (!isMoving && !rotatingToZero)
        {
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
                    Vector3 force = new Vector3(swipeDirection * swipeForce, 0f, 0.5f);

                    rb.AddForce(force, ForceMode.VelocityChange);
                    isMoving = true;

                    // Animator'ı kullanarak kanat animasyonunu tetikle
                    wingsAnimator.SetBool("OpenWing", true);
                    wingsAnimator.SetBool("CloseWing", false);
                    trailleft.enabled = true;
                    trailright.enabled = true;
                    rb.drag = 0f;
                    rotatingToZero = true;
                    targetRotation = Quaternion.Euler(Vector3.zero);

                    break;
                case TouchPhase.Ended:
                   
                    isMoving = false;
                   
               
                    // Animator'ı kullanarak kanat animasyonunu tetikle
                    wingsAnimator.SetBool("CloseWing", true);
                    rb.drag = 0.5f;
                    trailleft.enabled = false;
                    trailright.enabled = false;
                    wingsAnimator.SetBool("OpenWing", false);

                    rotatingToZero = false;
                    break;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cylinder"))
        {
            Debug.Log("cylinder Tag");
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 50f, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Cube"))
        {
            Debug.Log("Cube Tag");
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
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
