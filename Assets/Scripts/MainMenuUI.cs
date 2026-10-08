using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TanesHemburud
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text studentInfoText;

        private void Awake()
        {
            Button btnStart = transform.Find("BtnStart")?.GetComponent<Button>();
            if (btnStart != null) btnStart.onClick.AddListener(OnStartButtonPressed);

            Button btnOptions = transform.Find("BtnOptions")?.GetComponent<Button>();
            if (btnOptions != null) btnOptions.onClick.AddListener(OnOptionsButtonPressed);

            Button btnCredits = transform.Find("BtnCredits")?.GetComponent<Button>();
            if (btnCredits != null) btnCredits.onClick.AddListener(OnCreditsButtonPressed);

            Button btnExit = transform.Find("BtnExit")?.GetComponent<Button>();
            if (btnExit != null) btnExit.onClick.AddListener(OnExitButtonPressed);
        }

        private void Start()
        {
            if (studentInfoText == null)
            {
                studentInfoText = transform.Find("StudentInfo")?.GetComponent<TMP_Text>();
            }

            if (studentInfoText != null)
            {
                studentInfoText.text = $"Student Code: {GameData.StudentCode}\nName: {GameData.StudentName} ({GameData.StudentNameEnglish})";
            }
        }

        public void OnStartButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadStageSelection();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelection");
        }

        public void OnOptionsButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadOptionsAdditive();
            else
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Options", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        public void OnCreditsButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadCredits();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
        }

        public void OnExitButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.QuitGame();
            else
                Application.Quit();
        }
    }
}
