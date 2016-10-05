using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;

public class CreateServer : MonoBehaviour {

	public CSteamID m_Lobby;

	private List<LobbyValue> m_LobbyList = new List<LobbyValue>();

	//the creation lobby method
	protected Callback<LobbyGameCreated_t> m_LobbyGameCreated;

	private CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;

	//the list of lobbies for this game
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	// Use this for initialization
	void Start () {

		//register the callback to a OnLobbyGameCreated method
		//m_LobbyGameCreated = Callback<LobbyGameCreated_t>.Create(OnLobbyGameCreated);

		//OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);

		//register the callback so we can get a list of lobbies
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);

	}

	public void OnGUI()
	{
		GUILayout.Label ("Create A Lobby");

		if (GUILayout.Button ("Create Lobby")) 
		{
			
		}

		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Get A List of Lobbies")) 
		{
			RequestLobbyList ();
		}

		if (m_LobbyList.Count == 0)
			return;

		for (int i = 0; i < m_LobbyList.Count - 1; i++) 
		{
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (m_LobbyList[i].name)) 
			{

			}
			if (GUILayout.Button ("Join")) 
			{

			}
			GUILayout.EndHorizontal ();
		}

	}

	/*

	public void CreateLobby(ELobbyType lobbyType, int maxMembers) 
	{
		SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxMembers);
		OnLobbyCreatedCallResult.Set(handle);
		print("SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1) : " + handle);
	}

	public void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure) {
		Debug.Log("[" + LobbyCreated_t.k_iCallback + " - LobbyCreated] - " + pCallback.m_eResult + " -- " + pCallback.m_ulSteamIDLobby);
		m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
	}

	//the method called once the lobby is created
	public void OnLobbyGameCreated(LobbyGameCreated_t pCallback) 
	{
		Debug.Log("[" + LobbyGameCreated_t.k_iCallback +
					" - LobbyGameCreated] - " + pCallback.m_ulSteamIDLobby +
					" -- " + pCallback.m_ulSteamIDGameServer +
					" -- " + pCallback.m_unIP +
					" -- " + pCallback.m_usPort);
	}

	*/

	//lobby list requests

	/// <summary>
	/// Requests the lobby list.
	/// </summary>
	public void RequestLobbyList() 
	{
		SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
		OnLobbyMatchListCallResult.Set(handle);
		//Debug.Log("SteamMatchmaking.RequestLobbyList() : " + handle);
	}

	public void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure) 
	{
		//check if we failed to get a list of matches
		if (bIOFailure == false) 
		{
			Debug.Log ("[" + LobbyMatchList_t.k_iCallback + " - LobbyMatchList] - " + pCallback.m_nLobbiesMatching);
			for (int i = 0; i < (int)pCallback.m_nLobbiesMatching - 1; i++) 
			{
				GetLobbyByIndex (i);
			}
		}
		else
		{
			Debug.Log ("Unable to get LobbyMatchList");
		}

	}

	public void GetLobbyByIndex(int index)
	{
		CSteamID m_LobbyValue = SteamMatchmaking.GetLobbyByIndex(index);
		Debug.Log ("SteamMatchmaking.SteamMatchmaking.GetLobbyByIndex(0) : " + m_LobbyValue);

		var _lobby = new LobbyValue ();
		_lobby.name = SteamMatchmaking.GetLobbyData (m_LobbyValue, "name");
		_lobby.lobbyIndex = index;
		_lobby.lobby = m_LobbyValue;
		m_LobbyList.Add (_lobby);
	}

}

/// <summary>
/// A lobby class that stores everything for later use
/// </summary>
public class LobbyValue
{
	public string name;
	public int lobbyIndex;
	public CSteamID lobby;
}
