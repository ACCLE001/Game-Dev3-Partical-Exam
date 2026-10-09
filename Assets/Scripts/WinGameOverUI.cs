using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut
{
    public class WinGameOverUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Button btnTryAgain;
        [SerializeField] private Button btnBackToMain;

        private void Awake()
        {
            if (btnTryAgain == null) btnTryAgain = transform.Find("BtnTryAgain")?.GetComponent<Button>();
            if (btnTryAgain == null) btnTryAgain = transform.Find("BtnPlayAgain")?.GetComponent<Button>();
            if (btnBackToMain == null) btnBackToMain = transform.Find("BtnBackToMain")?.GetComponent<Button>();

            if (btnTryAgain != null) btnTryAgain.onClick.AddListener(OnTryAgainButtonPressed);
            if (btnBackToMain != null) btnBackToMain.onClick.AddListener(OnBackToMainMenuButtonPressed);
        }

        private void Start()
        {
            if (scoreText == null)
            {
                scoreText = transform.Find("ScoreText")?.GetComponent<TMP_Text>();
            }

            if (scoreText != null)
            {
                scoreText.text = $"Score: {GameData.CurrentScore}";
            }
        }

        public void OnTryAgainButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.TryAgain();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(GameData.LastPlayedStage);
        }

        public void OnBackToMainMenuButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadMainMenu();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
