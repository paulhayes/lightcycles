using UnityEngine;
using System.Collections;

public class DestroyOnGameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameController gameController = (GameController) FindObjectOfType<GameController>();
		gameController.OnGameStart+= ()=>{ if( this && gameObject ) Destroy(gameObject); };
		gameController.OnGameReset += ()=>{ if( this && gameObject ) Destroy(gameObject); };
	}
		
	void OnGameStart(){
		if( this && gameObject ) Object.Destroy( gameObject );
	}
}
