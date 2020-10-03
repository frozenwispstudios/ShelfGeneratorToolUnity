using UnityEngine;
using UnityEditor;

public class ShelfGeneratorToolWindow : EditorWindow
{
    [MenuItem("Tools/Frozenwispstudios/Shelf Generator Tool")]
    public static void ShelfGeneratorTool() => GetWindow<ShelfGeneratorToolWindow>("Shelf Generator Tool").minSize = new Vector2(415f,300f);//creates editor Window with min size settings
    
    public bool editMode = false;
    public int prefabCount = 5;
    public float bufferSpaceMin = 0.05f;
    public float bufferSpaceMax = 0.1f;

    public float bufferSpawnSpaceMinLimit = 0;
    public float bufferSpawnSpaceMaxLimit = 1;

    public GameObject[] prefabs;

    SerializedObject SO;
    SerializedProperty propPrefabCount;
    SerializedProperty propbufferSpaceMin;
    SerializedProperty propbufferSpaceMax;
    SerializedProperty propPrefabs;

    void OnEnable()
    {
        //init properties
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
        //updates GUI to be deactive on closing of window
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    public static void ClearShelf()
    {
        //loop though all selected objects and delete them from the shelf
        foreach (GameObject parentShelfs in Selection.gameObjects)
        {
            int currentChildCount = parentShelfs.transform.childCount;
            for (int i = 0; i < currentChildCount; i++)
            {
                Undo.RecordObject(parentShelfs.transform.GetChild(0).transform, "UndoClear");
                Object.DestroyImmediate(parentShelfs.transform.GetChild(0).gameObject);
            }
        }
    }

    
    public static void GenerateShelf(int _prefabCount, GameObject[] _prefabs, float bufferSpaceMin, float bufferSpaceMax)//this is a Validate Function for the funciton above
    {
        //gets the game objects selected in the scene in the editor 
        foreach (GameObject parentShelfs in Selection.gameObjects)
        {
            //Make parent objects undoable
            Undo.RecordObject(parentShelfs.transform, "UNDO_STR_SNAP");

            //get shelf render bounds
            Renderer rndShelf = parentShelfs.GetComponent<Renderer>();
            Vector3 rndShelfSize = rndShelf.bounds.size;

            float CurrentX = 0;
            float CurrentZ = 0;
            GameObject previousObject = null;

            //Create a list of childern to clean when you want to generate again 
            ClearShelf();

            for (int i = 0; i < _prefabCount; i++)
            {
                //default bufferspace needs to be the distance of the current objects bounds
                float _bufferSpace = Random.Range(bufferSpaceMin, bufferSpaceMax);
                GameObject currentPrefab = _prefabs[Random.Range(0, _prefabs.Length)];

                //get the render of the object we are spawning
                Renderer rnd = currentPrefab.GetComponent<Renderer>();
                Vector3 rndSize = rnd.bounds.size;
                Vector3 currentRndSize = rnd.bounds.size;
                Vector3 currentRndCenter = rnd.bounds.center;

                //check if the previous prefab spawn is bigger so that the next object spawns correctly
                if (previousObject != null)
                {
                    if (previousObject.GetComponent<Renderer>().bounds.size.x > currentPrefab.GetComponent<Renderer>().bounds.size.x)
                    {
                        //get the render of the object we are spawning
                        rnd = previousObject.GetComponent<Renderer>();
                        rndSize = rnd.bounds.size;
                    }
                }

                //make the first one spawn with out the buffer
                if (i != 0) 
                {
                    CurrentX += rndSize.x + _bufferSpace;
                    CurrentZ += rndSize.z + _bufferSpace;
                }

                //make sure it can fit on the shelf
                if (CurrentX <  rndShelfSize.x - rndSize.x)//end of spawning
                {
                    //spawn object
                    GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(currentPrefab);
                    Undo.RegisterCreatedObjectUndo(spawnedObject, "Undo Spawned Object");//make this object undoable/Ctrl + Z

                    float spawnX = (parentShelfs.transform.position.x - (rndShelfSize.x / 2) + rndSize.x) + CurrentX;//spawn at the start of the object then add the difference between them
                    float spawnY = 0f;

                    //if the center*2 is not equal to the hight/2 then its equal so do prefab spawn logic else do centre/2
                    if (currentRndCenter.y != currentRndSize.y/2)
                    {
                        spawnY = parentShelfs.transform.position.y + rndShelfSize.y / 2 + currentRndSize.y/2;//spawn on top of object
                    }
                    else
                    {
                        spawnY = parentShelfs.transform.position.y + rndShelfSize.y / 2 + currentRndCenter.y;//spawn on top of object
                    }
                    
                    
                    //float spawnZ = (parentShelfs.transform.position.z + rndSize.z) + CurrentZ;
                    float spawnZ = (parentShelfs.transform.position.z);

                    spawnedObject.transform.position = new Vector3(spawnX, spawnY, spawnZ);
                    spawnedObject.transform.parent = parentShelfs.transform;

                    previousObject = currentPrefab;
                }
            }
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

        if (GUILayout.Button("Clear Shelf", GUILayout.Height(50)) && Selection.gameObjects.Length > 0)
        {
            ClearShelf();
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

        //UnityEditor.DragAndDrop
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        //Create a start and end function for the mesh you want to spawn on
        //start of where the prefabs will spawn and the end for where they will stop as there is not more space

        //Use that to draw two lines on one each end of the shelf
    }
}
