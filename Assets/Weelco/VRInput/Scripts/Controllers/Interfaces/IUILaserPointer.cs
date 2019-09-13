using UnityEngine;

namespace Weelco.VRInput {

    abstract public class IUILaserPointer : IUIHitPointer {

        public bool IsRightHand;
        public bool UseHapticPulse;

        public float LaserThickness;

        private GameObject pointer;

        private float _distanceLimit;
        
        // Use this for initialization
        override public void Initialize() {
            
            if (UseCustomLaserPointer) {
                pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pointer.transform.SetParent(target.transform, false);
                pointer.transform.localScale = new Vector3(LaserThickness, LaserThickness, 100.0f);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);

                // remove the colliders on our primitives            
                Object.DestroyImmediate(pointer.GetComponent<BoxCollider>());
                Material material = new Material(Shader.Find("Unlit/Color"));
                material.SetColor("_Color", LaserColor);
                pointer.GetComponent<MeshRenderer>().material = material;                
            } else {
                if (target) {
                    Transform beam = target.Find("LaserPointer/LaserBeam");
                    if (beam)
                        pointer = beam.gameObject;
                }
            }
            
            base.Initialize();
        }
        
        protected override void UpdateRaycasting(bool isHit, float distance) {
            if (pointer) {
                pointer.transform.localScale = new Vector3(pointer.transform.localScale.x, pointer.transform.localScale.y, distance);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);
            }
            base.UpdateRaycasting(isHit, distance);
        }
    }
}