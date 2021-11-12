using System.Collections.Generic;
using UnityEngine;

public class DotsSetter : MonoBehaviour
{
    public GameObject dot;
    List<Vector3> list = new List<Vector3>();
    List<GameObject> layoutDots = new List<GameObject>();
    public GameObject controller;
    Vector3 lastpos;
    // Start is called before the first frame update
    void Start()
    {
        list.Add(new Vector3(1, 0, -1));
        list.Add(new Vector3(1, 1, 1));
        for (int i = 0; i < list.Count; i++)
        {
            layoutDots.Add(Instantiate(dot, list[i], Quaternion.identity));
        }
        layoutDots[1].transform.up = Vector3.up;
        layoutDots[1].transform.LookAt(layoutDots[0].transform.position);
        layoutDots.Add(Instantiate(dot, Vector3.up * Vector3.Distance(layoutDots[1].transform.position, layoutDots[0].transform.position), Quaternion.identity));
        Debug.Log(Vector3.Distance(layoutDots[1].transform.position, layoutDots[0].transform.position));
        Debug.Log(Vector3.Distance(layoutDots[1].transform.position, layoutDots[2].transform.position));

    }

    // Update is called once per frame
    void Update()
    {
        lastpos = Vector3.ProjectOnPlane(controller.transform.position - layoutDots[0].transform.position, layoutDots[0].transform.position - layoutDots[1].transform.position);
        layoutDots[2].transform.position = layoutDots[1].transform.position+lastpos;
    }
}
