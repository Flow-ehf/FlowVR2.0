using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Weelco.VRInput {

    public class VRHitInputModule : VRInputModule {

        // controller data
        private Dictionary<IUIHitPointer, VRInputControllerData> controllerData = new Dictionary<IUIHitPointer, VRInputControllerData>();
                
        public override void AddController(IUIPointer controller) {
            if (controller is IUIHitPointer)
                controllerData.Add(controller as IUIHitPointer, new VRInputControllerData());
        }

        public override void RemoveController(IUIPointer controller) {
            if (controller is IUIHitPointer)
                controllerData.Remove(controller as IUIHitPointer);
        }

        public override void Process()
        {
            foreach(var pair in controllerData) {

                IUIHitPointer controller = pair.Key;
                VRInputControllerData data = pair.Value;

                // test if UICamera is looking at a GUI element
                UpdateCameraPosition(controller);

                if(data.pointerEvent == null)
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
                if(data.pointerEvent.pointerCurrentRaycast.distance > 0.0f)
                    controller.LimitLaserDistance(data.pointerEvent.pointerCurrentRaycast.distance + 0.01f);

                // Send control enter and exit events to our controller
                var hitControl = data.pointerEvent.pointerCurrentRaycast.gameObject;
                if(data.currentPoint != hitControl) {
                    if(data.currentPoint != null)
                        controller.OnExitControl(data.currentPoint);

                    if(hitControl != null)
                        controller.OnEnterControl(hitControl);
                }

                data.currentPoint = hitControl;

                // handle enter and exit events on the GUI controlls that are hit
                base.HandlePointerExitAndEnter(data.pointerEvent, data.currentPoint);

                if(controller.ButtonDown()) {
                    ClearSelection();

                    data.pointerEvent.pressPosition = data.pointerEvent.position;
                    data.pointerEvent.pointerPressRaycast = data.pointerEvent.pointerCurrentRaycast;
                    data.pointerEvent.pointerPress = null;

                    // update current pressed if the curser is over an element
                    if(data.currentPoint != null) {
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

                        if (newPressed != null) {
                            data.pointerEvent.pointerPress = newPressed;
                            data.currentPressed = newPressed;
                            Select(data.currentPressed);
                        }

                        ExecuteEvents.Execute(data.currentPressed, data.pointerEvent, ExecuteEvents.beginDragHandler);
                        ExecuteEvents.Execute(controller.target.gameObject, data.pointerEvent, ExecuteEvents.beginDragHandler);

                        data.pointerEvent.pointerDrag = data.currentPressed;
                    }
                }

                if(controller.ButtonUp()) {                                        
                    if(data.currentPressed) {
                        data.pointerEvent.current = data.currentPressed;
                        ExecuteEvents.Execute(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        ExecuteEvents.Execute(controller.target.gameObject, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        data.pointerEvent.rawPointerPress = null;
                        data.pointerEvent.pointerPress = null;
                        data.currentPressed = null;
                    }
                    ClearSelection();
                }

                // update selected element for keyboard focus
                if (base.eventSystem.currentSelectedGameObject != null) {
                    data.pointerEvent.current = eventSystem.currentSelectedGameObject;
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
                }
            }
        }

        // select a game object
        private void Select(GameObject go) {
            ClearSelection();

            if (ExecuteEvents.GetEventHandler<ISelectHandler>(go)) {
                base.eventSystem.SetSelectedGameObject(go);
            }
        }
    }
}