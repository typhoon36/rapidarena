using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets.Utility
{
    public class SmoothFollow : MonoBehaviour
    {
        // The target we are following
        [SerializeField]
        public Transform target;

        // The offset from the target
        [SerializeField]
        private Vector3 offset = new Vector3(0, 1.5f, 0);

        // Use this for initialization
        void Start() { }

        void LateUpdate()
        {
            // Early out if we don't have a target
            if (!target)
                return;

            // Set the position of the camera to the target's position with the offset
            transform.position = target.position + offset;

        }
    }
}