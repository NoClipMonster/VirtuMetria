using UnityEngine;
using Valve.VR;


public class RigMovement : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean telepotrAction;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Vector2 trackPadDir;
    public SteamVR_Action_Boolean trackPadTouched;

    void Update()
    {
        if (GetTeleportDown())
        {
            print("Teleport " + handType);
        }

        if (GettrackPadTouch())
        {
            Vector2 dir = GetTrackPadDir();
            print("Pos = "+dir.x+":"+dir.y  +" in " + handType);
         //   print("PadTouch");
        }
        if (GetGrab())
        {
            print("Grab " + handType);
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
    public bool GetGrab()
    {
        return grabAction.GetState(handType);
    }

}
