using Sources.UI;
using UnityEngine;

namespace Sources.Infrastructure
{
    [DefaultExecutionOrder(-10)]
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private NetworkBootstrapper _networkBootstrapper;
        [SerializeField] private WelcomeUI _welcomeUI;

        private void Start()
        {
            _networkBootstrapper.Init();
            _welcomeUI.Init();
        }
    }
}
