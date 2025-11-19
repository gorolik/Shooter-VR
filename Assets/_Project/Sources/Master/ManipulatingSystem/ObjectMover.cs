using System;
using UnityEngine;

namespace Sources.Master.ManipulatingSystem
{
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField] private Camera _movingCamera;
        [SerializeField] private ObjectSelector _objectSelector;
        [SerializeField] private TransformInterface _transformInterface;
        
        private Transformable _transformable;

        private void Start()
        {
            _transformInterface.Init(_movingCamera);
            
            _objectSelector.ObjectSelected += OnObjectSelected;
        }
        
        private void OnDestroy()
        {
            _objectSelector.ObjectSelected -= OnObjectSelected;
        }

        private void OnObjectSelected(Selectable selectable)
        {
            if (!selectable)
            {
                ObjectDeselected();
                return;
            }

            if (selectable is Transformable transformable)
                _transformable = transformable;
            else
            {
                ObjectDeselected();
                return;
            }

            _transformInterface.SetTarget(_transformable);
        }

        private void ObjectDeselected()
        {
            _transformInterface.SetTarget(null);
            _transformable = null;
        }
    }
}