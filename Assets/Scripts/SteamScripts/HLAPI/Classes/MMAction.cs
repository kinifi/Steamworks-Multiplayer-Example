using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;
using UnityEngine;

public class MMAction 
{
	//the unique id each object has so we know which one to sent the packet
	public int ObjectID;
	//a string that tells which method they want to call on the object
	//uses send message on the current game object
	public string ActionMethod;
	//location transform
	public Vector3 NewLocation;
	//velocity of the player
	public float Velocity;
	//the message a user can have when they talk. Pops above their head
	public string Chat;
}
