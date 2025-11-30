using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Sources.Infrastructure
{
    public class XRLauncher : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Префаб [XR Device Simulator] со сцены. Нужен для управления с клавиатуры.")]
        [SerializeField] private GameObject _xrDeviceSimulatorPrefab;

        private XRLoader _currentManualLoader; // Ссылка на лоадер, если мы запустили его вручную
        private bool _isInited;
        
        public event Action OnXRInited;
        public event Action<string> OnXRFailed;

        public void StartVR(bool useSimulation) => 
            StartCoroutine(StartXRCoroutine(useSimulation));
        
        public void StopVR() => 
            StopXR();

        private IEnumerator StartXRCoroutine(bool useSimulation)
        {
            string errorMessage = "None";
            
            if (_isInited)
            {
                Debug.LogWarning("XR уже инициализирован");
                yield break;
            }
            
            Debug.Log($"Попытка инициализации XR (Симуляция: {useSimulation})...");

            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null)
            {
                errorMessage = "XRGeneralSettings не найдены! Проверьте Project Settings > XR Plug-in Management";
                OnXRFailed?.Invoke(errorMessage);
                Debug.LogError(errorMessage);
                yield break;
            }

            var manager = XRGeneralSettings.Instance.Manager;
            bool initSuccess = false;

            if (useSimulation)
            {
                foreach (var loader in manager.activeLoaders)
                {
                    if (loader.GetType().Name.Contains("Mock"))
                    {
                        Debug.Log("Найден Mock HMD Loader. Инициализация вручную...");
                        
                        if (loader.Initialize())
                        {
                            if (loader.Start())
                            {
                                _currentManualLoader = loader;
                                initSuccess = true;
                            }
                            else
                            {
                                errorMessage = "Mock Loader инициализирован, но не смог запуститься (Start failed).";
                            }
                        }
                        else
                        {
                            errorMessage = "Не удалось инициализировать Mock Loader.";
                        }
                        break;
                    }
                }

                if (!initSuccess && string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "Mock HMD Loader не найден в списке activeLoaders. Убедитесь, что он включен в Project Settings.";
                }
            }
            else
            {
                Debug.Log("Запускаем стандартный XR Loader через Manager...");
                
                yield return manager.InitializeLoader();

                if (manager.activeLoader != null)
                {
                    manager.StartSubsystems();
                    initSuccess = true;
                }
                else
                {
                    errorMessage = "Не удалось инициализировать реальный XR шлем (activeLoader is null).";
                }
            }

            if (!initSuccess)
            {
                OnXRFailed?.Invoke(errorMessage);
                Debug.LogError(errorMessage);
                yield break;
            }
            
            if (_xrDeviceSimulatorPrefab != null)
            {
                _xrDeviceSimulatorPrefab.SetActive(useSimulation);
            }

            _isInited = true;
            OnXRInited?.Invoke();
            
            Debug.Log($"VR успешно запущен! (Режим симуляции: {useSimulation})");
        }

        private void StopXR()
        {
            var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;


            if (_currentManualLoader != null)
            {
                _currentManualLoader.Stop();
                _currentManualLoader.Deinitialize();
                _currentManualLoader = null;
            }
 
            else if (manager != null && manager.isInitializationComplete)
            {
                manager.StopSubsystems();
                manager.DeinitializeLoader();
            }

            if (_xrDeviceSimulatorPrefab != null)
            {
                _xrDeviceSimulatorPrefab.SetActive(false);
            }
            
            _isInited = false;
            Debug.Log("VR остановлен.");
        }
    }
}