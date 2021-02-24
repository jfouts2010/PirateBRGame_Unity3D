using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Assets;
using System.Linq;

class Buoyancy : MonoBehaviour
{
    private ModifyBoatMesh modifyBoatMesh;
    private Mesh underWaterMesh;
    private Rigidbody rigidbody;
    private float rhoWater = 1027f;
    public bool Active = true;
    public void Start()
    {
        //Init the script that will modify the boat mesh
        modifyBoatMesh = new ModifyBoatMesh(gameObject);
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }
    public void Update()
    {
        if (Active)
        {
            modifyBoatMesh.GenerateUnderwaterMesh();

            //Display the under water mesh
            modifyBoatMesh.DisplayMesh(underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);
        }
    }
    void FixedUpdate()
    {
        if (Active)
        {
            //Add forces to the part of the boat that's below the water
            if (modifyBoatMesh.underWaterTriangleData.Count > 0)
            {
                AddUnderWaterForces();
            }
        }
    }
    //Add all forces that act on the squares below the water
    void AddUnderWaterForces()
    {
        //Get all triangles
        List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;
        int countSkip = 1;
        int count = 0;
        for (int i = 0; i < underWaterTriangleData.Count; i++)
        {
            //This triangle
            TriangleData triangleData = underWaterTriangleData[i];

            //Calculate the buoyancy force
            Vector3 buoyancyForce = BuoyancyForce(rhoWater, triangleData);

            //Add the force to the boat
            rigidbody.AddForceAtPosition(buoyancyForce, triangleData.center);


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
