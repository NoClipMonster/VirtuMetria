using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MeshEditor : MonoBehaviour
{
    #region Перменные
    Mesh oMesh;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
    Dots dots;

    bool Space = false;
    bool R = false;

    Vector3[] defaultVerts;
    #endregion

    #region Публичные переменные
    public float MaxDrawDistance = 7;
    public float MinDrawDistance = 3;
    public Vector3[] Vertices;
    public int[] Triangels;
    public GameObject DotLayoutObject;
    public GameObject TrackingObject;
    #endregion



    void Start()
    {
        dots = new Dots();

        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();
        oMesh = oMeshFilter.sharedMesh;

        dots.AllVertices = oMesh.vertices;
        Vertices = dots.VarVertices;
        defaultVerts = Vertices;
        dots.parent = GetComponent<Transform>();
        dots.standartLayoutDot = DotLayoutObject;
        dots.InitializeLODs();
    }
    private void FixedUpdate()
    {
        if (Space)
        {
            Vector3[] ve = dots.VarVertices;
            for (int i = 0; i < ve.Length; i++)
                ve[i] = (ve[i].y > 0 && ve[i].x > 0) ? ve[i] + Vector3.one / 100 : ve[i];

            dots.VarVertices = ve;
            UpdateMesh();
        }
        if (R)
        {
            dots.VarVertices = defaultVerts;
            UpdateMesh();
        }


        for (int i = 0; i < dots.VarVertices.Length; i++)
        {

            float dist = Vector3.Distance(dots.GetLayoutDot(i).AbsPosition + transform.position, TrackingObject.transform.position);

            float alp(float min, float max, float val)
            {
                if (val < min)
                    return 1;
                if (val > max)
                    return 0;
                else
                {
                    return 1 - ((val - min) / (max - min));
                }
            }

            Color color = dots.GetLayoutDot(i).Color;
            color.a = alp(MinDrawDistance, MaxDrawDistance, dist); ;
            dots.EditLayouDot(dots.VarVertices[i], color, i);
        }
    }
    void Update()
    {
        //Input.GetKey(KeyCode.Space)
        Space = Keyboard.current.spaceKey.isPressed;

        //Input.GetKeyDown(KeyCode.R)
        R = Keyboard.current.rKey.wasPressedThisFrame;


        /*  if (Input.GetKeyDown(KeyCode.N))
          // if (Keyboard.current.nKey.wasPressedThisFrame)
          {
              Vector3[] ve = dots.VarVertices;
              dots.VarVertices = ve;
              UpdateMesh();
          }*/
    }
    void UpdateMesh()
    {

        oMeshFilter.mesh.vertices = dots.AllVertices;
        oMeshFilter.mesh.RecalculateBounds();
        oMeshFilter.mesh.RecalculateNormals();
        oMeshFilter.mesh.RecalculateTangents();
        oMeshCollider.sharedMesh = oMeshFilter.mesh;
        Vertices = dots.VarVertices;
    }
    public class LayOutDot
    {
        GameObject gameObject;
        Transform parent;
        public LayOutDot(GameObject LayOutGameObject, Vector3 position, Transform parent)
        {
            this.parent = parent;
            gameObject = Instantiate(LayOutGameObject, position + parent.position, new Quaternion(0, 0, 0, 0), parent);
        }
        public Vector3 LocalPosition
        {
            get { return gameObject.transform.localPosition; }
            set { gameObject.transform.localPosition = value; }
        }
        public Vector3 AbsPosition
        {
            get { return gameObject.transform.position; }
            set { gameObject.transform.position = value; }
        }
        public Color Color
        {
            get { return gameObject.GetComponent<Renderer>().material.color; }
            set { gameObject.GetComponent<Renderer>().material.color = value; }
        }
    }
    public class Dots
    {
        List<Dot> Alldots = new List<Dot>();
        public GameObject standartLayoutDot;
        public Transform parent;
        public class Dot
        {
            public Vector3 Vector3;
            public List<int> similarDots = new List<int>();
            public LayOutDot layOutDot;
        }
        public void InitializeLODs()
        {
            for (int i = 0; i < Alldots.Count; i++)
            {
                Alldots[i].layOutDot = new LayOutDot(standartLayoutDot, Alldots[i].Vector3, parent);
            }
        }
        public Dot GetDot(int index)
        {
            foreach (var item in Alldots)
            {
                if (item.similarDots.Contains(index))
                    return item;
            }
            return null;
        }
        public Dot GetDot(Vector3 vector3)
        {
            foreach (var item in Alldots)
            {
                if (item.Vector3 == vector3)
                    return item;
            }
            return null;
        }

        public Vector3[] AllVertices
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
        public Vector3[] VarVertices
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
            set
            {
                int ind = 0;
                foreach (var item in value)
                {
                    Alldots[ind].Vector3 = item;
                    ind++;
                }
            }

        }
        public void EditLayouDot(Color color, int index)
        {
            for (int i = 0; i < Alldots.Count; i++)
                if (Alldots[i].similarDots.Contains(index))
                    Alldots[i].layOutDot.Color = color;
        }
        public void EditLayouDot(Vector3 pos, int index)
        {
            for (int i = 0; i < Alldots.Count; i++)
                if (Alldots[i].similarDots.Contains(index))
                    Alldots[i].layOutDot.LocalPosition = pos;
        }
        public void EditLayouDot(Vector3 pos, Color color, int index)
        {
            for (int i = 0; i < Alldots.Count; i++)
                if (Alldots[i].similarDots.Contains(index))
                {
                    Alldots[i].layOutDot.LocalPosition = pos;
                    Alldots[i].layOutDot.Color = color;
                }

        }
        public LayOutDot GetLayoutDot(int index)
        {
            for (int i = 0; i < Alldots.Count; i++)
                if (Alldots[i].similarDots.Contains(index))
                    return Alldots[i].layOutDot;
            return null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hit.gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
    private void OnCollisionExit(Collision collision)
    {
        for (int i = 0; i < dots.VarVertices.Length; i++)
        {
            dots.EditLayouDot(Color.black, i);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Cube")
            return;
        ContactPoint contactPoint = collision.GetContact(0);
        Debug.Log(contactPoint.normal * -1f);
        Vector3[] vector3s = dots.VarVertices;
        double bigest = 0;
        Debug.Log(contactPoint.point - gameObject.transform.position);
        for (int i = 0; i < vector3s.Length; i++)
        {
            double buf = SV(UmV(vector3s[i], contactPoint.normal*-1f));
            if (bigest < buf)
                bigest = buf;
        }
        for (int i = 0; i < vector3s.Length; i++)
        {
            if (SV(UmV(vector3s[i], contactPoint.normal*-1f)) == bigest)
                dots.EditLayouDot(Color.red, i);
        }

        double SV(Vector3 vector3)
        {
            return Math.Round((double)vector3.x + vector3.y + vector3.z, 3);
        }
        Vector3 UmV(Vector3 vector, Vector3 vector1)
        {
            Vector3 v = vector;
            v.x *= vector1.x;
            v.y *= vector1.y;
            v.z *= vector1.z;
            return v;
        }

    }
}
