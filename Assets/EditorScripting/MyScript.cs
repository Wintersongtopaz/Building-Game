using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyScript))]
public class MyScriptEditor : Editor // Derives from Editor
{
    // creates button that says test
    public override void OnInspectorGUI() // customize how scripts apear in inspector window
    {
        base.OnInspectorGUI();
        MyScript myScript = (MyScript)target;

        GUILayout.Label("Look at this button!");

        if (GUILayout.Button("Test")) Debug.Log("Test");
    }
    // when button hit shoud say "Hey look at this label!" in scene view
    private void OnSceneGUI()
    {
        Handles.Label(Vector3.zero, "Hey look at this label!"); 
    }
}

public class MyScript : MonoBehaviour // custom editor class
{
   
}
