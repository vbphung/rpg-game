using UnityEngine;

namespace UI
{
    public class CameraFacing : MonoBehaviour
    {
        private void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}