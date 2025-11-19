using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Sources.Items
{
    [RequireComponent(typeof(NetworkObject), typeof(XRGrabInteractable))]
    public class VROwnershipHandler : NetworkBehaviour
    {
        private NetworkObject _networkObject;
        private XRGrabInteractable _interactable;

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            _interactable = GetComponent<XRGrabInteractable>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _interactable.selectEntered.AddListener(OnSelectEntered);
                _interactable.selectExited.AddListener(OnSelectExited);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _interactable.selectEntered.RemoveListener(OnSelectEntered);
                _interactable.selectExited.RemoveListener(OnSelectExited);
            }
        }

        private void OnSelectEntered(SelectEnterEventArgs arg0)
        {
            if (!IsOwner) 
                RequestOwnershipServerRpc();
        }
        
        private void OnSelectExited(SelectExitEventArgs arg0)
        {
            if (IsOwner) 
                ReturnOwnershipServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestOwnershipServerRpc(ServerRpcParams serverRpcParams = default) => 
            _networkObject.ChangeOwnership(serverRpcParams.Receive.SenderClientId);

        [ServerRpc]
        private void ReturnOwnershipServerRpc() => 
            _networkObject.RemoveOwnership();
    }
}