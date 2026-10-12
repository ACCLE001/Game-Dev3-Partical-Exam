using UnityEngine;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ThanethHamburut
{
    [AddComponentMenu("ThanethHamburut/Player Interaction Detector")]
    public class PlayerInteractionDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [Tooltip("Maximum distance to detect interactable items")]
        [SerializeField] private float detectionRadius = 2.5f;

        [Tooltip("Layer mask for interactable objects")]
        [SerializeField] private LayerMask interactableLayers = ~0;

        [Tooltip("Transform from where detection starts (defaults to player origin)")]
        [SerializeField] private Transform detectionOrigin;

        [Header("Status")]
        [SerializeField] private bool canInteract = true;

        private IInteractable currentInteractable;
        private readonly Collider[] hitBuffer = new Collider[16];

        public IInteractable CurrentInteractable => currentInteractable;
        public bool CanInteract { get => canInteract; set => canInteract = value; }

        public event Action<IInteractable> OnInteractableTargetChanged;

        private void Start()
        {
            if (detectionOrigin == null)
            {
                detectionOrigin = transform;
            }
        }

        private void Update()
        {
            DetectInteractable();
            HandleInput();
        }

        private void DetectInteractable()
        {
            Vector3 origin = detectionOrigin != null ? detectionOrigin.position : transform.position;
            origin += Vector3.up * 0.5f;

            // Zero-allocation sphere check for optimal performance
            int hitCount = Physics.OverlapSphereNonAlloc(origin, detectionRadius, hitBuffer, interactableLayers);

            IInteractable bestTarget = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = hitBuffer[i];
                if (hit == null || hit.gameObject == gameObject) continue;

                var interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    float dist = Vector3.Distance(origin, hit.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        bestTarget = interactable;
                    }
                }
            }

            if (bestTarget != currentInteractable)
            {
                currentInteractable = bestTarget;
                OnInteractableTargetChanged?.Invoke(currentInteractable);
            }
        }

        private void HandleInput()
        {
            if (!canInteract || currentInteractable == null) return;

            bool isInteractTriggered = false;

#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
            {
                isInteractTriggered = true;
            }
#endif
            if (!isInteractTriggered && Input.GetKeyDown(KeyCode.E))
            {
                isInteractTriggered = true;
            }

            if (isInteractTriggered)
            {
                var target = currentInteractable;
                target.Interact(this);
                // Clear immediately after interaction
                if (currentInteractable == target)
                {
                    currentInteractable = null;
                    OnInteractableTargetChanged?.Invoke(null);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = detectionOrigin != null ? detectionOrigin.position : transform.position;
            origin += Vector3.up * 0.5f;

            Gizmos.color = currentInteractable != null ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(origin, detectionRadius);
        }
    }
}
