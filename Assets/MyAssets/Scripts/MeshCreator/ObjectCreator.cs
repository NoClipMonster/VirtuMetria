using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public GameObject dot;
    public GameObject line;
    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(4, 2, 3));
        list.Add(new Vector3(2, 3, 1));
        list.Add(new Vector3(3, 1, 4));   
        foreach (Vector3 v in list)
            Instantiate(dot, v, new Quaternion(0, 0, 0, 0));
        GameObject go = createPlane(list);
       
    }
    GameObject createPlane(List<Vector3> vectors)
    {
        /* 
        vectors.Add(new Vector3(4, 2, 3));
        vectors.Add(new Vector3(2, 3, 1));
        vectors.Add(new Vector3(3, 1, 4));   */
        GameObject plane = new GameObject();
        Plane pl = new Plane(vectors[0], vectors[1], vectors[2]);
        plane.transform.position = vectors[0];
        plane.transform.up = pl.normal;

        plane.name = "CreatedPlane";
        plane.AddComponent<MeshFilter>().mesh = gameObject.GetComponent<MeshFilter>().mesh;
        plane.AddComponent<MeshRenderer>().material = plane.GetComponentInParent<MeshRenderer>().material;


        return plane;
    }
}
