using UnityEngine;
using UnityEngine.EventSystems;

namespace Weelco.VRInput {

    public class VRInputEventData : PointerEventData
    {
        public GameObject current;
        public IUIPointer controller;
        public VRInputEventData(EventSystem e) : base(e) { }

        public override void Reset()
        {
            current = null;
            controller = null;
            base.Reset();
        }
    }
}

