using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		AssetBundle.LoadFromFile("/sdcard/Android/obb/com.oculus.demo/main.55.com.flowmeditation.flowvr.obb");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
