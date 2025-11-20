using Unity.Netcode;
using UnityEngine;

namespace Sources.Items
{
    public abstract class Damagable : NetworkBehaviour
    {
        public abstract void Damage(float amount);
    }
}