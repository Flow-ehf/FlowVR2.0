using UnityEngine;

namespace Weelco.VRInput {

    public abstract class IUIPointer {

        private float _distanceLimit;
        
        public virtual void Update() {

            Ray ray = new Ray(target.transform.position, target.transform.forward);
            RaycastHit hitInfo;
            bool isHit = Physics.Raycast(ray, out hitInfo);

            float distance = 100.0f;

            if (isHit) {
                distance = hitInfo.distance;
            }

            if (_distanceLimit > 0.0f) {
                distance = Mathf.Min(distance, _distanceLimit);
                isHit = true;
            }

            UpdateRaycasting(isHit, distance);

            // reset the previous distance limit
            _distanceLimit = -1.0f;
        }
        
        // limits the laser distance for the current frame
        public void LimitLaserDistance(float distance) {
            if (distance < 0.0f)
                return;

            if (_distanceLimit < 0.0f)
                _distanceLimit = distance;
            else
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
        }


        abstract public Transform target { get; }

        abstract public void Initialize();
        
        abstract public void OnEnterControl(GameObject control);

        abstract public void OnExitControl(GameObject control);        

        abstract protected void UpdateRaycasting(bool isHit, float distance);

    }
}
