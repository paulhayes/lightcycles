using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
	
	public Transform target;
	public float ease;
	public float closestDistance;
	public float crashedBackSpeed;
	public float crashedUpSpeed;
	public float height;

	
	protected Vector3 speed;
	protected Vector3 lastTarget;
	
	protected StartScript startScript;
	
	protected Vector3 startPosition;
	protected Quaternion startRotation;
	
	protected bool gameOver = false;
	
	// Use this for initialization
	void Start () {
		startScript = (StartScript) Object.FindObjectOfType(typeof( StartScript ) );
		startScript.OnGameStart += OnGameStart;
		startScript.OnGameReset += OnGameReset;
		startScript.OnWin += OnWin;
		
		startPosition = transform.position;
		startRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
		if( gameOver ) {
			
			transform.position += ( crashedUpSpeed * Vector3.up + crashedBackSpeed * -transform.forward ) * Time.deltaTime;
			transform.LookAt( lastTarget );
			
			return;
		}
		
		lastTarget = target.position;
		Vector3 targetPosition = target.position + closestDistance * -target.forward;
		targetPosition.y = height;
		Vector3 direction = targetPosition - transform.position;
		Vector3 position = transform.position;
		
		position += direction * ease;
		
		transform.position = position;
		transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.LookRotation( target.position - transform.position ), 0.1f );
		
	}
	
	void SetTarget(GameObject t){
		target = t.transform;
		Vector3 position = target.position + closestDistance * -target.forward;
		position.y = height;
		
		transform.position = position;
		
		transform.LookAt( target );
		
		t.GetComponent<PlayerInteger>().OnCrashed += OnCrashed;
	}
	
	void OnCrashed(){
		gameOver = true;
	}
	
	void OnWin(){
		gameOver = true;
	}
	
	void OnGameStart(){
		startScript.OnGameStart -= OnGameStart;
		startScript.OnGameReset -= OnGameReset;
		startScript.OnWin -= OnWin;
		Destroy( gameObject );
	}
	
	void OnGameReset(){
		gameOver = false;
		transform.position = startPosition;
		transform.rotation = startRotation;
	}
}
