using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {
		
	public float lightIntensityUp;
	public float lightIntensityDown;
	public float destinationIntensity;
	public float fadeSpeed;
	
	protected StartScript startScript;
	
	void Start () {
		startScript = (StartScript)Object.FindObjectOfType(typeof(StartScript));
		
		startScript.OnGameStart += LightUp;
		startScript.OnWin += LightDown;
		startScript.OnDraw += LightDown;
		startScript.OnGameReset += LightUp;
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
