using UnityEngine;
using UnityEngine.UI;

namespace Avinash.ArcheryGame
{
    public class ScoreManager : MonoBehaviour
    {
        public static int CurrentScore { get; private set; } = 0;
        public static int CurrentCombo { get; private set; } = 0; // Current combo count
        public static int MaxCombo { get; private set; } = 0; // Maximum combo count achieved

        private static int lastHitScore = 0;

        [SerializeField] private Text scoreText;
        public Text comboText; // Reference to the UI Text

        private void Start()
        {
            UpdateScoreUI();
        }
        private void Update()
        {
            // Check if the arrow is off-screen (this is just a simple example)
            if (transform.position.y < -5f) // Replace -5f with your off-screen value
            {
                ScoreManager.ResetCombo();
                Debug.Log("Combo Reset!"); // Debug line
                Destroy(gameObject); // Destroy the arrow
            }
            UpdateComboUI();
        }


        public void IncreaseScore(int amount)
        {
            CurrentScore += amount;
            Debug.Log("Score Increased: " + amount + " | New Score: " + CurrentScore); // Debug line
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + CurrentScore.ToString();
            }
            else
            {
                Debug.LogWarning("Score Text UI is not assigned in the ScoreManager."); // Warning if not assigned
            }
        }

        // Method to reset the combo
        public static void ResetCombo()
        {
            CurrentCombo = 0;
            lastHitScore = 0; // Reset last hit score
        }

        // Method to increase the combo
        public static void IncreaseCombo(int currentScore) // Accept current score as a parameter
        {
            if (currentScore == lastHitScore)
            {
                CurrentCombo++;
            }
            else
            {
                CurrentCombo = 1; // Start a new combo
                lastHitScore = currentScore; // Update last hit score
            }

            if (CurrentCombo > MaxCombo)
            {
                MaxCombo = CurrentCombo; // Update max combo
            }
        }
        private void UpdateComboUI()
        {
            if (comboText != null)
            {
                comboText.text = "Combo: " + CurrentCombo.ToString();
            }
        }
    }
}