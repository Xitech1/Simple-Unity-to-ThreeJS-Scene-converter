using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEditor;

public class ExportToThreeJS : EditorWindow
{
    private const string CAMERA_TYPE = "PerspectiveCamera";
    private const int CAMERA_ZOOM = 1;
    private const int CAMERA_FOCUS = 10;
    private const int CAMERA_FILM_GAUGE = 35;

    private const string ROOT_FOLDER_NAME = "Unity_to_HTML5";
    private const string JS_FOLDER_NAME = "js";
    private const string SCENE_FOLDER_NAME = "scene";
    private const string SCENE_OBJECTS_BOOKMARK = "[[SceneObjects]]";
    private const string CAMERA_OBJECT_BOOKMARK = "[[CameraObject]]";
    private const string SCENE_FILE_NAME = "Scene.JSON";
    private const string CAMERA_FILE_NAME = "Camera.JSON";
    private const string THREEJS_FILE_NAME = "three.js";
    private const string HTML_FILE_NAME = "index.html";

    private static string cameraFile;
    private static string sceneFile;
    private static string threeJSFile;
    private static string desktopPath;
    private static string indexHTMLFile;

    [MenuItem("Converter/Export to HTML5")]
    static void Init()
    {
        SetupFolderStructure();
        GetFilesAndFileReferences();
        CreateSceneryFile();

        if (Directory.Exists(Path.Combine(desktopPath, ROOT_FOLDER_NAME)))
        {
            Application.OpenURL(Path.Combine(desktopPath, ROOT_FOLDER_NAME));
        }
    }

    static void GetFilesAndFileReferences()
    {
        StreamReader reader = new StreamReader("Assets/Resources/Files/JSON/Camera.JSON");
        cameraFile = reader.ReadToEnd();
        reader.Close();

        reader = new StreamReader("Assets/Resources/Files/JSON/Scene.JSON");
        sceneFile = reader.ReadToEnd();
        reader.Close();

        reader = new StreamReader("Assets/Resources/Files/ThreeJS/three.js");
        threeJSFile = reader.ReadToEnd();
        reader.Close();

        Debug.Log("" + desktopPath + " | " + ROOT_FOLDER_NAME + " | " + JS_FOLDER_NAME);

        CreateFile(threeJSFile, Path.Combine(desktopPath, ROOT_FOLDER_NAME, JS_FOLDER_NAME, THREEJS_FILE_NAME));

        reader = new StreamReader("Assets/Resources/Files/HTML/index.html");
        indexHTMLFile = reader.ReadToEnd();
        reader.Close();

        CreateFile(indexHTMLFile, Path.Combine(desktopPath, ROOT_FOLDER_NAME, HTML_FILE_NAME));
    }

    static void SetupFolderStructure()
    {
        desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        if (!Directory.Exists(Path.Combine(desktopPath, ROOT_FOLDER_NAME)))
        {
            Directory.CreateDirectory(Path.Combine(desktopPath, ROOT_FOLDER_NAME));
        }

        if (!Directory.Exists(Path.Combine(desktopPath, ROOT_FOLDER_NAME, JS_FOLDER_NAME)))
        {
            Directory.CreateDirectory(Path.Combine(desktopPath, ROOT_FOLDER_NAME, JS_FOLDER_NAME));
        }

        if (!Directory.Exists(Path.Combine(desktopPath, ROOT_FOLDER_NAME, SCENE_FOLDER_NAME)))
        {
            Directory.CreateDirectory(Path.Combine(desktopPath, ROOT_FOLDER_NAME, SCENE_FOLDER_NAME));
        }
    }

    static void CreateSceneryFile()
    {
        var fullJSON = sceneFile.Replace(SCENE_OBJECTS_BOOKMARK, JsonUtility.ToJson(CreateSceneModelFromRootGameObjects(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())));
        CreateFile(fullJSON, Path.Combine(desktopPath, ROOT_FOLDER_NAME, SCENE_FOLDER_NAME, SCENE_FILE_NAME));
    }

    static void CreateFile(string fileInText, string path)
    {
        using (StreamWriter sw = File.CreateText(path))
        {
            sw.Write(fileInText);
        }
    }

    static Vector3 ConvertFromUnityPositionToThreeJSPos(Vector3 unityPos)
    {
        return new Vector3(unityPos.x * -1, unityPos.y, unityPos.z);
    }

