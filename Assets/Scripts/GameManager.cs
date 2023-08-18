using UnityEngine;
/// <summary>
/// Represents different states of the game.
/// </summary>
public enum GameStatus
{
    Stick,   // Rocketball is attached to the stick.
    Fly,     // Rocketball is flying through the air.
    Death,   // The game is over due to some reason.
    Reset    // Resetting the game to its initial state.
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton instance of the GameManager.
    public GameObject Rocketball;        
    public Transform topboneend;         // the end of the rod on which the ball is attached
    public GameStatus currentStatus;   
    public GameObject Restart_UI;       

    Vector3 initialPosition;             // Initial position of the Rocketball.
    public StickController stickcontrol; 

    private void Awake()
    {
        // Singleton pattern implementation.
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

        // Set the initial game status when the game starts.
        SetGameStatus(GameStatus.Stick);
    }

    private void Update()
    {
        // Check for a specific key input to trigger game reset.
        if (Input.GetKey(KeyCode.B))
        {
            ResetGame();
        }
    }

    // Method to set the game status and perform necessary actions.
    public void SetGameStatus(GameStatus newStatus)
    {
        currentStatus = newStatus;

        // Actions based on the new game status.
        switch (newStatus)
        {
            case GameStatus.Stick:
                // Prepare for sticking phase.
                stickcontrol.enabled = false;
                stickcontrol.IsStickReleased = false;
                stickcontrol.enabled = true;
                Rocketball.transform.SetParent(topboneend);
                Restart_UI.SetActive(false);
                Time.timeScale = 1;
                break;

            case GameStatus.Fly:
                // Rocketball is in the flying phase.
                Debug.Log("Fly Status");
                Rocketball.GetComponent<Animator>().enabled = true;
                Rocketball.GetComponent<BallController>().enabled = true;
                BallController.rotatingToZero = false;
                BallController.isMoving = false;
                break;

            case GameStatus.Death:
                // The ball fell to the ground
                Debug.Log("Death Status");
                Time.timeScale = 0;
                Restart_UI.SetActive(true);
                break;

            case GameStatus.Reset:
                // Reset the game to its initial state.
                Debug.Log("Reset Status");
                Rocketball.transform.rotation = Quaternion.identity;
                Rocketball.transform.position = initialPosition;
                Rocketball.GetComponent<BallController>().TriggerWingAnimation(false);
                Rocketball.GetComponent<BallController>().enabled = false;
                Rocketball.GetComponent<Rigidbody>().isKinematic = true;
                StickController.Instance.ResetStick();
                break;

            default:
                break;
        }
    }

    // Method to change the game status.
    public void ChangeGameStatus(GameStatus newStatus)
    {
        SetGameStatus(newStatus);
    }

    public void ResetGame()
    {
        SetGameStatus(GameStatus.Reset);
    }
}
