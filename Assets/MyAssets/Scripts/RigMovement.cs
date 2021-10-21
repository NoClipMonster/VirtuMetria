using UnityEngine;
using Valve.VR;


public class RigMovement : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean telepotrAction;
    public SteamVR_Action_Boolean grabAction;

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
    }
    public bool GetTeleportDown()
    {
        return telepotrAction.GetStateDown(handType);
    }
    public bool GetGrab()
    {
        return grabAction.GetState(handType);
    }
}
