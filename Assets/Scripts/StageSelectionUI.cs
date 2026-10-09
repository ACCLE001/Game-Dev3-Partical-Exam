using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut
{
    public class StageSelectionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text studentInfoText;

        [Header("Buttons")]
        [SerializeField] private Button btnStage1;
        [SerializeField] private Button btnStage2;
        [SerializeField] private Button btnBack;

        private void Awake()
        {
            if (btnStage1 == null) btnStage1 = transform.Find("BtnStage1")?.GetComponent<Button>();
            if (btnStage2 == null) btnStage2 = transform.Find("BtnStage2")?.GetComponent<Button>();
            if (btnBack == null) btnBack = transform.Find("BtnBack")?.GetComponent<Button>();

            if (btnStage1 != null) btnStage1.onClick.AddListener(OnStage1ButtonPressed);
            if (btnStage2 != null) btnStage2.onClick.AddListener(OnStage2ButtonPressed);
            if (btnBack != null) btnBack.onClick.AddListener(OnBackButtonPressed);
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

        public void OnStage1ButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadStage1();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1");
        }

        public void OnStage2ButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadStage2();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Stage2");
        }

        public void OnBackButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadMainMenu();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
