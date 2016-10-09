/*
 *  Game Manager
 *  This is how we will save our persistent data such as: 
 *  Logged in save data from a json file and assigning variables which we access in game
 *  With this Singleton we can store data we need for later use
 *  Example on how to use: GameManager.Instance.[Variable Name / Method Name]
 *  Methods and Variables must be public
 *  Note: A new singleton should be created per platform for achievements string literals and specific achievement methods 
 *
 */

/// Note: that the more namespaces we use the more loading this screen has to do
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;

public class SteamMultiplayerManager
{

	//Class Level Members Start

	///The current lobby, its CSteamID, and its players
	public CurrentLobby m_Lobby = null;

	//a list of the lobbies from the lobby request
	public List<Lobby> m_LobbyList = new List<Lobby>();

	public bool DebugTextOn = false;

	//Class Level Members End


	/// Steam CallResults

	//chat messages
	protected Callback<LobbyChatMsg_t> m_LobbyChatMsg;

	//lobbychat update
	protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;

	//the list of lobbies for this game
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	//the lobby enter callback
	private CallResult<LobbyEnter_t> OnLobbyEnterCallResult;

	//the lobby update method that is called when any lobby info is called
	protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;

	//lobby creation callback
	private CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;

	//Method Start

    /// <summary>
    /// load all of the data on setup
    /// This should be called on the title screen
    /// </summary>
    public void Setup()
    {

		//register the lobby creation callback
		OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);

		//register the callback so we can get a list of lobbies
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
	
		//register the callresult for entering the lobby
		OnLobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(OnLobbyEnter);

