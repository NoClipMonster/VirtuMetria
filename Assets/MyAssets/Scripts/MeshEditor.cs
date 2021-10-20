using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MeshEditor : MonoBehaviour
{
    Mesh oMesh;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;

    public Vector3[] Vertices;
    public Dots dots;

    void Start()
    {
        dots = new Dots();

        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();
        oMesh = oMeshFilter.sharedMesh;

        dots.AllVectors = oMesh.vertices;
        Vertices = dots.VarVectors;
    }
    
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            foreach (var item in dots.AllVectors)
            {
                Debug.Log(getVector(item));
            }
            string getVector(Vector3 v)
            {
                string str = v.x + " " + v.y + " " + v.z;

                return str;
            }
            foreach (var item in dots.Alldots)
            {
                if (item.Vector3.y > 0 && item.Vector3.x > 0)
                {
                    dots.Alldots[item.similarDots[0]].Vector3 += Vector3.one / 100;
                }
            }
            oMeshFilter.mesh.vertices = dots.AllVectors;
            oMeshFilter.mesh.RecalculateNormals();
            oMeshFilter.mesh.RecalculateBounds();
            oMeshFilter.mesh.RecalculateTangents();
            oMeshCollider.sharedMesh = oMeshFilter.mesh;
            Vertices = dots.VarVectors;
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            dots.AllVectors = oMesh.vertices;
            oMeshFilter.mesh.vertices = oMesh.vertices;
            oMeshFilter.mesh.RecalculateNormals();
            oMeshFilter.mesh.RecalculateBounds();
            oMeshFilter.mesh.RecalculateTangents();
            oMeshCollider.sharedMesh = oMeshFilter.mesh;
            Vertices = dots.VarVectors;
        }

    }

    public class Dots
    {
        public List<Dot> Alldots = new List<Dot>();
        public int Count { get {return Alldots.Count; } }
       public class Dot
        {
            public Vector3 Vector3;
            public List<int> similarDots = new List<int>();
        }

        public Dot GetDots(int index)
        {
            return Alldots[index];
        }

        public Dot GetDots(Vector3 vector3)
        {
            foreach (var item in Alldots)
            {
                if (item.Vector3 == vector3)
                    return item;
            }
            return null;
        }
        public Vector3[] AllVectors
        {
            get
            {
                int kol = 0;
                foreach (var dot in Alldots)
                    foreach (var i in dot.similarDots)
                        kol++;
                Vector3[] vectors = new Vector3[kol];
                foreach (var dot in Alldots)
                    foreach (var i in dot.similarDots)
                        vectors[i] = dot.Vector3;
                return vectors;
            }
            set
            {
                Alldots.Clear();
                bool[] visited = new bool[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    if (visited[i])
                    {
                        continue;
                    }
                    Alldots.Add(new Dots.Dot() { Vector3 = value[i], similarDots = new List<int>() });
                    for (int j = 0; j < value.Length; j++)
                    {
                        if (value[i] == value[j])
                        {
                            Alldots[i].similarDots.Add(j);
                            visited[j] = true;
                        }
                    }
                }
            }

        }
        public Vector3[] VarVectors
        {
            get
            {
                List<Vector3> vectors = new List<Vector3>();
                foreach (var item in Alldots)
                {
                    vectors.Add(item.Vector3);
                }
                return vectors.ToArray();
            }

        }
    }
 
}
