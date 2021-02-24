using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Assets;
using System.Linq;

public class BoatHealth : MonoBehaviour
{
    public Health health;
    public bool sinking;
    private Rigidbody boatRB;
    int SHIPMASS = 170000;
    GameObject ExplosionPrefab;
    void Start()
    {
        //shooting positions
        //ShootingPosition LeftShootingPos = new ShootingPosition() { StandingPosition = new Vector3(-2.22f, 2.6f, 0), Name = "LeftShootingPos" };
        //LeftShootingPos.CannonBallSpawnPoints.Add(gameObject.transform.Find("Cannon1").gameObject);
        //ShootingPositionsList.Add(LeftShootingPos);
        //transform.Find(LeftShootingPos.Name).GetComponent<CannonTrigger>().connectedShootingPosition = LeftShootingPos;

        health = transform.GetComponent<Health>();
        //Get the boat's rigidbody
        boatRB = gameObject.GetComponent<Rigidbody>();
    }
    public void Died()
    {
        //died
        health.currentHealth = 100;
        Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(-50, 50), 0, UnityEngine.Random.Range(-50, 50));
        transform.position = spawnPoint;
        sinking = false;
        transform.rotation = Quaternion.identity;
        boatRB.mass = SHIPMASS;
        gameObject.GetComponent<Buoyancy>().Active = true;
    }
    void Update()
    {
        if (sinking && transform.position.y < -30)
        {
            Died();
        }
        if (health.currentHealth < 0 && !sinking)
        {
            sinking = true;
            gameObject.GetComponent<Buoyancy>().Active = false;
            boatRB.mass = boatRB.mass * 3.0f;
            //GameObject exp = Instantiate(ExplosionPrefab, gameObject.transform.position, Quaternion.identity);
            //Destroy(exp, 10);
        }
    }
}

