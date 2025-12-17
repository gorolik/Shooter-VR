using System;
using Sources.Infrastructure;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;

namespace Sources.UI
{
    public class WelcomeUI : MonoBehaviour
    {
        [SerializeField] private XRLauncher _xrLauncher;
        [SerializeField] private NetworkBootstrapper _networkBootstrapper;
        [Header("UI elements")] 
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Camera _defaultCamera;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private TMP_InputField _hostPortInputField;
        [SerializeField] private TMP_InputField _addressInputField;
        [SerializeField] private TMP_InputField _clientPortInputField;
        [SerializeField] private Toggle _vrSetToggle;
        [SerializeField] private Toggle _masterToggle;
        [SerializeField] private TMP_Text _errorMessage;
        
        public void Init()
        {
            _hostButton.onClick.AddListener(OnHostButtonClick);
            _clientButton.onClick.AddListener(OnClientButtonClick);
            _xrLauncher.OnXRInited += OnXRInited;
            _xrLauncher.OnXRFailed += OnXRFailed;
            NetworkManager.Singleton.OnClientStarted += OnGameStarted;
            NetworkManager.Singleton.OnServerStarted += OnGameStarted;
        }

        private void OnDestroy()
        {
            _hostButton.onClick.RemoveListener(OnHostButtonClick);
            _clientButton.onClick.RemoveListener(OnClientButtonClick);
            _xrLauncher.OnXRInited -= OnXRInited;
            _xrLauncher.OnXRFailed += OnXRFailed;

            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientStarted -= OnGameStarted;
                NetworkManager.Singleton.OnServerStarted -= OnGameStarted;
            }
        }

        private void OnHostButtonClick()
        {
            _canvasGroup.interactable = false;
            
            try
            {
                _networkBootstrapper.IsMasterNeed = _masterToggle.isOn;
                
                string ip = "0.0.0.0";
                string port = _hostPortInputField.text;
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, ushort.Parse(port));
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception e)
            {
                _canvasGroup.interactable = true;
                _errorMessage.text = e.ToString();
                Console.WriteLine(e);
                throw;
            }
        }

        private void OnClientButtonClick()
        {
            _canvasGroup.interactable = false;
            
            _xrLauncher.StartVR(!_vrSetToggle.isOn);
        }

        private void OnXRInited()
        {
            try
            {
                string ip = _addressInputField.text;
                string port = _clientPortInputField.text;
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, ushort.Parse(port));
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception e)
            {
                _canvasGroup.interactable = true;
                _errorMessage.text = e.ToString();
                Console.WriteLine(e);
                throw;
            }
        }

        private void OnXRFailed(string message)
        {
            _errorMessage.text = message;
            _canvasGroup.interactable = true;
        }

        private void OnGameStarted() => 
            HideWindowAndCamera();

        private void HideWindowAndCamera()
        {
            _defaultCamera.gameObject.SetActive(false);
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}