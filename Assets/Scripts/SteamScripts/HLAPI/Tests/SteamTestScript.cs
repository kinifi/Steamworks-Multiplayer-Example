/*
 This is a test script to see how I can make this not so cluttered later down the road
 Probably not the best way but having all of the steam callbacks hidden would be the best
*/

using UnityEngine;
using System.Collections;

public class SteamTestScript : MonoBehaviour {

	private Vector2 scrollPosition;

	// Use this for initialization
	void Start () {

        //set debug to be true
        SMM.Instance.DebugTextOn = true;
	
		SMM.Instance.Setup();
        SMM.Instance.RequestLobbyList();
	}
	
	public void OnGUI()
	{
		if(SMM.Instance.m_Lobby == null)
		{
			MainMenu();
		}
		else
		{
			LobbyMenu();
		}
	}
	
	private void LobbyMenu()
	{

		if(GUILayout.Button("Leave Lobby", GUILayout.Height(30)))
		{
			SMM.Instance.LeaveLobby(SMM.Instance.m_Lobby.lobby);
		}

	}

	private void MainMenu()
	{

		GUILayout.BeginArea(new Rect(Screen.width/2-100, 0, 200, 400));

		GUILayout.Label("Game Title");

        //only display the lobbies if there are actually lobbies
        if (SMM.Instance.m_LobbyList.Count == 0)
        {
            GUILayout.Label("No Lobbies");
            GUILayout.EndArea();
            return;
        }

        //create a lobby
        if(GUILayout.Button("Create Lobby"))
        {
            SMM.Instance.CreateLobby();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        //list all the lobbies and create join buttons for them
        for (int i = 0; i < SMM.Instance.m_LobbyList.Count; i++)
        {

            GUILayout.BeginHorizontal();

            //display lobby name
            GUILayout.Label(SMM.Instance.m_LobbyList[i].name);
            //join button
            if (GUILayout.Button("Join"))
            {
                //join the lobby
                SMM.Instance.JoinLobby(SMM.Instance.m_LobbyList[i].lobby);
                //set the lobby to our class level var
                SMM.Instance.m_Lobby.lobby = SMM.Instance.m_LobbyList[i].lobby;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

	    if(GUILayout.Button("Quit", GUILayout.Height(30)))
	    {
		    Application.Quit();
	    }
		

		GUILayout.EndArea();

	}
	
}
