using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TanesHemburud
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text creditsDetailText;
        [SerializeField] private TMP_Text studentInfoText;

        private void Awake()
        {
            Transform panel = transform.Find("CreditsPanel");
            if (panel != null)
            {
                Button btnBack = panel.Find("BtnBack")?.GetComponent<Button>();
                if (btnBack != null) btnBack.onClick.AddListener(OnBackButtonPressed);
            }
        }

        private void Start()
        {
            if (creditsDetailText == null)
            {
                creditsDetailText = transform.Find("CreditsPanel/TitleLabel")?.GetComponent<TMP_Text>();
            }

            if (studentInfoText == null)
            {
                studentInfoText = transform.Find("CreditsPanel/StudentInfo")?.GetComponent<TMP_Text>();
            }

            if (creditsDetailText != null)
            {
                creditsDetailText.text = "Credits";
            }

            if (studentInfoText != null)
            {
                studentInfoText.text = $"Student Code: {GameData.StudentCode}\nName: {GameData.StudentName} ({GameData.StudentNameEnglish})";
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
