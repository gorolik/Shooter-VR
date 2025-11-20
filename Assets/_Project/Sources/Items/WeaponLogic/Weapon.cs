using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sources.Items.WeaponLogic
{
    [RequireComponent(typeof(NetworkObject))]
    public class Weapon : NetworkBehaviour
    {
        [Header("Weapon Stats")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _range = 100f;
        [SerializeField] private float _fireRate = 5f;
        
        [Header("References")]
        [SerializeField] private Transform _firePoint;

        [Header("VR Input")]
        [SerializeField] private InputActionProperty _leftHandFireAction;
        [SerializeField] private InputActionProperty _rightHandFireAction;

        private float _nextTimeToFire = 0f;

        public Action<bool, Vector3, Vector3> OnFire;

        private void Update()
        {
            if (!IsOwner) 
                return;

            bool shouldFire = _leftHandFireAction.action?.IsPressed() ?? false;

            if (shouldFire && Time.time >= _nextTimeToFire)
            {
                _nextTimeToFire = Time.time + 1f / _fireRate;
                ShootServerRpc(_firePoint.position, _firePoint.forward);
            }
        }

        [ServerRpc]
        private void ShootServerRpc(Vector3 rayOrigin, Vector3 rayDirection)
        {
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _range))
                if (hit.transform.TryGetComponent<Damagable>(out var target)) 
                    target.Damage(_damage);
            
            ShootClientRpc(hit.collider, hit.point, hit.normal);
        }

        [ClientRpc]
        private void ShootClientRpc(bool isHit, Vector3 hitPoint, Vector3 hitNormal) => 
            OnFire?.Invoke(isHit, hitPoint, hitNormal);
    }
}