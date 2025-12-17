using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Sources.Items.WeaponLogic
{
    [RequireComponent(typeof(NetworkObject))]
    public class Weapon : NetworkBehaviour
    {
        [Header("Weapon Stats")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _range = 100f;
        [SerializeField] private float _fireRate = 5f;

        [Header("Registration")] 
        [SerializeField] private LayerMask _layerMask;
        
        [Header("Recoil")] 
        [SerializeField] private Vector3 _force;
        [SerializeField] private Vector3 _torque;
        [SerializeField] private float _twiceHandsRecoilFactor = 0.2f;
        [Space]
        [SerializeField] private float _hapticAmplitude = 0.2f;
        [SerializeField] private float _hapticDuration = 0.02f;
        [SerializeField] private float _twiceHandsHapticFactor = 0.7f;
        
        [Header("References")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody _rigidbody;

        [Header("VR Input")]
        [SerializeField] private InputActionProperty _leftHandFireAction;
        [SerializeField] private InputActionProperty _rightHandFireAction;

        private float _nextTimeToFire = 0f;
        private XRGrabInteractable _interactable;

        public Action<bool, Vector3, Vector3> OnFire;

        private void Awake() => 
            _interactable = GetComponent<XRGrabInteractable>();

        private void Update()
        {
            if (!IsOwner) 
                return;
            
            TryFireHandle();

            if (_interactable.firstInteractorSelecting is XRBaseInputInteractor controllerInteractor)
            {
                if (Vector3.Distance(controllerInteractor.transform.position, transform.position) > 1.5f) 
                    _rigidbody.position = controllerInteractor.transform.position;
            }
        }

        private void TryFireHandle()
        {
            bool shouldFire = false;
            
            if (_interactable.IsSelectedByRight())
                shouldFire = _rightHandFireAction.action?.IsPressed() ?? false;
            else if (_interactable.IsSelectedByLeft()) 
                shouldFire = _leftHandFireAction.action?.IsPressed() ?? false;

            if (shouldFire && Time.time >= _nextTimeToFire)
            {
                _nextTimeToFire = Time.time + 1f / _fireRate;
                ShootServerRpc(_firePoint.position, _firePoint.forward);

                MakeRecoil();
            }
        }

        private void MakeRecoil()
        {
            float factor = 1;

            if (_interactable.interactorsSelecting.Count == 2)
                factor = _twiceHandsRecoilFactor;
            
            _rigidbody.AddRelativeForce(_force * factor, ForceMode.Impulse);
            _rigidbody.AddRelativeTorque(_torque * factor, ForceMode.Impulse);

            foreach (IXRSelectInteractor interactor in _interactable.interactorsSelecting)
            {
                if (interactor is XRBaseInputInteractor controllerInteractor)
                {
                    float amplitude = _hapticAmplitude;

                    if (_interactable.interactorsSelecting.Count == 2)
                        amplitude = _hapticAmplitude * _twiceHandsHapticFactor;
                    
                    controllerInteractor.SendHapticImpulse(amplitude, _hapticDuration);
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc(Vector3 rayOrigin, Vector3 rayDirection)
        {
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _range, _layerMask))
            {
                Damagable damagable = hit.transform.GetComponentInParent<Damagable>();
                
                if (damagable)
                    damagable.Damage(_damage);
            }

            ShootClientRpc(hit.collider, hit.point, hit.normal);
        }

        [ClientRpc]
        private void ShootClientRpc(bool isHit, Vector3 hitPoint, Vector3 hitNormal) => 
            OnFire?.Invoke(isHit, hitPoint, hitNormal);
    }
}