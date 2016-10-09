/*
 This is a test script to see how I can make this not so cluttered later down the road
 Probably not the best way but having all of the steam callbacks hidden would be the best
*/

using UnityEngine;
using System.Collections;

public class SteamTestScript : MonoBehaviour {

	private Vector2 scrollPosition;

	private bool hasLobby = false;

	// Use this for initialization
	void Start () {
	
		SteamMultiplayerManager.Instance.Setup();

		StartCoroutine(SteamLobbyTest());

	}
	
	public void OnGUI()
	{
		if(hasLobby == false)
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
			hasLobby = false;
			SteamMultiplayerManager.Instance.LeaveLobby(SteamMultiplayerManager.Instance.m_Lobby.lobby);
		}
	}

	private void MainMenu()
	{

		GUILayout.BeginArea(new Rect(Screen.width/2-100, 0, 200, 400));

		GUILayout.Label("Game Title");

		if(SteamMultiplayerManager.Instance.m_LobbyList.Count == 0)
		{
			if(GUILayout.Button("Singleplayer", GUILayout.Height(30)))
			{
				
			}

			GUILayout.Space(10);

			if(GUILayout.Button("Exit", GUILayout.Height(30)))
			{
				Application.Quit();
			}
		
		}
		else
		{

			if(GUILayout.Button("Singleplayer", GUILayout.Height(30)))
			{

			}

			GUILayout.Space(10);
			
			GUILayout.Label("Multiplayer", GUILayout.Height(30));
			
			if(GUILayout.Button("Create Game", GUILayout.Height(30)))
			{
				//create a lobby here - public, 2 players
				SteamMultiplayerManager.Instance.CreateLobby ();
				Debug.Log("Creating Lobby");
				hasLobby = true;
			}

			GUILayout.Space(10);			

			GUILayout.Label("Lobbies", GUILayout.Height(30));

			if(SteamMultiplayerManager.Instance.m_LobbyList.Count != 0)
			{

				scrollPosition = GUILayout.BeginScrollView(scrollPosition);

				//list all the lobbies and create join buttons for them
				for (int i = 0; i < SteamMultiplayerManager.Instance.m_LobbyList.Count; i++) {

					GUILayout.BeginHorizontal ();

					//display lobby name
					GUILayout.Label (SteamMultiplayerManager.Instance.m_LobbyList[i].name);
					//join button
					if (GUILayout.Button ("Join")) {
						//join the lobby
						SteamMultiplayerManager.Instance.JoinLobby (SteamMultiplayerManager.Instance.m_LobbyList[i].lobby);
						//set the lobby to our class level var
						SteamMultiplayerManager.Instance.m_Lobby.lobby = SteamMultiplayerManager.Instance.m_LobbyList[i].lobby;
					}

					GUILayout.EndHorizontal ();
				}

				GUILayout.EndScrollView();

			}
			else
			{
				GUILayout.Label("...Loading Lobbies...");
			}

			GUILayout.Space(10);


			if(GUILayout.Button("Exit", GUILayout.Height(30)))
			{
				Application.Quit();
			}
		}

		GUILayout.EndArea();

	}

	public IEnumerator SteamLobbyTest()
	{

		yield return new WaitForSeconds(0.1f);

		SteamMultiplayerManager.Instance.RequestLobbyList();

		// yield return new WaitForSeconds(1);

		// Debug.Log(SteamMultiplayerManager.Instance.m_LobbyList[0].name);
	}	
	
}
