using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace mj.gist {
    [VFXBinder("Transform/Distance")]
    public class MousePositionBinder : VFXBinderBase {
        [VFXPropertyBinding("UnityEngine.Vector3")]
        public ExposedProperty mouseWorldProperty;
        [SerializeField] private float depth = 0;

        public override bool IsValid(VisualEffect component) {
            return component.HasVector3(mouseWorldProperty);
        }

        public override void UpdateBinding(VisualEffect component) {
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = depth;
            component.SetVector3(mouseWorldProperty, worldPosition);
        }
    }
}
