using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    // Start is called before the first frame update
    static bool initialized = false;
    void Start()
    {
        if (initialized) return;
        initialized = true;

        Application.targetFrameRate = 90;
        Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, Screen.fullScreen);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
