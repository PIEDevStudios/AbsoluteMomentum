using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColliderScaler))]
public class ColliderScaleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector fields

        ColliderScaler scaler = (ColliderScaler)target;
        
        if (GUILayout.Button("Reset & Scale Collider"))
        {
            scaler.ScaleColliderToFitScene();
        }
    }
}