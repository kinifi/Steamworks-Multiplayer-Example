# Steamworks-Multiplayer-Example

This is an attempt at making Steam Multiplayer Easier in Unity. 



```csharp
using UnityEngine;
using System.Collections;

public class SteamTestScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
		SteamMultiplayerManager.Instance.Setup();
		//just made this a coroutine to show that it takes time to get the lobby information sometimes
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

```



This makes accessing lobbies and getting the information you need easy! Note: Lobby.summary isn't filled out sometimes due to the Lobby Owner not setting that property.

