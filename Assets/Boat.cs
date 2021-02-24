using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Assets;
using System.Linq;


public class Boat : MonoBehaviour
{
    //Drags
    public float SHIPMASS = 170000;
    public GameObject underWaterObj;
    public Vector3 LastFrameTransform;
    public Vector3 LastFrameQuaternion;
    public Vector3 LastFrameTransformChange;
    public Vector3 LastFrameQuaternionChange;
    public GameObject CannonBallPrefab;

    public Health health;
    //Script that's doing everything needed with the boat mesh, such as finding out which part is above the water
    private ModifyBoatMesh modifyBoatMesh;
    public double WheelDirection = 1;
    //Mesh for debugging
    private Mesh underWaterMesh;
    public List<ShootingPosition> ShootingPositionsList = new List<ShootingPosition>();
    public bool sinking;
    private Rigidbody boatRB;
    public GameObject ExplosionPrefab;
    //The density of the water the boat is traveling in
    private float rhoWater = 1027f;

    void Start()
    {
        //shooting positions
        //ShootingPosition LeftShootingPos = new ShootingPosition() { StandingPosition = new Vector3(-2.22f, 2.6f, 0), Name = "LeftShootingPos" };
        //LeftShootingPos.CannonBallSpawnPoints.Add(gameObject.transform.Find("Cannon1").gameObject);
        //ShootingPositionsList.Add(LeftShootingPos);
        //transform.Find(LeftShootingPos.Name).GetComponent<CannonTrigger>().connectedShootingPosition = LeftShootingPos;
       
        LastFrameTransform = transform.position;
        LastFrameQuaternion = transform.rotation.eulerAngles;
        health = transform.GetComponent<Health>();
        //Get the boat's rigidbody
        boatRB = gameObject.GetComponent<Rigidbody>();

        //Init the script that will modify the boat mesh
        modifyBoatMesh = new ModifyBoatMesh(gameObject);

        //Meshes that are below and above the water
        underWaterMesh = underWaterObj.GetComponent<MeshFilter>().sharedMesh;
    }
    public void Died()
    {

        //died
        health.currentHealth = 100;
        Vector3 spawnPoint = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
        transform.position = spawnPoint;
        sinking = false;
        transform.rotation = Quaternion.identity;
        boatRB.mass = SHIPMASS;
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
            boatRB.mass = boatRB.mass * 3.0f;
            GameObject exp = Instantiate(ExplosionPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(exp, 10);
        }

        if (WheelDirection > 1)
            WheelDirection = 1;
        if (WheelDirection < -1)
            WheelDirection = -1;
        
      
        //Generate the under water mesh
        modifyBoatMesh.GenerateUnderwaterMesh();

        //Display the under water mesh
        modifyBoatMesh.DisplayMesh(underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);

        LastFrameTransformChange = transform.position - LastFrameTransform;
        LastFrameQuaternionChange = transform.rotation.eulerAngles - LastFrameQuaternion;
        LastFrameTransform = transform.position;
        LastFrameQuaternion = transform.rotation.eulerAngles;
    }

    void FixedUpdate()
    {
        //Add forces to the part of the boat that's below the water
        if (modifyBoatMesh.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
       
    }

    //Add all forces that act on the squares below the water
    void AddUnderWaterForces()
    {
        //Get all triangles
        List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;

        for (int i = 0; i < underWaterTriangleData.Count; i++)
        {
            //This triangle
            TriangleData triangleData = underWaterTriangleData[i];

            //Calculate the buoyancy force
            Vector3 buoyancyForce = BuoyancyForce(rhoWater, triangleData);

            //Add the force to the boat
            boatRB.AddForceAtPosition(buoyancyForce, triangleData.center);


            //Debug

            //Normal
            Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

            //Buoyancy
            Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
        }
    }

    //The buoyancy force so the boat can float
    private Vector3 BuoyancyForce(float rho, TriangleData triangleData)
    {
        //Buoyancy is a hydrostatic force - it's there even if the water isn't flowing or if the boat stays still

        // F_buoyancy = rho * g * V
        // rho - density of the mediaum you are in
        // g - gravity
        // V - volume of fluid directly above the curved surface 

        // V = z * S * n 
        // z - distance to surface
        // S - surface area
        // n - normal to the surface
        Vector3 buoyancyForce = rho * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }

}
