using UnityEngine;

namespace ThanethHamburut
{
    [AddComponentMenu("ThanethHamburut/Smooth Camera Follow")]
    public class SmoothCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.5f, -6f);
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private float lookAheadDistance = 1.5f;
        [SerializeField] private bool followTargetRotation = false;

        public Transform Target { get => target; set => target = value; }
        public Vector3 Offset { get => offset; set => offset = value; }
        public float SmoothSpeed { get => smoothSpeed; set => smoothSpeed = value; }
        public bool FollowTargetRotation { get => followTargetRotation; set => followTargetRotation = value; }

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

            Vector3 desiredPosition;
            if (followTargetRotation)
            {
                desiredPosition = target.position + target.rotation * offset;
            }
            else
            {
                desiredPosition = target.position + offset;
            }

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.up * 1.2f + (followTargetRotation ? Vector3.zero : target.forward * lookAheadDistance);
            transform.LookAt(lookTarget);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
