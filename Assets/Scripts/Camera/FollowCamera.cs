using UnityEngine;

namespace SillySeal.CameraControl
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3.5f, -6f);
        [SerializeField] private float followSmoothTime = 0.2f;
        [SerializeField] private float lookSmoothTime = 0.1f;

        private Vector3 positionVelocity;
        private Vector3 currentLookVelocity;
        private Vector3 currentLookPoint;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + target.TransformDirection(offset);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, followSmoothTime);

            currentLookPoint = Vector3.SmoothDamp(currentLookPoint, target.position, ref currentLookVelocity, lookSmoothTime);
            transform.LookAt(currentLookPoint);
        }
    }
}
