using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChatServer : NetworkBehaviour
{

    public ChatUI chat_UI;
    const ulong SYSTEM_ID = ulong.MaxValue;
    private ulong[] dmClientIds = new ulong[2];
    private ulong[] sClientIds = new ulong[1];

    void Start()
    {
        chat_UI.printEnteredText = false;
        chat_UI.MessageEntered += OnChatUIMessageEntered;

        if (IsServer) {
            NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
            if (IsHost)
            {
                DisplayMessageLocally(SYSTEM_ID, $"You are the host AND client {NetworkManager.LocalClientId}");
            }
            else
            {
                DisplayMessageLocally(SYSTEM_ID, "You are the server");
            }
            } else {
                DisplayMessageLocally(SYSTEM_ID, $"You are a client{NetworkManager.LocalClientId}");
            }
        }



    private void ServerOnClientConnected(ulong clientId)
    {
        ServerSendDirectMessage(
            $"({NetworkManager.LocalClientId}) : ({clientId}) connected to the server" , 
            NetworkManager.LocalClientId, 
            clientId);

        ToEveryone(
           $"({clientId}) has connected",
           NetworkManager.LocalClientId,
           clientId);

    }

    private void ServerOnClientDisconnected(ulong clientId)
    {
        
        ToEveryone(
           $"I ({clientId}) has disconnected",
           NetworkManager.LocalClientId,
           clientId);
    }

    private void DisplayMessageLocally(ulong from, string message)
        {
            string fromStr = $"Player {from}";
            Color textColor = chat_UI.defaultTextColor;

            if (from == NetworkManager.LocalClientId)
            {
                fromStr = "you";
                textColor = Color.magenta;
            } else if (from == SYSTEM_ID)
            {
                fromStr = "SYS";
                textColor = Color.green;
            }

            chat_UI.addEntry(fromStr, message, textColor);
        }

        private void OnChatUIMessageEntered(string message)
        {
            SendChatMessageServerRpc(message);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
        {
        if (message.StartsWith("@")) {
            string[] parts = message.Split(" ");
            string clientIdStr = parts[0].Replace("@", "");
            ulong toClientId = ulong.Parse(clientIdStr);

            ServerSendDirectMessage(message, serverRpcParams.Receive.SenderClientId, toClientId);

        } else {
            ReceiveChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);

        }
            
        }
    
        [ClientRpc]
        public void ReceiveChatMessageClientRpc(string message, ulong from, ClientRpcParams clientRpcParams = default)
        {
            DisplayMessageLocally(from, message);
        }

    private void ServerSingleMessage(string message, ulong to)
    {
        sClientIds[0] = to;
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = sClientIds;
        ReceiveChatMessageClientRpc(message, SYSTEM_ID, rpcParams);

    }
    private void ServerSendDirectMessage(string message, ulong from, ulong to)
    {
        bool connected = false;
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds) {
            if (clientId == to) connected = true;
        }

        if (connected)
        {
            dmClientIds[0] = from;
            dmClientIds[1] = to;

            ClientRpcParams rpcParams = default;
            rpcParams.Send.TargetClientIds = dmClientIds;

            //clientIds[0] = from;
            // ReceiveChatMessageClientRpc($"<whisper> {message}", from, rpcParams);
            // clientIds[0] = to;
            ReceiveChatMessageClientRpc(message, from, rpcParams);
        }
        else
        {
            ServerSingleMessage($"No connected user ({to})", from);
        }
    }

    private void ToEveryone(string message, ulong from, ulong to)
    {
        ClientRpcParams rpcParams = default;
        ReceiveChatMessageClientRpc(message, from, rpcParams);
    }
}