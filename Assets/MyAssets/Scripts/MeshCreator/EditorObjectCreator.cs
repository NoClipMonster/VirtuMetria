using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[ExecuteAlways]

public class EditorObjectCreator : EditorWindow
{

    #region Cube
    [MenuItem("Shape/Cube/Create")]
    static void CreateCube()
    {
        GameObject gO;
        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(1, 0, 1));
        list.Add(new Vector3(0, 0, 0));
        list.Add(new Vector3(0, 1, 0));
        ObjectCreator oC = new ObjectCreator();
        gO = oC.CreateSquare(list);
        gO.name = "Cube";
        gO.tag = "CreatedByMenu";
        gO.GetComponent<MeshCollider>().convex = true;
        gO.GetComponent<MeshCollider>().sharedMesh = gO.GetComponent<MeshFilter>().sharedMesh;
        gO.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("BlackMaterial");        
        gO.AddComponent<MeshEditor>().TrackingObject = GameObject.FindGameObjectWithTag("MainCamera");
        Undo.RegisterCreatedObjectUndo(gO, "Создание куба");
    }

    [MenuItem("Shape/Cube/Delete Last")]
    static void DeleteLastCube()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("CreatedByMenu");
        for (int i = gameObjects.Length - 1; i >= 0; i--)
            if (gameObjects[i].name == "Cube")
            {
                Undo.DestroyObjectImmediate(gameObjects[i]);
                break;
            } 
    }

    [MenuItem("Shape/Cube/Delete All")]
    static void DeleteAllCubes()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("CreatedByMenu"))
            if (item.name == "Cube")
                Undo.DestroyObjectImmediate(item);
    }
    #endregion

    #region Square Plane
    [MenuItem("Shape/Square Plane/Create")]
    static void CreateSqrPlane()
    {
        GameObject gO;
        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(1, 0.25f, 1));
        list.Add(new Vector3(0, 0.25f, 0));
        list.Add(new Vector3(0, 0.25f, 1));
        Plane plane = new Plane(list[0], list[1], list[2]);
        ObjectCreator oC = new ObjectCreator();
        gO = oC.CreateSqrPlane((list[0]+ list[1])/2,plane.normal);
        gO.name = "Square Plane";
        gO.tag = "CreatedByMenu";
        gO.GetComponent<MeshCollider>().convex = true;
        gO.GetComponent<MeshCollider>().sharedMesh = gO.GetComponent<MeshFilter>().sharedMesh;
        gO.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("BlackMaterial");
        gO.AddComponent<MeshEditor>().TrackingObject = GameObject.FindGameObjectWithTag("MainCamera");
        Undo.RegisterCreatedObjectUndo(gO, "Создание квадратной плоскости");
       
    }

    [MenuItem("Shape/Square Plane/Delete Last")]
    static void DeleteLastSqrPlane()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("CreatedByMenu");
        for (int i = gameObjects.Length - 1; i >= 0; i--)
            if (gameObjects[i].name == "Square Plane")
            {
                Undo.DestroyObjectImmediate(gameObjects[i]);
                break;
            }
    }

    [MenuItem("Shape/Square Plane/Delete All")]
    static void DeleteAllSqrPlane()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("CreatedByMenu"))
            if (item.name == "Square Plane")
                Undo.DestroyObjectImmediate(item);
    }
    #endregion

    #region Section
    [MenuItem("Shape/Section Plane/Create")]
    static void CreateSection()
    {
        GameObject gO;
        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(1, 0.25f, 1));
        list.Add(new Vector3(0, 0.25f, 0));
        list.Add(new Vector3(0, 0.25f, 1));
        Plane plane = new Plane(list[0], list[1], list[2]);
        ObjectCreator oC = new ObjectCreator();
        gO = oC.CreateSqrPlane((list[0] + list[1]) / 2, plane.normal,2);
        gO.name = "Section Plane";
        gO.tag = "CreatedByMenu";
        gO.layer = 7;
        gO.GetComponent<MeshCollider>().convex = true;
        gO.GetComponent<MeshCollider>().isTrigger = true;
        gO.GetComponent<MeshCollider>().sharedMesh = gO.GetComponent<MeshFilter>().sharedMesh;
        gO.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("RedTranspMaterial");
        Undo.RegisterCreatedObjectUndo(gO, "Создание сечения");
    }

    [MenuItem("Shape/Section Plane/Delete Last")]
    static void DeleteLastSection()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("CreatedByMenu");
        for (int i = gameObjects.Length - 1; i >= 0; i--)
            if (gameObjects[i].name == "Section Plane")
            {
                Undo.DestroyObjectImmediate(gameObjects[i]);
                break;
            }
    }

    [MenuItem("Shape/Section Plane/Delete All")]
    static void DeleteAllSection()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("CreatedByMenu"))
            if (item.name == "Section Plane")
                Undo.DestroyObjectImmediate(item);
    }
    #endregion
}
