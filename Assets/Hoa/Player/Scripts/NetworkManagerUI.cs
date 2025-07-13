using UnityEngine;
using UnityEngine.UI;
using PurrNet;
using PurrNet.Transports;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private TMP_InputField port;
    [SerializeField]
    private UDPTransport uDPTransport;
    [SerializeField] private GameObject networkUI;
    [SerializeField] private GameObject mainMenuUI;

    private void Awake()
    {
        mainMenuUI.SetActive(false);
        networkUI.SetActive(true);
        // serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        hostButton.onClick.AddListener(() => StartHost());
        clientButton.onClick.AddListener(() => StartClient());
    }

    void StartHost()
    {
        uDPTransport.address = ipAddressInputField.text;
        bool portResult = ushort.TryParse(port.text, out ushort result);
        if (portResult && ipAddressInputField.text != "")
        {
            uDPTransport.serverPort = result;  // Set the port to match the server's
            NetworkManager.main.onNetworkStarted += (NetworkManager manager, bool asServer) =>
            {
                Debug.Log("Network started as " + (asServer ? "server" : "client"));
                HideNetworkUI();
            };
            NetworkManager.main.StartHost();
        }
    }

    void StartClient()
    {
        uDPTransport.address = ipAddressInputField.text;
        bool portResult = ushort.TryParse(port.text, out ushort result);
        if (portResult && ipAddressInputField.text != "")
        {
            uDPTransport.serverPort = result;  // Set the port to match the server's
            NetworkManager.main.onNetworkStarted += (NetworkManager manager, bool asServer) =>
            {
                Debug.Log("Network started as " + (asServer ? "server" : "client"));
                HideNetworkUI();
            };
            NetworkManager.main.StartClient();
        }
    }

    void HideNetworkUI()
    {
        networkUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
