using UnityEngine;

namespace Behaviour
{
    public class SmoothCameraBehaviour : MonoBehaviour
    {
        public Transform target;
        public Vector2 offset;
        public float damping;

        private static Vector3 _velocity = Vector3.zero;

        public void FixedUpdate()
        {
            var moveDirection = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, moveDirection, ref _velocity, damping);
        }
    }
}