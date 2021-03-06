using UnityEngine;
public class EntityCreator : MonoBehaviour
{

    GameObject myGameObject;
    public EntityCreator(GameObject parent)
    {
        myGameObject = new GameObject();

        myGameObject.name = "Generated:";
    
        myGameObject.transform.position = parent.transform.position + Vector3.one;
        myGameObject.transform.rotation = parent.transform.rotation;

        myGameObject.AddComponent<MeshFilter>();
        myGameObject.GetComponent<MeshFilter>().sharedMesh = parent.GetComponent<MeshFilter>().sharedMesh;

        myGameObject.AddComponent<MeshRenderer>();

        myGameObject.AddComponent<MeshEditor>();
        myGameObject.GetComponent<MeshEditor>().DotLayoutObject = parent.GetComponent<MeshEditor>().DotLayoutObject;
        myGameObject.GetComponent<MeshEditor>().TrackingObject = parent.GetComponent<MeshEditor>().TrackingObject;

        myGameObject.AddComponent<MeshCollider>();
        myGameObject.GetComponent<MeshCollider>().convex = true;
        myGameObject.GetComponent<MeshCollider>().sharedMesh = parent.GetComponent<MeshCollider>().sharedMesh;

        myGameObject.AddComponent<Rigidbody>();

        myGameObject.GetComponent<Renderer>().material = parent.GetComponent<Renderer>().material;
    }
}
