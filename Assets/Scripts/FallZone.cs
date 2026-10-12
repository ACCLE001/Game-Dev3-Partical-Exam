using UnityEngine;

namespace ThanethHamburut
{
    [AddComponentMenu("ThanethHamburut/Fall Zone")]
    public class FallZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            bool isPlayer = other.CompareTag("Player") ||
                            other.GetComponent<CapsulePlayerController>() != null ||
                            other.GetComponent<PlayerInteractionDetector>() != null ||
                            other.GetComponentInParent<PlayerInteractionDetector>() != null ||
                            other.GetComponent<StarterAssets.ThirdPersonController>() != null ||
                            other.GetComponentInParent<StarterAssets.ThirdPersonController>() != null;

            if (isPlayer)
            {
                if (Stage2Manager.Instance != null)
                {
                    Stage2Manager.Instance.PlayerFell();
                }
                else if (Stage1Manager.Instance != null)
                {
                    Stage1Manager.Instance.PlayerFell();
                }
            }
        }
    }
}
