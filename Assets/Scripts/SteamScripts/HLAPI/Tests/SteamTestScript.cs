/*
 This is a test script to see how I can make this not so cluttered later down the road
 Probably not the best way but having all of the steam callbacks hidden would be the best
*/

using UnityEngine;
using System.Collections;

public class SteamTestScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
		SteamMultiplayerManager.Instance.Setup();

		StartCoroutine(SteamLobbyTest());

	}
	
	
	public IEnumerator SteamLobbyTest()
	{

		yield return new WaitForSeconds(1);

		SteamMultiplayerManager.Instance.RequestLobbyList();

		yield return new WaitForSeconds(1);

		Debug.Log(SteamMultiplayerManager.Instance.m_LobbyList[0].name);
	}	
	
}
