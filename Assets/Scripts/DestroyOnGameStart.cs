using UnityEngine;
using System.Collections;

public class DestroyOnGameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartScript startScript = (StartScript) Object.FindObjectOfType(typeof(StartScript));
		startScript.OnGameStart+= ()=>{ if( this && gameObject ) Destroy(gameObject); };
		startScript.OnGameReset += ()=>{ if( this && gameObject ) Destroy(gameObject); };
	}
		
	void OnGameStart(){
		if( this && gameObject ) Object.Destroy( gameObject );
	}
}
