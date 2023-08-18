using UnityEngine;

public enum GameStatus
{
    Stick,
    Fly,
    Death,
    Reset
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Rocketball;
    public Transform topboneend;
    public GameStatus currentStatus;
    public GameObject Restart_UI;
    Vector3 initialPosition; //rocketball position
    public StickController stickcontrol;

    public void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            ResetGame();
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        initialPosition = Rocketball.transform.position;
        Debug.Log("RocketBall's Initial Position: " + initialPosition);

        // Gamestatus when start
       SetGameStatus(GameStatus.Stick);
    }

    public void SetGameStatus(GameStatus newStatus)
    {
        currentStatus = newStatus;

        // Durum deðiþikliðine göre yapýlmasý gerekenleri burada uygula
        switch (newStatus)
        {
            case GameStatus.Stick:
                stickcontrol.enabled = false;
                stickcontrol.IsStickReleased = false;
                stickcontrol.enabled = true;
                Rocketball.transform.SetParent(topboneend);
                Restart_UI.SetActive(false);
                Time.timeScale = 1;
                break;

            case GameStatus.Fly:
             
                Debug.Log("Fly Status");
                Rocketball.GetComponent<Animator>().enabled = true;
                Rocketball.GetComponent<BallController>().enabled = true;
                break;

            case GameStatus.Death:
                Debug.Log("Death Status");
                Time.timeScale = 0;
                Restart_UI.SetActive(true);
                break;

            case GameStatus.Reset:
                Debug.Log("Reset Status");
                Rocketball.transform.rotation = Quaternion.identity;
                Rocketball.transform.position = initialPosition;
                Rocketball.GetComponent<Animator>().Play("Armature|2_Close_wings");
                Rocketball.GetComponent<BallController>().enabled = false;
                Rocketball.GetComponent<Rigidbody>().isKinematic = true;
                StickController.Instance.ResetStick();
                break;

            default:
                break;
        }
    }
    //  metodu çaðýr
    public void ChangeGameStatus(GameStatus newStatus)
    {
        SetGameStatus(newStatus);
    }
    public void ResetGame()
    {
        SetGameStatus(GameStatus.Reset);
    }
}
