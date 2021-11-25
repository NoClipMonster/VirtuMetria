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
            entity.dots.Add(new Entity.Dot(oMeshFilter.mesh.vertices[i], Instantiate(DotLayoutObject, transform.TransformPoint(oMeshFilter.mesh.vertices[i]), new Quaternion(0, 0, 0, 0), transform)));
            for (int j = 0; j < oMeshFilter.mesh.vertices.Length; j++)
            {
                if (oMeshFilter.mesh.vertices[i] == oMeshFilter.mesh.vertices[j])
                {
                    entity.dots[entity.dots.Count - 1].SimilarDots.Add(j);
                    entity.dots[entity.dots.Count - 1].Norms.Add(oMeshFilter.mesh.normals[j]);
                    visited[j] = true;
                    for (int k = 0; k < oMeshFilter.mesh.triangles.Length; k++)
                    {
                        if (oMeshFilter.mesh.triangles[k] == j && !entity.dots[entity.dots.Count - 1].triangles.Contains(k))
                            entity.dots[entity.dots.Count - 1].triangles.Add(k / 3);
                    }
                }

            }
        }

    }

    Vector3[] v;
    void Update()
    {

        GameObject g = GameObject.Find("Section Plane");
        RaycastHit hit1;
        int layerMask = 1 << 6;

        // This would cast rays only against colliders in layer 6.
        // But instead we want to collide against everything except layer 6. The ~ operator does this, it inverts a bitmask.

        // layerMask = ~layerMask;
        if (Physics.Raycast(g.transform.position, transform.position - g.transform.position, out hit1, Vector3.Distance(transform.position, g.transform.position), layerMask))
        {
            Debug.DrawLine(g.transform.position, hit1.point, Color.blue);
            v = new Vector3[] {
                GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[hit1.triangleIndex*3]],
                GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[hit1.triangleIndex*3+1]],
                GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[hit1.triangleIndex*3+2]]
            };

        }
        /* if (v != null)
         {
             for (int i = 0; i < v.Length - 1; i++)
             {
                 Debug.DrawLine(transform.TransformPoint(v[i]), transform.TransformPoint(v[i + 1]), Color.red);
             }
             Debug.DrawLine(transform.TransformPoint(v[0]), transform.TransformPoint(v[v.Length - 1]), Color.red);

         }*/

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
        if (other.gameObject.name == "Section Plane")
        {
            gameObject.GetComponent<Renderer>().material = (Material)Resources.Load("BlackMaterial");
            foreach (var item in entity.dots)
                item.LayOutDot.GetComponent<Renderer>().material.color = Color.blue;
            return;
        }
        SteamVR_Behaviour_Pose SBP = other.GetComponent<SteamVR_Behaviour_Pose>();
        if (SBP.inputSource == entity.dotsOnPlane[0].side)
            entity.dotsOnPlane[0].Excuse();
        else entity.dotsOnPlane[1].Excuse();

    }
    private void OnTriggerStay(Collider other)
    {
        Plane pl = new Plane(other.gameObject.transform.forward, other.gameObject.transform.position);

        List<Vector3> v = new List<Vector3>();
        foreach (var item in entity.dots)
        {
            if (pl.GetSide(item.LayOutDot.transform.position))
            {

                foreach (var item2 in item.triangles)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        RaycastHit hit;
                        int layerMask = 1 << 7;
                        Vector3 p = transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[item2 * 3 + i]]);
                        if (Physics.Raycast(item.LayOutDot.transform.position, (p - item.LayOutDot.transform.position).normalized, out hit, Vector3.Distance(item.LayOutDot.transform.position, p), layerMask))
                        {
                            //Instantiate(DotLayoutObject, hit.point, Quaternion.identity);
                           // Debug.DrawLine(item.LayOutDot.transform.position, hit.point, Color.green);

                            if (!v.Contains(hit.point))
                                    v.Add(hit.point);
                        }
                    }
                }
                item.LayOutDot.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                item.LayOutDot.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
        List<float> fl = new List<float>();
        Vector3 avgVect = Vector3.zero;
        foreach (var item in v)
            avgVect += item;
        avgVect /= v.Count;
        avgVect = other.transform.InverseTransformPoint(avgVect);


        foreach (var item in v)
        {
            Vector3 buf = other.transform.InverseTransformPoint(item);
            fl.Add(Vector3.SignedAngle(other.transform.InverseTransformPoint(v[0]) - avgVect, buf - avgVect, other.transform.forward));
        }
        for (int i = 0; i < fl.Count; i++)
        {
            for (int j = i + 1; j < fl.Count; j++)
            {
                if (fl[i] > fl[j])
                {
                    float buf = fl[i];
                    fl[i] = fl[j];
                    fl[j] = buf;
                    Vector3 buff = v[i];
                    v[i] = v[j];
                    v[j] = buff;
                }
            }
        }
        if (v != null && v.Count != 0)
        {
            for (int i = 0; i < v.Count - 1; i++)
            {
                    Debug.DrawLine(v[i], v[i + 1], Color.green);
            }
            Debug.DrawLine(v[v.Count - 1], v[0], Color.green);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Section Plane")
        {

            gameObject.GetComponent<Renderer>().material = (Material)Resources.Load("BlackTranspMaterial");
            return;
        }
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


    }
}


