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

    [AddComponentMenu("ThanethHamburut/Game Item")]
    public class GameItem : MonoBehaviour, IInteractable
    {
        [Header("Item Properties")]
        [SerializeField] private ItemEffectType effectType = ItemEffectType.ScoreBonus;
        [SerializeField] private float timeModifier = 3f;      // 3 for bonus, 5 for penalty
        [SerializeField] private int scoreModifier = 500;       // 500 for bonus, 500 for penalty
        [Tooltip("If true, requires pressing E to interact instead of trigger collision (Stage 2)")]
        [SerializeField] private bool requireInteraction = false;

        [Header("Visual Animation")]
        [SerializeField] private bool rotate = true;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private bool bob = true;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float bobSpeed = 2f;

        private Vector3 startPos;
        private bool isCollected = false;

        public ItemEffectType EffectType => effectType;
        public bool RequireInteraction { get => requireInteraction; set => requireInteraction = value; }

        public string InteractionPrompt
        {
            get
            {
                switch (effectType)
                {
                    case ItemEffectType.ScoreBonus:
                        return "[E] Collect Score (+500 pts)";
                    case ItemEffectType.ScorePenalty:
                        return "[E] Danger Obstacle (-500 pts)";
                    case ItemEffectType.TimeBonus:
                        return "[E] Time Bonus (+3s)";
                    case ItemEffectType.TimePenalty:
                        return "[E] Time Trap (-5s)";
                    default:
                        return "[E] Interact";
                }
            }
        }

        private void Start()
        {
            startPos = transform.position;

            // If in Stage 2 scene, automatically enforce interaction mode for Q.8
            if (Stage2Manager.Instance != null)
            {
                requireInteraction = true;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            if (rotate)
            {
                transform.Rotate(Vector3.up, rotationSpeed * dt, Space.World);
            }

            if (bob)
            {
                float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                Vector3 p = transform.position;
                p.y = newY;
                transform.position = p;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // For Stage 1: collision / trigger based collection
            // In Stage 2: requireInteraction is true, so player must press 'E' via IInteractable instead
            if (isCollected || requireInteraction) return;

            if (other.CompareTag("Player") || other.GetComponent<CapsulePlayerController>() != null)
            {
                Collect();
            }
        }

        public void Interact(PlayerInteractionDetector interactor)
        {
            if (isCollected) return;
            Collect();
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
            // Stage 2 Manager support (Q.8)
            if (Stage2Manager.Instance != null)
            {
                switch (effectType)
                {
                    case ItemEffectType.TimeBonus:
                        Stage2Manager.Instance.AddTime(timeModifier);
                        break;
                    case ItemEffectType.TimePenalty:
                        Stage2Manager.Instance.AddTime(-timeModifier);
                        break;
                    case ItemEffectType.ScoreBonus:
                        Stage2Manager.Instance.AddScore(scoreModifier);
                        break;
                    case ItemEffectType.ScorePenalty:
                        Stage2Manager.Instance.AddScore(-scoreModifier);
                        break;
                }
                return;
            }

            // Stage 1 Manager support (Q.4, Q.5)
            if (Stage1Manager.Instance != null)
            {
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
        }

        public void Configure(ItemEffectType type, float timeMod = 0f, int scoreMod = 0)
        {
            effectType = type;
            if (timeMod > 0f) timeModifier = timeMod;
            if (scoreMod > 0) scoreModifier = scoreMod;
        }
    }
}
