using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class DotsSetter : MonoBehaviour
{
    public GameObject dot;
    List<Vector3> list = new List<Vector3>();
    List<GameObject> layoutDots = new List<GameObject>();
    public SteamVR_Action_Boolean triger1;
    public SteamVR_Action_Boolean triger2;
    public SteamVR_Action_Boolean dragDot;
    public SteamVR_ActionSet s;
    public SteamVR_Behaviour_Pose l;
    public SteamVR_Behaviour_Pose r;
    Vector3 lastposContr;
    Vector3 lastposDot;
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

        if (triger1.state)
        {
            Debug.Log("1");
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

    }
}