		//register for the lobby update callback
		m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);

		//onlobbychat message callback
		m_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg);

		//chat update callback
		m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

    }

	/////////////////////////////////////////////////////////////////////////////
	// Lobby Chat Messaging

    ///Send a message to the lobby
    public void SendLobbyChatMsg(string message, CSteamID lobby)
    {
		byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(message);
		SteamMatchmaking.SendLobbyChatMsg(lobby, MsgBody, MsgBody.Length);
    }

	private void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback) 
	{
		if(DebugTextOn)
		{
			Debug.Log("[" + LobbyChatUpdate_t.k_iCallback + " - LobbyChatUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUserChanged + " -- " + pCallback.m_ulSteamIDMakingChange + " -- " + pCallback.m_rgfChatMemberStateChange);
		}
	}

	private void OnLobbyChatMsg(LobbyChatMsg_t pCallback)
	{
		// Debug.Log("[" + LobbyChatMsg_t.k_iCallback + " - LobbyChatMsg] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUser + " -- " + pCallback.m_eChatEntryType + " -- " + pCallback.m_iChatID);
		CSteamID SteamIDUser;
		byte[] Data = new byte[4096];
		EChatEntryType ChatEntryType;
		int ret = SteamMatchmaking.GetLobbyChatEntry((CSteamID)pCallback.m_ulSteamIDLobby, (int)pCallback.m_iChatID, out SteamIDUser, Data, Data.Length, out ChatEntryType);

		// Debug.Log("SteamMatchmaking.GetLobbyChatEntry(" + (CSteamID)pCallback.m_ulSteamIDLobby + ", " + (int)pCallback.m_iChatID + ", out SteamIDUser, Data, Data.Length, out ChatEntryType) : " + ret + " -- " + SteamIDUser + " -- " + System.Text.Encoding.UTF8.GetString(Data) + " -- " + ChatEntryType);
		
		//create a new lobby chat and add it to the chat message list
		LobbyChatMessageValue _chat = new LobbyChatMessageValue();
		_chat.steamPersonaName = SteamFriends.GetFriendPersonaName(SteamIDUser);
		_chat.message = System.Text.Encoding.UTF8.GetString(Data);
		m_Lobby.m_ChatMessages.Add(_chat);
		
		if(DebugTextOn)
		{
			Debug.Log("Chat Messages Total: " + m_Lobby.m_ChatMessages.Count + "| Newest Message: " + _chat.message);
		}
	}


	/////////////////////////////////////////////////////////////////////////////
	// Join / Update / Entering Lobbies / Leaving Lobbies


	public void CreateLobby() 
	{
		SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
		OnLobbyCreatedCallResult.Set(handle);
		
		if(DebugTextOn)
		{
			Debug.Log("SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1) : " + handle);
		}
	}

	///Gets the number of members in the lobby
	public int GetNumberOfLobbyMembers(CSteamID lobby)
	{
		return SteamMatchmaking.GetNumLobbyMembers(lobby);
	}

	//get the limit for the given lobby
	public int GetLobbyMemberLimit(CSteamID lobby)
	{
		return SteamMatchmaking.GetLobbyMemberLimit(lobby);
	}

	private void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure) 
	{
		if (bIOFailure == false)
		{
			Debug.Log ("[" + LobbyCreated_t.k_iCallback + " - LobbyCreated] - " + pCallback.m_eResult + " -- " + pCallback.m_ulSteamIDLobby);
			// //set the lobby to our class level
			// m_Lobby = ;
			// currentLobby = GetLobbyDataInfo (
			m_Lobby = new CurrentLobby();
			m_Lobby.lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
		}
		else
		{
			Debug.LogError ("Failed to create a lobby");
		}

	}

	public void SetLobbyData(CSteamID lobby, string name, string summary)
	{
		SteamMatchmaking.SetLobbyData (lobby, name, summary);
		
		if(DebugTextOn)
		{
			Debug.Log("SteamMatchmaking.SetLobbyData() : " + name + " : " + summary);
		}
	}

	//will be called when someone enters/leaves/updates lobby data/ joins the game
	//updating the player list here would be a good idea
	private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		Debug.Log ("Lobby Update: SteamIDMember - " + pCallback.m_ulSteamIDMember
			+ " - Success: " + pCallback.m_bSuccess + " - Lobby: " + pCallback.m_ulSteamIDLobby);

		//update the player list here

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
		
		if(DebugTextOn)
		{
			Debug.Log("SteamMatchmaking.JoinLobby(" + _Lobby + ") : " + handle);
		}
	}

	//called when a user enters the lobby. 
	//updating players list would be a good time to do something here
	private void OnLobbyEnter(LobbyEnter_t pCallback, bool bIOFailure) 
	{
		if(bIOFailure == false)
		{
			Debug.Log("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);

			//assign the joined lobby to our currentlobby object
			m_Lobby.lobbyName = SteamMatchmaking.GetLobbyData ((CSteamID)pCallback.m_ulSteamIDLobby, "name");
			m_Lobby.lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
			Lobby LobbyData = GetLobbyDataInfo(m_Lobby.lobby);
			m_Lobby.lobbyName = LobbyData.name;
		}
		else
		{
			Debug.LogError("Unable to Join Lobby");
		}

	}

	/////////////////////////////////////////////////////////////////////////////
	// Lobby information

	///returns the CSteamID of the current lobby owner
	public CSteamID GetLobbyOwnerId(CSteamID lobby)
	{
		return SteamMatchmaking.GetLobbyOwner(lobby);
	}

	///returns the lobby owners Steam persona
	public string GetLobbyOwnerPersona(CSteamID owner)
	{
		CSteamID _owner = GetLobbyOwnerId(owner);
		return SteamFriends.GetFriendPersonaName(_owner);
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
		
		if(DebugTextOn)
		{
			Debug.Log("SteamMatchmaking.RequestLobbyList() : " + handle);
		}
	}

	private void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure) 
	{
		//check if we failed to get a list of matches
		if (bIOFailure == false) 
		{
			Debug.Log ("[" + LobbyMatchList_t.k_iCallback + " - LobbyMatchList] - " + pCallback.m_nLobbiesMatching);
			
			for (int i = 0; i < (int)pCallback.m_nLobbiesMatching; i++) 
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

		if(DebugTextOn)
		{
			Debug.Log ("SteamMatchmaking.SteamMatchmaking.GetLobbyByIndex(0) : " + m_LobbyValue);
		}

		Lobby _lobby = new Lobby ();
		_lobby.name = SteamMatchmaking.GetLobbyData (m_LobbyValue, "name");
		_lobby.lobbyIndex = index;
		_lobby.lobby = m_LobbyValue;
		m_LobbyList.Add (_lobby);
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



	//Method End

	//create a local instance of GameManager
    private static SteamMultiplayerManager instance;

    //If there isn't a GameManager instance, create one.
    public static SteamMultiplayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SteamMultiplayerManager();
            }
            return instance;
        }
    }
	
    private void Shutdown()
    {
        if (instance != null) {
            instance = null;
        }	
    
    }

}