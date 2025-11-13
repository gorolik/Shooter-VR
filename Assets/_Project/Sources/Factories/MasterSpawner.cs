using Sources.Master;
using UnityEngine;

namespace Sources.Factories
{
    public class MasterSpawner : MonoBehaviour
    {
        [SerializeField] private MasterRoot _masterPrefab;
        [SerializeField] private Transform _spawnPoint;
        
        public MasterRoot Spawn(ulong clientId)
        {
            MasterRoot master = Instantiate(_masterPrefab, _spawnPoint.position, _spawnPoint.rotation);
            master.NetworkObject.SpawnAsPlayerObject(clientId);

            return master;
        }
    }
}