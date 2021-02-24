using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootinPositionTrigger : vp_Interactable
{

    public ShootingPosition SP;
    public CannonManager CM;
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
            m_Player.ManCannons.TryStop();
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
        return m_Player.ManCannons.TryStart();

    }
    protected virtual void OnStart_ManCannons()
    {
        Driver = true;
        m_canInteract = Time.time + 1;
        int PhotonId = m_Player.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID;
        m_Player.Interactable.Set(null);
        vp_FPPlayerEventHandler peh = m_Player.gameObject.GetComponent<vp_FPPlayerEventHandler>();
        // disallow normal input while climbing
        //peh.InputAllowGameplay.Set(false);

        // stop any movement on our controller. this is helpful in case we jumped onto the climbable.
        peh.Stop.Send();

        //peh.gameObject.transform.position = transform.position;
        peh.Move.Send(transform.position - peh.gameObject.transform.position);
        peh.SetWeapon.TryStart(0);
        peh.Drive.Start(1);
        CM.ChangeShooterIDRPC(PhotonId, SP.Name);
        peh.gameObject.GetComponent<vp_FPInteractManager>().m_CurrentInteractable = this;
      
        Debug.Log(m_Player.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID);

    }
    protected virtual void OnStop_ManCannons()
    {
        m_canInteract = Time.time + 1;
        Driver = false;
        m_Player.Interactable.Set(null);
        m_Player.Unregister(this);
        CM.ChangeShooterIDRPC(-1, SP.Name);
        m_Player.Drive.Stop(1);
        m_Player.SetWeapon.TryStart(1);
        // disallow normal input while climbing
        //m_Player.InputAllowGameplay.Set(true);
        m_Player.gameObject.GetComponent<vp_FPInteractManager>().m_CurrentInteractable = null;
        // re-allow normal input
    }
    protected virtual bool CanStart_Interact()
    {
        /*if (Time.time < m_canInteract)
            return false;*/
        if (Driver)
            m_Player.ManCannons.TryStop();

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
            
            //moving the cannons
            float axisHRaw = vp_Input.GetAxisRaw("Horizontal");
            float axisVRaw = vp_Input.GetAxisRaw("Vertical");
            if (axisHRaw != 0 || axisVRaw != 0)
            {
                CM.ChangeCannonDirectionRPC(axisHRaw, axisVRaw, SP.Name);
            }
            //shooting the cannons
            if (SP.canFire && (vp_Input.GetButtonDown("Attack") || vp_Input.GetAxisRaw("RightTrigger") > 0.5f))     // fire using the right gamepad trigger
            {
                CM.FireCannonsRPC(SP.Name);
                Debug.Log("fire");
            }
            //reloading the cannons
            if (SP.canFire == false && vp_Input.GetButton("Reload"))
            {
                CM.ReloadCannonsRPC(SP.Name);
            }
            //display reload icon
            if(SP.canFire == false && SP.ReloadCurrent > 0)
            {
                vp_SimpleHUD HUD = m_Player.gameObject.GetComponent<vp_SimpleHUD>();
                HUD.ReloadUpdate(SP.ReloadCurrent / SP.ReloadTime);
            }
        }
    }
    public void FindTrajectory()
    {
        foreach(Cannon cannon in SP.Cannons)
        {
            Vector3 rotationDegrees = cannon.CannonBallInstantiationSpot.transform.eulerAngles;
            float x = cannon.CannonBallInstantiationSpot.position.x + 1;
            //float y = Mathf.Tan(rotationDegrees.y)*x - ((-9.8))
        }
    }
    // Use this for initialization
  /*  private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        other.GetComponent<vp_SimpleHUD>().controlls = "";
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        int PhotonId = other.gameObject.GetComponent<vp_MPPlayerStats>().NPlayer.ID;
        if (vp_Input.GetButtonDown("Interact"))
        {
            vp_FPPlayerEventHandler peh = other.gameObject.GetComponent<vp_FPPlayerEventHandler>();
            if (peh != null)
            {
                if (peh.Drive.Active && peh.Drive.NextAllowedStopTime < Time.time)
                {
                    CM.ChangeShooterIDRPC(-1, SP.Name);
                    peh.Drive.Stop(1);
                    peh.SetWeapon.TryStart(1);
                    // disallow normal input while climbing
                    peh.InputAllowGameplay.Set(true);
                }
                else if (!peh.Drive.Active && peh.Drive.NextAllowedStartTime < Time.time && CM.Position_Driver[SP.Name] == -1)
                {
                    // disallow normal input while climbing
                    peh.InputAllowGameplay.Set(false);

                    // stop any movement on our controller. this is helpful in case we jumped onto the climbable.
                    peh.Stop.Send();

                    //peh.gameObject.transform.position = transform.position;
                    peh.Move.Send(transform.position - peh.gameObject.transform.position);
                    peh.SetWeapon.TryStart(0);
                    peh.Drive.Start(1);
                    CM.ChangeShooterIDRPC(PhotonId, SP.Name);
                }
            }
        }
        //code if the user is the shooter
        else if (PhotonId == CM.Position_Driver[SP.Name])
        {
            vp_FPPlayerEventHandler peh = other.gameObject.GetComponent<vp_FPPlayerEventHandler>();
            if (peh != null)
            {
                //moving the cannons
                float axisHRaw = vp_Input.GetAxisRaw("Horizontal");
                float axisVRaw = vp_Input.GetAxisRaw("Vertical");
                if (axisHRaw != 0 || axisVRaw != 0)
                {
                    CM.ChangeCannonDirectionRPC(axisHRaw, axisVRaw, SP.Name);
                }
                //shooting the cannons
                if (SP.canFire && (vp_Input.GetButtonDown("Attack") || vp_Input.GetAxisRaw("RightTrigger") > 0.5f))     // fire using the right gamepad trigger
                {
                    CM.FireCannonsRPC(SP.Name);
                    Debug.Log("fire");
                }
                //reloading the cannons
                if (SP.canFire == false && vp_Input.GetButton("Reload"))
                {
                    CM.ReloadCannonsRPC(SP.Name);
                }
            }
            other.GetComponent<vp_SimpleHUD>().controlls = "";
            //reload
        }
        else
        {
            other.GetComponent<vp_SimpleHUD>().controlls = "Press F to Man Cannons";
        }
    }*/
}
