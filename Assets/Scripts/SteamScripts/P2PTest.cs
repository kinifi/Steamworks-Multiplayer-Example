using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;
using System.Text;
using TinyJSON;

//Helper gist: https://gist.github.com/rlabrecque/c93201041cb0ccee99eed0e9e4a7d45f

public class P2PTest : MonoBehaviour {

	private CallResult<P2PSessionConnectFail_t> OnP2PSessionConnectFailResult;

	private CallResult<P2PSessionRequest_t> m_P2PSessionRequest;

	private Callback<SocketStatusCallback_t> m_SocketStatusCallback_t;

	private List<CSteamID> m_AcceptedIDs = new List<CSteamID>();

	private Player m_CurrentPlayer;

	//check to see if we are even connected. 
	public bool m_hasConnected = false;

	private int m_ID;

	// Use this for initialization
	public void Start () {

		//register the fail connect callresult
		OnP2PSessionConnectFailResult = CallResult<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);

		m_P2PSessionRequest = CallResult<P2PSessionRequest_t>.Create(OnP2PSessionRequest);

		m_SocketStatusCallback_t = Callback<SocketStatusCallback_t>.Create(OnSocketStatusCallback);

		//make sure we have a lobby
		if(SteamMultiplayerManager.Instance.m_Lobby.lobby != null)
		{
			m_hasConnected = true;
			GetPlayer();
			//get a unique id for this object
			m_ID = SteamMultiplayerManager.Instance.GenerateUniqueID();

			if(SteamMultiplayerManager.Instance.DebugTextOn)
			{
				Debug.Log("ID: " + m_ID + " | Player: " + m_CurrentPlayer.steamPersonaName);
			}
		}

	}

	private void GetPlayer()
	{
		for (int i = 0; i < SteamMultiplayerManager.Instance.m_PlayerList.Count; i++) 
		{
			if(SteamMultiplayerManager.Instance.GetSteamPersonaName().ToLower() == SteamMultiplayerManager.Instance.m_PlayerList[i].steamPersonaName.ToLower())
			{
				Debug.Log("Found My Player Name");
				m_CurrentPlayer = SteamMultiplayerManager.Instance.m_PlayerList[i];
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	


	}

	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2, 0, 200, 200));

		if(GUILayout.Button("Test Packet"))
		{
			SendUpdate(m_ID, "none", transform.position, 0, "none");
		}
		GUILayout.EndArea();
	}

	/// <summary>
	/// Sends a json string to all the players
	/// </summary>
	/// <param name="json">Json.</param>
	public void SendUpdate(int ObjectID, string ActionMethod, Vector3 NewLocation, float NewVelocity, string NewChat)
	{
		//make sure we are connected
		if (m_hasConnected == false)
		{
			return;
		}

		//assemble the data to send to users
		MMAction NewAction = new MMAction();
		NewAction.ObjectID = ObjectID;
		NewAction.ActionMethod = ActionMethod;
		NewAction.NewLocation = NewLocation;
		NewAction.Velocity = NewVelocity;
		NewAction.Chat = NewChat;

		//convert to json
		var PacketData = JSON.Dump( NewAction, true );

		if(SteamMultiplayerManager.Instance.DebugTextOn)
		{
			Debug.Log("" + m_ID + ": " + PacketData);
		}

		for (int i = 0; i < SteamMultiplayerManager.Instance.m_PlayerList.Count; i++) 
		{
			if(SteamMultiplayerManager.Instance.GetSteamPersonaName().ToLower() != SteamMultiplayerManager.Instance.m_PlayerList[i].steamPersonaName.ToLower())
			{
				//convert our json to bytes
				byte[] packetBytes = Encoding.ASCII.GetBytes(PacketData);
				//send the packets to the user
				SteamNetworking.SendP2PPacket(m_CurrentPlayer.lobbyID, packetBytes, (uint)packetBytes.Length, Steamworks.EP2PSend.k_EP2PSendReliable, 0);
			}

		}


	}

	private void OnP2PSessionRequest(P2PSessionRequest_t pCallback, bool bIOFailure) 
	{
		Debug.Log("[" + P2PSessionRequest_t.k_iCallback + " - P2PSessionRequest] - " + pCallback.m_steamIDRemote);

		bool ret = SteamNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);

		Debug.Log("SteamNetworking.AcceptP2PSessionWithUser(" + pCallback.m_steamIDRemote + ") - " + ret);
		//add this to the list of id's we have accepted
		m_AcceptedIDs.Add(pCallback.m_steamIDRemote);
	}

	public void ReadP2PPacket(byte[] pubDest, uint cubDest, uint pcubMsgSize,CSteamID psteamIDRemote, int nChannel = 0) 
	{
		Debug.Log("ReadPacket: " + pubDest + " | " + cubDest + " | " + pcubMsgSize + " | " + psteamIDRemote + " | " + nChannel);
	}

	// private void OnP2PSessionRequest(P2PSessionRequest_t pCallback, bool bIOFailure)
	// {
	// 	Debug.Log("OnP2PSessionRequest: Acct: %d " + pCallback.m_steamIDRemote.GetAccountID());
	// 	// always accept packets
	// 	// the packet itself will come through when you call m_SteamNetworking->ReadP2PPacket()
	// 	SteamNetworking.AcceptP2PSessionWithUser (pCallback.m_steamIDRemote);

	// }

	private void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback, bool bIOFailure)
	{
		Debug.Log("OnP2PSessionConnectFail: Acct: %d - %d " + pCallback.m_steamIDRemote.GetAccountID() + " | " + pCallback.m_eP2PSessionError);
	}

	private void OnSocketStatusCallback(SocketStatusCallback_t pCallback) {
		Debug.Log("[" + SocketStatusCallback_t.k_iCallback + " - SocketStatusCallback] - " + pCallback.m_hSocket + " -- " + pCallback.m_hListenSocket + " -- " + pCallback.m_steamIDRemote + " -- " + pCallback.m_eSNetSocketState);
	}


}
