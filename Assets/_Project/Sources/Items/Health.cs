using System;
using Unity.Netcode;
using UnityEngine;

namespace Sources.Items
{
    public class Health : Damagable
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private bool _destroyOnDie = true;
        
        public readonly NetworkVariable<float> CurrentHealth = new();

        public float maxHealth => _maxHealth;

        public event Action OnDie;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer) 
                CurrentHealth.Value = _maxHealth;
        }

        public void Restore() => 
            CurrentHealth.Value = _maxHealth;

        public override void Damage(float amount)
        {
            if (amount < 0)
                return;

            CurrentHealth.Value -= amount;
            
            Debug.Log($"{gameObject.name} took {amount} damage. Current health: {CurrentHealth.Value}");

            if (CurrentHealth.Value <= 0)
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