using UnityEngine;
using UnityEngine.UI;
using Weelco.VRKeyboard;

namespace Weelco {

    public class VRKeyboardDemo : MonoBehaviour {

        public int maxOutputChars = 30;

        public Text inputFieldLabel;
        public VRKeyboardFull keyboard;

        void Start() {
            if (keyboard) {
                keyboard.OnVRKeyboardBtnClick += HandleClick;
                keyboard.Init();
            }
        }

        void OnDestroy() {
            if (keyboard) {
                keyboard.OnVRKeyboardBtnClick -= HandleClick;
            }
        }
        
        private void HandleClick(string value) {
            if (value.Equals(VRKeyboardData.BACK)) {
                BackspaceKey();
            }
            else if (value.Equals(VRKeyboardData.ENTER)) {
                EnterKey();
            }
            else {
                TypeKey(value);
            }
        }

        private void BackspaceKey() {
            if (inputFieldLabel.text.Length >= 1) {
                inputFieldLabel.text = inputFieldLabel.text.Remove(inputFieldLabel.text.Length - 1, 1);
            }
        }    

        private void EnterKey() {
            // Add enter key handler
        }

        private void TypeKey(string value) {
            char[] letters = value.ToCharArray();
            for (int i = 0; i < letters.Length; i++) {
                TypeKey(letters[i]);
            }
        }

        private void TypeKey(char key) {
            if (inputFieldLabel.text.Length < maxOutputChars) {
                inputFieldLabel.text += key.ToString();
            }
        }    
    }
}