using UnityEngine;

namespace ThanethHamburut
{
    public class FallZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<CapsulePlayerController>() != null)
            {
                if (Stage1Manager.Instance != null)
                {
                    Stage1Manager.Instance.PlayerFell();
                }
            }
        }
    }
}
