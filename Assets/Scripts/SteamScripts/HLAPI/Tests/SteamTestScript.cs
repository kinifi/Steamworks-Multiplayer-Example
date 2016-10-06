/*
 This is a test script to see how I can make this not so cluttered later down the road
 Probably not the best way but having all of the steam callbacks hidden would be the best
*/

using UnityEngine;
using System.Collections;

public class SteamTestScript : SteamServerCreator {

	// public SteamServerCreator m_SteamServer;

	public P2PTest m_p2ptest;

	// Use this for initialization
	void Start () {

		//setup the server
		Setup();

		//request all the lobbbies
		RequestLobbyList();

		//m_p2ptest.Start ();
	
	}
	
	
	
}
