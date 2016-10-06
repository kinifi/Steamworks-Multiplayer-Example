using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;

public class SteamServerCreator : MonoBehaviour
{

	/// Public Variables

	///<summary>
	/// the lobby list from RequestLobbyList
	///</summary>
	public List<Lobby> m_lobbyList = new List<Lobby>();

	///<summary>
	/// The current Lobby we are using
	///</summary>
	public CSteamID m_currentLobby;

	/// Private Variables



	/// Steam CallResults

	//the list of lobbies for this game
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	//the lobby enter callback
	private CallResult<LobbyEnter_t> OnLobbyEnterCallResult;


	///<summary>
	/// Use this for initialization
	///</summary>
	public void Setup() 
	{
		//register the callback so we can get a list of lobbies
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
	
		//register the callresult for entering the lobby
		OnLobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(OnLobbyEnter);
	}

	/////////////////////////////////////////////////////////////////////////////
	// Join / Update / Entering Lobbies / Leaving Lobbies

	

	//will be called when someone enters/leaves/updates lobby data/ joins the game
	//updating the player list here would be a good idea
	private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		Debug.Log ("Lobby Update: SteamIDMember - " + pCallback.m_ulSteamIDMember
			+ " - Success: " + pCallback.m_bSuccess + " - Lobby: " + pCallback.m_ulSteamIDLobby);
	}

	///<summary>
	/// Join a lobby 
	/// <param>CSteamID object of the lobby wanting to Leave</param>
	///</summary>
	public void LeaveLobby(CSteamID lobby)
	{
		SteamMatchmaking.LeaveLobby (lobby);
		// currentLobby = null;
		// m_ChatMessages = null;
	}

	///<summary>
	/// Join a lobby 
	/// <param>CSteamID object of the lobby wanting to join</param>
	///</summary>
	public void JoinLobby(CSteamID _Lobby)
	{
		SteamAPICall_t handle = SteamMatchmaking.JoinLobby(_Lobby);
		OnLobbyEnterCallResult.Set(handle);
		print("SteamMatchmaking.JoinLobby(" + _Lobby + ") : " + handle);

		//set the current lobby 
		//currentLobby.lobby = _Lobby;

	}

	//called when a user enters the lobby. 
	//updating players list would be a good time to do something here
	private void OnLobbyEnter(LobbyEnter_t pCallback, bool bIOFailure) 
	{
		if(bIOFailure == false)
		{
			Debug.Log("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);

			LobbyValue _lobby = new LobbyValue ();
			_lobby.name = SteamMatchmaking.GetLobbyData ((CSteamID)pCallback.m_ulSteamIDLobby, "name");
			_lobby.lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
			// currentLobby = _lobby;
		}
		else
		{
			Debug.LogError("Unable to Join Lobby");
		}


	}


	/////////////////////////////////////////////////////////////////////////////
	// Lobby Listing

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

	//// End Lobby Listing
	/////////////////////////////////////////////////////////////////////////////////
	
}
