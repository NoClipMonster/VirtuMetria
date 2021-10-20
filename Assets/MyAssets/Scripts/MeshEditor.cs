using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MeshEditor : MonoBehaviour
{
    Mesh oMesh;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
    int[] triangles;
    public GameObject layoutDot;

    List<List<int>> SimilarIndexes = new List<List<int>>();
    public Vector3[] Vertices;
    public Dots dots;
    public class Dots
    {
        public List<Dot> dots = new List<Dot>();
        public GameObject LayoutDot;
        public class Dot : Dots
        {
            Vector3 vector3;
            public Vector3 Vector3
            {
                get { return vector3; }
                set
                {
                    vector3 = value;
                }
            }
            public List<int> similarDots = new List<int>();
            public bool showDot = false;
            public new GameObject LayoutDot;
        }

        public void ShowDot(int index, MeshFilter mf)
        {
            Dot dot = dots[index];
            dot.showDot = true;
            dot.LayoutDot = LayoutDot;
            Instantiate(dot.LayoutDot, mf.gameObject.transform.position + dot.Vector3, new Quaternion(0, 0, 0, 0));
        }
        public Dot GetDots(int index)
        {
            return dots[index];
        }
        public Dot GetDots(Vector3 vector3)
        {
            foreach (var item in dots)
            {
                if (item.Vector3 == vector3)
                    return item;
            }
            return null;
        }

    }

    void FillDot()
    {
        for (int i = 0; i < SimilarIndexes.Count; i++)
        {
            dots.dots.Add(new Dots.Dot { Vector3 = Vertices[SimilarIndexes[i][0]], similarDots = SimilarIndexes[i] });
        }
    }
    void ShowDot()
    {
        foreach (var dot in dots.dots)
        {
            Debug.Log(getVector(dot.Vector3) + "  :::Точки с данным вектором: " + getDots(dot.similarDots));
        }
        string getVector(Vector3 v)
        {
            string str = v.x + " " + v.y + " " + v.z;

            return str;
        }
        string getDots(List<int> dots)
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

        dots = new Dots();
        dots.LayoutDot = layoutDot;

        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();
        oMesh = oMeshFilter.sharedMesh;

        Vertices = oMesh.vertices;

        bool[] visited = new bool[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            if (visited[i])
            {
                continue;
            }
            dots.dots.Add(new Dots.Dot() { Vector3 = Vertices[i], similarDots = new List<int>() });
            for (int j = 0; j < Vertices.Length; j++)
            {
                if (Vertices[i] == Vertices[j])
                {
                    dots.dots[i].similarDots.Add(j);
                    visited[j] = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            FillDot();
            ShowDot();
            for (int i = 0; i < dots.dots.Count; i++)
            {
                dots.ShowDot(i, oMeshFilter);
            }
        }
        if (Keyboard.current.spaceKey.isPressed)
        {
            Vector3[] vectors = oMeshFilter.mesh.vertices;

            foreach (var item in dots.dots)
            {
                if (item.Vector3.y < 0 && item.Vector3.x < 0)
                {
                    item.Vector3 += Vector3.one / 100;
                    foreach (var item1 in item.similarDots)
                    {
                        oMeshFilter.mesh.vertices[item1] = item.Vector3;
                    }
                    
                }
            }
            
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
