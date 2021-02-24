using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Assets;
using System.Linq;



class WindController : MonoBehaviour
{
    public float SailStrength;
    public float BaseSpeed;
    private Rigidbody rb;
    private Wind windDirection;
    public double WindForce = 0;
    public void Start()
    {
        windDirection = GameObject.Find("Wind").GetComponent<Wind>();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    public void Update()
    {
        Vector3 MovementVector = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
        float WindAngle = Vector3.Angle(MovementVector, windDirection.WindDirection);
        if (WindAngle < 90)
        {
            //find the forward factor of the force
            float perc = Mathf.Cos(Mathf.Deg2Rad * WindAngle);
            WindForce = SailStrength * perc + BaseSpeed;
            float yRad = Mathf.Deg2Rad * transform.rotation.eulerAngles.y;
            rb.AddForce(new Vector3(Mathf.Sin(yRad) * (float)WindForce, 0, Mathf.Cos(yRad) * (float)WindForce));
            //rb.AddTorque(new Vector3(0, (float)AddedForce * (float)WheelDirection));
        }
    }
}
