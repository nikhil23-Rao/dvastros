using UnityEngine;
using TMPro;

public class StopwatchTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timer; // Reference to the TextMeshPro Text
    private float elapsedTime = 0f;             // Elapsed time in seconds
    private bool isRunning = false;            // Is the timer running?

    void Start()
    {
        StartTimer(); // Automatically start the timer
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime; // Add time since the last frame
            UpdateTimerDisplay(elapsedTime);
        }
    }

    // Start the timer
    public void StartTimer()
    {
        isRunning = true;
    }

    // Stop the timer
    public void StopTimer()
    {
        isRunning = false;
    }

    // Reset the timer
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay(elapsedTime);
    }

    // Update the timer display
    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f); // Convert seconds to minutes
        int seconds = Mathf.FloorToInt(time % 60f); // Remaining seconds
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f); // Remaining milliseconds

        timer.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}"; // Format the time
    }
}