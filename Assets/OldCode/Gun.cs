using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Gun : NetworkBehaviour {

    public float Damage = 100;
    public int maxAmmo = 1;
    public int currentAmmo = 1;
    public GameObject Muzzle;
    public GameObject BulletPrefab;
    public float reloadSeconds = 2;
    public bool reloading = false;
    public Animator animator;
    public float nextTimeToFire = 0f;
    public float fireRate = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   
   
}
