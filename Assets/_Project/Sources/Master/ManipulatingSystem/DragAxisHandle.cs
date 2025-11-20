using Sources.Items;
using UnityEngine;

namespace Sources.Master.ManipulatingSystem
{
    public class DragAxisHandle : MonoBehaviour
    {
        [SerializeField] private Transformable _target;
        [SerializeField] private float _sensitivity = 0.01f;

        private Camera _camera;
        private bool _isDragging;
        private Vector3 _dragStartMousePos;
        private Vector3 _dragStartTargetPos;

        public void Init(Camera handleCamera) => 
            _camera = handleCamera;

        public void SetTarget(Transformable target) => 
            _target = target;

        private void OnMouseDown()
        {
            if (!_target) 
                return;

            _isDragging = true;
            _dragStartMousePos = Input.mousePosition;
            _dragStartTargetPos = _target.transform.position;
            
            _target.BeginPCManipulation();
        }

        private void OnMouseUp()
        {
            if (!_isDragging)
                return;
            
            _isDragging = false;
            _target.EndPCManipulation();
        }

        private void OnMouseDrag()
        {
            if (!_isDragging) 
                return;

            Vector3 moveDirection = transform.forward;
            Vector3 mouseDelta = Input.mousePosition - _dragStartMousePos;
            Vector3 axisScreenDir = _camera.WorldToScreenPoint(_target.transform.position + moveDirection) -
                                    _camera.WorldToScreenPoint(_target.transform.position);
            axisScreenDir.z = 0f;

            float distanceAlongAxis = Vector3.Dot(mouseDelta, axisScreenDir.normalized) * _sensitivity;

            _target.transform.position = _dragStartTargetPos + moveDirection * distanceAlongAxis;
        }
    }
}