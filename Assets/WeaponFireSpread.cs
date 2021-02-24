using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFireSpread : MonoBehaviour {

    private vp_FPWeaponShooter m_FPWeapon = null;
    private vp_PlayerEventHandler m_Player = null;

    private float StartingSpread = 0.0f;
    private float FiringRate = 0.0f;
    public float SpreadIncrease = 0.1f; // Allow changes in Inspector.
    public float MaxRunSpread = 5;
    void Start()
    {
        m_FPWeapon = transform.GetComponentInChildren<vp_FPWeaponShooter>();
        // Get the event handler so that 
        //we can know when the weapon is firing.

        m_Player = transform.root.GetComponentInChildren<vp_PlayerEventHandler>();


        // Get this weapons default projectile spread
        // so that we can reset the spread as desired.

        StartingSpread = m_FPWeapon.ProjectileSpread;

        // Get this weapons default firing rate.

        FiringRate = m_FPWeapon.ProjectileFiringRate;
    }
    private void Update()
    {
        // Get access to this weapon
        // so that we can change parameters
        m_FPWeapon = transform.GetComponentInChildren<vp_FPWeaponShooter>();

        if (m_Player.InputMoveVector.Get().magnitude > 0)
        {
            if (m_FPWeapon.ProjectileRunSpread < MaxRunSpread)
                m_FPWeapon.ProjectileRunSpread++;
        }
        else if (m_FPWeapon.ProjectileRunSpread > 0)
            m_FPWeapon.ProjectileRunSpread--;
        if (m_FPWeapon.ProjectileRunSpread > MaxRunSpread)
            m_FPWeapon.ProjectileRunSpread = MaxRunSpread;
        if ( m_FPWeapon.ProjectileRunSpread < 0)
            m_FPWeapon.ProjectileRunSpread = 0;
    }

    // Connect / Disconnect event handler.

    protected virtual void OnEnable()
    {
        if (m_Player == null) return;
        m_Player.Register(this);
    }


    protected virtual void OnDisable()
    {
        if (m_Player != null)
            m_Player.Unregister(this);
    }


    // Event handler notifies us of Start/Stop.

    protected virtual void OnStart_Attack()
    {
        //	Debug.Log("Start Attack");

        // Do our work in another method.

        StartCoroutine("ChangeTheSpread");
    }

    protected virtual void OnStop_Attack()
    {
        //	Debug.Log("Stop Attack");

        StopCoroutine("ChangeTheSpread");

        // Reset to default projectile spred
        // when we stop pulling the trigger.

        m_FPWeapon.ProjectileShootSpread = 0;
    }

    IEnumerator ChangeTheSpread()
    {
        while (true)
        {
            // Use abundance of caution ...
            // the weapon might hold tons of ammo!

            if (m_FPWeapon.ProjectileTotalSpread >= 360f)
            {
                // Reset to original spread? Or decrement?

                m_FPWeapon.ProjectileShootSpread = 0;
                //	m_FPWeapon.ProjectileSpread = m_FPWeapon.ProjectileSpread - SpreadIncrease;
            }

            else

                m_FPWeapon.ProjectileShootSpread = m_FPWeapon.ProjectileShootSpread + SpreadIncrease;

            //  Debug.Log(m_FPWeapon.ProjectileSpread);

            yield return new WaitForSeconds(FiringRate - 0.01f);
        }
    }
}
