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

    
    private void PrintMe(){
        
        if(IsServer) {
            NetworkHelper.Log($"I AM the Server! {NetworkManager.ServerClientId}");
        }
        if(IsHost) {
            NetworkHelper.Log($"I AM the Host! {NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if(IsClient) {
            NetworkHelper.Log($"I AM the Client! {NetworkManager.LocalClientId}");
        }
         if(!IsServer && !IsClient) {
            NetworkHelper.Log("I AM Nothing yet!");
            hasPrinted = false;
        }
    }
    //
    // Client Actions
    //
    private void OnClientStarted()
    {
        NetworkHelper.Log("Client Started");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnServerStopped += ClientOnClientStopped;
        PrintMe();
    }
   private void ClientOnClientStopped(bool indicator){
                NetworkHelper.Log("!! Client Stopped !!");
                NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
                NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
                NetworkManager.OnServerStopped += ClientOnClientStopped;
                PrintMe();
        }
    private void ClientOnClientConnected(ulong clientId){
            PrintMe();
            //if the client id is the networks managers client id, then it is the same client
            if (NetworkManager.LocalClientId == clientId){
                NetworkHelper.Log($"I {clientId} have connected to the server.");
            } else
            {
                NetworkHelper.Log($"Another Client {clientId} has connected to the server.");
            }
        }

    private void ClientOnClientDisconnected(ulong clientId){
            PrintMe();
            //same logic applies here
            if (NetworkManager.LocalClientId == clientId){
                NetworkHelper.Log($"I {clientId} have disconnected from the server.");
            } else{
                NetworkHelper.Log($"Another Client {clientId} has disconnected from the server.");
            }
    
        }

    
    //
    // server actions
    //

    private void OnServerStarted()
    {
        NetworkHelper.Log("Server Started");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }
     private void ServerOnServerStopped(bool indicator){
            NetworkHelper.Log("Server Stopped");
            //disconnects
            NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
            NetworkManager.OnServerStopped -= ServerOnServerStopped;
            PrintMe();
    }
   
    private void ServerOnClientConnected(ulong clientId){ 
        NetworkHelper.Log($"Client {clientId} connected to server");
    }
    private void ServerOnClientDisconnected(ulong clientId){ 
        NetworkHelper.Log($"Client {clientId} disconnected from server");
    }
   
}



