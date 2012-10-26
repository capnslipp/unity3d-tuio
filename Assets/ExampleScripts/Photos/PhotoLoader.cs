using UnityEngine;
using System.IO;
using System.Collections;

public class PhotoLoader : MonoBehaviour {

	public GameObject PhotoPrefab;
	
    void Start () 
	{
        StartCoroutine("loadImages");
    }

    IEnumerator loadImages()
    {
        string[] filePaths = Directory.GetFiles(@"C:\Users\Simon Work PC\Pictures", "*.jpg"); // get every file in chosen directory with the extension.png
		
		foreach (string file in filePaths)
		{
        	WWW www = new WWW("file://" + file);
        	yield return www;
			Texture2D tex = www.texture;
			GameObject go = (GameObject)Instantiate(PhotoPrefab, transform.position, transform.rotation);
			go.renderer.material.mainTexture = tex;
		}
    }
}
