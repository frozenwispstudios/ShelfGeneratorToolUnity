using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ShelfGeneratorToolWindow : EditorWindow
{
    [MenuItem("Tools/Shelf Generator Tool")]
    public static void ShelfGeneratorTool() => GetWindow<ShelfGeneratorToolWindow>("Shelf Generator Tool").minSize = new Vector2(415f,300f);//creates editor Window with min size settings
    
    public bool editMode = false;
    public int prefabCount;
    public float bufferSpaceMin = 0;
    public float bufferSpaceMax = 3;

    public float bufferSpawnSpaceMinLimit = 0;
    public float bufferSpawnSpaceMaxLimit = 10;

    public bool localShelfScale;

    public GameObject[] prefabs;

    SerializedObject SO;
    SerializedProperty propPrefabCount;
    SerializedProperty propbufferSpaceMin;
    SerializedProperty propbufferSpaceMax;
    SerializedProperty propPrefabs;
    SerializedProperty propLocalShelfScale;


    void OnEnable()
    {
        SO = new SerializedObject(this);
        propPrefabCount = SO.FindProperty("prefabCount");
        propbufferSpaceMin = SO.FindProperty("bufferSpaceMin");
        propbufferSpaceMax = SO.FindProperty("bufferSpaceMax");
        propPrefabs = SO.FindProperty("prefabs");
        propLocalShelfScale = SO.FindProperty("localShelfScale");

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

    public static void GenerateShelf(int _prefabCount, GameObject[] _prefabs, float bufferSpaceMin, float bufferSpaceMax)//this is a Validate Function for the funciton above
    {
        //gets the game objects selected in the scene in the editor 
        foreach (GameObject parentShelfs in Selection.gameObjects)
        {
            //Shelf/Parent spawn point
            Undo.RecordObject(parentShelfs.transform, "UNDO_STR_SNAP");

            //get shelf render bounds
            Renderer rndShelf = parentShelfs.GetComponent<Renderer>();
            Vector3 rndShelfCentre = rndShelf.bounds.center;
            Vector3 rndShelfSize = rndShelf.bounds.size;

            float CurrentX = 0;
            Debug.Log(rndShelfSize);
            //Create a list of childern to clean when you want to generate again 

            //get the bounding box of the prefab that i want to spawn and get its height to place it there when spawning
            //get the length of the shelf/surfaceand place X amount along it until you go of the length

            for (int i = 0; i < _prefabCount; i++)
            {
                //default bufferspace needs to be the distance of the current objects bounds
                float _bufferSpace = Random.Range(bufferSpaceMin, bufferSpaceMax);
                GameObject currentPrefab = _prefabs[Random.Range(0, _prefabs.Length)];

                //get the render of the object we are spawning
                Renderer rnd = currentPrefab.GetComponent<Renderer>();
                Vector3 rndCentre = rnd.bounds.center;
                Vector3 rndSize =  rnd.bounds.size;

                if (i != 0) { CurrentX += rndSize.x + _bufferSpace; }

                //make sure it can fit on the shelf
                if (CurrentX <= parentShelfs.transform.position.x + rndShelfSize.x + rndSize.x)
                {
                    Debug.Log("Stop");

                    //spawn object
                    GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(currentPrefab);
                    Undo.RegisterCreatedObjectUndo(spawnedObject, "Undo Spawned Object");//make this object undoable/Ctrl + Z

                    float spawnY = parentShelfs.transform.position.y + rndShelfSize.y / 2 + rndSize.y / 2;
                    float spawnX = (parentShelfs.transform.position.x - (rndShelfSize.x / 2) + rndSize.x) + CurrentX;
                    spawnedObject.transform.position = new Vector3(spawnX, spawnY, parentShelfs.transform.position.z);
                    //spawnedObject.transform.parent = parentShelfs.transform;

                    //spawnedObject.transform.rotation = parentShelfs.transform.rotation;//rotation;
                    //get range to spawn them in here generating it randomly
                }
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
            GenerateShelf(prefabCount,prefabs,bufferSpaceMin, bufferSpaceMax);
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


        EditorGUILayout.PropertyField(propLocalShelfScale);
        SO.ApplyModifiedProperties();

    }

    void DuringSceneGUI(SceneView sceneView)
    {


    }
}
