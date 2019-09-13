using UnityEngine;

#if VR_INPUT_VIVE
using Valve.VR;
#endif

namespace Weelco.VRInput {

    public class ViveUILaserPointer : IUILaserPointer {

#if VR_INPUT_VIVE

        public EVRButtonId InteractionButton;

        private SteamVR_TrackedObject _trackedObject;
        private bool _connected = false;

        public override void Initialize() {
            SteamVR_ControllerManager steamVRManager = Object.FindObjectOfType<SteamVR_ControllerManager>();
            if (IsRightHand)
                _trackedObject = steamVRManager.right.GetComponent<SteamVR_TrackedObject>();
            else
                _trackedObject = steamVRManager.left.GetComponent<SteamVR_TrackedObject>();

            if (_trackedObject != null) {
                _connected = true;
            }

            base.Initialize();
        }

        public override bool ButtonDown() {
            if (!_connected)
                return false;

            int index = controllerIndex;
            if (controllerIndex != -1) {
                var device = SteamVR_Controller.Input(controllerIndex);
                if (device != null) {
                    var result = device.GetPressDown(InteractionButton);
                    return result;
                }
            }

            return false;
        }

        public override bool ButtonUp() {
            if (!_connected)
                return false;

            int index = controllerIndex;
            if (controllerIndex != -1) {
                var device = SteamVR_Controller.Input(controllerIndex);
                if (device != null)
                    return device.GetPressUp(InteractionButton);
            }

            return false;
        }

        public override void OnEnterControl(GameObject control) {
            if (_connected)
                applyHapticPulse(600);
        }

        public override void OnExitControl(GameObject control) {
            if (_connected)
                applyHapticPulse(1000);
        }


        private void applyHapticPulse(ushort value) {
            if (UseHapticPulse) {
                var device = SteamVR_Controller.Input(controllerIndex);
                device.TriggerHapticPulse(value);
            }
        }


        int controllerIndex {
            get {
                if (!_connected) return 0;
                return (int)(_trackedObject.index);
            }
        }

        public override Transform target {
            get {
                if (_trackedObject)
                    return _trackedObject.transform;
                else
                    throw new System.Exception("Couldn't instantiate Vive controllers!");
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