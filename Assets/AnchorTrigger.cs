using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    private BoatMovementController Controller;
    // Use this for initialization
    void Start()
    {
        Controller = (BoatMovementController)transform.root.GetComponentInChildren(typeof(BoatMovementController));
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        int PhotonId = other.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID;
        if (vp_Input.GetButtonDown("Interact"))
        {
            if (Controller.AnchorUp)
            {
                //put it down
                Controller.DropAnchorRPC();
            }
        }
        if(vp_Input.GetButton("Interact"))
        {
            if (Controller.AnchorDown)
            {
                //pull it back up!
                Controller.RaiseAnchorRPC();
            }
        }

    }
}
