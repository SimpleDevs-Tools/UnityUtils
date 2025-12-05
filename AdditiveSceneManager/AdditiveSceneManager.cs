using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AdditiveSceneRef {
    public string scene_name;
    public Object scene_ref;
}

public class AdditiveSceneManager : MonoBehaviour
{

    // Global instance
    public static AdditiveSceneManager Instance;

    // List of scenes
    public AdditiveSceneRef[] scenes;

    // Adictionary of scenes based on `scenes`; generated 


    // Awake
    private void Awake() {
        Instance = this;
    }

    // Add Scene from `scenes` to scene
    public void AddScene(string quuery) {

    } 


}
