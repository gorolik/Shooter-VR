using Sources.Player;
using UnityEngine;

namespace Sources.Factories
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerRoot _playerPrefab;
        [SerializeField] private Transform _spawnPoint;
        
        public PlayerRoot Spawn(ulong clientId)
        {
            PlayerRoot player = Instantiate(_playerPrefab, _spawnPoint.position, _spawnPoint.rotation);
            player.NetworkObject.SpawnAsPlayerObject(clientId);

            return player;
        }
    }
}