using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeshEditor : MonoBehaviour
{
    Mesh oMesh;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
    int[] triangles;

    List<List<int>> SimilarIndexes = new List<List<int>>();
    public Vector3[] Vertices;
    public Dots dots = new Dots();
    public class Dots
    {
        public List<Dot> dots = new List<Dot>();
        public class Dot
        {
            public Vector3 vector3;
            public int[] similarDots;
        }
        public Dot GetDots(int index)
        {
            return dots[index];
        }
        public Dot GetDots(Vector3 vector3)
        {
            foreach (var item in dots)
            {
                if (item.vector3 == vector3)
                    return item;
                
            }
            return null;
        }
    }
  
   
    void FillDot()
    {
        for (int i = 0; i < SimilarIndexes.Count; i++)
        {
            dots.dots.Add(new Dots.Dot { vector3 = Vertices[SimilarIndexes[i][0]], similarDots = SimilarIndexes[i].ToArray() });
        }
    }
    void ShowDot()
    {
        foreach (var dot in dots.dots)
        {
            Debug.Log(getVector(dot.vector3)+ "  :::Точки с данным вектором: "+ getDots(dot.similarDots));
        }
        string getVector(Vector3 v)
        {
            string str = v.x + " " + v.y+" " + v.z;
         
            return str;
        }
        string getDots(int[] dots)
        {
            string str = "";
            foreach (var item in dots)
            {
                str += item + " ";
            }
            return str;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bool[] visited;
        List<Vector3> buf = new List<Vector3>();

        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();

        oMesh = oMeshFilter.sharedMesh; //1

        Vertices = oMesh.vertices; //4
        triangles = oMesh.triangles;
        Debug.Log("Init & Cloned");

        for (int i = 0; i < Vertices.Length; i++)
        {
            buf.Add(Vertices[i]);
        }

        visited = new bool[Vertices.Length];

        for (int i = 0; i < buf.Count; i++)
        {
            if (visited[i])
            {
                continue;
            }

            SimilarIndexes.Add(new List<int>());
            for (int j = i; j < buf.Count; j++)
            {
                if (buf[i] == buf[j])
                {
                    SimilarIndexes[i].Add(j);
                    visited[j] = true;
                }
            }
        }
        int h = 0;
        Vertices = new Vector3[SimilarIndexes.Count];
        foreach (var item in SimilarIndexes)
        {

            Vertices[h] = oMesh.vertices[item[0]];
            h++;
            string str = "";
            foreach (var item2 in item)
            {
                str += item2 + " ";
            }
            Debug.Log(str);

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        { FillDot();
            ShowDot();
        }
            if (Keyboard.current.spaceKey.isPressed)
        {
            Vector3[] vectors = oMeshFilter.mesh.vertices;

            foreach (var item in SimilarIndexes)
            {
                if (vectors[item[0]].y < 0 && vectors[item[0]].x < 0)
                {
                    foreach (var index in item)
                    {
                        vectors[index] -= Vector3.up / 100;
                    }

                }
            }

            oMeshFilter.mesh.vertices = vectors;
            oMeshFilter.mesh.RecalculateNormals();
            oMeshFilter.mesh.RecalculateBounds();
            oMeshFilter.mesh.RecalculateTangents();
            oMeshCollider.sharedMesh = oMeshFilter.mesh;
            UpdateVerts();
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            oMeshFilter.mesh = oMesh;
            oMeshFilter.mesh.RecalculateNormals();
            oMeshFilter.mesh.RecalculateBounds();
            oMeshFilter.mesh.RecalculateTangents();
            oMeshCollider.sharedMesh = oMeshFilter.mesh;
            UpdateVerts();
        }
        void UpdateVerts()
        {
            foreach (var item in SimilarIndexes)
            {
                Vertices[item[0]] = oMeshFilter.mesh.vertices[item[0]];
            }
        }
    }
}
