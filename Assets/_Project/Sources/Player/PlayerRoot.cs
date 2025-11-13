using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Sources.Player
{
    public class PlayerRoot : NetworkBehaviour
    {
        [SerializeField] private GameObject _xrOrigin;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
                DisableXRInput();
        }

        private void DisableXRInput()
        {
            var camera = _xrOrigin.GetComponentInChildren<Camera>();
            var audioListener = _xrOrigin.GetComponentInChildren<AudioListener>();
            if (camera) 
                camera.enabled = false;
            if (audioListener) 
                audioListener.enabled = false;
            
            var trackedPoseDrivers = _xrOrigin.GetComponentsInChildren<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            foreach (var tpd in trackedPoseDrivers)
                tpd.enabled = false;
            
            var interactors = _xrOrigin.GetComponentsInChildren<XRBaseInteractor>();
            foreach (var interactor in interactors)
                interactor.enabled = false;

            var interactables = _xrOrigin.GetComponentsInChildren<XRBaseInteractable>();
            foreach (var interactable in interactables)
                interactable.enabled = false;
        }
    }
}