using UnityEngine;



public class Driver : MonoBehaviour
{
   
    protected vp_PlayerEventHandler m_Player = null;

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake()
    {
        m_Player = (vp_PlayerEventHandler)transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
    }



    /// <summary>
    /// registers this component with the event handler (if any)
    /// </summary>
    protected virtual void OnEnable()
    {

        // allow this monobehaviour to talk to the player event handler
        if (m_Player != null)
            m_Player.Register(this);

    }


    /// <summary>
    /// unregisters this component from the event handler (if any)
    /// </summary>
    protected virtual void OnDisable()
    {

        // unregister this monobehaviour from the player event handler
        if (m_Player != null)
            m_Player.Unregister(this);

    }


    /// <summary>
    /// adds a condition (a rule set) that must be met for the
    /// event handler 'Reload' activity to successfully activate.
    /// NOTE: other scripts may have added conditions to this
    /// activity aswell
    /// </summary>
    protected virtual bool CanStart_Drive()
    {

       /* // can't reload if current weapon isn't fully wielded
        if (m_Player.CurrentWeaponWielded.Get() == false)
            return false;

        // can't reload if weapon is full
        if (m_Player.CurrentWeaponMaxAmmoCount.Get() != 0 &&    // only check if max capacity is reported
            (m_Player.CurrentWeaponAmmoCount.Get() == m_Player.CurrentWeaponMaxAmmoCount.Get()))
            return false;

        // can't reload if the inventory has no additional ammo for this weapon
        if (m_Player.CurrentWeaponClipCount.Get() < 1)
        {
            return false;
        }*/

        return true;

    }


    /// <summary>
    /// this callback is triggered right after the 'Reload' activity
    /// has been approved for activation
    /// </summary>
    protected virtual void OnStart_Drive()
    {
        Debug.Log("Driving!");
    }


    /// <summary>
    /// this callback is triggered when the 'Reload' activity
    /// deactivates
    /// </summary>
    protected virtual void OnStop_Drive()
    {
        Debug.Log("Stopped Driving!");
    }

    protected virtual void OnCancelStop_Drive()
    {
       
    }





}