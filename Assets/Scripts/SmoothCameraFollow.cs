using UnityEngine;

namespace ThanethHamburut
{
    public class SmoothCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.5f, -6f);
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private float lookAheadDistance = 1.5f;

        private void LateUpdate()
        {
            if (target == null)
            {
                // Try to find player if not assigned
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                else return;
            }

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.up * 1f + target.forward * lookAheadDistance;
            transform.LookAt(lookTarget);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
