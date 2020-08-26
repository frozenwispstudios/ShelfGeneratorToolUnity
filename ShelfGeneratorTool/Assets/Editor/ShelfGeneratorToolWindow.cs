using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ShelfGeneratorToolWindow : EditorWindow
{
    [MenuItem("Tools/Shelf Generator Tool")]
    public static void ShelfGeneratorTool() => GetWindow<ShelfGeneratorToolWindow>("Shelf Generator Tool");//creates editor Window

    SerializedObject SO;

    void OnEnable()
    {
        SO = new SerializedObject(this);

        Selection.selectionChanged += Repaint;//updates GUI to be active
        SceneView.duringSceneGui += DuringSceneGUI;//draw live in scene
    }

    void OnDisable()
    {
        Selection.selectionChanged -= Repaint;//updates GUI to be deactive
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    void OnGUI()
    {
        //GUI Layout
        GUILayout.Label("Shelf Generator", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Shelf", GUILayout.Height(50)) && Selection.gameObjects.Length > 0)
        {
            //GenerateShelf();
        }

        SO.Update();
        SO.ApplyModifiedProperties();
    }

    void DuringSceneGUI(SceneView sceneView)
    {

    }
}
