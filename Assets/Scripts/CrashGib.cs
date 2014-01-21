using UnityEngine;
using System.Collections;

public class CrashGib : MonoBehaviour {
		
	public Vector3 velocity;
	public float drag;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition += velocity * Time.deltaTime;
		
		velocity *= drag;
		
		
	}
	
	public void Setup(Vector3 velocity, float drag){
		this.velocity = velocity;
		this.drag = drag;
	}
	
}
