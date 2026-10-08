using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TanesHemburud
{
    public class StageSelectionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text studentInfoText;

        private void Awake()
        {
            Button btnStage1 = transform.Find("BtnStage1")?.GetComponent<Button>();
            if (btnStage1 != null) btnStage1.onClick.AddListener(OnStage1ButtonPressed);

            Button btnStage2 = transform.Find("BtnStage2")?.GetComponent<Button>();
            if (btnStage2 != null) btnStage2.onClick.AddListener(OnStage2ButtonPressed);

            Button btnBack = transform.Find("BtnBack")?.GetComponent<Button>();
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
                studentInfoText.text = $"Student Code: {GameData.StudentCode} Name: {GameData.StudentName}";
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