    static Quaternion ConvertFromUnityRotationToThreeJSRotion(Transform unityTransform)
    {
        Quaternion converted = new Quaternion(-unityTransform.localRotation.x, -unityTransform.localRotation.y, unityTransform.localRotation.z, -unityTransform.localRotation.w);
        Vector3 eulerConverted = new Vector3(converted.eulerAngles.x * -1, converted.eulerAngles.y - 180, converted.eulerAngles.z * -1);
        return Quaternion.Euler(eulerConverted);
    }

    static float[] GetMatrixFromTransform(Transform trans)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(ConvertFromUnityPositionToThreeJSPos(trans.localPosition), ConvertFromUnityRotationToThreeJSRotion(trans), trans.localScale);
        float[] matrixFloats = new float[16];

        //Convert and create into ThreeJS serialzable format.
        //Position
        matrixFloats[12] = matrix[0, 3];
        matrixFloats[13] = matrix[1, 3];
        matrixFloats[14] = matrix[2, 3];
        //Scale
        matrixFloats[0] = matrix[0, 0];
        matrixFloats[5] = matrix[1, 1];
        matrixFloats[10] = matrix[2, 2];
        //Rotation
        matrixFloats[1] = matrix[0, 1];
        matrixFloats[2] = matrix[0, 2];
        matrixFloats[4] = matrix[1, 0];
        matrixFloats[6] = matrix[1, 2];
        matrixFloats[8] = matrix[2, 0];
        matrixFloats[9] = matrix[2, 1];

        return matrixFloats;
    }

    static float[] CreateSceneMatrix()
    {
        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
        float[] matrixFloats = new float[16];

        //Position
        matrixFloats[12] = matrix[0, 3];
        matrixFloats[13] = matrix[1, 3];
        matrixFloats[14] = matrix[2, 3];
        //Scale
        matrixFloats[0] = matrix[0, 0];
        matrixFloats[5] = matrix[1, 1];
        matrixFloats[10] = matrix[2, 2];
        //Rotation
        matrixFloats[1] = matrix[0, 1];
        matrixFloats[2] = matrix[0, 2];
        matrixFloats[4] = matrix[1, 0];
        matrixFloats[6] = matrix[1, 2];
        matrixFloats[8] = matrix[2, 0];
        matrixFloats[9] = matrix[2, 1];

        return matrixFloats;
    }

    static void CreateCameraModelFile(Camera mainCamera)
    {
        CameraModel model = new CameraModel(new Guid().ToString(), CAMERA_TYPE, "Camera", GetMatrixFromTransform(mainCamera.transform), mainCamera.fieldOfView, CAMERA_ZOOM, mainCamera.nearClipPlane, mainCamera.farClipPlane, CAMERA_FOCUS, 1.7f, CAMERA_FILM_GAUGE);

        var fullJson = cameraFile.Replace(CAMERA_OBJECT_BOOKMARK, JsonUtility.ToJson(model));
        CreateFile(fullJson, Path.Combine(desktopPath, ROOT_FOLDER_NAME, SCENE_FOLDER_NAME, CAMERA_FILE_NAME));
    }

    static SceneModel CreateSceneModelFromRootGameObjects(GameObject[] rootGameObjects)
    {
        SceneModel sceneModelToReturn = new SceneModel(new Guid().ToString(), "scene", "scene", CreateSceneMatrix(), new List<BoxModel>());
        foreach (GameObject go in rootGameObjects)
        {
            if (go.GetComponent<Camera>() != null)
            {
                CreateCameraModelFile(go.GetComponent<Camera>());
            }
            else
            {
                sceneModelToReturn.children.Add(new BoxModel(new Guid().ToString(), "Mesh", go.name, GetMatrixFromTransform(go.transform), "D65AB9D7-FC40-4302-9A08-F8B8431E0D98", new Guid().ToString(), GetChildrenFromModels(go.gameObject)));
            }
        }
        return sceneModelToReturn;
    }

    static List<BoxModel> GetChildrenFromModels(GameObject gameObject)
    {
        List<BoxModel> sceneObjectToReturn = null;
        if (gameObject.GetComponentInChildren<Transform>().childCount > 0)
        {
            sceneObjectToReturn = new List<BoxModel>();
            foreach (Transform transform in gameObject.GetComponentInChildren<Transform>())
            {
                if (transform.GetComponent<Camera>() != null)
                {
                    CreateCameraModelFile(transform.GetComponent<Camera>());
                }
                else
                {
                    sceneObjectToReturn.Add(new BoxModel(new Guid().ToString(), "Mesh", transform.name, GetMatrixFromTransform(transform), "D65AB9D7-FC40-4302-9A08-F8B8431E0D98", new Guid().ToString(), GetChildrenFromModels(transform.gameObject)));
                }
            }
        }

        return sceneObjectToReturn;
    }
}