using System;
using Unity.Netcode;
using UnityEngine;

namespace Sources.Player
{
    public class PlayerViewController : NetworkBehaviour
    {
        [SerializeField] private GameObject _view;
        [SerializeField] private GameObject _vignette;
        [SerializeField] private Transform _camera;
        [SerializeField] private bool _disableViewForOwner = true;
        [SerializeField] private Transform _leftLegRayPoint;
        [SerializeField] private Transform _rightLegRayPoint;
        [SerializeField] private Transform _leftLegTarget;
        [SerializeField] private Transform _rightLegTarget;
        [SerializeField] private float _legTargetOffset = 0.1f;
        [SerializeField] private LayerMask _layerMask;
        
        private void Start()
        {
            if (IsOwner && _disableViewForOwner)
                foreach (Renderer renderer in _view.GetComponentsInChildren<Renderer>()) 
                    renderer.enabled = false;

            if (!IsOwner)
                _vignette.SetActive(false);
        }

        private void Update()
        {
            SetLegTargetPosition(_rightLegRayPoint, _rightLegTarget);
            SetLegTargetPosition(_leftLegRayPoint, _leftLegTarget);
            
            _view.transform.rotation = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
        }

        private void SetLegTargetPosition(Transform rayPoint, Transform legTarget)
        {
            Ray rRay = new Ray(rayPoint.position, Vector3.down);
            if (Physics.Raycast(rRay, out var rHit, Mathf.Infinity, _layerMask))
                legTarget.position = rHit.point + Vector3.up * _legTargetOffset;
        }
    }
}