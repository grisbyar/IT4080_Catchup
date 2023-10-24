using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Lobby : NetworkBehaviour
{
    public LobbyUI lobby_UI;
    public NetworkedPlayers networkedPlayers;

    void Start()
    {
        if (IsServer)
        {
            networkedPlayers.allNetPlayers.OnListChanged += ServerOnNetworkPlayersChanged;
            ServerPopulateCards();
            lobby_UI.ShowStart(true);
            lobby_UI.OnStartClicked += ServerStartClicked;
        }
        else
        {
            ClientPopulateCards();
            networkedPlayers.allNetPlayers.OnListChanged += ClientNetPlayerChanged;
            lobby_UI.ShowStart(false);
            lobby_UI.OnReadyToggled += ClientOnReadyToggled;
            NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnect;
        }

        lobby_UI.OnChangeNameClicked += OnChangeNameClicked;

    }

    private void OnChangeNameClicked(string newValue)
    {
        UpdatePlayerNameServerRpc(newValue);
    }

    private void PopulateMyInfo()
    {
        NetworkPlayerInfo myInfo = networkedPlayers.GetMyPlayerInfo();
        if (myInfo.clientId != ulong.MaxValue)
        {
            lobby_UI.SetPlayerName(myInfo.playerName.ToString());
        }
    }

    private void ServerPopulateCards()
    {
        lobby_UI.playerCards.Clear();
        foreach (NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobby_UI.playerCards.AddCard("Some player");
            pc.ready = info.ready;
            pc.color = info.color;
            pc.clientId = info.clientId;
            pc.playerName = info.playerName.ToString();
            if (info.clientId == NetworkManager.LocalClientId)
            {
                pc.ShowKick(false);
            }
            else
            {
                pc.ShowKick(true);
            }
            pc.OnKickClicked += ServerOnKickClicked;
            pc.UpdateDisplay();
        }


    }


    private void ClientPopulateCards()
    {
        lobby_UI.playerCards.Clear();
        foreach (NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobby_UI.playerCards.AddCard("Player");
            pc.ready = info.ready;
            pc.color = info.color;
            pc.playerName = info.playerName.ToString();
            pc.clientId = info.clientId;
            pc.ShowKick(false);
            pc.UpdateDisplay();
        }

    }

    private void ServerStartClicked()
    {
        NetworkManager.SceneManager.LoadScene("Arena1Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void ClientOnReadyToggled(bool newValue)
    {
        UpdateReadyServerRpc(newValue);
    }


    private void ServerOnNetworkPlayersChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent)
    {
        ServerPopulateCards();
        PopulateMyInfo();
        lobby_UI.EnableStart(networkedPlayers.AllPlayersReady());
    }


    private void ServerOnKickClicked(ulong clientId)
    {
        NetworkManager.DisconnectClient(clientId);
    }

    private void ClientNetPlayerChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent)
    {
        ClientPopulateCards();
        PopulateMyInfo();
    }


    private void ClientOnClientDisconnect(ulong clientId)
    {
        lobby_UI.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void PopulateClientInfoClientRpc(ClientRpcParams clientRpcParams = default)
    {
        PopulateMyInfo();
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadyServerRpc(bool newValue, ServerRpcParams rpcParams = default)
    {
        networkedPlayers.UpdateReady(rpcParams.Receive.SenderClientId, newValue);
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerNameServerRpc(string newValue, ServerRpcParams rpcParams = default)
    {
        string newName = networkedPlayers.UpdatePlayerName(rpcParams.Receive.SenderClientId, newValue);
        if (newName != newValue)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { rpcParams.Receive.SenderClientId }
                }
            };
            PopulateClientInfoClientRpc(clientRpcParams);
        }
    }

}