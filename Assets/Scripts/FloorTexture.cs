using UnityEngine;
using System.Collections;

public class FloorTexture : MonoBehaviour {
	
	public Color foreground;
	public Color background;
	public Color glow;
	public int size;
	public int border;
	public int glowBorder;
		
	void Start () {
		Texture2D tex = new Texture2D(size,size,TextureFormat.RGB24, true );
		Texture2D illum = new Texture2D(size,size,TextureFormat.Alpha8, true );
		for( int i=0;i<size;i++){
			for(int j=0;j<size;j++){
				Color c = ( i < border || i > size-border || j < border || j > size-border ) ? background : foreground ;
				tex.SetPixel( i, j, c );
				Color g = ( i < glowBorder || i > size-glowBorder || j < glowBorder || j > size-glowBorder ) ? glow : new Color(0,0,0,0) ;
				illum.SetPixel( i, j, g );
			}
		}
		
		tex.Apply();
		illum.Apply();
		
		renderer.material.mainTexture = tex;
		renderer.material.SetTexture( "_Illum", illum );
		
		Object.Destroy( this );
	}
	
	
}
