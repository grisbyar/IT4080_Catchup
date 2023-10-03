using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHandler : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private bool hasPrinted = false;
    private void PrintMe(){
        if(hasPrinted){
            return;
        }
        Debug.Log("I AM");
        hasPrinted = true;
        if(IsServer) {
            Debug.Log($" the Server! {NetworkManager.ServerClientId}");
        }
        if(IsHost) {
            Debug.Log($" the Host! {NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if(IsClient) {
            Debug.Log($" the Client! {NetworkManager.LocalClientId}");
        }
         if(!IsServer && !IsClient) {
            Debug.Log(" Nothing!");
            hasPrinted = false;
        }
    }

    private void OnClientStarted()
    {
        Debug.Log("Client Started");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnServerStopped += ClientOnClientStopped;
        PrintMe();
    }
    //
    // Client Actions
    //
        private void ClientOnClientConnected(ulong clientId){
            PrintMe();
            //if the client id is the networks managers client id, then it is the same client
            if (NetworkManager.LocalClientId == clientId){
                Debug.Log($"I {clientId} have connected to the server.");
            } else
            {
                Debug.Log($"Another {clientId} has connected to the server.");
            }
        }

        private void ClientOnClientDisconnected(ulong clientId){
            PrintMe();
            //same logic applies here
            if (NetworkManager.LocalClientId == clientId){
                Debug.Log($"I {clientId} have disconnected from the server.");
            } else{
                Debug.Log($"Someone {clientId} has disconnected from the server.");
            }
    
        }

        private void ClientOnClientStopped(bool indicator){
                Debug.Log("!! Client Stopped !!");
                hasPrinted = false;
                NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
                NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
                NetworkManager.OnServerStopped += ClientOnClientStopped;
        }
    //
    // server actions
    //

    private void OnServerStarted()
    {
        Debug.Log("Server Started");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }
    private void ServerSetup(){
        
    }
    private void ServerOnClientConnected(ulong clientId){ 
        Debug.Log($"Client {clientId} connected to server");
    }
    private void ServerOnClientDisconnected(ulong clientId){ 
        Debug.Log($"Client {clientId} disconnected from server");
    }
    private void ServerOnServerStopped(bool indicator){
            hasPrinted = false;
            Debug.Log("Server Stopped");
            //disconnects
            NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
            NetworkManager.OnServerStopped -= ServerOnServerStopped;

     }
}



