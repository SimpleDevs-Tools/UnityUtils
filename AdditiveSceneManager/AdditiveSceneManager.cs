using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class AdditiveSceneManager : MonoBehaviour
{

    // Global instance
    public static AdditiveSceneManager Instance;
    
    [Header("=== Scene Lists ===")]
    [Tooltip("List of additive scenes. These scenes must be added to \"Build Settings\" too.")]
    public Object[] scenes;
    public Dictionary<string, Object> sceneDict;
    private List<string> activeScenes = new List<string>();

    [Header("=== Callbacks ===")]
    public UnityEvent onSceneLoadedCallback;
    public UnityEvent onSceneUnloadedCallback;

    // Awake
    private void Awake() {
        Instance = this;
        InitializeScenes();
    }

    private void InitializeScenes() {
        // Load listeners
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Initialize arrays and dictionary
        activeScenes = new List<string>();
        sceneDict = new Dictionary<string, Object>();
        foreach(Object s in scenes) {
            sceneDict.Add(s.name, s);
        }
    }

    // Add Scene from `scenes` to scene
    public void LoadScene(string query, LoadSceneMode mode = LoadSceneMode.Additive) {
        if(!sceneDict.ContainsKey(query)) {
            Debug.LogError($"Query scene \"{query}\" doesn't exist in this scene manager!");
            return;
        }
        if (activeScenes.Contains(query)) {
            Debug.LogError($"Query scene \"{query}\" is already loaded additively. Will not add scene again.");
            return;
        }
        SceneManager.LoadScene(sceneDict[query].name, mode);
    }

    public void UnloadScene(string query) {
        if (!sceneDict.ContainsKey(query)) {
            Debug.LogError($"Query scene \"{query}\" doesn't exist in this scene manager!");
            return;
        }
        if (!activeScenes.Contains(query)) {
            Debug.LogError($"Query scene \"{query}\" is not loaded.");
            return;
        }
        SceneManager.UnloadSceneAsync(sceneDict[query].name);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.SetActiveScene(scene);
        if (!activeScenes.Contains(scene.name)) activeScenes.Add(scene.name);
        Debug.Log($"Scene \"{scene.name}\" loaded!");
        onSceneLoadedCallback?.Invoke();
    }

    public void OnSceneUnloaded(Scene scene) {
        if (activeScenes.Contains(scene.name)) activeScenes.Remove(scene.name);
        Debug.Log($"Scene \"scene.name\" unloaded!");
        onSceneUnloadedCallback?.Invoke();
    }

    public bool QuerySceneLoaded(string query) {
        return activeScenes.Contains(query);
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }






}
