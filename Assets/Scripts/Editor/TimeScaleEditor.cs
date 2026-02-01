using UnityEditor;
using UnityEngine;

public class TimeScaleEditor : EditorWindow
{
    private float timeScale = 1.0f;
    private bool isPaused = false;
    
    [MenuItem("Tools/Time Scale Control")]
    public static void ShowWindow()
    {
        GetWindow<TimeScaleEditor>("Time Scale");
    }

    private void OnEnable()
    {
        // Don't reset when entering play mode - keep the editor value
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        
        // Time scale slider with reset button
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Time Scale:", GUILayout.Width(80));
        timeScale = EditorGUILayout.Slider(timeScale, 0.0f, 2.0f);
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            timeScale = 1.0f;
        }
        EditorGUILayout.EndHorizontal();
        
        // Display current value
        EditorGUILayout.LabelField($"Current: {Time.timeScale:F2}x", EditorStyles.miniLabel);
    
        
     
        // Apply time scale continuously while in play mode
        if (Application.isPlaying && !isPaused)
        {
            Time.timeScale = timeScale;
        }
    }

    private void Update()
    {
        // Repaint window during play mode to keep UI responsive
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
