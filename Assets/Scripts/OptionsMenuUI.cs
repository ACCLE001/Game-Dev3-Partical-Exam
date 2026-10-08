using UnityEngine;
using UnityEngine.UI;

namespace TanesHemburud
{
    public class OptionsMenuUI : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private void Awake()
        {
            Transform panel = transform.Find("OptionsPanel");
            if (panel != null)
            {
                Button btnOK = panel.Find("BtnOK")?.GetComponent<Button>();
                if (btnOK != null) btnOK.onClick.AddListener(OnOKButtonPressed);
            }
        }

        private void Start()
        {
            if (bgmSlider != null)
            {
                bgmSlider.value = GameData.BGMVolume;
                bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = GameData.SFXVolume;
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }

        public void OnBGMVolumeChanged(float value)
        {
            GameData.BGMVolume = value;
        }

        public void OnSFXVolumeChanged(float value)
        {
            GameData.SFXVolume = value;
        }

        public void OnOKButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
                SceneNavigationManager.Instance.UnloadOptionsAdditive();
            else
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Options");
        }
    }
}
