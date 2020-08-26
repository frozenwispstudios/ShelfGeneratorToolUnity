using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ShelfGeneratorToolWindow : EditorWindow
{
    [MenuItem("Tools/Shelf Generator Tool")]
    public static void ShelfGeneratorTool() => GetWindow<ShelfGeneratorToolWindow>("Shelf Generator Tool").minSize = new Vector2(415f,300f);//creates editor Window
    

    public bool editMode = false;
    public int prefabCount;
    public float bufferSpaceMin = 0;
    public float bufferSpaceMax = 3;

    public float bufferSpawnSpaceMinLimit = 0;
    public float bufferSpawnSpaceMaxLimit = 10;

    public GameObject[] prefabs;

    SerializedObject SO;
    SerializedProperty propPrefabCount;
    SerializedProperty propbufferSpaceMin;
    SerializedProperty propbufferSpaceMax;
    SerializedProperty propPrefabs;


    void OnEnable()
    {
        SO = new SerializedObject(this);
        propPrefabCount = SO.FindProperty("prefabCount");
        propbufferSpaceMin = SO.FindProperty("bufferSpaceMin");
        propbufferSpaceMax = SO.FindProperty("bufferSpaceMax");
        propPrefabs = SO.FindProperty("prefabs");

        Selection.selectionChanged += Repaint;//updates GUI to be active
        SceneView.duringSceneGui += DuringSceneGUI;//draw live in scene
    }

    void OnDisable()
    {
        Selection.selectionChanged -= Repaint;//updates GUI to be deactive
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    public static void ClearShelf()
    {
        //checks if a list of objects that have been generated on the shelf are loaded and if so destorys them
    }

    
    public static void GenerateShelf(int _prefabCount, GameObject[] _prefabs)//this is a Validate Function for the funciton above
    {
        //gets the game objects selected in the scene in the editor 
        foreach (GameObject parentShelfs in Selection.gameObjects)
        {
            Undo.RecordObject(parentShelfs.transform, "UNDO_STR_SNAP");
            //get the size and the give it a buffer then spawn

            //have the amount wanted to be spawned on there
            //loop though that for each one
            //spawn the object based on size or based on how buffer
            //spawn based of mesh size then they can change the buffer or have the buffer be a range

            //Create a list of childern to clean when you want to generate again 


            for (int i = 0; i < _prefabCount; i++)
            {
                GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefabs[0]);
                Undo.RegisterCreatedObjectUndo(spawnedObject, "Undo Spawned Object");//make this object undoable/Ctrl + Z
                spawnedObject.transform.position = new Vector3(0f + ((float)i),0f,0f);
                spawnedObject.transform.parent = parentShelfs.transform;
                //spawnedObject.transform.rotation = //rotation;
                //get range to spawn them in here generating it randomly
            }


            //gameobj.transform.position = placement math here
        }
    }

    void OnGUI()
    {
        //GUI Layout
        GUILayout.Label("Shelf Generator", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Shelf", GUILayout.Height(50)) && Selection.gameObjects.Length > 0)
        {
            //GenerateShelf(prefabCount,prefabs, bufferSpawnSpace);
        }

        //Update Editor properties/variables
        SO.Update();
        EditorGUILayout.PropertyField(propPrefabCount, GUILayout.ExpandWidth(false));
        EditorGUILayout.PropertyField(propPrefabs);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(propbufferSpaceMin, GUILayout.ExpandWidth(false));
        EditorGUILayout.PropertyField(propbufferSpaceMax , GUILayout.ExpandWidth(false));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.MinMaxSlider("Spawn Space",ref bufferSpaceMin, ref bufferSpaceMax, bufferSpawnSpaceMinLimit, bufferSpawnSpaceMaxLimit, GUILayout.ExpandWidth(false));
        EditorGUILayout.EndVertical();

        SO.ApplyModifiedProperties();

    }

    void DuringSceneGUI(SceneView sceneView)
    {


    }
}
