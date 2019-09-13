using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Weelco.VRInput {

    public class VRInputSettings : MonoBehaviour {

        public InputControlMethod ControlMethod;

        [SerializeField]
        Hand usedHand = Hand.Right;

#if VR_INPUT_OCULUS
        [SerializeField]
        OVRInput.Button oculusTouchInteractionButton = OVRInput.Button.PrimaryIndexTrigger;

        [SerializeField]
        OVRInput.Button oculusInputInteractionButton = OVRInput.Button.Start;
#endif

#if VR_INPUT_VIVE
        [SerializeField]
        Valve.VR.EVRButtonId viveInteractionButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
#endif

        [SerializeField]
        Transform gazeCanvas;

        [SerializeField]
        Image gazeProgressBar;

        [SerializeField]
        float gazeClickTimer = 1.0f;

        [SerializeField]
        float gazeClickTimerDelay = 1.0f;

        [SerializeField]
        float laserThickness = 0.01f;

        [SerializeField]
        float laserHitScale = 1.0f;

        [SerializeField]
        Color laserColor = Color.red;

        [SerializeField]
        bool useHapticPulse = false;

        [SerializeField]
        bool useCustomLaserPointer = false;

        [SerializeField]
        bool hitAlwaysOn = true;

        private List<IUIPointer> _pointersList;

        public static VRInputSettings instance { get { return _instance; } }
        private static VRInputSettings _instance = null;

        void Awake() {
            if (_instance != null) {
                Debug.LogWarning("Trying to instantiate multiple VRInputSystems.");
                DestroyImmediate(this.gameObject);
            }

            _instance = this;
        }


        void Start() {

            _pointersList = new List<IUIPointer>();

            createEventSystem(ControlMethod);

            switch (ControlMethod) {
                case InputControlMethod.OCULUS_TOUCH: {
#if VR_INPUT_OCULUS
                        bool isRightHand = usedHand.Equals(Hand.Right);
                        createOculusTouch(isRightHand);
                        if (usedHand.Equals(Hand.Both))
                            createOculusTouch(!isRightHand);
#endif
                        break;
                    }
                case InputControlMethod.OCULUS_INPUT: {
#if VR_INPUT_OCULUS
                        createOculusInput();
#endif
                        break;
                    }
                case InputControlMethod.GAZE: {
                        createGazePointer();
                        break;
                    }

                case InputControlMethod.VIVE: {
#if VR_INPUT_VIVE
                        bool isRightHand = usedHand.Equals(Hand.Right);
                        createVivePointer(isRightHand);
                        if (usedHand.Equals(Hand.Both))
                            createVivePointer(!isRightHand);
#endif
                        break;
                    }
            }
        }

        void Update() {
            foreach (IUIPointer pointer in _pointersList) {
                pointer.Update();
            }
        }

        void OnDestroy() {

            if (VRInputModule.instance != null) {
                foreach (IUIPointer pointer in _pointersList) {
                    VRInputModule.instance.RemoveController(pointer);
                }
                _pointersList.Clear();
            }
        }

        private void createEventSystem(InputControlMethod method) {
            GameObject eventSystem = null;
            if (method.Equals(InputControlMethod.MOUSE) ||
                method.Equals(InputControlMethod.GOOGLEVR))
                return;

            if (GameObject.FindObjectOfType<EventSystem>() != null) eventSystem = GameObject.FindObjectOfType<EventSystem>().gameObject;
            if (eventSystem == null) {
                eventSystem = new GameObject("EventSystem");
            }
            MonoBehaviour[] comps = eventSystem.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in comps) {
                if (comp is EventSystem)
                    continue;
                else
                    comp.enabled = false;
            }
            if (eventSystem.GetComponent<VRInputModule>() == null) {
                if (VRInputModule.instance == null) {
                    VRInputModule vrim;
                    switch (method) {
                        case InputControlMethod.GAZE:
                        case InputControlMethod.GOOGLEVR:
                            vrim = eventSystem.AddComponent<VRGazeInputModule>();
                            break;
                        case InputControlMethod.VIVE:
                        case InputControlMethod.OCULUS_TOUCH:
                        case InputControlMethod.OCULUS_INPUT:
                            vrim = eventSystem.AddComponent<VRHitInputModule>();
                            break;
                        default:
                            vrim = eventSystem.AddComponent<VRInputModule>();
                            break;
                    }
                    vrim.enabled = true;
                }
            }
        }


#if VR_INPUT_OCULUS
        private void createOculusTouch(bool isRightHand) {

            OVRInput.Controller controller = isRightHand ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;
            OVRUILaserPointer pointer = new OVRUILaserPointer();
            pointer.primaryTrigger = oculusTouchInteractionButton;
            pointer.controller = controller;
            pointer.IsRightHand = isRightHand;
            pointer.LaserThickness = laserThickness;
            pointer.LaserHitScale = laserHitScale;
            pointer.LaserColor = laserColor;
            pointer.UseHapticPulse = useHapticPulse;
            pointer.UseCustomLaserPointer = useCustomLaserPointer;
            pointer.HitAlwaysOn = false;
            initPointer(pointer);
        }

        private void createOculusInput() {

            OVRUIHitPointer pointer = new OVRUIHitPointer();
            pointer.primaryTrigger = oculusInputInteractionButton;
            pointer.controller = OVRInput.Controller.Active;
            pointer.LaserHitScale = laserHitScale;
            pointer.LaserColor = laserColor;
            pointer.HitAlwaysOn = hitAlwaysOn;
            pointer.UseCustomLaserPointer = useCustomLaserPointer;
            initPointer(pointer);
        }
#endif

#if VR_INPUT_VIVE
        private void createVivePointer(bool isRightHand) {
               
            ViveUILaserPointer pointer = new ViveUILaserPointer();        
            pointer.InteractionButton = viveInteractionButton;
            pointer.IsRightHand = isRightHand;
            pointer.LaserThickness = laserThickness;
            pointer.LaserHitScale = laserHitScale;
            pointer.LaserColor = laserColor;
            pointer.UseHapticPulse = useHapticPulse;
            pointer.UseCustomLaserPointer = useCustomLaserPointer;
            initPointer(pointer);    
        }
#endif

        private void createGazePointer() {
            UIGazePointer pointer = new UIGazePointer();
            pointer.GazeCanvas = gazeCanvas;
            pointer.GazeProgressBar = gazeProgressBar;
            pointer.GazeClickTimer = gazeClickTimer;
            pointer.GazeClickTimerDelay = gazeClickTimerDelay;
            initPointer(pointer);
        }

        private void initPointer(IUIPointer pointer) {
            VRInputModule.instance.AddController(pointer);
            pointer.Initialize();
            _pointersList.Add(pointer);
        }


#if VR_INPUT_OCULUS
        public OVRInput.Button OculusTouchInteractionButton {
            get { return oculusTouchInteractionButton; }
            set {
                oculusTouchInteractionButton = value;
            }
        }

        public OVRInput.Button OculusInputInteractionButton {
            get { return oculusInputInteractionButton; }
            set {
                oculusInputInteractionButton = value;
            }
        }

#endif

#if VR_INPUT_VIVE
        public Valve.VR.EVRButtonId ViveInteractionButton {
            get { return viveInteractionButton; }
            set {
                viveInteractionButton = value;
            }
        }
#endif

        public float LaserThickness {
            get { return laserThickness; }
            set { laserThickness = value; }
        }

        public float LaserHitScale {
            get { return laserHitScale; }
            set { laserHitScale = value; }
        }

        public Color LaserColor {
            get { return laserColor; }
            set { laserColor = value; }
        }

        public bool UseHapticPulse {
            get { return useHapticPulse; }
            set { useHapticPulse = value; }
        }

        public bool UseCustomLaserPointer {
            get { return useCustomLaserPointer; }
            set { useCustomLaserPointer = value; }
        }

        public bool HitAlwaysOn {
            get { return hitAlwaysOn; }
            set { hitAlwaysOn = value; }
        }

        public Hand UsedHand {
            get { return usedHand; }
            set { usedHand = value; }
        }

        public float GazeClickTimer {
            get { return gazeClickTimer; }
            set { gazeClickTimer = value; }
        }

        public float GazeClickTimerDelay {
            get { return gazeClickTimerDelay; }
            set { gazeClickTimerDelay = value; }
        }

        public Transform GazeCanvas {
            get { return gazeCanvas; }
            set { gazeCanvas = value; }
        }

        public Image GazeProgressBar {
            get { return gazeProgressBar; }
            set { gazeProgressBar = value; }
        }

        //enums
        public enum InputControlMethod {
            MOUSE = 0,
            GOOGLEVR = 1,
            GAZE = 2,
            VIVE = 3,
            OCULUS_INPUT = 4,
            OCULUS_TOUCH = 5
        }

        public enum Hand {
            Both = 0,
            Right = 1,
            Left = 2,
        }
    }
}
