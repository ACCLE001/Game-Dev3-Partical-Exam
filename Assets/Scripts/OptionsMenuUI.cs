using UnityEngine;
using UnityEngine.UI;

namespace ThanethHamburut
{
    public class OptionsMenuUI : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Buttons")]
        [SerializeField] private Button btnOK;

        private void Awake()
        {
            Transform panel = transform.Find("OptionsPanel");
            if (panel != null)
            {
                if (btnOK == null) btnOK = panel.Find("BtnOK")?.GetComponent<Button>();
                if (bgmSlider == null) bgmSlider = panel.Find("BGMSlider")?.GetComponent<Slider>();
                if (sfxSlider == null) sfxSlider = panel.Find("SFXSlider")?.GetComponent<Slider>();
            }

            if (btnOK != null)
            {
                btnOK.onClick.AddListener(OnOKButtonPressed);
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
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
        }

        public void OnSFXVolumeChanged(float value)
        {
            GameData.SFXVolume = value;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
        }

        public void OnOKButtonPressed()
        {
            if (SceneNavigationManager.Instance != null)
            {
                SceneNavigationManager.Instance.LoadMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
