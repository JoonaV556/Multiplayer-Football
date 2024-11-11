using UnityEngine;

namespace FootBall
{
    /// <summary>
    /// Smootly follows target object with customizable params
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Follow")]
        [Range(0f, 1f)]
        public float FollowAlpha;
        public float FollowAlphaMultiplier;

        [Header("Orient")]
        [Range(0f, 1f)]
        public float OrientAlpha;
        public float OrientAlphaMultiplier;

        private Transform _followTransform;
        private Vector3 _lastPosition;
        private Quaternion _lastOrientation;

        public void Init(Transform FollowTransform)
        {
            _followTransform = FollowTransform;
            _lastPosition = Vector3.zero;
            _lastOrientation = Quaternion.identity;
        }

        private void Update()
        {
            if (_followTransform == null) return;

            // follow
            transform.position = Vector3.Lerp(
                _lastPosition,
                _followTransform.position,
                FollowAlpha * FollowAlphaMultiplier * Time.deltaTime
                );
            _lastPosition = transform.position;

            // Orient
            transform.rotation = Quaternion.Slerp(
                _lastOrientation,
                _followTransform.rotation,
                OrientAlpha * OrientAlphaMultiplier * Time.deltaTime
            );
            _lastOrientation = transform.rotation;
        }
    }
}

