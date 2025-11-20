using System;
using Sources.Items;
using UnityEngine;

namespace Sources.Master.ManipulatingSystem
{
    public class TransformInterface : MonoBehaviour
    {
        [SerializeField] private GameObject _view;
        [SerializeField] private DragAxisHandle[] _dragAxisHandles;
        
        private Transformable _target;

        public void Init(Camera handleCamera)
        {
            foreach (DragAxisHandle axisHandle in _dragAxisHandles) 
                axisHandle.Init(handleCamera);
            
            SetTarget(null);
        }
        
        private void LateUpdate()
        {
            if (!_target)
                return;
            
            transform.position = _target.transform.position;
            transform.rotation = Quaternion.identity;
        }

        public void SetTarget(Transformable target)
        {
            _target = target;
            _view.SetActive(target);
            
            foreach (DragAxisHandle axisHandle in _dragAxisHandles) 
                axisHandle.SetTarget(target);
        }
    }
}