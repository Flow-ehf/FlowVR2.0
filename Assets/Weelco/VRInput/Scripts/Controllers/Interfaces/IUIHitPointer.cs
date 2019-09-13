using UnityEngine;

namespace Weelco.VRInput {

    abstract public class IUIHitPointer : IUIPointer {

        public float LaserHitScale;
        public Color LaserColor;        

        public bool HitAlwaysOn;
        public bool UseCustomLaserPointer;

        private GameObject hitPoint;

        abstract public bool ButtonDown();

        abstract public bool ButtonUp();        

        // Use this for initialization
        public override void Initialize() {

            if (UseCustomLaserPointer) {

                hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitPoint.transform.SetParent(target.transform, false);
                hitPoint.transform.localScale = new Vector3(LaserHitScale, LaserHitScale, LaserHitScale);
                hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);

                // remove the colliders on our primitives
                Object.DestroyImmediate(hitPoint.GetComponent<SphereCollider>());
                Material material = new Material(Shader.Find("Unlit/Color"));
                material.SetColor("_Color", LaserColor);
                hitPoint.GetComponent<MeshRenderer>().material = material;                
            } else {
                if (target) {
                    Transform dot = target.Find("LaserPointer/LaserBeamDot");
                    if (dot)
                        hitPoint = dot.gameObject;
                }                
            }

            if (hitPoint && !HitAlwaysOn)
                hitPoint.SetActive(false);
        }

        protected override void UpdateRaycasting(bool isHit, float distance) {
            if (hitPoint) {
                if (isHit)
                    hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);

                if (!HitAlwaysOn)
                    hitPoint.SetActive(isHit);
            }
        }
    }
}