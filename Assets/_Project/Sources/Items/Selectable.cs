using UnityEngine;

namespace Sources.Items
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        private Material _defaultMaterial;

        private void Start()
        {
            _defaultMaterial = _meshRenderer.material;
        }

        public void HighlightByMaterial(Material material)
        {
            if (material)
                _meshRenderer.material = material;
            else
                _meshRenderer.material = _defaultMaterial;
        }
        
        public virtual void Select() { }

        public virtual void Deselect() { }
    }
}