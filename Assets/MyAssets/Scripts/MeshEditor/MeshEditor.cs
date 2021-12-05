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
    static Vector3 Converter(Vector3 ve)
    {
        return new Vector3((float)Math.Round(ve.x, 1), (float)Math.Round(ve.y, 1), (float)Math.Round(ve.z, 1));
    }


    void Start()
    {

        entity = new Entity();
        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();
        bool[] visited = new bool[oMeshFilter.mesh.vertices.Length];

        int delKol = 0;
        List<Vector3> vectors = new List<Vector3>(oMeshFilter.mesh.vertices);
        List<int> triangles = new List<int>(oMeshFilter.mesh.triangles);

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
                        //TODO: Перевести  entity.dots[^1].triangles из mesh.triangles в в dots[]
                        delKol++;
                        if (triangles[k] == j && !entity.dots[^1].triangles.Contains(k))
                            entity.dots[^1].triangles.Add(k - (k % 3));
                        for (int l = 0; l < 3; l++)
                        {
                            float dist = Math.Abs(Vector3.Distance(oMeshFilter.mesh.vertices[oMeshFilter.mesh.triangles[(k - (k % 3)) + l]], entity.dots[^1].vector3));

                            if (entity.BigestEdge < dist)
                                entity.BigestEdge = dist;
                        }

                    }
                }

            }
        }
    }

    /*
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

        }*/
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

    private void OnTriggerEnter(Collider other)
    {
        RaycastHit hit;
        if (Physics.Raycast(other.transform.position,other.transform.position - transform.position, out hit))
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            entity.dotsOnPlane[0] = new Entity.DotsOnPlane(entity.dots, hit, transform);
            List<Vector3> dots = new List<Vector3>();
            dots.Capacity = entity.dotsOnPlane[0].indexesOfDots.Count;
            foreach (var dot in entity.dotsOnPlane[0].indexesOfDots)
                dots.Add(entity.dots[dot].vector3);
            go.AddComponent<LineRenderer>().SetPositions(dots.ToArray());
        }
        //   int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

        //  layerMask = ~layerMask;

        /* RaycastHit hit;
         if (Physics.Raycast(other.transform.position, transform.position - other.transform.position, out hit, 10000))
         {
             if (other.GetComponent<SteamVR_Behaviour_Pose>().inputSource == SteamVR_Input_Sources.LeftHand)
                 entity.dotsOnPlane[0] = new Entity.DotsOnPlane(entity.dots, hit, transform, other);
             else entity.dotsOnPlane[1] = new Entity.DotsOnPlane(entity.dots, hit, transform, other);
         }
        */

    }
}


