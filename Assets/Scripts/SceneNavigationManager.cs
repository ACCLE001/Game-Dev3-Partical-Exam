using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThanethHamburut
{
    public class SceneNavigationManager : MonoBehaviour
    {
        public static SceneNavigationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadStageSelection()
        {
            SceneManager.LoadScene("StageSelection");
        }

        public void LoadStage1()
        {
            GameData.LastPlayedStage = "Stage1";
            GameData.ResetGameData();
            SceneManager.LoadScene("Stage1");
        }

        public void LoadStage2()
        {
            GameData.LastPlayedStage = "Stage2";
            GameData.ResetGameData();
            SceneManager.LoadScene("Stage2");
        }

        public void LoadWin()
        {
            SceneManager.LoadScene("Win");
        }

        public void LoadGameOver()
        {
            SceneManager.LoadScene("GameOver");
        }

        public void LoadOptions()
        {
            SceneManager.LoadScene("Options");
        }

        public void LoadCredits()
        {
            SceneManager.LoadScene("Credits");
        }

        public void TryAgain()
        {
            GameData.ResetGameData();
            if (!string.IsNullOrEmpty(GameData.LastPlayedStage))
            {
                SceneManager.LoadScene(GameData.LastPlayedStage);
            }
            else
            {
                SceneManager.LoadScene("Stage1");
            }
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
