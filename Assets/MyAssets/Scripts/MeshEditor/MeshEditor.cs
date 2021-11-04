using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class MeshEditor : MonoBehaviour
{

    #region Переменные
    Entity entity;
    MeshFilter oMeshFilter;
    MeshCollider oMeshCollider;
    GameObject Rig;
    List<Vector3> defaultVerts;
    Vector3 controllerPos;
    Vector3 keyAxis;
    
    #endregion

    #region Публичные переменные
    public float MaxDrawDistance = 7;
    public float MinDrawDistance = 3;
    public GameObject DotLayoutObject;
    public GameObject TrackingObject;
   // public SteamVR_Action_Boolean trigerAction;
    public Vector3 normal;
    public bool KeyboardDebug = false;
    #endregion

    void Start()
    {
        entity = new Entity();
        Rig = GameObject.Find("[CameraRig]");
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
                    entity.dots[i].SimilarDots.Add(j);
                    entity.dots[i].Norms.Add(oMeshFilter.mesh.normals[j]);
                    visited[j] = true;
                }

            }
        }

    }

    void FixedUpdate()
    {
      /*  if (trigerAction.stateDown)
        {
            controllerPos = TrackingObject.transform.position;
        }
        if (entity.dotsOnPlane != null && trigerAction.state)
        {
            entity.dotsOnPlane.Translate(controllerPos - TrackingObject.transform.position);
            UpdateMesh();
            controllerPos = TrackingObject.transform.position;
        }*/
       



    }

    void Update()
    {
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
        if (KeyboardDebug && entity.dotsOnPlane.HasPlane == true)
        {
            keyAxis = new Vector3 { x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical"), z = 0 };
            if (keyAxis.magnitude != 0)
            {
                entity.dotsOnPlane.Translate(keyAxis / 100);

                UpdateMesh();
            }
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Controller" && entity.dotsOnPlane.HasPlane == true)
            entity.dotsOnPlane.Excuse();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Событие");
        if (collision.gameObject.tag == "Controller" && entity.dotsOnPlane.HasPlane == false)
            entity.dotsOnPlane = new Entity.DotsOnPlane(entity.dots, collision.GetContact(0).normal * -1f, transform);
    }
    
}


