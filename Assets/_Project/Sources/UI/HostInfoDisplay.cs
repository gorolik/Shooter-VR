using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Sources.UI
{
    public class HostInfoDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;

        private void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted += UpdateHostInfo;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= UpdateHostInfo;
            }
        }

        private void UpdateHostInfo()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                string ip = GetLocalIPAddress();
                ushort port = GetPort();

                _infoText.text = $"<b>Host IP:</b> {ip}\n<b>Port:</b> {port}";
                _infoText.gameObject.SetActive(true);
            }
            else
            {
                _infoText.gameObject.SetActive(false);
            }
        }
        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
        
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!IPAddress.IsLoopback(ip))
                    {
                        return ip.ToString();
                    }
                }
            }
            
            return "IP не найден";
        }
        
        private ushort GetPort()
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport != null)
            {
                return transport.ConnectionData.Port;
            }
            return 0;
        }
    }
}