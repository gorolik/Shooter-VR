using System.Linq;
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

            ForceDropClientRpc();
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
        
        [ClientRpc]
        private void ForceDropClientRpc()
        {
            if (_interactable && _interactable.isSelected)
            {
                var manager = _interactable.interactionManager;
                var hand = _interactable.interactorsSelecting.FirstOrDefault();

                if (manager != null && hand != null) 
                    manager.SelectExit(hand, _interactable);
            }
        }
    }
}