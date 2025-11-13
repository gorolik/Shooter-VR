using UnityEngine;

namespace Sources.Infrastructure
{
    [DefaultExecutionOrder(-10)]
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private NetworkBootstrapper _networkBootstrapper;

        private void Start()
        {
            _networkBootstrapper.Init();
        }
    }
}
