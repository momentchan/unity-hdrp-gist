using UnityEngine;
using UnityEngine.Rendering;

namespace mj.gist
{
    [RequireComponent(typeof(Volume))]

    public class VolumeWrapper : MonoBehaviour
    {
        private Volume volume;
        private VolumeProfile profile => volume.profile;

        public T Get<T>() where T : VolumeComponent
        {
            if (!profile.TryGet(out T component))
                return null;
            else 
                return component;
        }

        void Start()
        {
            volume = GetComponent<Volume>();
        }
    }
}