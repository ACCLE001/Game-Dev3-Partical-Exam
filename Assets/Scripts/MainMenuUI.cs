using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text studentInfoText;

        [Header("Buttons")]
        [SerializeField] private Button btnStart;
        [SerializeField] private Button btnOptions;
        [SerializeField] private Button btnCredits;
        [SerializeField] private Button btnExit;

        private void Awake()
        {
            if (btnStart == null) btnStart = transform.Find("BtnStart")?.GetComponent<Button>();
            if (btnOptions == null) btnOptions = transform.Find("BtnOptions")?.GetComponent<Button>();
            if (btnCredits == null) btnCredits = transform.Find("BtnCredits")?.GetComponent<Button>();
            if (btnExit == null) btnExit = transform.Find("BtnExit")?.GetComponent<Button>();

            if (btnStart != null) btnStart.onClick.AddListener(OnStartButtonPressed);
            if (btnOptions != null) btnOptions.onClick.AddListener(OnOptionsButtonPressed);
            if (btnCredits != null) btnCredits.onClick.AddListener(OnCreditsButtonPressed);
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
                SceneNavigationManager.Instance.LoadOptions();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
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
