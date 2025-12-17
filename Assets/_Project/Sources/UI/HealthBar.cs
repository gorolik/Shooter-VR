using Sources.Items;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.UI
{
    public class HealthBar : NetworkBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Slider _slider;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _slider.maxValue = _health.maxHealth;
            _slider.value = _health.maxHealth;
            _health.CurrentHealth.OnValueChanged += OnHealthChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            _health.CurrentHealth.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float previousValue, float newValue) => 
            _slider.value = newValue;
    }
}