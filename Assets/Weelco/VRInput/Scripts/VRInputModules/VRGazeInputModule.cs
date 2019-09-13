using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Weelco.VRInput {

    public class VRGazeInputModule : VRInputModule {
        
        private GameObject lastActiveButton;
        private GameObject currentLook;
        private float lookTimer;

        //private float timeToLookPress = 1.0f;

        // controller data
        private Dictionary<UIGazePointer, VRInputControllerData> controllerData = new Dictionary<UIGazePointer, VRInputControllerData>();

        public override void AddController(IUIPointer controller) {
            if (controller is UIGazePointer)
                controllerData.Add(controller as UIGazePointer, new VRInputControllerData());
        }

        public override void RemoveController(IUIPointer controller) {
            if (controller is UIGazePointer)
                controllerData.Remove(controller as UIGazePointer);
        }
        

        public override void Process() {

            foreach (var pair in controllerData) {
                UIGazePointer controller = pair.Key;
                VRInputControllerData data = pair.Value;

                // test if UICamera is looking at a GUI element
                UpdateCameraPosition(controller);

                if (data.pointerEvent == null)
                    data.pointerEvent = new VRInputEventData(eventSystem);
                else
                    data.pointerEvent.Reset();

                data.pointerEvent.controller = controller;
                data.pointerEvent.delta = Vector2.zero;
                data.pointerEvent.position = new Vector2(GetCameraSize().x * 0.5f, GetCameraSize().y * 0.5f);

                // trigger a raycast
                eventSystem.RaycastAll(data.pointerEvent, m_RaycastResultCache);
                data.pointerEvent.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                m_RaycastResultCache.Clear();

                // make sure our controller knows about the raycast result
                if (data.pointerEvent.pointerCurrentRaycast.distance > 0.0f)
                    controller.LimitLaserDistance(data.pointerEvent.pointerCurrentRaycast.distance + 0.01f);

                // send control enter and exit events to our controller
                var hitControl = data.pointerEvent.pointerCurrentRaycast.gameObject;
                if (data.currentPoint != hitControl) {
                    if (data.currentPoint != null) {
                        ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        controller.OnExitControl(data.currentPoint);
                    }
                    if (hitControl != null) {
                        controller.OnEnterControl(hitControl);
                    }
                }

                data.currentPoint = hitControl;

                currentLook = hitControl;
                if (hitControl == null) {
                    ClearSelection();
                }

                // handle enter and exit events on the GUI controlls that are hit
                base.HandlePointerExitAndEnter(data.pointerEvent, data.currentPoint);

                Image circleProgressBar = controller.GazeProgressBar;
                float timeToLookPress = controller.GazeClickTimer;
                float timeToLookPressDelay = controller.GazeClickTimerDelay;

                if ((currentLook != null) && (timeToLookPress > 0)) {
                    bool clickable = false;
                    if (currentLook.transform.gameObject.GetComponent<Button>() != null) clickable = true;
                    if (currentLook.transform.parent != null) {
                        if (currentLook.transform.parent.gameObject.GetComponent<Button>() != null) clickable = true;
                        if (currentLook.transform.parent.gameObject.GetComponent<Toggle>() != null) clickable = true;
                        if (currentLook.transform.parent.gameObject.GetComponent<Slider>() != null) clickable = true;
                        if (currentLook.transform.parent.parent != null) {
                            if (currentLook.transform.parent.parent.gameObject.GetComponent<Slider>() != null) {
                                if (currentLook.name != "Handle") clickable = true;
                            }
                            if (currentLook.transform.parent.parent.gameObject.GetComponent<Toggle>() != null) clickable = true;
                        }
                    }

                    if (clickable) {
                        if (lastActiveButton == currentLook) {
                            if (circleProgressBar) {
                                if (circleProgressBar.isActiveAndEnabled) {
                                    circleProgressBar.fillAmount = (Time.realtimeSinceStartup - lookTimer) / timeToLookPress;
                                }
                                else if (Time.realtimeSinceStartup - lookTimer > 0) {
                                    circleProgressBar.fillAmount = 0f;
                                    circleProgressBar.gameObject.SetActive(true);
                                    ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                                }
                            }

                            if (Time.realtimeSinceStartup - lookTimer > timeToLookPress) {
                                if (circleProgressBar) {
                                    circleProgressBar.gameObject.SetActive(false);
                                }
                                // simulate Press state
                                data.pointerEvent.pressPosition = data.pointerEvent.position;
                                data.pointerEvent.pointerPressRaycast = data.pointerEvent.pointerCurrentRaycast;
                                data.pointerEvent.pointerPress = null;

                                // update current pressed if the curser is over an element
                                if (data.currentPoint != null) {
                                    data.currentPressed = data.currentPoint;
                                    data.pointerEvent.current = data.currentPressed;
                                    GameObject newPressed = ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerDownHandler);
                                    ExecuteEvents.Execute(controller.target.gameObject, data.pointerEvent, ExecuteEvents.pointerDownHandler);

                                    if (newPressed == null) {
                                        // some UI elements might only have click handler and not pointer down handler
                                        newPressed = ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                                        ExecuteEvents.Execute(controller.target.gameObject, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                                        if (newPressed != null) {
                                            data.currentPressed = newPressed;
                                        }
                                    }
                                    else {
                                        data.currentPressed = newPressed;
                                        ExecuteEvents.Execute(newPressed, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                                        ExecuteEvents.Execute(controller.target.gameObject, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                                    }
                                }
                                // end simulation
                                lookTimer = Time.realtimeSinceStartup + timeToLookPress * timeToLookPressDelay;
                            }
                        }
                        else {
                            lastActiveButton = currentLook;
                            lookTimer = Time.realtimeSinceStartup;
                            if (circleProgressBar && circleProgressBar.isActiveAndEnabled) {
                                circleProgressBar.gameObject.SetActive(false);
                            }
                        }
                    }
                    else {
                        lastActiveButton = null;
                        if (circleProgressBar && circleProgressBar.isActiveAndEnabled) {
                            circleProgressBar.gameObject.SetActive(false);
                        }
                        ClearSelection();
                    }
                }
                else {
                    if (circleProgressBar) {
                        circleProgressBar.gameObject.SetActive(false);
                    }
                    lastActiveButton = null;
                    ClearSelection();
                }
            }
        }
    }
}