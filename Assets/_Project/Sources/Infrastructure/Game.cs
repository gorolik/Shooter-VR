using System.Collections.Generic;
using Sources.Factories;
using Sources.Player;
using UnityEngine;

namespace Sources.Infrastructure
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private MasterSpawner _masterSpawner;
        [SerializeField] private PlayerSpawner _playerSpawner;
        
        private List<PlayerRoot> _players = new();

        public void InitMaster(ulong clientId)
        {
            _masterSpawner.Spawn(clientId);
        }

        public void InitPlayer(ulong clientId)
        {
            PlayerRoot player = _playerSpawner.Spawn(clientId);
            player.OnDie += OnPlayerDie;
            _players.Add(player);
        }

        private void OnPlayerDie(PlayerRoot player)
        {
            _playerSpawner.Respawn(player);
        }
    }
}