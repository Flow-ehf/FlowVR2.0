using UnityEngine;

namespace Weelco {

    public class MouseCameraRotation: MonoBehaviour {

        [Tooltip("If false, press Left Ctrl button for rotation")]
        public bool alwaysRotate = false;
        public bool lerpBack = true;

        public float speedH = 2.5f;
        public float speedV = 2.5f;
        public float speedL = 7.0f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;

        // Use this for initialization
        void Start () {
            yaw = transform.rotation.eulerAngles.y;
        }

        void Update () {
            if (alwaysRotate) {
                MouseRotate();
            } else {
                if (Input.GetKey(KeyCode.LeftControl)) {
                    MouseRotate();
                }
                else if (lerpBack) {
                    LerpBack();
                }
            }            
        }

        private void MouseRotate() {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        private void LerpBack() {
            if (!transform.rotation.Equals(Quaternion.Euler(Vector3.zero))) {
                yaw = 0.0f;
                pitch = 0.0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * speedL);
            }
        }
    }
}