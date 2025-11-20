using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Sources.Items
{
    [RequireComponent(typeof(NetworkObject))]
    public class Transformable : Selectable
    {
        private Rigidbody _rigidbody;
        private XRGrabInteractable _interactable;
        
        private static XRInteractionManager _interactionManager;

        private void Awake()
        {
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _interactable);
        }

        public void BeginPCManipulation() => 
            BeginPCManipulationServerRpc();

        public void EndPCManipulation() => 
            EndPCManipulationServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void BeginPCManipulationServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var networkObject = GetComponent<NetworkObject>();
            networkObject.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
            
            if (_interactable != null && _interactable.isSelected)
            {
                if (_interactionManager == null) 
                    _interactionManager = FindObjectOfType<XRInteractionManager>();

                if (_interactionManager != null) 
                    _interactionManager.CancelInteractableSelection((IXRSelectInteractable)_interactable);
            }
            
            SetKinematicStateClientRpc(true);
        }

        [ServerRpc]
        private void EndPCManipulationServerRpc()
        {
            SetKinematicStateClientRpc(false);
            
            var networkObject = GetComponent<NetworkObject>();
            networkObject.RemoveOwnership();
        }

        [ClientRpc]
        private void SetKinematicStateClientRpc(bool isKinematic)
        {
            if (_rigidbody) 
                _rigidbody.isKinematic = isKinematic;
        }
    }
}