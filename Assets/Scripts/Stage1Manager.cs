using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut
{
    public class Stage1Manager : MonoBehaviour
    {
        public static Stage1Manager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float initialTime = 30f;
        [SerializeField] private int targetWinningScore = 10000;

        [Header("HUD References")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text goalText;
        [SerializeField] private Button btnBack;

        private float remainingTime;
        private int currentScore = 0;
        private bool isGameOver = false;

        public float RemainingTime => remainingTime;
        public int CurrentScore => currentScore;
        public bool IsGameOver => isGameOver;

        private void Awake()
        {
            Instance = this;
            GameData.LastPlayedStage = "Stage1";
        }

        private void Start()
        {
            remainingTime = initialTime;
            currentScore = 0;
            GameData.CurrentScore = 0;
            isGameOver = false;

            if (btnBack == null)
            {
                var bGO = GameObject.Find("BtnBack");
                if (bGO != null) btnBack = bGO.GetComponent<Button>();
            }

            if (btnBack != null)
            {
                btnBack.onClick.AddListener(OnBackButtonPressed);
            }

            UpdateHUD();
        }

        private void Update()
        {
            if (isGameOver) return;

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                UpdateHUD();
                OnTimeExpired();
                return;
            }

            UpdateHUD();
        }

        public void AddScore(int amount)
        {
            if (isGameOver) return;

            currentScore += amount;
            if (currentScore < 0) currentScore = 0;
            GameData.CurrentScore = currentScore;
            UpdateHUD();
        }

        public void AddTime(float amount)
        {
            if (isGameOver) return;

            remainingTime += amount;
            if (remainingTime < 0f) remainingTime = 0f;
            UpdateHUD();
        }

        private void UpdateHUD()
        {
            if (timerText != null)
            {
                timerText.text = $"Time: {remainingTime:F1}s";
            }

            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore:N0} / {targetWinningScore:N0}";
            }

            if (goalText != null)
            {
                goalText.text = $"Target: >{targetWinningScore:N0} to Win";
            }
        }

        private void OnTimeExpired()
        {
            if (isGameOver) return;
            isGameOver = true;

            GameData.CurrentScore = currentScore;

            if (currentScore > targetWinningScore)
            {
                // Winning Condition: > 10,000 points
                if (AudioManager.Instance != null) AudioManager.Instance.PlayWinSFX();
                if (SceneNavigationManager.Instance != null)
                    SceneNavigationManager.Instance.LoadWin();
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Win");
            }
            else
            {
                // Losing Condition: <= 10,000 points
                if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverSFX();
                if (SceneNavigationManager.Instance != null)
                    SceneNavigationManager.Instance.LoadGameOver();
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
        }

        public void PlayerFell()
        {
            if (isGameOver) return;
            isGameOver = true;

            GameData.CurrentScore = currentScore;

            // Losing Condition: Fell into void
            if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverSFX();
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadGameOver();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }

        public void OnBackButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadStageSelection();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelection");
        }
    }
}
