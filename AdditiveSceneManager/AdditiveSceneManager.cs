using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Linq;

public class AdditiveSceneManager : MonoBehaviour
{

    [System.Serializable]
    public class Ref
    {
        public string key;
        public Object reference;
    }

    // Global instance
    public static AdditiveSceneManager Instance;
    
    [Header("=== Scene Lists ===")]
    [Tooltip("List of additive scenes. These scenes must be added to \"Build Settings\" too.")]
    #if UNITY_EDITOR
    public Object[] scenes;
    #endif
    [HideInInspector] 
    public List<string> scene_names;
    private Dictionary<string, Object> sceneDict;
    private List<string> activeScenes = new List<string>();
    private string switchSceneName = null;

    [Header("=== Reference Management ===")]
    public List<Ref> references = new List<Ref>();
    private Dictionary<string, Object> refDict;

    [Header("=== Callbacks ===")]
    public UnityEvent<string> onSceneLoadedCallback;
    public UnityEvent<string> onSceneUnloadedCallback;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        scene_names.Clear();
        foreach (Object s in scenes) {
            if (s != null) scene_names.Add(s.name);
        }
    }
    #endif

    // ===========================================
    // On Awake - we need to intiialize everything
    // ===========================================
    private void Awake() {
        Instance = this;
        switchSceneName = null;
        InitializeScenes();
        InitializeRefs();
    }

    // ===========================================
    // Called during `Awake()`. Ensure that the scene manager calls `OnSceneLoaded` and `OnSceneUnloaded` whenever a scene change occurs
    // This enables us to control certain events, such as if certain events should be invoked upon scenes changing.
    // ===========================================
    private void InitializeScenes() {
        // Load listeners
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        // Initialize arrays and dictionary
        activeScenes = new List<string>();
    }

    // ===========================================
    // If we need to pass any refs to new scenes, then we do so here.
    // You can also technically use Singleton logic with your components, so this isn't strictly necessary.
    // ===========================================
    private void InitializeRefs() {
        refDict = new Dictionary<string, Object>();
        foreach(Ref r in references) refDict.Add(r.key, r.reference);
    }

    // ===========================================
    // Call this function if you want to load a scene by name. This variant forces you to define a load scene mode.
    // The overload version doesn't require the 2nd parameter and assumes you want to add a scene additively.
    // ===========================================
    public bool LoadScene(string query, LoadSceneMode mode) {
        // Check if our scene is actually one we have reference to. We can't do anything if we don't have a ref to it.
        if(!scene_names.Contains(query)) {
            Debug.LogError($"Query scene \"{query}\" doesn't exist in this scene manager!");
            return false;
        }
        // If the scene is already loaded, then we don't additively add it again.
        if (activeScenes.Contains(query)) {
            Debug.LogError($"Query scene \"{query}\" is already loaded additively. Will not add scene again.");
            return false;
        }
        // Assuming all checks pass, we safely load the scene.
        SceneManager.LoadScene(query, mode);
        return true;
    }
    public bool LoadScene(string query) {
        return LoadScene(query, LoadSceneMode.Additive);
    }

    // ===========================================
    // Call this function to unload a loaded scene.
    // ===========================================
    public bool UnloadScene(string query) {
        // Check if our scene is actually one we have reference to. We can't do anything if we don't have a ref to it.
        if (!scene_names.Contains(query)) {
            Debug.LogError($"Query scene \"{query}\" doesn't exist in this scene manager!");
            return false;
        }
        // If the scene is not loaded, then there's no point unloaded an unloaded scene
        if (!activeScenes.Contains(query)) {
            Debug.LogError($"Query scene \"{query}\" is not loaded.");
            return false;
        }
        // Safety checks, we call load scene async. This is a bit safer, as `UnloadScene` is no longer recommended
        SceneManager.UnloadSceneAsync(query);
        return true;
    }

    // ===========================================
    // This is a more unique function that allows us to `toggle` scenes on-off. 
    // I don't recommend doing this as memory issues can occur if you keep toggling scenes on-off, but it's here if needed
    // ===========================================
    public void ToggleScene(string query) {
        if (activeScenes.Contains(query)) UnloadScene(query);
        else LoadScene(query, LoadSceneMode.Additive);
    }

    // ===========================================
    // This is a more unique function that allows you to `switch` between additive scenes.
    // Note that because scenes may load asynchronously (or however much "asynchronous" the main thread allows), 
    // It's recommended that you attach listeners to `onSceneLoadedCallback` and `onSceneUnloadedCallback` for optimal event invokation.
    // In this case, we must wait for the scene to be unloaded first before we load in the second one, to avoid potential singleton issues.
    // We use a private string that's stored in memory for this, and then when `OnSceneUnloaded` is called it'll call `LoadScene`
    // ===========================================
    public void SwitchScenes(string toUnload, string toLoad) {
        // Set a reference to the scene-to-load in memory
        switchSceneName = toLoad;
        // It' not the end of the world if we can't unload the scene of choice. We just have to handle if that case happens
        if (!UnloadScene(toUnload)) {
            // Failure to unload scene. We can just call `LoadScene` from here
            switchSceneName = null;
            LoadScene(toLoad, LoadSceneMode.Additive);
            return;
        }
        // Getting here, we've unloaded the scene. We can now just rely on `OnSceneUnloaded` to update the scene.
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.SetActiveScene(scene);
        if (!activeScenes.Contains(scene.name)) activeScenes.Add(scene.name);
        Debug.Log($"Scene \"{scene.name}\" loaded!");
        onSceneLoadedCallback?.Invoke(scene.name);
    }

    public void OnSceneUnloaded(Scene scene) {
        if (activeScenes.Contains(scene.name)) activeScenes.Remove(scene.name);
        Debug.Log($"Scene \"scene.name\" unloaded!");
        onSceneUnloadedCallback?.Invoke(scene.name);
        // Unique case: is there a scene we need to switch to?
        if (!string.IsNullOrEmpty(switchSceneName)) {
            LoadScene(switchSceneName, LoadSceneMode.Additive);
            switchSceneName = null;
        }
    }

    public bool QuerySceneLoaded(string query) {
        return activeScenes.Contains(query);
    }
    public bool QuerySceneReferenced(string query) {
        return scene_names.Contains(query);
    }

    public bool TryGetRef(string query, out GameObject g) {
        bool found = refDict.ContainsKey(query);
        g = found ? refDict[query] as GameObject : null;
        return found;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }






}
