using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CannonManager : Photon.MonoBehaviour
{

    public List<ShootingPosition> Positions = new List<ShootingPosition>();
    public Dictionary<string, int> Position_Driver = new Dictionary<string, int>();
    public GameObject CannonBall;
    public GameObject Smoke;
    public float ProjectileForceMultiplier = 100;
    // Use this for initialization
    void Start()
    {
        //lets find all shooting positions
        Transform CannonsObject = gameObject.transform.Find("Cannons");
        for (int i = 0; i < CannonsObject.childCount; i++)
        {
            //each of these should be a shooter position
            Transform Position = CannonsObject.GetChild(i);
            ShootingPosition SP = new ShootingPosition()
            {
                Name = Position.name
            };
            //now to get Cannon/Trigger
            for (int iCannon = 0; iCannon < Position.childCount; iCannon++)
            {
                if (Position.GetChild(iCannon).name != "Trigger")
                {
                    Transform c = Position.GetChild(iCannon);
                    Cannon cannon = new Cannon()
                    {
                        Body = c.gameObject,
                        Barrel = c.Find("Barrel").gameObject
                    };
                    cannon.CannonBallInstantiationSpot = cannon.Barrel.transform.GetChild(0).transform;
                    cannon.DefaultRotation = c.localEulerAngles;
                    SP.Cannons.Add(cannon);
                }
                else
                {
                    GameObject TriggerGo = Position.GetChild(iCannon).gameObject;
                    SP.Trigger = TriggerGo;
                    ShootinPositionTrigger SPT = TriggerGo.GetComponent<ShootinPositionTrigger>();
                    SPT.SP = SP;
                    SPT.CM = this;
                }
            }
            Positions.Add(SP);
            Position_Driver.Add(SP.Name, -1);
        }
    }
    public void Update()
    {
        foreach (ShootingPosition sp in Positions)
        {
            foreach (Cannon c in sp.Cannons)
            {
                c.Body.transform.eulerAngles = new Vector3(0, sp.yRotation + c.DefaultRotation.y, 0) + transform.rotation.eulerAngles;
                c.Barrel.transform.eulerAngles = new Vector3(sp.zRotation, sp.yRotation + c.DefaultRotation.y, 0) + transform.rotation.eulerAngles;
            }
            if (sp.canFire == false && sp.ReloadCurrent > 0)
            {
                photonView.RPC("ReduceReloadTime", PhotonTargets.AllBufferedViaServer, sp.Name);
            }
        }
    }
    public void ReloadCannonsRPC(string Name)
    {
        photonView.RPC("ReloadCannons", PhotonTargets.AllBufferedViaServer, Name);
    }
    public void FireCannonsRPC(string Name)
    {
        photonView.RPC("FireCannons", PhotonTargets.AllBufferedViaServer, Name);
    }
    public void ChangeCannonDirectionRPC(float HDirection, float VDirection, string Name)
    {
        photonView.RPC("ChangeShooterPositions", PhotonTargets.AllBufferedViaServer, HDirection, VDirection, Name);
    }
    public void ChangeShooterIDRPC(int id, string name)
    {
        photonView.RPC("ChangeShooterID", PhotonTargets.AllBufferedViaServer, name, id);
    }
    [PunRPC]
    void ReloadCannons(string name, PhotonMessageInfo info)
    {
        ShootingPosition sp = Positions.First(p => p.Name == name);
        sp.ReloadCurrent += Time.deltaTime * 2;
        if(sp.ReloadCurrent > sp.ReloadTime)
        {
            sp.canFire = true;
            sp.ReloadCurrent = 0;
        }
    }
    [PunRPC]
    void ReduceReloadTime(string name, PhotonMessageInfo info)
    {
        ShootingPosition sp = Positions.First(p => p.Name == name);
        sp.ReloadCurrent -= Time.deltaTime;
        if (sp.ReloadCurrent < 0)
            sp.ReloadCurrent = 0;
    }
    [PunRPC]
    void FireCannons(string name, PhotonMessageInfo info)
    {
        ShootingPosition sp = Positions.First(p => p.Name == name);
        if (sp.canFire)
        {
            System.Random r = new System.Random();
            foreach (Cannon c in sp.Cannons)
            {
                float delay = (float)(r.NextDouble() / 2);
                Vector3 pos = c.CannonBallInstantiationSpot.position;
                Vector3 rot = gameObject.transform.eulerAngles + c.Barrel.transform.eulerAngles;
                StartCoroutine(FireCannon(pos, rot, c.Barrel, delay));
            }
            sp.canFire = false;
        }
    }
    IEnumerator FireCannon(Vector3 spawnPoint, Vector3 rotation, GameObject cannonBarrel, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject cannonBall = Instantiate(CannonBall, spawnPoint, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
        cannonBall.GetComponent<CannonBall>().ProjectionForce = cannonBarrel.transform.forward * ProjectileForceMultiplier;
        GameObject smoke = Instantiate(Smoke, spawnPoint, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
        GameObject.Destroy(cannonBall, 10);
        GameObject.Destroy(smoke, 5);
    }
    [PunRPC]
    void ChangeShooterID(string name, int id, PhotonMessageInfo info)
    {
        Position_Driver[name] = id;
    }
    [PunRPC]
    void ChangeShooterPositions(float HDirection, float VDirection, string name, PhotonMessageInfo info)
    {
        ShootingPosition sp = Positions.First(p => p.Name == name);
        sp.yRotation += HDirection / 3;
        sp.zRotation += VDirection / 3;
        if (sp.zRotation > 8)
            sp.zRotation = 8;
        if (sp.zRotation < -16)
            sp.zRotation = -16;

        if (sp.yRotation > 8)
            sp.yRotation = 8;
        if (sp.yRotation < -6)
            sp.yRotation = -6;
    }
}
[Serializable]
public class ShootingPosition
{
    public string Name;
    public Vector3 StandingPosition;
    public float yRotation = 0;
    public float zRotation = 0;
    public List<Cannon> Cannons = new List<Cannon>();
    public GameObject Trigger;
    public int ShooterId = 0;
    public bool canFire = true;
    public DateTime StartReloadTime;
    public float ReloadTime = 5;
    public float ReloadCurrent = 0;
}
[Serializable]
public class Cannon
{
    public Vector3 DefaultRotation;
    public GameObject Body;
    public GameObject Barrel;
    public Transform CannonBallInstantiationSpot;
}
