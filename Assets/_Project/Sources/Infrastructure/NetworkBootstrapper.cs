using System;
using Sources.Factories;
using Unity.Netcode;
using UnityEngine;

namespace Sources.Infrastructure
{
    public class NetworkBootstrapper : NetworkBehaviour
    {
        [SerializeField] private MasterSpawner _masterSpawner;
        [SerializeField] private PlayerSpawner _playerSpawner;
        
        public void Init()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
            }
        }

        private void ClientConnected(ulong clientId)
        {
            if (!IsServer)
                return;
            
            if (NetworkManager.Singleton.SpawnManager.PlayerObjects.Count == 0)
                InitAsMaster(clientId);
            else
                InitAsPlayer(clientId);
        }

        private void InitAsMaster(ulong clientId)
        {
            _masterSpawner.Spawn(clientId);
        }

        private void InitAsPlayer(ulong clientId)
        {
            _playerSpawner.Spawn(clientId);
        }
    }
}
