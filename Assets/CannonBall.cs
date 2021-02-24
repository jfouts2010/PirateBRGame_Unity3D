using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CannonBall : Photon.MonoBehaviour
{
    public Vector3 ProjectionForce;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (ProjectionForce) * Time.deltaTime;
        ProjectionForce += new Vector3(0, -9.81f, 0) * Time.deltaTime;
        //transform.position += transform.forward;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.ToLower() == "boat" || other.gameObject.tag.ToLower() == "player")
        {
            other.GetComponent<Health>().TakeDamage(50);
        }
        Destroy(gameObject);
    }

}
