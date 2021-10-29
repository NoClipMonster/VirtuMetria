using UnityEngine;
using Valve.VR;


public class RigMovement : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean telepotrAction;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Vector2 trackPadDir;
    public SteamVR_Action_Boolean trackPadTouched;
   
    GameObject Rig;
     void Start()
    {
        Rig = GameObject.Find("[CameraRig]");
    }
    void Update()
    {
        if (GetTeleportDown())
        {
            print("Teleport " + handType);
        }
        if (GetGrab())
        {
            print("Grab " + handType);
        }
        if (TrackPadTouched())
        {
            Vector2 dir = GetTrackPadDir()/100;
            Rig.transform.Translate(new Vector3(dir.x,0,dir.y));
        }
    }
    public bool GetTeleportDown()
    {
        return telepotrAction.GetStateDown(handType);
    }
    public Vector2 GetTrackPadDir()
    {
        return trackPadDir.GetAxis(handType);
    }
    public bool GettrackPadTouch()
    {
        return trackPadTouched.GetStateDown(handType);
    }
    public bool TrackPadTouched()
    {
        return trackPadTouched.GetState(handType);
    }
    public bool GetGrab()
    {
        return grabAction.GetState(handType);
    }

}
