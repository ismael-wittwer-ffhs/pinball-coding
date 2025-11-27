// Pinball_TriggerForBall : Description : Detect when a ball is lost : Use by Out_Hole_TriggerDestroyBall gameObject on the hierarchy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinball_TriggerForBall : MonoBehaviour {

	private ManagerGame gameManager;											// access ManagerGame component from ManagerGame GameObject on the hierarchy

	void Start(){																	// --> Function Start
		gameManager = ManagerGame.Instance;										// Access ManagerGame gameComponent from singleton
	}

	void OnTriggerEnter (Collider other) {										// --> Function OnTriggerEnter
		if(other.transform.tag == "Ball"){												// If it's a ball 
			gameManager.gamePlay(other.gameObject);										// Send Message to the obj_Game_Manager.  
		}
	}

}
