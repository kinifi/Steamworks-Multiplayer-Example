using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;

public class SteamServerCreator : MonoBehaviour
{

	///<summary>
	/// the lobby list from RequestLobbyList
	///</summary>
	public List<Lobby> m_lobbyList = new List<Lobby>();

	//the list of lobbies for this game
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	///<summary>
	/// Use this for initialization
	///</summary>
	public void Setup() 
	{
		//register the callback so we can get a list of lobbies
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
	}

	///<summary>
	/// Request a list of lobbies
	///</summary>
	public void RequestLobbyList() 
	{
		SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
		OnLobbyMatchListCallResult.Set(handle);
		//Debug.Log("SteamMatchmaking.RequestLobbyList() : " + handle);
	}

	private void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure) 
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

		Lobby _lobby = new Lobby ();
		_lobby.name = SteamMatchmaking.GetLobbyData (m_LobbyValue, "name");
		_lobby.lobbyIndex = index;
		_lobby.lobby = m_LobbyValue;
		m_lobbyList.Add (_lobby);
	}

	public Lobby GetLobbyDataInfo(CSteamID lobby)
	{
		Lobby _lobby = new Lobby ();
		_lobby.name = SteamMatchmaking.GetLobbyData (lobby, "name");
		_lobby.lobby = lobby;
		return _lobby;
	}


	
}
