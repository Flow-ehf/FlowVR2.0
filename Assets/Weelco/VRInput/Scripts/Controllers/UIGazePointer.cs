using UnityEngine;
using UnityEngine.UI;

namespace Weelco.VRInput {

    public class UIGazePointer : IUIPointer {
                
        public Transform GazeCanvas;
        public Image GazeProgressBar;

        public float GazeClickTimer;
        public float GazeClickTimerDelay;
                                
        private bool _isOver = false;

        // Use this for initialization
        public override void Initialize() {            
            // For override
        }
        
        public override void OnEnterControl(GameObject control) {
            _isOver = true;
        }

        public override void OnExitControl(GameObject control) {
            _isOver = false;
        }

        public bool IsOver() {
            return _isOver;
        }        
        
        public override Transform target {
            get {
                if (GazeCanvas == null) {
                    throw new System.NullReferenceException("VRInputSettings::While Gaze Input , must contain Gaze Dot GameObject");
                }
                return GazeCanvas;
            }
        }
                
        protected override void UpdateRaycasting(bool isHit, float distance) {
            // For override
        }
    }
}