using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;

public class CreateServer : MonoBehaviour {

	private CSteamID m_Lobby;
	public LobbyValue currentLobby;

	private List<LobbyValue> m_LobbyList = new List<LobbyValue>();

	//the creation lobby method
	protected Callback<LobbyGameCreated_t> m_LobbyGameCreated;

	private CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;

	//the list of lobbies for this game
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	//onLobby enter callback
	private CallResult<LobbyEnter_t> OnLobbyEnterCallResult;

	protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;

	//chat messages
	protected Callback<LobbyChatMsg_t> m_LobbyChatMsg;
	protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;

	private string lobbyChatMessage = "";
	private List<LobbyChatMessageValue> m_ChatMessages = new List<LobbyChatMessageValue>();
	//end chat message stuff

	private string lobbyName = "", lobbySummary = "";

	// Use this for initialization
	void Start () {


		//register the lobby creation callback
		OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);

		//register the callback so we can get a list of lobbies
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);

		//register the callback for entering the lobby
		OnLobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(OnLobbyEnter);

		//register for the lobby update callback
		m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);

		//register for lobby chat messages
		m_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg);
		m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

	}

	public void OnGUI()
	{
		//only show this menu if we have a lobby
		if (currentLobby == null) {
			
			GUILayout.Label ("Create A Lobby");

			GUILayout.Label ("Lobby Name:");
			lobbyName = GUILayout.TextField (lobbyName, 20);
			GUILayout.Label ("Lobby Summary: ");
			lobbySummary = GUILayout.TextField (lobbySummary, 20);

			if (GUILayout.Button ("Create Lobby")) {
				//create a lobby here - public, 2 players
				CreateLobby (ELobbyType.k_ELobbyTypePublic, 2);
			}


			GUILayout.FlexibleSpace ();

			if (GUILayout.Button ("Get A List of Lobbies")) {
				RequestLobbyList ();
			}

			//list all the lobbies and create join buttons for them
			for (int i = 0; i < m_LobbyList.Count; i++) {

				GUILayout.BeginHorizontal ();

				//display lobby name
				GUILayout.Label (m_LobbyList [i].name);
				//join button
				if (GUILayout.Button ("Join")) {
					//join the lobby
					JoinLobby (m_LobbyList [i].lobby);
					//set the lobby to our class level var
					m_Lobby = m_LobbyList [i].lobby;
				}

				GUILayout.EndHorizontal ();
			}
		}
		else
		{

			//general lobby information
			GUILayout.Label ("Lobby Name: " + currentLobby.name);
			GUILayout.Label ("Lobby: " + currentLobby.lobby);
			GUILayout.Label ("GetNumLobbyMembers(m_Lobby) : " + SteamMatchmaking.GetNumLobbyMembers(currentLobby.lobby)  + " | " + SteamMatchmaking.GetLobbyMemberLimit(currentLobby.lobby));
			GUILayout.Label ("Lobby Owner: " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyOwner(currentLobby.lobby)));

			//give the user an option to leave the lobby
			if (GUILayout.Button ("Leave Lobby")) 
			{
				LeaveLobby (currentLobby.lobby);

			}

			if(m_ChatMessages.Count != 0)
			{
				//display all the chat messages we need
				GUILayout.Label ("Chat Messages: ");

				//list all the lobbies and create join buttons for them
				for (int i = 0; i < m_ChatMessages.Count; i++) 
				{

					GUILayout.BeginHorizontal ();
					//display the steam user name that sent the message
					GUILayout.Label (m_ChatMessages[i].steamPersonaName + ": ");
					//display the chat message
					GUILayout.Label (m_ChatMessages[i].message);
					GUILayout.EndHorizontal ();
				}
			}

			//create an area for us to write chat messages
			GUILayout.Space(10);
			GUILayout.Label ("Lobby Chat Message: ");
			lobbyChatMessage = GUILayout.TextField (lobbyChatMessage, 100);
			if (GUILayout.Button(">> Send Lobby Chat Message <<")) 
			{
				byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(lobbyChatMessage);
				SteamMatchmaking.SendLobbyChatMsg(currentLobby.lobby, MsgBody, MsgBody.Length);
				// print("SteamMatchmaking.SendLobbyChatMsg(" + currentLobby.lobby + ", MsgBody, MsgBody.Length) : " + SteamMatchmaking.SendLobbyChatMsg(currentLobby.lobby, MsgBody, MsgBody.Length));
			}
			GUILayout.Space(10);

			//only display this button if we have 2 people in the lobby
			if(SteamMatchmaking.GetNumLobbyMembers(currentLobby.lobby) == 2)
			{
				if(GUILayout.Button("Start Game"))
				{
					Debug.Log("Punch It Chewie");
				}
			}
		}

	}

	void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback) 
	{
		Debug.Log("[" + LobbyChatUpdate_t.k_iCallback + " - LobbyChatUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUserChanged + " -- " + pCallback.m_ulSteamIDMakingChange + " -- " + pCallback.m_rgfChatMemberStateChange);
	}

	void OnLobbyChatMsg(LobbyChatMsg_t pCallback)
	{
		// Debug.Log("[" + LobbyChatMsg_t.k_iCallback + " - LobbyChatMsg] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUser + " -- " + pCallback.m_eChatEntryType + " -- " + pCallback.m_iChatID);
		CSteamID SteamIDUser;
		byte[] Data = new byte[4096];
		EChatEntryType ChatEntryType;
		int ret = SteamMatchmaking.GetLobbyChatEntry((CSteamID)pCallback.m_ulSteamIDLobby, (int)pCallback.m_iChatID, out SteamIDUser, Data, Data.Length, out ChatEntryType);
		// Debug.Log("SteamMatchmaking.GetLobbyChatEntry(" + (CSteamID)pCallback.m_ulSteamIDLobby + ", " + (int)pCallback.m_iChatID + ", out SteamIDUser, Data, Data.Length, out ChatEntryType) : " + ret + " -- " + SteamIDUser + " -- " + System.Text.Encoding.UTF8.GetString(Data) + " -- " + ChatEntryType);
		LobbyChatMessageValue _chat = new LobbyChatMessageValue();
		_chat.steamPersonaName = SteamFriends.GetFriendPersonaName(SteamIDUser);
		_chat.message = System.Text.Encoding.UTF8.GetString(Data);
		m_ChatMessages.Add(_chat);
		Debug.Log("Chat Messages Total: " + m_ChatMessages.Count + "| Newest Message: " + _chat.message);
		//clear the chat
		lobbyChatMessage = "";

	}


	public void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		Debug.Log ("Lobby Update: SteamIDMember - " + pCallback.m_ulSteamIDMember
			+ " - Success: " + pCallback.m_bSuccess + " - Lobby: " + pCallback.m_ulSteamIDLobby);
	}

	public void LeaveLobby(CSteamID lobby)
	{
		SteamMatchmaking.LeaveLobby (lobby);
		currentLobby = null;
		m_ChatMessages = null;
	}

	public void SetLobbyData(CSteamID lobby, string name, string lobbySummary)
	{
		SteamMatchmaking.SetLobbyData (lobby, name, lobbySummary);
		Debug.Log("SteamMatchmaking.SetLobbyData() : " + lobbyName + " : " + lobbySummary);
	}

	////////////////
	/// Lobby Creation Methods and callbacks
	///////////////

	public void CreateLobby(ELobbyType lobbyType, int maxMembers) 
	{
		SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxMembers);
		OnLobbyCreatedCallResult.Set(handle);
		Debug.Log("SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1) : " + handle);
	}


	public void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure) {
		if (bIOFailure == false)
		{
			Debug.Log ("[" + LobbyCreated_t.k_iCallback + " - LobbyCreated] - " + pCallback.m_eResult + " -- " + pCallback.m_ulSteamIDLobby);
			//set the lobby to our class level
			m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
			currentLobby = GetLobbyDataInfo (m_Lobby);
			//set the lobby name and summary
			SetLobbyData(currentLobby.lobby, lobbyName, lobbySummary);
		}
		else
		{
			Debug.LogError ("Failed to create a lobby");
		}

	}


	////////////////////////////////////////////////////
	//Join Lobby Requests
	////////////////////////////////////////////////////

	public void JoinLobby(CSteamID _Lobby)
	{
		SteamAPICall_t handle = SteamMatchmaking.JoinLobby(_Lobby);
		OnLobbyEnterCallResult.Set(handle);
		// currentLobby.lobby = _Lobby;
		print("SteamMatchmaking.JoinLobby(" + _Lobby + ") : " + handle);
	}

	void OnLobbyEnter(LobbyEnter_t pCallback, bool bIOFailure) 
	{
		if(bIOFailure == false)
		{
			Debug.Log("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);

			LobbyValue _lobby = new LobbyValue ();
			_lobby.name = SteamMatchmaking.GetLobbyData ((CSteamID)pCallback.m_ulSteamIDLobby, "name");
			_lobby.lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
			currentLobby = _lobby;
		}
		else
		{
			Debug.LogError("Unable to Join Lobby");
		}


	}

	////////////////////////////////////////////////////
	//lobby list requests
	////////////////////////////////////////////////////

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

		LobbyValue _lobby = new LobbyValue ();
		_lobby.name = SteamMatchmaking.GetLobbyData (m_LobbyValue, "name");
		_lobby.lobbyIndex = index;
		_lobby.lobby = m_LobbyValue;
		m_LobbyList.Add (_lobby);
	}

	public LobbyValue GetLobbyDataInfo(CSteamID lobby)
	{
		LobbyValue _lobby = new LobbyValue ();
		_lobby.name = SteamMatchmaking.GetLobbyData (lobby, "name");
		_lobby.lobby = lobby;
		return _lobby;
	}

	///////////////////////////////////////
	//end  list requests
	////////////////////////////////////
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

public class LobbyChatMessageValue
{
	public string steamPersonaName;
	public string message;
}
