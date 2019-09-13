using UnityEngine;

namespace Weelco.VRInput {

    public class OVRUIHitPointer : IUIHitPointer {

#if VR_INPUT_OCULUS

	    public OVRInput.Button primaryTrigger;
        public OVRInput.Controller controller;
        
        OVRCameraRig oculusRig;

	    public override bool ButtonDown() {
            return OVRInput.GetDown(primaryTrigger, controller);
        }

	    public override bool ButtonUp() {
	        return OVRInput.GetUp(primaryTrigger, controller);
	    }

        public override void OnEnterControl(GameObject control) {
            
        }

        public override void OnExitControl(GameObject control) {
            
        }

        public override Transform target {
            get {
                if (oculusRig == null) {
                    oculusRig = OVRManager.instance.GetComponent<OVRCameraRig>();
                }
                
                return oculusRig.centerEyeAnchor;
            }
        }     
#else 
        public override bool ButtonDown() {
            return false;
        }

        public override bool ButtonUp() {
            return false;
        }

        public override void OnEnterControl(GameObject control) {
            throw new System.NotImplementedException();
        }

        public override void OnExitControl(GameObject control) {
            throw new System.NotImplementedException();
        }

        public override Transform target {
            get {
                throw new System.NotImplementedException();
            }
        }
#endif

    }
}