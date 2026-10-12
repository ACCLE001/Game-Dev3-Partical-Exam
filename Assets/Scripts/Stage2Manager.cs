using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut
{
    [AddComponentMenu("ThanethHamburut/Stage 2 Manager")]
    public class Stage2Manager : MonoBehaviour
    {
        public static Stage2Manager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float initialTime = 30f;
        [SerializeField] private int targetWinningScore = 10000;

        [Header("HUD References")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text goalText;
        [SerializeField] private TMP_Text interactionPromptText;
        [SerializeField] private GameObject interactionPromptPanel;
        [SerializeField] private Button btnBack;

        private float remainingTime;
        private int currentScore = 0;
        private bool isGameOver = false;

        // Performance caching for zero-alloc UI updates
        private int lastDisplayedTenth = -1;
        private int lastDisplayedScore = -1;

        public float RemainingTime => remainingTime;
        public int CurrentScore => currentScore;
        public bool IsGameOver => isGameOver;

        private void Awake()
        {
            Instance = this;
            GameData.LastPlayedStage = "Stage2";

            // Performance: Stabilize frame rate to 60 FPS
            Application.targetFrameRate = 60;
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

            // Hook player interaction detector
            var detector = FindFirstObjectByType<PlayerInteractionDetector>();
            if (detector != null)
            {
                detector.OnInteractableTargetChanged += HandleInteractableTargetChanged;
            }

            SetInteractionPrompt(null);
            ForceUpdateHUD();
        }

        private void OnDestroy()
        {
            var detector = FindFirstObjectByType<PlayerInteractionDetector>();
            if (detector != null)
            {
                detector.OnInteractableTargetChanged -= HandleInteractableTargetChanged;
            }
        }

        private void Update()
        {
            if (isGameOver) return;

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                ForceUpdateHUD();
                OnTimeExpired();
                return;
            }

            // Optimized update: only update timer text when tenth of a second changes
            int currentTenth = Mathf.CeilToInt(remainingTime * 10f);
            if (currentTenth != lastDisplayedTenth)
            {
                lastDisplayedTenth = currentTenth;
                if (timerText != null)
                {
                    timerText.text = $"Time: {remainingTime:F1}s";
                }
            }
        }

        public void AddScore(int amount)
        {
            if (isGameOver) return;

            currentScore += amount;
            if (currentScore < 0) currentScore = 0;
            GameData.CurrentScore = currentScore;
            UpdateScoreHUD();
        }

        public void AddTime(float amount)
        {
            if (isGameOver) return;

            remainingTime += amount;
            if (remainingTime < 0f) remainingTime = 0f;
            lastDisplayedTenth = -1; // force refresh
        }

        private void HandleInteractableTargetChanged(IInteractable target)
        {
            SetInteractionPrompt(target != null ? target.InteractionPrompt : null);
        }

        public void SetInteractionPrompt(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                if (interactionPromptPanel != null) interactionPromptPanel.SetActive(false);
                if (interactionPromptText != null) interactionPromptText.text = "";
            }
            else
            {
                if (interactionPromptPanel != null) interactionPromptPanel.SetActive(true);
                if (interactionPromptText != null) interactionPromptText.text = prompt;
            }
        }

        private void ForceUpdateHUD()
        {
            if (timerText != null)
            {
                timerText.text = $"Time: {remainingTime:F1}s";
            }
            UpdateScoreHUD();

            if (goalText != null)
            {
                goalText.text = $"Target: >{targetWinningScore:N0} to Win";
            }
        }

        private void UpdateScoreHUD()
        {
            if (scoreText != null && currentScore != lastDisplayedScore)
            {
                lastDisplayedScore = currentScore;
                scoreText.text = $"Score: {currentScore:N0} / {targetWinningScore:N0}";
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
