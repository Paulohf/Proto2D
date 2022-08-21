using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Trigger : MonoBehaviour
{
    public Player_Control.Direction direction; //Direction that the player need to face, to interact with the actor
	Shop shop;
	//Event event;
	//NPC npc;

	public enum Behavior
	{
		Shop,
		Event,
		NPC
	};
	public Behavior behavior;

	private void Start()
	{
		switch (behavior)
		{
			case Behavior.Shop:
				shop = GetComponent<Shop>();
				break;
			case Behavior.Event:
				//script = GetComponent<Event>();
				break;
			case Behavior.NPC:
				//script = GetComponent<NPC>();
				break;
			default:
				break;
		}		
	}

	public void StartActorBehavior()
	{
		switch (behavior)
		{
			case Behavior.Shop:
				shop.shopInteracting = true;
				shop.ActorBehavior();
				break;
			case Behavior.Event:
				//event.ActorBehavior();
				break;
			case Behavior.NPC:
				//npc.ActorBehavior();
				break;
			default:
				break;
		}
	}
}
