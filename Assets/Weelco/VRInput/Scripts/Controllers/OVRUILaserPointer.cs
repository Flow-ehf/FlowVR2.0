using UnityEngine;

namespace Weelco.VRInput {

    public class OVRUILaserPointer : IUILaserPointer {

#if VR_INPUT_OCULUS

	    public OVRInput.Button primaryTrigger;
        public OVRInput.Controller controller;

        private OVRCameraRig oculusRig;
        private OVRHapticsClip enterHapticClip;
        private OVRHapticsClip exitHapticClip;

        public override void Initialize() {
	        base.Initialize();        
            InitHaptics();
        }

	    public override bool ButtonDown()  {
	        return OVRInput.GetDown(primaryTrigger, controller);
	    }

	    public override bool ButtonUp() {
	        return OVRInput.GetUp(primaryTrigger, controller);
	    }

        public override void OnEnterControl(GameObject control) {
            if (UseHapticPulse) {
                if (controller == OVRInput.Controller.LTouch) {
                    OVRHaptics.LeftChannel.Mix(enterHapticClip);
                }

                if (controller == OVRInput.Controller.RTouch) {
                    OVRHaptics.RightChannel.Mix(exitHapticClip);
                }
            }
        }

        public override void OnExitControl(GameObject control) {
            if (UseHapticPulse) {
                if (controller == OVRInput.Controller.LTouch) {
                    OVRHaptics.LeftChannel.Mix(exitHapticClip);
                }

                if (controller == OVRInput.Controller.RTouch) {
                    OVRHaptics.RightChannel.Mix(exitHapticClip);
                }
            }
        }

        public override Transform target {
            get {
                if (oculusRig == null) {
                    oculusRig = OVRManager.instance.GetComponent<OVRCameraRig>();
                }
                if (IsRightHand)
                    return oculusRig.rightHandAnchor;
                else
                    return oculusRig.leftHandAnchor;
            }
        }

        private void InitHaptics() {
            int duration = 10;
            int exitAmplitude = 40;
            int enterAmplitude = 80;
            float freq = 50f / OVRHaptics.Config.SampleRateHz;
            enterHapticClip = new OVRHapticsClip(duration);
            exitHapticClip = new OVRHapticsClip(duration);
            WriteHapticSamples(enterHapticClip, freq, enterAmplitude, duration);
            WriteHapticSamples(exitHapticClip, freq, exitAmplitude, duration);
        }

        private void WriteHapticSamples(OVRHapticsClip clip, float freq, float amplitude, int duration) {
            for (int i = 0; i < duration; i++) {
                clip.WriteSample((byte)(Mathf.RoundToInt(amplitude * 0.5f * (Mathf.Sin(i * freq * 2 * Mathf.PI) + 1))));
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