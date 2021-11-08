using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public GameObject dot;
    public TriangleNetMesh triangleNetMesh;
    // Start is called before the first frame update
    void Start()
    {

        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(4, 2, 3));
        list.Add(new Vector3(2, 3, 1));
        list.Add(new Vector3(3, 1, 4));

        GameObject go = createPlane(list);

    }

    GameObject createPlane(List<Vector3> vectors)
    {
        GameObject plane = new GameObject();
        Plane pl = new Plane(vectors[0], vectors[1], vectors[2]);
        plane.transform.position = vectors[0];
        //plane.transform.LookAt( pl.normal);
        plane.name = "CreatedPlane";
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>().material = plane.GetComponentInParent<MeshRenderer>().material;

        Polygon poly = new Polygon();
        List<Vector2> countur = new List<Vector2>();

        foreach (var item in vectors)
        {
            Vector3 v = plane.transform.InverseTransformPoint(item);
            countur.Add(new Vector2(v.x, v.z));
            Instantiate(dot, item, new Quaternion(0, 0, 0, 0));
        }

        poly.Add(countur);
        triangleNetMesh = (TriangleNetMesh)poly.Triangulate();
        MeshFilter f = plane.GetComponent<MeshFilter>();
       
        f.mesh = triangleNetMesh.GenerateUnityMesh();
      
        /* 
         f.mesh.RecalculateNormals();
         f.mesh.RecalculateBounds();
         f.mesh.RecalculateTangents();
        */

        f.mesh.RecalculateNormals();
        //Vector3[] n2 = f.mesh.normals;


        return plane;
    }
    private void OnDrawGizmos()
    {
        triangleNetMesh.DrawGizmos();
    }
}
