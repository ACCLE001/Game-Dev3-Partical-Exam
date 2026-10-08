using UnityEngine;
using UnityEngine.UI;

namespace TanesHemburud
{
    public class StageTestFlowUI : MonoBehaviour
    {
        private void Awake()
        {
            Button btnWin = transform.Find("BtnTestWin")?.GetComponent<Button>();
            if (btnWin != null) btnWin.onClick.AddListener(TestWin);

            Button btnLose = transform.Find("BtnTestLose")?.GetComponent<Button>();
            if (btnLose != null) btnLose.onClick.AddListener(TestLose);

            Button btnBack = transform.Find("BtnBack")?.GetComponent<Button>();
            if (btnBack != null) btnBack.onClick.AddListener(BackToSelection);
        }

        public void TestWin()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadWin();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Win");
        }

        public void TestLose()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadGameOver();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }

        public void BackToSelection()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.LoadStageSelection();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelection");
        }
    }
}
