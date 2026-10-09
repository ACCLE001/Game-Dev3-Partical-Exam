using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text creditsDetailText;
        [SerializeField] private TMP_Text studentInfoText;
        [SerializeField] private Button btnBack;

        private void Awake()
        {
            Transform panel = transform.Find("CreditsPanel");
            if (panel != null)
            {
                if (btnBack == null) btnBack = panel.Find("BtnBack")?.GetComponent<Button>();
                if (creditsDetailText == null) creditsDetailText = panel.Find("TitleLabel")?.GetComponent<TMP_Text>();
                if (studentInfoText == null) studentInfoText = panel.Find("StudentInfo")?.GetComponent<TMP_Text>();
            }

            if (btnBack != null)
            {
                btnBack.onClick.AddListener(OnBackButtonPressed);
            }
        }

        private void Start()
        {
            if (creditsDetailText != null)
            {
                creditsDetailText.text = "Credits";
            }

            if (studentInfoText != null)
            {
                studentInfoText.text = $"Course: 958321 Game Development 3\nPractical Examination 1/2569\n\nStudent Code: {GameData.StudentCode}\nName: {GameData.StudentName} ({GameData.StudentNameEnglish})";
            }
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
