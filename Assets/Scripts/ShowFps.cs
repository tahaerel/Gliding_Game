using UnityEngine;
using TMPro;
/// <summary>
/// Updates a TextMeshProUGUI component to display the frames per second (FPS).
/// </summary>
public class ShowFps : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float updateInterval = 0.5f;

    private float accumulatedFrames = 0f;
    private float elapsedTime = 0f;

    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        accumulatedFrames++;
        elapsedTime += deltaTime;

        if (elapsedTime >= updateInterval)
        {
            float fps = accumulatedFrames / elapsedTime;
            fpsText.text = "FPS: " + Mathf.RoundToInt(fps).ToString();

            accumulatedFrames = 0f;
            elapsedTime = 0f;
        }
    }
}
