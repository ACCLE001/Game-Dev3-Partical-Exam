using UnityEngine;

namespace ThanethHamburut
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] private AudioClip positiveItemClip;
        [SerializeField] private AudioClip negativeItemClip;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip gameOverClip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ApplyVolumes();
            PlayBGM();
        }

        private void SetupAudioSources()
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        public void PlayBGM(AudioClip clip = null)
        {
            if (clip != null)
            {
                bgmClip = clip;
            }

            if (bgmSource != null && bgmClip != null)
            {
                if (bgmSource.clip != bgmClip || !bgmSource.isPlaying)
                {
                    bgmSource.clip = bgmClip;
                    bgmSource.volume = GameData.BGMVolume;
                    bgmSource.Play();
                }
            }
        }

        public void StopBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, GameData.SFXVolume);
            }
        }

        public void PlayPositiveItemSFX()
        {
            PlaySFX(positiveItemClip);
        }

        public void PlayNegativeItemSFX()
        {
            PlaySFX(negativeItemClip);
        }

        public void PlayWinSFX()
        {
            PlaySFX(winClip);
        }

        public void PlayGameOverSFX()
        {
            PlaySFX(gameOverClip);
        }

        public void SetBGMVolume(float volume)
        {
            GameData.BGMVolume = Mathf.Clamp01(volume);
            if (bgmSource != null)
            {
                bgmSource.volume = GameData.BGMVolume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            GameData.SFXVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = GameData.SFXVolume;
            }
        }

        public void ApplyVolumes()
        {
            if (bgmSource != null) bgmSource.volume = GameData.BGMVolume;
            if (sfxSource != null) sfxSource.volume = GameData.SFXVolume;
        }

        public void SetClips(AudioClip bgm, AudioClip positive, AudioClip negative, AudioClip win, AudioClip lose)
        {
            bgmClip = bgm;
            positiveItemClip = positive;
            negativeItemClip = negative;
            winClip = win;
            gameOverClip = lose;
        }
    }
}
