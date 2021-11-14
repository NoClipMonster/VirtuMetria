using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class MeshEditor : MonoBehaviour
{

    #region Переменные
    Entity entity;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
    List<Vector3> defaultVerts;
    Vector3 keyAxis;
    bool[] anyMeshEdit = { false, false };
    bool[] anySizeEdit = { false, false };
    Vector3[] AnyPossition = new Vector3[2];
    float distAcrosContrs;
    #endregion

    #region Публичные переменные
    public float MaxDrawDistance = 7;
    public float MinDrawDistance = 3;
    public GameObject DotLayoutObject;
    public GameObject TrackingObject;
    public SteamVR_Action_Boolean meshEdit;
    public SteamVR_Action_Boolean sizeEdit;

    public bool KeyboardDebug = false;
    #endregion

    void Start()
    {

        entity = new Entity();
        oMeshFilter = GetComponent<MeshFilter>();
        oMeshCollider = GetComponent<MeshCollider>();

        bool[] visited = new bool[oMeshFilter.mesh.vertices.Length];

        for (int i = 0; i < oMeshFilter.mesh.vertices.Length; i++)
        {
            if (visited[i])
            {
                continue;
            }
            entity.dots.Add(new Entity.Dot(oMeshFilter.mesh.vertices[i], new List<int>(), new List<Vector3>(), Instantiate(DotLayoutObject, transform.TransformPoint(oMeshFilter.mesh.vertices[i]), new Quaternion(0, 0, 0, 0), transform)));
            for (int j = 0; j < oMeshFilter.mesh.vertices.Length; j++)
            {
                if (oMeshFilter.mesh.vertices[i] == oMeshFilter.mesh.vertices[j])
                {
                    entity.dots[entity.dots.Count - 1].SimilarDots.Add(j);
                    entity.dots[entity.dots.Count - 1].Norms.Add(oMeshFilter.mesh.normals[j]);
                    visited[j] = true;
                }

            }
        }

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
            float v =Vector3.Distance( entity.dotsOnPlane[0].hand.transform.position, entity.dotsOnPlane[1].hand.transform.position);
            entity.TransformSize(v-distAcrosContrs);
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

        if (anyMeshEdit[0] || anyMeshEdit[1]||(anySizeEdit[0] && anySizeEdit[1]))
            UpdateMesh();

        for (int i = 0; i < entity.dots.Count; i++)
        {
            float dist = Vector3.Distance(transform.TransformPoint(entity.dots[i].vector3), TrackingObject.transform.position);
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
            Color color = entity.dots[i].LayOutDot.GetComponent<Renderer>().material.color;
            color.a = alp(MinDrawDistance, MaxDrawDistance, dist);
            entity.dots[i].LayOutDot.GetComponent<Renderer>().material.color = color;
        }
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

    private void OnTriggerExit(Collider other)
    {
        SteamVR_Behaviour_Pose SBP = other.GetComponent<SteamVR_Behaviour_Pose>();
        if (SBP.inputSource == entity.dotsOnPlane[0].side)
            entity.dotsOnPlane[0].Excuse();
        else entity.dotsOnPlane[1].Excuse();

    }
    private void OnTriggerEnter(Collider other)
    {
        //   int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

        //  layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(other.transform.position, transform.position - other.transform.position, out hit, 10000))
            if (other.GetComponent<SteamVR_Behaviour_Pose>().inputSource == SteamVR_Input_Sources.LeftHand)
                entity.dotsOnPlane[0] = new Entity.DotsOnPlane(entity.dots, hit, transform, other);
            else entity.dotsOnPlane[1] = new Entity.DotsOnPlane(entity.dots, hit, transform, other);

    }

}


