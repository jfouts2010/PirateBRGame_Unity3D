using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ShipWheelTrigger : vp_Interactable
{
    private BoatMovementController boatMovementController;
    private bool Driver = false;
    private float m_canInteract;
    vp_FPPlayerEventHandler m_FPPlayer = null;
    vp_FPPlayerEventHandler FPPlayer
    {
        get
        {
            if (m_FPPlayer == null)
                m_FPPlayer = (m_Player as vp_FPPlayerEventHandler);
            return m_FPPlayer;
        }
    }
    protected override void Start()
    {
        base.Start();
        m_canInteract = Time.time;
        boatMovementController = gameObject.transform.parent.gameObject.GetComponent<BoatMovementController>();
    }
    public override bool TryInteract(vp_PlayerEventHandler player)
    {
        if (!enabled)
            return false;

        if (!(player is vp_FPPlayerEventHandler))
            return false;

       /* if (Time.time < m_canInteract)
            return false;*/

        if (m_Player == null)
            m_Player = player;
        if (Driver && m_Player.Drive.NextAllowedStopTime < Time.time)
        {
            m_Player.DriveShip.TryStop();
            return false;
        }
        if (m_Player.Interactable.Get() != null)
            return false;

        if (m_Controller == null)
            m_Controller = m_Player.GetComponent<vp_FPController>();

        if (m_Camera == null)
            m_Camera = m_Player.GetComponentInChildren<vp_FPCamera>();


        m_Player.Register(this);

        m_Player.Interactable.Set(this); // sets what the player is currently interacting with
        return m_Player.DriveShip.TryStart();

        // disallow norma
        /* if (peh != null)
         {
             if (peh.Drive.Active && peh.Drive.NextAllowedStopTime < Time.time)
             {
                 shipWheel.ChangeDriverIdRPC(-1);
                 peh.Drive.Stop(1);
                 peh.SetWeapon.TryStart(1);
                 // re-allow normal input
                 peh.InputAllowGameplay.Set(true);
                 return true;
             }
             else if (!peh.Drive.Active && peh.Drive.NextAllowedStartTime < Time.time && shipWheel.Driver_view_id == -1)
             {

                 l input while climbing

                 return true;
             }
         }
         return false;*/
    }
    protected virtual void OnStart_DriveShip()
    {
        Driver = true;
        m_canInteract = Time.time + 1;
        int PhotonId = m_Player.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID;
        m_Player.Interactable.Set(null);
        vp_FPPlayerEventHandler peh = m_Player.gameObject.GetComponent<vp_FPPlayerEventHandler>();
        peh.Move.Send(transform.position - peh.gameObject.transform.position + new Vector3(0,0,-1));

        // disallow normal input while climbing
        //  peh.InputAllowGameplay.Set(false);

        // stop any movement on our controller. this is helpful in case we jumped onto the climbable.
        peh.Stop.Send();
        
        //peh.gameObject.transform.position = transform.position;
        peh.SetWeapon.TryStart(0);
        peh.Drive.Start(1);
        peh.gameObject.GetComponent<vp_FPInteractManager>().m_CurrentInteractable = this;
        boatMovementController.ChangeDriverIdRPC(PhotonId);
        Debug.Log(m_Player.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID);
       
    }
    protected virtual void OnStop_DriveShip()
    {
        m_canInteract = Time.time + 1;
        Driver = false;
        m_Player.Interactable.Set(null);
        m_Player.Unregister(this);
        boatMovementController.ChangeDriverIdRPC(-1);
        m_Player.Drive.Stop(1);
        m_Player.SetState("Default");
        m_Player.SetWeapon.TryStart(1);
        m_Player.gameObject.GetComponent<vp_FPInteractManager>().m_CurrentInteractable = null;
        // re-allow normal input
    }
    protected virtual bool CanStart_Interact()
    {
        /*if (Time.time < m_canInteract)
            return false;*/
        if (Driver)
            m_Player.DriveShip.TryStop();

        return true;

    }
    public override void FinishInteraction()
    {

        /*if (Driver)
            m_Player.DriveShip.TryStop();*/

    }
    public override void Update()
    {
        base.Update();
        if (!Driver)
            return;

        if (m_Player == null)
            return;
      
        vp_FPPlayerEventHandler peh = m_Player.gameObject.GetComponent<vp_FPPlayerEventHandler>();
        if (peh != null)
        {
            float axisRaw = vp_Input.GetAxisRaw("Horizontal");
            bool boomRight = vp_Input.GetButtonAny("SetPrevWeapon");
            bool boomLeft = vp_Input.GetButtonAny("SetNextWeapon");
            if (axisRaw != 0)
            {
                boatMovementController.ChangeWheelDirectionRPCCall(axisRaw);
            }
            if (boomRight && !boomLeft)
            {
                Debug.Log("right");
                boatMovementController.ChangeBoomRPC(1);
            }
            if (!boomRight && boomLeft)
            {
                Debug.Log("left");
                boatMovementController.ChangeBoomRPC(-1);
            }
        }
    }
    /* public void Start()
     {
         shipWheel = gameObject.transform.parent.gameObject.GetComponent<BoatMovementController>();
     }
     private void OnTriggerExit(Collider other)
     {
         if (other.gameObject.tag != "Player")
             return;
         other.GetComponent<vp_SimpleHUD>().controlls = "";
     }
     void OnTriggerStay(Collider other)
     {
         if (other.gameObject.tag != "Player")
             return;

         if (vp_Input.GetButtonDown("Interact"))
         {
             vp_FPPlayerEventHandler peh = other.gameObject.GetComponent<vp_FPPlayerEventHandler>();
             if (peh != null)
             {
                 if (peh.Drive.Active && peh.Drive.NextAllowedStopTime < Time.time)
                 {
                     shipWheel.ChangeDriverIdRPC(-1);
                     peh.Drive.Stop(1);
                     peh.SetWeapon.TryStart(1);
                     // re-allow normal input
                     peh.InputAllowGameplay.Set(true);
                 }
                 else if (!peh.Drive.Active && peh.Drive.NextAllowedStartTime < Time.time && shipWheel.Driver_view_id == -1)
                 {
                     peh.Move.Send(transform.position - peh.gameObject.transform.position);
                     // disallow normal input while climbing
                     peh.InputAllowGameplay.Set(false);

                     // stop any movement on our controller. this is helpful in case we jumped onto the climbable.
                     peh.Stop.Send();

                     //peh.gameObject.transform.position = transform.position;
                     peh.SetWeapon.TryStart(0);
                     peh.Drive.Start(1);
                     shipWheel.ChangeDriverIdRPC(PhotonId);
                     Debug.Log(other.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID);
                     // disallow normal input while climbing
                 }
             }
         }
         else if (PhotonId != null && PhotonId == shipWheel.Driver_view_id)
         {
             vp_FPPlayerEventHandler peh = other.gameObject.GetComponent<vp_FPPlayerEventHandler>();
             if (peh != null)
             {
                 double axisRaw = vp_Input.GetAxisRaw("Horizontal");
                 if (axisRaw != 0)
                 {
                     shipWheel.ChangeWheelDirectionRPCCall(axisRaw);
                 }
             }
             other.GetComponent<vp_SimpleHUD>().controlls = "";
         }
         else
         {
             other.GetComponent<vp_SimpleHUD>().controlls = "Press F to Steer";
         }
     }*/

}
