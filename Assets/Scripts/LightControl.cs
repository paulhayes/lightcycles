using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {
		
	public float lightIntensityUp;
	public float lightIntensityDown;
	public float destinationIntensity;
	public float fadeSpeed;
	
	protected GameController gameController;
	
	void Start () {
		gameController = FindObjectOfType<GameController>();
		
		gameController.OnGameStart += LightUp;
		gameController.OnWin += LightDown;
		gameController.OnDraw += LightDown;
		gameController.OnGameReset += LightUp;
	}
	
	void LightUp(){
		destinationIntensity = lightIntensityUp;
		StartCoroutine(FadeLight());
	}
	
	void LightDown(){
		destinationIntensity = lightIntensityDown;
		StartCoroutine(FadeLight());
	}
	
	IEnumerator FadeLight(){
		while( light.intensity != destinationIntensity ){
			light.intensity = Mathf.MoveTowards( light.intensity, destinationIntensity, Time.deltaTime * fadeSpeed );
			
			yield return null;
		}
	}
}
