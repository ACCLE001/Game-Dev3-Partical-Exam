using UnityEngine;

namespace ThanethHamburut
{
    public enum ItemEffectType
    {
        TimeBonus,    // +3 seconds
        TimePenalty,  // -5 seconds
        ScoreBonus,   // +500 points
        ScorePenalty  // -500 points
    }

    public class GameItem : MonoBehaviour
    {
        [Header("Item Properties")]
        [SerializeField] private ItemEffectType effectType = ItemEffectType.ScoreBonus;
        [SerializeField] private float timeModifier = 3f;      // 3 for bonus, 5 for penalty
        [SerializeField] private int scoreModifier = 500;       // 500 for bonus, 500 for penalty

        [Header("Visual Animation")]
        [SerializeField] private bool rotate = true;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private bool bob = true;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float bobSpeed = 2f;

        private Vector3 startPos;
        private bool isCollected = false;

        public ItemEffectType EffectType => effectType;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            if (rotate)
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            }

            if (bob)
            {
                float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // For Stage 1: Trigger / collision based collection
            if (isCollected) return;

            if (other.CompareTag("Player") || other.GetComponent<CapsulePlayerController>() != null)
            {
                Collect();
            }
        }

        public void Collect()
        {
            if (isCollected) return;
            isCollected = true;

            ApplyEffect();

            // Play SFX
            if (AudioManager.Instance != null)
            {
                if (effectType == ItemEffectType.ScoreBonus || effectType == ItemEffectType.TimeBonus)
                {
                    AudioManager.Instance.PlayPositiveItemSFX();
                }
                else
                {
                    AudioManager.Instance.PlayNegativeItemSFX();
                }
            }

            Destroy(gameObject);
        }

        private void ApplyEffect()
        {
            if (Stage1Manager.Instance == null) return;

            switch (effectType)
            {
                case ItemEffectType.TimeBonus:
                    Stage1Manager.Instance.AddTime(timeModifier);
                    break;
                case ItemEffectType.TimePenalty:
                    Stage1Manager.Instance.AddTime(-timeModifier);
                    break;
                case ItemEffectType.ScoreBonus:
                    Stage1Manager.Instance.AddScore(scoreModifier);
                    break;
                case ItemEffectType.ScorePenalty:
                    Stage1Manager.Instance.AddScore(-scoreModifier);
                    break;
            }
        }

        public void Configure(ItemEffectType type, float timeMod = 0f, int scoreMod = 0)
        {
            effectType = type;
            if (timeMod > 0f) timeModifier = timeMod;
            if (scoreMod > 0) scoreModifier = scoreMod;
        }
    }
}
