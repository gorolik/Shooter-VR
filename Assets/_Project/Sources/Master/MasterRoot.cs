using Unity.Netcode;
using UnityEngine;

namespace Sources.Master
{
    public class MasterRoot : NetworkBehaviour
    {
        [SerializeField] private GameObject _cameraGameObject;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _cameraGameObject.SetActive(IsOwner);
        }
    }
}