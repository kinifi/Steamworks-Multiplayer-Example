using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;
using System.Text;

//Helper gist: https://gist.github.com/rlabrecque/c93201041cb0ccee99eed0e9e4a7d45f

public class P2PTest : MonoBehaviour {

	private CallResult<P2PSessionRequest_t> OnP2PSessionRequestResult;

	private CallResult<P2PSessionConnectFail_t> OnP2PSessionConnectFailResult;

	private Player m_CurrentPlayer;

	//check to see if we are even connected. 
	public bool m_hasConnected = false;

	private int m_ID;

	// Use this for initialization
	public void Start () {
	
		//register the p2prequest callresult
		OnP2PSessionRequestResult = CallResult<P2PSessionRequest_t>.Create(OnP2PSessionRequest);

		//register the fail connect callresult
		OnP2PSessionConnectFailResult = CallResult<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);

		//make sure we have a lobby
		if(SteamMultiplayerManager.Instance.m_Lobby.lobby != null)
		{
			m_hasConnected = true;
			GetPlayer();
			//get a unique id for this object
			m_ID = SteamMultiplayerManager.Instance.GenerateUniqueID();
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
	void Update () {
	


	}

	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2, 0, 200, 200));

		if(GUILayout.Button("Test Packet"))
		{
			SendUpdate("hi");
		}
		GUILayout.EndArea();
	}

	/// <summary>
	/// Sends a json string to all the players
	/// </summary>
	/// <param name="json">Json.</param>
	public void SendUpdate(string json)
	{
		//make sure we are connected
		// if (m_hasConnected == false)
			// return;

		// for (int i = 0; i < m_Players.Count; i++) 
		// {
		// 	byte[] toBytes = Encoding.ASCII.GetBytes(json);
		// 	//SendP2PPacket(CSteamID steamIDRemote, byte[] pubData, uint cubData, EP2PSend eP2PSendType, int nChannel = 0)
		// 	SteamNetworking.SendP2PPacket(m_Players[i].lobbyID, toBytes, (uint)toBytes.Length, Steamworks.EP2PSend.k_EP2PSendReliable, 0);
		// }

		//TODO: Compare the data sending before sending it. Otherwise we could be sending
		// more than we need to.
		//send the data to every player
		/*
		for (int i = 0; i < max; i++) 
		{
			//TODO: Compress the data before sending
			byte[] toBytes = Encoding.ASCII.GetBytes(json);

			//TODO come up with a data structure to send 
			SteamNetworking.SendP2PPacket(data goes here);
		}
		*/


	}

	public void ReadP2PPacket(byte[] pubDest, uint cubDest, uint pcubMsgSize,CSteamID psteamIDRemote, int nChannel = 0) 
	{
		Debug.Log("ReadPacket: " + pubDest + " | " + cubDest + " | " + pcubMsgSize + " | " + psteamIDRemote + " | " + nChannel);
	}

	private void OnP2PSessionRequest(P2PSessionRequest_t pCallback, bool bIOFailure)
	{
		Debug.Log("OnP2PSessionRequest: Acct: %d " + pCallback.m_steamIDRemote.GetAccountID());
		// always accept packets
		// the packet itself will come through when you call m_SteamNetworking->ReadP2PPacket()
		SteamNetworking.AcceptP2PSessionWithUser (pCallback.m_steamIDRemote);

	}

	private void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback, bool bIOFailure)
	{
		Debug.Log("OnP2PSessionConnectFail: Acct: %d - %d " + pCallback.m_steamIDRemote.GetAccountID() + " | " + pCallback.m_eP2PSessionError);
	}


}
