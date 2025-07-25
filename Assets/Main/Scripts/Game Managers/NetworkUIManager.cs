using System;
using System.Linq;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.Scripts.Game_Managers
{
    public class NetworkUIManager : MonoBehaviour
    {
        [Header("Connection UI Elements")]
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text localIPDisplayText;
        
        [Header("Map Selection UI Elements")]
        [SerializeField] private TMP_Text selectMapText;
        [SerializeField] private Button dustyButton;
        
        [Header("Managers")]
        [SerializeField] private GameStateManager gameStateManager;
        
        private UnityTransport _transport;

        private void Awake()
        {
            if (FindAnyObjectByType<EventSystem>()) return;
            Type inputType = typeof(StandaloneInputModule);
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
            
            ActivateConnectingUI();
            DeactivateMapSelectionUI();
        }

        private void Start()
        {
            _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);
            dustyButton.onClick.AddListener(() => StartGame("Dusty"));
            
            DisplayLocalIPAddress();
        }

        private void ApplyConnectionData()
        {
            string ip = ipAddressInputField.text;
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

            string portStr = portInputField.text;
            ushort port = 7777;
            
            if (!string.IsNullOrEmpty(portStr) && ushort.TryParse(portStr, out ushort parsedPort))
            {
                port = parsedPort;
            }
            _transport.SetConnectionData(ip, port);
        }

        private void StartClient()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartClient();
            DeactivateConnectingUI();
        }

        private void StartHost()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartHost();
            gameStateManager.StartLobby();
            DeactivateConnectingUI();
            ActivateMapSelectionUI();
        }

        private void StartGame(string mapName)
        {
            gameStateManager.StartGame(mapName);
            DeactivateMapSelectionUI();
        }
        
        private void ActivateConnectingUI()
        {
            startClientButton.gameObject.SetActive(true);
            startHostButton.gameObject.SetActive(true);
            ipAddressInputField.gameObject.SetActive(true);
            portInputField.gameObject.SetActive(true);
            localIPDisplayText.gameObject.SetActive(true);
            
        }
        private void DeactivateConnectingUI()
        {
            startClientButton.gameObject.SetActive(false);
            startHostButton.gameObject.SetActive(false);
            ipAddressInputField.gameObject.SetActive(false);
            portInputField.gameObject.SetActive(false);
            localIPDisplayText.gameObject.SetActive(false);
        }

        private void ActivateMapSelectionUI()
        {
            selectMapText.gameObject.SetActive(true);
            dustyButton.gameObject.SetActive(true);
        }

        private void DeactivateMapSelectionUI()
        {
            selectMapText.gameObject.SetActive(false);
            dustyButton.gameObject.SetActive(false);
        }

        private void DisplayLocalIPAddress()
        {
            string ip = GetLocalIPv4();
            localIPDisplayText.text = $"Your IP: {ip} \nRecommended port: 7777";
        }

        private static string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }
    }
}
