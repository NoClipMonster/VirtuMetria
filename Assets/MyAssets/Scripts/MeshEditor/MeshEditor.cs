using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class MeshEditor : MonoBehaviour
{

    #region Переменные
    Entity entity;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
#pragma warning disable CS0649 // Field 'MeshEditor.defaultVerts' is never assigned to, and will always have its default value null
    List<Vector3> defaultVerts;
#pragma warning restore CS0649 // Field 'MeshEditor.defaultVerts' is never assigned to, and will always have its default value null
    bool[] anyMeshEdit = { false, false };
    bool[] anySizeEdit = { false, false };
    Vector3[] AnyPossition = new Vector3[2];
    float distAcrosContrs;
    #endregion

    #region Публичные переменные
    public float MaxDrawDistance = 7;
    public float MinDrawDistance = 3;
    public GameObject TrackingObject;
    public SteamVR_Action_Boolean meshEdit;
    public SteamVR_Action_Boolean sizeEdit;

    public bool KeyboardDebug = false;
    #endregion

    void Start()
    {
        int gg = 0;
        RaycastHit raycastHit;
        DateTime dateTime = DateTime.Now;
        for (int i = 0; i < 1000; i++)
        {
            if (Physics.Raycast(Vector3.zero, Vector3.up, out raycastHit, 10))
                gg++;
        }
        var dateTime1 = DateTime.Now - dateTime;


        static Vector3 Converter(Vector3 ve)
        {
            //return ve;
            return new Vector3((float)Math.Round(ve.x, 1), (float)Math.Round(ve.y, 1), (float)Math.Round(ve.z, 1));
        }
        entity = new Entity();
        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();

        bool[] visited = new bool[oMeshFilter.mesh.vertices.Length];

        int delKol = 0;
        List<Vector3> vectors = new List<Vector3>(oMeshFilter.mesh.vertices);
        List<int> triangles = new List<int>(oMeshFilter.mesh.triangles);
        DateTime time = DateTime.Now;

        for (int i = 0; i < vectors.Count; i++)
        {
            if (visited[i])
                continue;
            entity.dots.Add(new Entity.Dot(vectors[i]));
            for (int j = i; j < vectors.Count; j++)
            {
                if (Converter(vectors[i]) == Converter(vectors[j]))
                {
                    visited[j] = true;
                    entity.dots[^1].SimilarDots.Add(j);

                    for (int k = 0; k < oMeshFilter.mesh.triangles.Length; k++)
                    {
                        delKol++;
                        if (triangles[k] == j && !entity.dots[^1].triangles.Contains(k))
                            entity.dots[^1].triangles.Add(k / 3);
                    }
                }

            }
        }
        Debug.Log(DateTime.Now - time);
        Debug.Log(delKol);
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            anyMeshEdit[0] = !anyMeshEdit[0];
            AnyPossition[0] = entity.dotsOnPlane[0].hand.transform.position;
            anyMeshEdit[1] = !anyMeshEdit[1];
            AnyPossition[1] = entity.dotsOnPlane[1].hand.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            anySizeEdit[0] = !anySizeEdit[0];
            anySizeEdit[1] = !anySizeEdit[1];
            distAcrosContrs = Vector3.Distance(entity.dotsOnPlane[0].hand.transform.position, entity.dotsOnPlane[1].hand.transform.position);
        }
        if (anySizeEdit[0] && anySizeEdit[1])
        {
            float dist = Vector3.Distance(entity.dotsOnPlane[0].hand.transform.position, entity.dotsOnPlane[1].hand.transform.position);
            entity.TransformSize(dist - distAcrosContrs);
            distAcrosContrs = Vector3.Distance(entity.dotsOnPlane[0].hand.transform.position, entity.dotsOnPlane[1].hand.transform.position);
            transform.position = (entity.dotsOnPlane[0].hand.transform.position + entity.dotsOnPlane[1].hand.transform.position) / 2;
        }

        if (meshEdit != null)
        {
            if (meshEdit.stateDown)
                if (meshEdit.activeDevice == SteamVR_Input_Sources.LeftHand)
                {
                    anyMeshEdit[0] = true;
                    AnyPossition[0] = entity.dotsOnPlane[0].hand.transform.position;
                }
                else
                {
                    anyMeshEdit[1] = true;
                    AnyPossition[1] = entity.dotsOnPlane[1].hand.transform.position;
                }
            if (meshEdit.stateUp)
                if (meshEdit.activeDevice == SteamVR_Input_Sources.LeftHand)
                    anyMeshEdit[0] = false;
                else anyMeshEdit[1] = false;


        }
        if (anyMeshEdit[0])
        {
            entity.dotsOnPlane[0].Translate(entity.dotsOnPlane[0].hand.transform.position - AnyPossition[0]);
            AnyPossition[0] = entity.dotsOnPlane[0].hand.transform.position;
        }

        if (anyMeshEdit[1])
        {
            entity.dotsOnPlane[1].Translate(entity.dotsOnPlane[1].hand.transform.position - AnyPossition[1]);
            AnyPossition[1] = entity.dotsOnPlane[1].hand.transform.position;
        }

        if (anyMeshEdit[0] || anyMeshEdit[1] || (anySizeEdit[0] && anySizeEdit[1]))
            UpdateMesh();

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < defaultVerts.Count; i++)
            {
                entity.dots[i].vector3 = defaultVerts[i];
            }
            UpdateMesh();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            new EntityCreator(gameObject);
        }

    }
    void UpdateMesh()
    {
        Vector3[] vertices = new Vector3[oMeshFilter.mesh.vertices.Length];
        foreach (var dots in entity.dots)
        {
            foreach (var dot in dots.SimilarDots)
            {
                vertices[dot] = dots.vector3;
            }
        }
        oMeshFilter.mesh.vertices = vertices;
        oMeshCollider.sharedMesh = oMeshFilter.sharedMesh;
        oMeshFilter.mesh.RecalculateBounds();
        oMeshFilter.mesh.RecalculateNormals();
        oMeshFilter.mesh.RecalculateTangents();

    }

    /* private void OnTriggerExit(Collider other)
     {

          SteamVR_Behaviour_Pose SBP = other.GetComponent<SteamVR_Behaviour_Pose>();
          if (SBP.inputSource == entity.dotsOnPlane[0].side)
              entity.dotsOnPlane[0].Excuse();
          else entity.dotsOnPlane[1].Excuse();

     }*/
    private void OnTriggerStay(Collider other)
    {
        Plane pl = new Plane(other.gameObject.transform.forward, other.gameObject.transform.position);
       
        List<Vector3> v = new List<Vector3>();
        List<int> side = new List<int>();
        List<int> otherSide = new List<int>();
        DateTime dt = DateTime.Now;
        for (int i = 0; i < entity.dots.Count; i++)
        {
            Vector3 pos = transform.TransformPoint(entity.dots[i].vector3);
            //Проверка, что хотя бы один сосед лежит на другой стороне
            if (pl.GetSide(pos))
                side.Add(i);
            else
                otherSide.Add(i);
        }
        Debug.Log("1 :: " + (DateTime.Now - dt));
        dt = DateTime.Now;
        if (side.Count <= otherSide.Count)
            DoStuf(side, true);
        else DoStuf(otherSide, false);

        void DoStuf(List<int> indexes, bool side)
        {
            int count = 0;
            foreach (int i in indexes)
            {
                Vector3 pos = transform.TransformPoint(entity.dots[i].vector3);

                foreach (var item2 in entity.dots[i].triangles)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        
                        Vector3 p = transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[item2 * 3 + j]]);
                        if (pl.GetSide(p) == side)
                            continue;

                        Debug.DrawLine(p, pos, Color.blue, 0f, false);
                      
                        RaycastHit hit;
                        if (Physics.Raycast(pos, (p - pos).normalized, out hit, Vector3.Distance(pos, p), 1 << 7))
                        {
                          
                            Vector3 buf = new Vector3((float)Math.Round(hit.point.x, 5), (float)Math.Round(hit.point.y, 5), (float)Math.Round(hit.point.z, 5));
                            if (!v.Contains(buf))
                                v.Add(buf);
                        }
                    }
                }

            }
            Debug.Log(count);
            
            Debug.Log("2 :: " + (DateTime.Now - dt));
            dt = DateTime.Now;

            List<float> fl = new List<float>();
            Vector3 avgVect = Vector3.zero;
            foreach (var item in v)
                avgVect += item;
            avgVect /= v.Count;

            for (int i = 0; i < v.Count; i++)
            {
                fl.Add(Vector3.SignedAngle(other.transform.up, v[i] - avgVect, other.transform.forward));
            }
            for (int i = 1; i < fl.Count; i++)
            {
                if (fl[i] < fl[i - 1])
                {
                    (fl[i], fl[i - 1]) = (fl[i - 1], fl[i]);
                    (v[i], v[i - 1]) = (v[i - 1], v[i]);
                    if (i > 1)
                        i -= 2;
                }
            }
            bool btv(Vector3 first, Vector3 second, Vector3 third)
            {
                if (Math.Round((Vector3.Distance(first, second) + Vector3.Distance(second, third)), 5) == Math.Round(Vector3.Distance(first, third), 5))
                    return true;
                return false;
            }
            Debug.Log("3 :: " + (DateTime.Now - dt));
            dt = DateTime.Now;
            if (!(v == null || v.Count < 3))
            {
                for (int i = 0; i < v.Count; i++)
                {
                    if (i == 0)
                    {
                        if (btv(v[^1], v[0], v[1]))
                        {
                            fl.RemoveAt(0);
                            v.RemoveAt(0);
                            i--;
                        }
                    }
                    else if (i == v.Count - 1)
                    {
                        if (btv(v[^2], v[^1], v[0]))
                        {
                            fl.RemoveAt(i);
                            v.RemoveAt(i);
                            i--;
                        }
                    }
                    else if (btv(v[i - 1], v[i], v[i + 1]))
                    {
                        fl.RemoveAt(i);
                        v.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < v.Count - 1; i++)
                {
                    Debug.DrawLine(v[i], v[i + 1], Color.green, 0, false);
                }
                Debug.DrawLine(v[^1], v[0], Color.green, 0, false);

                Debug.Log("4 :: " + (DateTime.Now - dt));
                dt = DateTime.Now;
                Debug.Break();
            }
        }


    }
    /* private void OnTriggerEnter(Collider other)
     {

         //   int layerMask = 1 << 8;

         // This would cast rays only against colliders in layer 8.
         // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

         //  layerMask = ~layerMask;

         RaycastHit hit;
         if (Physics.Raycast(other.transform.position, transform.position - other.transform.position, out hit, 10000))
         {
             if (other.GetComponent<SteamVR_Behaviour_Pose>().inputSource == SteamVR_Input_Sources.LeftHand)
                 entity.dotsOnPlane[0] = new Entity.DotsOnPlane(entity.dots, hit, transform, other);
             else entity.dotsOnPlane[1] = new Entity.DotsOnPlane(entity.dots, hit, transform, other);
         }


     }*/
}


