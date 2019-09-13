using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Weelco.VRInput {

    [ExecuteInEditMode]
    [CustomEditor(typeof(VRInputSettings))]
    public class VRInputSettingsEditor : Editor {

        bool loadingCustomDefine = false;

        // Use this for initialization
        void Start() {

        }

        public override void OnInspectorGUI() {

            VRInputSettings myTarget = (VRInputSettings)target;

            if (target == null) return;

            // Initial settings
            GUI.changed = false;
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = 180;

            // Draw control methods
            myTarget.ControlMethod = (VRInputSettings.InputControlMethod)EditorGUILayout.EnumPopup("Control Method", myTarget.ControlMethod);

            switch (myTarget.ControlMethod) {
                case VRInputSettings.InputControlMethod.MOUSE: {
                        CreateLabel("Use Mouse on screen to interact with keyboard", EditorStyles.helpBox);
                        break;
                    }
                case VRInputSettings.InputControlMethod.GOOGLEVR: {
                        CreateLabel("Use Gvr Viewer to interact with keyboard", EditorStyles.helpBox);
                        break;
                    }

                case VRInputSettings.InputControlMethod.OCULUS_TOUCH: {
#if VR_INPUT_OCULUS
                        // oculus enabled, we can show settings
                        CreateLabel("Use Touch controllers to interact with keyboard.", EditorStyles.helpBox);
                        // hand property
                        myTarget.UsedHand = (VRInputSettings.Hand)EditorGUILayout.EnumPopup("Hand", myTarget.UsedHand);
                        // button property
                        myTarget.OculusTouchInteractionButton = (OVRInput.Button)EditorGUILayout.EnumPopup("Interaction Button", myTarget.OculusTouchInteractionButton);
                        // laser property              
                        myTarget.UseHapticPulse = EditorGUILayout.Toggle("UseHapticPulse", myTarget.UseHapticPulse);
                        myTarget.UseCustomLaserPointer = EditorGUILayout.Toggle("UseCustomLaserPointer", myTarget.UseCustomLaserPointer);
                        if (myTarget.UseCustomLaserPointer) {
                            myTarget.LaserThickness = EditorGUILayout.FloatField("LaserThickness", myTarget.LaserThickness);
                            myTarget.LaserHitScale = EditorGUILayout.FloatField("LaserHitScale", myTarget.LaserHitScale);
                            myTarget.LaserColor = EditorGUILayout.ColorField("LaserColor", myTarget.LaserColor);
                        }
#else
                        DrawCustomDefineSwitcher("VR_INPUT_OCULUS");
#endif
                        break;
                    }
                case VRInputSettings.InputControlMethod.OCULUS_INPUT: {
#if VR_INPUT_OCULUS
                        // oculus enabled, we can show settings
                        CreateLabel("Use Gamepad or Remote control to interact with keyboard.", EditorStyles.helpBox);
                        // button property
                        myTarget.OculusInputInteractionButton = (OVRInput.Button)((int)(OVRInput.Button)EditorGUILayout.EnumMaskField("Interaction Button", (OVRInput.Button)((int)(myTarget.OculusInputInteractionButton) << 1)) >> 1);
                        // 
                        myTarget.HitAlwaysOn = EditorGUILayout.Toggle("LaserDotAlwaysVisible", myTarget.HitAlwaysOn);
                        // laser property  
                        myTarget.UseCustomLaserPointer = EditorGUILayout.Toggle("UseCustomLaserPointer", myTarget.UseCustomLaserPointer);
                        if (myTarget.UseCustomLaserPointer) {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(180);
                            GUILayout.BeginVertical();
                            GUILayout.Label("Setup your settings", EditorStyles.helpBox);
                            myTarget.LaserHitScale = EditorGUILayout.FloatField("LaserHitScale", myTarget.LaserHitScale);
                            myTarget.LaserColor = EditorGUILayout.ColorField("LaserColor", myTarget.LaserColor);
                            GUILayout.Space(10);
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        }
#else
                        DrawCustomDefineSwitcher("VR_INPUT_OCULUS");
#endif
                        break;
                    }
                case VRInputSettings.InputControlMethod.VIVE: {
#if VR_INPUT_VIVE
                        // oculus enabled, we can show settings
                        CreateLabel("Use Vive controllers to interact with keyboard.", EditorStyles.helpBox);
                        //hand property
                        myTarget.UsedHand = (VRInputSettings.Hand)EditorGUILayout.EnumPopup("Hand", myTarget.UsedHand);
                        //button property
                        myTarget.ViveInteractionButton = (Valve.VR.EVRButtonId)EditorGUILayout.EnumPopup("Interaction Button", myTarget.ViveInteractionButton);
                        //laser property                    
                        myTarget.UseHapticPulse = EditorGUILayout.Toggle("UseHapticPulse", myTarget.UseHapticPulse);
                        myTarget.UseCustomLaserPointer = EditorGUILayout.Toggle("UseCustomLaserPointer", myTarget.UseCustomLaserPointer);
                        if (myTarget.UseCustomLaserPointer) {
                            myTarget.LaserThickness = EditorGUILayout.FloatField("LaserThickness", myTarget.LaserThickness);
                            myTarget.LaserHitScale = EditorGUILayout.FloatField("LaserHitScale", myTarget.LaserHitScale);
                            myTarget.LaserColor = EditorGUILayout.ColorField("LaserColor", myTarget.LaserColor);
                        }
#else
                        DrawCustomDefineSwitcher("VR_INPUT_VIVE");
#endif
                        break;
                    }
                case VRInputSettings.InputControlMethod.GAZE: {
                        CreateLabel("Center of Canvas's Event Camera acts as a pointer. Require Canvas for interaction with keyboard", EditorStyles.helpBox);
                        myTarget.GazeCanvas = (Transform)EditorGUILayout.ObjectField("Gaze Canvas [Required]", myTarget.GazeCanvas, typeof(Transform), true);
                        myTarget.GazeProgressBar = (Image)EditorGUILayout.ObjectField("Gaze ProgressBar [Optional]", myTarget.GazeProgressBar, typeof(Image), true);
                        myTarget.GazeClickTimer = EditorGUILayout.FloatField("GazeClickTimer", myTarget.GazeClickTimer);
                        myTarget.GazeClickTimerDelay = EditorGUILayout.FloatField("GazeClickTimerDelay", myTarget.GazeClickTimerDelay);
                        break;
                    }
            }

            EditorGUILayout.Space();

            // Final settings
            if (GUI.changed && myTarget != null)
                EditorUtility.SetDirty(myTarget);
        }

        void DrawCustomDefineSwitcher(string switcho) {

            GUILayout.BeginHorizontal();
            GUILayout.Space(180);
            GUILayout.BeginVertical();

            GUILayout.Label("Press the button below to recompile for this control method.", EditorStyles.helpBox);

            if (GUILayout.Button(loadingCustomDefine ? "Recompiling..." : "Enable")) {
                loadingCustomDefine = true;
                string str = "";
                str += PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

                //remove curvedui defines
                foreach (string define in new string[] { "VR_INPUT_VIVE", "VR_INPUT_OCULUS" }) {
                    if (str.Contains(define)) {
                        if (str.Contains((";" + define)))
                            str = str.Replace((";" + define), "");
                        else
                            str = str.Replace(define, "");
                    }
                }


                //add this one, if not present.
                if (switcho != "" && !str.Contains(switcho))
                    str += ";" + switcho;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void CreateLabel(string label, GUIStyle style) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(180);
            GUILayout.BeginVertical();
            GUILayout.Label(label, style);
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}