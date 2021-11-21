using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class DotsSetter : MonoBehaviour
{
    public GameObject dot;
    List<Vector3> listDots = new List<Vector3>();
    List<GameObject> layoutDots = new List<GameObject>();
    public SteamVR_Action_Boolean triger1;
    public int DotAmount; 
    Vector3 lastposContr;
    Vector3 lastposDot;
    // Start is called before the first frame update
    void Start()
    {
        /*
        listDots.Add(new Vector3(1, 0, -1));
        listDots.Add(new Vector3(1, 1, 1));
        for (int i = 0; i < listDots.Count; i++)
        {
            layoutDots.Add(Instantiate(dot, listDots[i], Quaternion.identity));
        }
        layoutDots[1].transform.up = Vector3.up;
        layoutDots[1].transform.LookAt(layoutDots[0].transform.position);
        layoutDots.Add(Instantiate(dot, Vector3.up * Vector3.Distance(layoutDots[1].transform.position, layoutDots[0].transform.position), Quaternion.identity));
        Debug.Log(Vector3.Distance(layoutDots[1].transform.position, layoutDots[0].transform.position));
        Debug.Log(Vector3.Distance(layoutDots[1].transform.position, layoutDots[2].transform.position));
        */
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triger1.stateDown)
        {
            foreach (var item in gameObject.GetComponentsInChildren<SteamVR_Behaviour_Pose>())
            {
                if (triger1.activeDevice == item.inputSource)
                {
                    listDots.Add(item.transform.position);
                    break;
                }
            }

        }

        /*
        if (triger1.state)
        {
           /* Debug.Log("1");
            s.Activate(SteamVR_Input_Sources.Any, 0, true);
            r.poseAction = s.poseActions[0];
           
        }
        if (triger2.state)
        {
            Debug.Log(r.transform.localPosition);
        }
        if (dragDot.stateDown)
        {
            lastposContr = r.transform.position;
            lastposDot = layoutDots[2].transform.position;
        }
        if (dragDot.state)
        {
            Vector3 delt =  r.transform.position - lastposContr;
            delt = Vector3.ProjectOnPlane(delt, layoutDots[0].transform.position - layoutDots[1].transform.position);
            layoutDots[2].transform.position= lastposDot+delt;
        }
        */
    }
}
