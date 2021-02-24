using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Networking;

public class CannonTrigger : NetworkBehaviour
{
    public GameObject ShootingGuidePrefab;
    public ShootingPosition connectedShootingPosition;
    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        
        /*if (Input.GetKey(KeyCode.E))
        {
            FirstPersonController fpsObj = other.GetComponent<FirstPersonController>();
           
            fpsObj.Shooting = true;
            fpsObj.shootingPos = connectedShootingPosition;
            GameObject shootingObj = fpsObj.Parent.transform.Find(connectedShootingPosition.Name).gameObject;
            GameObject guide = Instantiate(ShootingGuidePrefab, fpsObj.Parent.transform.position + shootingObj.transform.position, fpsObj.Parent.transform.rotation);
            ShootingGuide shootingPositionComponent = guide.GetComponent<ShootingGuide>();
            shootingPositionComponent.Boat = fpsObj.Parent;
            shootingPositionComponent.ShootingPositionObject = shootingObj;
            shootingPositionComponent.pos = connectedShootingPosition;
        }*/
    }
}


