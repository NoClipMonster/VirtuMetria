using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
public class MeshEditor : MonoBehaviour
{
    #region Переменные
    Mesh oMesh;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
    Dots dots;
    PlaneWithDots planeWithDots;
    bool Space = false;
    bool R = false;
    GameObject Rig;
    Vector3[] defaultVerts;
    #endregion

    #region Публичные переменные

    public float MaxDrawDistance = 7;
    public float MinDrawDistance = 3;
    public Vector3[] Vertices;
    public int[] Triangels;
    public GameObject DotLayoutObject;
    public GameObject TrackingObject;
    public SteamVR_Action_Boolean trigerAction;
    Vector3 controllerPos;
    #endregion
    void Start()
    {
        Rig = GameObject.Find("[CameraRig]");
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
        if (trigerAction.stateDown)
        {
            controllerPos = TrackingObject.transform.position;
        }
        if (planeWithDots != null && trigerAction.state)
        {
            planeWithDots.translate(controllerPos - TrackingObject.transform.position);
            dots.VarVertices = planeWithDots.Vertices;
            UpdateMesh();
            controllerPos = TrackingObject.transform.position;
        }
        if (R)
        {
            dots.VarVertices = defaultVerts;
            UpdateMesh();
        }


        for (int i = 0; i < dots.VarVertices.Length; i++)
        {

            float dist = Vector3.Distance(dots.GetLayoutDot(i).AbsPosition, TrackingObject.transform.position);

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
        public bool trigered;
        public LayOutDot(GameObject LayOutGameObject, Vector3 position, Transform parent)
        {
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
        public bool LayoutDotTrigered(int index)
        {
            
            for (int i = 0; i < Alldots.Count; i++)
                if (Alldots[i].similarDots.Contains(index))
                {
                    return Alldots[i].layOutDot.trigered;
                }
            return false;
        }
        public LayOutDot GetLayoutDot(int index)
        {
            for (int i = 0; i < Alldots.Count; i++)
                if (Alldots[i].similarDots.Contains(index))
                    return Alldots[i].layOutDot;
            return null;
        }
    }

    public class PlaneWithDots
    {
        public Vector3[] Vertices;
        public List<int> dots = new List<int>();
        Vector3 norm; 
        public PlaneWithDots(Vector3[] vertices, Collision collision, Transform transform)
        {
            Vertices = vertices;
            norm = transform.InverseTransformVector(collision.GetContact(0).normal)*-1;
            double bigest = double.MinValue;

            for (int i = 0; i < Vertices.Length; i++)
                bigest = (bigest < DoStuf(Vertices[i])) ? DoStuf(Vertices[i]) : bigest;

            for (int i = 0; i < Vertices.Length; i++)
                if (DoStuf(Vertices[i]) == bigest)
                {
                    dots.Add(i);
                }

            double DoStuf(Vector3 vector)
            {
                Vector3 v = transform.TransformPoint(vector);

                v.x *= norm.x;
                v.y *= norm.y;
                v.z *= norm.z;
                return Math.Round((double)v.x + v.y + v.z, 1);
            }
        }
        public void translate(Vector3 vector)
        {
           
            vector.x *= norm.x;
            vector.y *= norm.y;
            vector.z *= norm.z;
            foreach (int i in dots)
            {
                Vertices[i] += vector * -1f;
            }
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Controller" && planeWithDots != null)
        {
            foreach (var item in planeWithDots.dots)
            {
                
                dots.EditLayouDot(DotLayoutObject.GetComponent<Renderer>().sharedMaterial.color, item);
                Debug.Log(item);
            }

            planeWithDots = null;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Событие");
        if (collision.gameObject.tag == "Controller" && planeWithDots == null)
        {
            planeWithDots = new PlaneWithDots(dots.VarVertices, collision, transform);
            Debug.Log(collision.GetContact(0).normal);
            foreach (var item in planeWithDots.dots)
                dots.EditLayouDot(Color.red, item);
        }
    }

}

