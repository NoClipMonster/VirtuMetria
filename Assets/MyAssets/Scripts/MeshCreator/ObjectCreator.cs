using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;


public class ObjectCreator
{
    
    

    public GameObject CreateSquare(List<Vector3> dots)
    {
        GameObject gO = new GameObject();
        gO.transform.position = (dots[2] + dots[0]) / 2;
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i] = gO.transform.InverseTransformPoint(dots[i]);
        }
        gO.AddComponent<MeshRenderer>();
        gO.AddComponent<MeshFilter>();
        gO.AddComponent<MeshCollider>();
        float EdgeLengh = Vector3.Distance(dots[1], dots[2]);
        Vector3 downedge1 = new Vector3(dots[0].x, (dots[0].y), dots[1].z);
        Vector3 downedge2 = new Vector3(dots[1].x, (dots[0].y), dots[0].z);

        Vector3 upedge0 = new Vector3(dots[0].x, dots[2].y, dots[0].z);

        Plane pl1 = new Plane(dots[2] - dots[1], dots[0]);
        Plane pl2 = new Plane(downedge1 - dots[0], downedge1);
        Plane pl3 = new Plane(downedge1 - dots[1], downedge1);

        List<GameObject> gameObjects = new List<GameObject>();
        gameObjects.Add(CreateSqrPlane(dots[0] + ((dots[1] - dots[0]) / 2), pl1.normal, EdgeLengh));
        gameObjects.Add(CreateSqrPlane(upedge0 + ((dots[2] - upedge0) / 2), pl1.normal, EdgeLengh));

        gameObjects.Add(CreateSqrPlane(downedge1 + ((dots[2] - downedge1) / 2), pl2.normal, EdgeLengh));
        gameObjects.Add(CreateSqrPlane(downedge2 + ((upedge0 - downedge2) / 2), pl2.normal, EdgeLengh));

        gameObjects.Add(CreateSqrPlane(downedge1 + ((upedge0 - downedge1) / 2), pl3.normal, EdgeLengh));
        gameObjects.Add(CreateSqrPlane(downedge2 + ((dots[2] - downedge2) / 2), pl3.normal, EdgeLengh));

        gO.GetComponent<MeshFilter>().sharedMesh = CombineObjects(gO, gameObjects).GetComponent<MeshFilter>().sharedMesh;
        foreach (var item in gameObjects)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(item);
            Debug.Log("Editor");
            continue;
