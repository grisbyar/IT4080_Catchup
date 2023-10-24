using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLobby : MonoBehaviour
{

    public LobbyUI lobby_UI;
    void Start()
    {
        CreateTestCards();
        lobby_UI.OnReadyToggled += TestOnReadyToggled;
        lobby_UI.OnStartClicked += TestOnStartClicked;
        lobby_UI.ShowStart(true);
        lobby_UI.OnChangeNameClicked += TestOnChangeNameClicked;
    }

    private void CreateTestCards()
    {
        PlayerCard pc = lobby_UI.playerCards.AddCard("Player 1");
        pc.color = Color.red;
        pc.ready = true;
        pc.ShowKick(false);
        pc.clientId = 99;
        pc.OnKickClicked += TestOnKickClicked;
        pc.UpdateDisplay();

        pc = lobby_UI.playerCards.AddCard("Player 2");
        pc.color = Color.blue;
        pc.ready = false;
        pc.ShowKick(true);
        pc.clientId = 50;
        pc.OnKickClicked += TestOnKickClicked;
        pc.UpdateDisplay();
    }

    private void TestOnKickClicked(ulong clientId)
    {
        Debug.Log($"Kick {clientId}");
    }

    private void TestOnReadyToggled(bool newValue)
    {
        Debug.Log($"Ready = {newValue}");
    }

    private void TestOnStartClicked()
    {
        lobby_UI.ShowStart(false);
    }

    private void TestOnChangeNameClicked(string newName)
    {
        Debug.Log($"Name Changed to: {newName}");
    }
}