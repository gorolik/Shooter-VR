using System;
using Unity.Netcode;
using UnityEngine;

namespace Sources.Master.ManipulatingSystem
{
    public class ObjectSelector : NetworkBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Material _highlightedMaterial;
        [SerializeField] private Material _selectedMaterial;

        private Selectable _currentHighlighted;
        private Selectable _currentSelected;

        public Action<Selectable> ObjectSelected;

        private void Update()
        {
            if (!IsOwner)
                return;
            
            HandleHover();
            HandleClick();
        }

        private void HandleHover()
        {
            var hitSelectable = RaycastForSelectable();
            
            if (hitSelectable == _currentHighlighted)
                return;
            
            if (_currentHighlighted && _currentHighlighted != _currentSelected)
                _currentHighlighted.HighlightByMaterial(null);

            _currentHighlighted = hitSelectable;
            
            if (_currentHighlighted && _currentHighlighted != _currentSelected)
                _currentHighlighted.HighlightByMaterial(_highlightedMaterial);
        }

        private void HandleClick()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var hitSelectable = RaycastForSelectable();

            if (hitSelectable && hitSelectable != _currentSelected)
                SelectNew(hitSelectable);
            else if (!hitSelectable && _currentSelected && !RaycastForDragAxisHandle()) 
                DeselectCurrent();
        }

        private Selectable RaycastForSelectable()
        {
            Collider col = RaycastForCollider();
            if (col)
                return col.GetComponent<Selectable>();
            return null;
        }
        
        private DragAxisHandle RaycastForDragAxisHandle()
        {
            Collider col = RaycastForCollider();
            if (col)
                return col.GetComponent<DragAxisHandle>();
            return null;
        }

        private Collider RaycastForCollider()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask)
                ? hit.collider
                : null;
        }

        private void SelectNew(Selectable selectable)
        {
            if (_currentSelected)
            {
                _currentSelected.Deselect();
                _currentSelected.HighlightByMaterial(null);
            }
            
            _currentSelected = selectable;
            _currentSelected.Select();
            _currentSelected.HighlightByMaterial(_selectedMaterial);
            ObjectSelected?.Invoke(selectable);
        }

        private void DeselectCurrent()
        {
            _currentSelected.Deselect();
            _currentSelected.HighlightByMaterial(null);
            _currentSelected = null;
            ObjectSelected?.Invoke(null);
        }
    }
}