#else 
Destroy(item);        
Debug.Log("Game");
#endif
        }
        return gO;
    }

    public GameObject CreateSqrPlane(Vector3 position, Vector3 normal, float size = 1)
    {
        TriangleNetMesh triangleNetMesh;
        GameObject plane;
        GameObject InvPlane;
        GameObject gObject = new GameObject();
        gObject.AddComponent<MeshFilter>();
        gObject.AddComponent<MeshRenderer>();
        gObject.AddComponent<MeshCollider>();
        List<Vector3> dotList = new List<Vector3>();
        dotList.Add(new Vector3(-1, 0, -1));
        dotList.Add(new Vector3(-1, 0, 1));
        dotList.Add(new Vector3(1, 0, 1));
        dotList.Add(new Vector3(1, 0, -1));

        gObject.transform.position = position;
        gObject.transform.forward = normal;

        plane = new GameObject();
        plane.transform.parent = gObject.transform;
        plane.name = "CreatedMesh";
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>();


        InvPlane = new GameObject();
        InvPlane.transform.parent = gObject.transform;
        InvPlane.name = "CreatedInversedMesh";
        InvPlane.AddComponent<MeshFilter>();
        InvPlane.AddComponent<MeshRenderer>();


        MeshFilter meshFilter = plane.GetComponent<MeshFilter>();
        MeshFilter invMeshFilter = InvPlane.GetComponent<MeshFilter>();
        Polygon poly = new Polygon();
        List<Vector2> countur = new List<Vector2>();

        foreach (var item in dotList)
        {
            countur.Add(new Vector2(item.x, item.z) * (size / 2));
        }
        poly.Add(countur);
        triangleNetMesh = (TriangleNetMesh)poly.Triangulate();


        meshFilter.sharedMesh = triangleNetMesh.GenerateUnityMesh();
        Mesh inversMesh = triangleNetMesh.GenerateUnityMesh();

        List<int> triangles = new List<int>();
        triangles.AddRange(inversMesh.triangles);
        triangles.Reverse();
        inversMesh.SetTriangles(triangles, 0);
        invMeshFilter.sharedMesh = inversMesh;

        List<GameObject> gameObjects = new List<GameObject>();
        gameObjects.Add(plane);
        gameObjects.Add(InvPlane);

        MeshFilter mainFilter = gObject.GetComponent<MeshFilter>();
        mainFilter.sharedMesh = new Mesh();
        gObject = CombineObjects(gObject, gameObjects);
        mainFilter.sharedMesh.RecalculateBounds();
        mainFilter.sharedMesh.RecalculateNormals();
        mainFilter.sharedMesh.RecalculateTangents();
#if UNITY_EDITOR
        Object.DestroyImmediate(plane);
        Object.DestroyImmediate(InvPlane);
#else
        Destroy(plane);
        Destroy(InvPlane);         
#endif


        return gObject;
    }

    void CreateShape(List<Vector3> vectors)
    {/*
        Plane pl = new Plane(vectors[0], vectors[1], vectors[2]);
        Vector3 pos = Vector3.zero;
        foreach (var item in vectors)
        {
            pos += item;
        }
        pos /= vectors.Count;
        transform.position = pos;
        transform.forward = pl.normal;
        for (int i = 0; i < vectors.Count; i++)
        {
            vectors[i] = transform.InverseTransformPoint(vectors[i]);
        }
        plane = new GameObject();
        plane.SetActive(false);
        plane.transform.parent = transform;
        plane.name = "CreatedMesh";
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>();


        InvPlane = new GameObject();
        InvPlane.SetActive(false);
        InvPlane.transform.parent = transform;
        InvPlane.name = "CreatedInversedMesh";
        InvPlane.AddComponent<MeshFilter>();
        InvPlane.AddComponent<MeshRenderer>();


        MeshFilter meshFilter = plane.GetComponent<MeshFilter>();
        MeshFilter invMeshFilter = InvPlane.GetComponent<MeshFilter>();
        Polygon poly = new Polygon();
        List<Vector2> countur = new List<Vector2>();

        foreach (var item in vectors)
        {
            countur.Add(new Vector2(item.x, item.y));
            // Instantiate(dot,transform.TransformPoint(item), new Quaternion(0, 0, 0, 0));
        }
        poly.Add(countur);
        triangleNetMesh = (TriangleNetMesh)poly.Triangulate();


        meshFilter.mesh = triangleNetMesh.GenerateUnityMesh();
        Mesh inversMesh = triangleNetMesh.GenerateUnityMesh();

        List<int> triangles = new List<int>();
        triangles.AddRange(inversMesh.triangles);
        triangles.Reverse();
        inversMesh.SetTriangles(triangles, 0);
        invMeshFilter.mesh = inversMesh;

        List<GameObject> gameObjects = new List<GameObject>();
        gameObjects.Add(plane);
        gameObjects.Add(InvPlane);

        MeshFilter mainFilter = transform.GetComponent<MeshFilter>();
        mainFilter.mesh = new Mesh();
        this.gameObject = CombineObjects(gameObject, gameObjects);
        mainFilter.mesh.RecalculateBounds();
        mainFilter.mesh.RecalculateNormals();
        mainFilter.mesh.RecalculateTangents();
        GetComponent<MeshCollider>().sharedMesh = mainFilter.mesh;
        Destroy(plane);
        Destroy(InvPlane);*/
    }

    GameObject CombineObjects(GameObject mainGameObject, List<GameObject> gameObjects)
    {
        CombineInstance[] combine = new CombineInstance[gameObjects.Count];
        for (int i = 0; i < gameObjects.Count; i++)
        {
            combine[i].mesh = gameObjects[i].GetComponent<MeshFilter>().sharedMesh;
            combine[i].transform = gameObjects[i].GetComponent<MeshFilter>().transform.localToWorldMatrix;
        }
        mainGameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        mainGameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        return mainGameObject;
    }
}
