using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace mj.gist {
    [VFXBinder("Transform/Distance")]
    public class MousePositionBinder : VFXBinderBase {
        [VFXPropertyBinding("UnityEngine.Vector3")]
        public ExposedProperty mouseWorldProperty;
        [VFXPropertyBinding("UnityEngine.Vector3")]
        public ExposedProperty mouseViewportProperty;
        [SerializeField] private float depth = 0;

        public override bool IsValid(VisualEffect component) {
            return component.HasVector3(mouseWorldProperty) || component.HasVector3(mouseViewportProperty);
        }

        public override void UpdateBinding(VisualEffect component) {
            var mouseScreen = Input.mousePosition;
            mouseScreen.z = depth;

            var worldPosition = Camera.main.ScreenToWorldPoint(mouseScreen);
            var viewPosition = Camera.main.ScreenToViewportPoint(mouseScreen);

            if (component.HasVector3(mouseWorldProperty))
                component.SetVector3(mouseWorldProperty, worldPosition);
            if (component.HasVector3(mouseViewportProperty))
                component.SetVector3(mouseViewportProperty, viewPosition);
        }
    }
}
