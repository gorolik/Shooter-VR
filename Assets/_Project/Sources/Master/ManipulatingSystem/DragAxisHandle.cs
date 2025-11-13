using UnityEngine;

namespace Sources.Master.ManipulatingSystem
{
    public class DragAxisHandle : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _sensitivity = 0.01f;

        private Camera _camera;
        private bool _isDragging;
        private Vector3 _dragStartMousePos;
        private Vector3 _dragStartTargetPos;

        public void Init(Camera handleCamera) => 
            _camera = handleCamera;

        public void SetTarget(Transform target) => 
            _target = target;

        private void OnMouseDown()
        {
            if (!_target) 
                return;

            _isDragging = true;
            _dragStartMousePos = Input.mousePosition;
            _dragStartTargetPos = _target.position;
        }

        private void OnMouseUp() => 
            _isDragging = false;

        private void OnMouseDrag()
        {
            if (!_isDragging) 
                return;

            Vector3 moveDirection = transform.forward;
            Vector3 mouseDelta = Input.mousePosition - _dragStartMousePos;
            Vector3 axisScreenDir = _camera.WorldToScreenPoint(_target.position + moveDirection) -
                                    _camera.WorldToScreenPoint(_target.position);
            axisScreenDir.z = 0f;

            float distanceAlongAxis = Vector3.Dot(mouseDelta, axisScreenDir.normalized) * _sensitivity;

            _target.position = _dragStartTargetPos + moveDirection * distanceAlongAxis;
        }
    }
}