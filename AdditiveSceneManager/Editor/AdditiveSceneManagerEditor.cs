using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdditiveSceneManager))]
public class AdditiveSceneManagerEditor : Editor
{
    AdditiveSceneManager manager;

    public void OnEnable() {
        manager = (AdditiveSceneManager)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        // Don't do anything if not playing
        if (!Application.isPlaying) return;

        // Render controls for each scene
        EditorGUILayout.LabelField("=== Scene Controls ===", EditorStyles.boldLabel);
        foreach(Object s in manager.scenes) SceneButtons(s);
    }

    public void SceneButtons(Object s) {

        // Query if scene is active
        bool sceneActive = manager.QuerySceneLoaded(s.name);

        // Start a horizontal group for the buttons
        GUILayout.BeginHorizontal();

        GUI.enabled = !sceneActive;
        if (GUILayout.Button($"Load \"{s.name}\"")) manager.LoadScene(s.name);
        GUI.enabled = sceneActive;
        GUILayout.Space(5);
        if (GUILayout.Button($"Unload \"{s.name}\"")) manager.UnloadScene(s.name);
        GUI.enabled = true;

        // End the horizontal group
        GUILayout.EndHorizontal();
    }

}
