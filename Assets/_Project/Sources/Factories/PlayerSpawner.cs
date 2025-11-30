using Sources.Player;
using UnityEngine;

namespace Sources.Factories
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerRoot _playerPrefab;
        [SerializeField] private Transform _spawnPoint1;
        [SerializeField] private Transform _spawnPoint2;

        public void Respawn(PlayerRoot player)
        {
            Transform point = GetSpawnPointByClientId(player.OwnerClientId);
            player.Respawn(point.position, point.rotation);
        }

        public PlayerRoot Spawn(ulong clientId)
        {
            Transform spawnPoint = GetSpawnPointByClientId(clientId);

            PlayerRoot player = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);
            player.NetworkObject.SpawnAsPlayerObject(clientId);

            return player;
        }

        private Transform GetSpawnPointByClientId(ulong clientId)
        {
            Transform spawnPoint;
            if (clientId % 2 == 0)
                spawnPoint = _spawnPoint1;
            else
                spawnPoint = _spawnPoint2;
            return spawnPoint;
        }
    }
}