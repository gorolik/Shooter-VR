using System;
using Unity.Netcode;
using UnityEngine;

namespace Sources.Items
{
    public class Health : Damagable
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private bool _destroyOnDie = true;
        
        private readonly NetworkVariable<float> _currentHealth = new();

        public event Action OnDie;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer) 
                _currentHealth.Value = _maxHealth;
        }

        public void Restore() => 
            _currentHealth.Value = _maxHealth;

        public override void Damage(float amount)
        {
            if (amount < 0)
                return;

            _currentHealth.Value -= amount;
            Debug.Log($"{gameObject.name} took {amount} damage. Current health: {_currentHealth.Value}");

            if (_currentHealth.Value <= 0)
            {
                OnDie?.Invoke();
                
                if (_destroyOnDie)
                    DestroyOnDie();
            }
        }

        private void DestroyOnDie()
        {
            Debug.Log($"{gameObject.name} has died.");

            if (TryGetComponent<NetworkObject>(out var networkObject))
                networkObject.Despawn(true);
            else
                Destroy(gameObject);
        }
    }
}