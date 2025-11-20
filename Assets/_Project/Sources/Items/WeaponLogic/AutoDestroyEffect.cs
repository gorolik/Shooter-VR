using UnityEngine;

namespace Sources.Items.WeaponLogic
{
    public class AutoDestroyEffect : MonoBehaviour
    {
        [SerializeField] private float _destroyTime = 2;
        
        private void Start() => 
            Destroy(gameObject, _destroyTime);
    }
}