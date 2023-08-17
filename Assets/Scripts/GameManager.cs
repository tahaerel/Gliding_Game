using UnityEngine;

public enum GameStatus
{
    Stick,
    Fly,
    Death
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Rocketball;
    public GameStatus currentStatus;
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
        // Baþlangýçta oyun durumunu 
        SetGameStatus(GameStatus.Stick);
    }

    public void SetGameStatus(GameStatus newStatus)
    {
        currentStatus = newStatus;

        // Durum deðiþikliðine göre yapýlmasý gerekenleri burada uygula
        switch (newStatus)
        {
            case GameStatus.Stick:
                // Stick durumuyla 
                break;
            case GameStatus.Fly:
                // Fly durumu
                Rocketball.GetComponent<Animator>().enabled = true;
                Rocketball.GetComponent<BallController>().enabled = true;
                break;
            case GameStatus.Death:
                // Death durumu
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

}
