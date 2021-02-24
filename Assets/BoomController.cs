using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomController : MonoBehaviour {
    //controller to adjust the boom based on ships "boatmovementcontroller.cs" boom angle
    public float angleMultiplier = 15;
    public float speed = 1;
    public BoatMovementController bmv;
	// Use this for initialization
	void Start () {
        bmv = transform.parent.root.GetComponent<BoatMovementController>();
        foreach(Cloth c in this.transform.GetComponentsInChildren<Cloth>())
        {
            c.externalAcceleration = bmv.windDirection.WindDirection * 100;
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.localRotation =
            Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(new Vector3(0, bmv.BoomAngle * angleMultiplier,0)), Time.time * speed);
    }
}
