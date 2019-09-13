using UnityEngine;
using UnityEngine.EventSystems;

namespace Weelco.VRInput {

    public abstract class VRInputModule : BaseInputModule {
        
        public static VRInputModule instance { get { return _instance; } }
        private static VRInputModule _instance = null;

        private Camera UICamera;

        abstract public void AddController(IUIPointer controller);
        abstract public void RemoveController(IUIPointer controller);

        protected override void Awake() {
            base.Awake();

            if (_instance != null) {
                Debug.LogWarning("Trying to instantiate multiple VRInputModule::" + this);
                DestroyImmediate(this.gameObject);
            }

            _instance = this;
        }

        protected override void Start() {
            base.Start();

            // Create a new camera that will be used for raycasts
            UICamera = new GameObject("DummyCamera").AddComponent<Camera>();
            UICamera.clearFlags = CameraClearFlags.Nothing;
            UICamera.enabled = false;
            UICamera.fieldOfView = 5;
            UICamera.nearClipPlane = 0.01f;

            // Find canvases in the scene and assign our custom
            // UICamera to them
            Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas canvas in canvases) {
                canvas.worldCamera = UICamera;
            }
        }
        
        protected void UpdateCameraPosition(IUIPointer controller) {
            UICamera.transform.position = controller.target.transform.position;
            UICamera.transform.rotation = controller.target.transform.rotation;            
        }

        protected Vector2 GetCameraSize() {
            return new Vector2(UICamera.pixelWidth, UICamera.pixelHeight);
        }

        // clear the current selection
        protected void ClearSelection() {
            if (base.eventSystem.currentSelectedGameObject) {
                base.eventSystem.SetSelectedGameObject(null);
            }
        }
    }
}