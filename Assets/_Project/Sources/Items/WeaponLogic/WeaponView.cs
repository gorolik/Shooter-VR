using UnityEngine;

namespace Sources.Items.WeaponLogic
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private GameObject _hitEffectPrefab;
        [SerializeField] private ParticleSystem _fireParticle;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _fireClip;
        
        private void OnEnable() => 
            _weapon.OnFire += OnFire;

        private void OnDisable() => 
            _weapon.OnFire -= OnFire;

        private void OnFire(bool isHit, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (isHit && _hitEffectPrefab) 
                Instantiate(_hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            
            _audioSource.PlayOneShot(_fireClip);
            _fireParticle.Play();
        }
    }
}