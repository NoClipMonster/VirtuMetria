using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ObjectCreator : MonoBehaviour
{
    public GameObject controller;
    public GameObject dot;
    GameObject plane;
    GameObject InvPlane;
    TriangleNetMesh triangleNetMesh;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshCollider>().convex = true;

        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(3, 1, 3));
        list.Add(new Vector3(0, 1, 0));
        list.Add(new Vector3(0, 4, 0));
        transform.position = (list[2] + list[0]) / 2;

        foreach (var item in list)
        {
            Instantiate(dot, item, Quaternion.identity);
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i] = transform.InverseTransformPoint(list[i]);
        }
        CreateSquare(list);
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
        gameObject.AddComponent<MeshEditor>().DotLayoutObject = dot;
        GetComponent<MeshEditor>().KeyboardDebug = true;
        if (GetComponent<MeshEditor>().KeyboardDebug)
            GetComponent<MeshEditor>().TrackingObject = Camera.main.gameObject;
        else GetComponent<MeshEditor>().TrackingObject = controller;

    }
    private void Update()
    {

    }
    void CreateSquare(List<Vector3> dots)
    {
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

        GetComponent<MeshFilter>().mesh = CombineObjects(gameObject, gameObjects).GetComponent<MeshFilter>().mesh;
        foreach (var item in gameObjects)
        {
            Destroy(item);
        }
    }

    GameObject CreateSqrPlane(Vector3 position, Vector3 normal, float size)
    {
        GameObject gObject = new GameObject();
        gObject.SetActive(false);
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
        plane.SetActive(false);
        plane.transform.parent = gObject.transform;
        plane.name = "CreatedMesh";
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>();


        InvPlane = new GameObject();
        InvPlane.SetActive(false);
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

        MeshFilter mainFilter = gObject.GetComponent<MeshFilter>();
        mainFilter.mesh = new Mesh();
        gObject = CombineObjects(gObject, gameObjects);
        gObject.GetComponent<Renderer>().material = GetComponent<MeshRenderer>().material;
        mainFilter.mesh.RecalculateBounds();
        mainFilter.mesh.RecalculateNormals();
        mainFilter.mesh.RecalculateTangents();

        Destroy(plane);
        Destroy(InvPlane);

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
            combine[i].mesh = gameObjects[i].GetComponent<MeshFilter>().mesh;
            combine[i].transform = gameObjects[i].GetComponent<MeshFilter>().transform.localToWorldMatrix;
        }
        mainGameObject.GetComponent<MeshFilter>().mesh = new Mesh();
        mainGameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        return mainGameObject;
    }
}
