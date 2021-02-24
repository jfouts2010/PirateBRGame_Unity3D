using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Swim : Photon.MonoBehaviour
{
    vp_FPController Controller;
    public void Start()
    {
        Controller = gameObject.GetComponent<vp_FPController>();
    }
    public void Update()
    {
        if (gameObject.transform.position.y < -1f)
        {
            Controller.SetPosition(new Vector3(gameObject.transform.position.x, -1, gameObject.transform.position.z));
        }
        else
        {
            Controller.MotorFreeFly = false;
            Controller.PhysicsGravityModifier = 0.2f;
        }
    }
}

