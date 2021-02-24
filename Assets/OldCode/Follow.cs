using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public GameObject Platform;
	// Use this for initialization
	void Start () {
        Platform = GameObject.Find("PolygonBoat");
	}
	
	// Update is called once per frame
	void Update () {
        if (Platform != null)
        {
            Boat BoatClass = Platform.GetComponent<Boat>();
            //Vector3 rot = RotatePointAroundPivot(transform.position, BoatPlatform.transform.position, BoatClass.LastFrameQuaternionChange) - transform.position;
            this.transform.position += BoatClass.LastFrameTransformChange;
        }
    }
}
