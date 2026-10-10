using UnityEngine;

namespace ThanethHamburut
{
    [AddComponentMenu("ThanethHamburut/Stage 2 Audio Setup")]
    public class Stage2AudioSetup : MonoBehaviour
    {
        [Header("BGM Override (Optional)")]
        [SerializeField] private AudioClip stage2BGM;

        private void Start()
        {
            SetupAudio();
        }

        public void SetupAudio()
        {
            if (AudioManager.Instance != null)
            {
                if (stage2BGM != null)
                {
                    AudioManager.Instance.PlayBGM(stage2BGM);
                }
                else
                {
                    AudioManager.Instance.PlayBGM();
                }
            }
            else
            {
                // Fallback AudioSource if testing directly in Stage2 without MainMenu
                AudioSource localAudio = GetComponent<AudioSource>();
                if (localAudio == null && stage2BGM != null)
                {
                    localAudio = gameObject.AddComponent<AudioSource>();
                    localAudio.clip = stage2BGM;
                    localAudio.loop = true;
                    localAudio.volume = GameData.BGMVolume;
                    localAudio.Play();
                }
            }
        }
    }
}
