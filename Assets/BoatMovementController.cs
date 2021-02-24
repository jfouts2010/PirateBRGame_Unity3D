using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Assets;
using System.Linq;

public class BoatMovementController : Photon.MonoBehaviour
{
    public float WheelDirection = 0.5f;
    private WindController windController;
    private Rigidbody rb;
    public GameObject WheelGameObject;
    private GameObject WindlassGameObject;
    private SpringJoint sj;
    public Wind windDirection;
    public int Driver_view_id = -1;
    public bool AnchorDown = true;
    public bool AnchorUp = false;
    public float AnchorPosition = -5;
    public float AnchorBottom = -5;
    public bool Released = false;
    public float xVel;
    public float zVel;
    public float BoomAngle = 0;
    public void Awake()
    {
        windDirection = GameObject.Find("Wind").GetComponent<Wind>();
        WheelGameObject = this.transform.Find("ShipWheel").gameObject;
        WindlassGameObject = this.transform.Find("Windlass").gameObject;
        windController = gameObject.GetComponent<WindController>();
        rb = gameObject.GetComponent<Rigidbody>();
        SpawnAnchor();
    }
    public void SpawnAnchor()
    {
        sj = this.gameObject.AddComponent<SpringJoint>();
        sj.spring = 170000;
        sj.damper = 100000;
        sj.anchor = new Vector3(0, 0, 0);
        sj.autoConfigureConnectedAnchor = false;
        sj.connectedAnchor = this.transform.position;
    }
    public void DestroyAnchor()
    {
        Destroy(sj);
    }
    public void Update()
    {
        //get the dot product between sail and wind
        Vector3 first = windDirection.WindDirection.normalized;
        Vector3 second = (Quaternion.Euler(0, 180 - BoomAngle * 90 + this.transform.eulerAngles.y, 0)*Vector3.forward).normalized;
        float dotProd = Vector3.Dot(first, second);
        float ang = Mathf.Rad2Deg * Mathf.Acos(dotProd);
        
        float multi = (Mathf.Sin(ang * Mathf.Deg2Rad) );//1 - ((Mathf.Abs(ang) - 90f) / 90f);
        xVel = rb.velocity.x;
        zVel = rb.velocity.z;
        WheelGameObject.transform.eulerAngles = new Vector3(0, 0, (float)WheelDirection * -45f) + transform.rotation.eulerAngles;
        Debug.Log(WheelDirection);
        Vector3 MovementVector = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
        float WindAngle = Vector3.Angle(MovementVector, windDirection.WindDirection);
        if (WindAngle < 90 && AnchorUp)
        {
            //find the forward factor of the force
            float perc = Mathf.Cos(Mathf.Deg2Rad * WindAngle);
            double AddedForce = 1000000 * perc + 10000;
            AddedForce *= multi;
            float yRad = Mathf.Deg2Rad * transform.rotation.eulerAngles.y;
            rb.AddForce(new Vector3(Mathf.Sin(yRad) * (float)AddedForce, 0, Mathf.Cos(yRad) * (float)AddedForce));
            rb.AddTorque(new Vector3(0, (float)AddedForce * (float)WheelDirection));
        }
        if (Released && AnchorPosition > AnchorBottom)
            photonView.RPC("LowerAnchor", PhotonTargets.AllBufferedViaServer);
        if (AnchorPosition > 0)
            photonView.RPC("RaisedAnchor", PhotonTargets.AllBufferedViaServer);
        if (AnchorPosition > 0)
            photonView.RPC("SetPosition", PhotonTargets.AllBufferedViaServer, 0f);
        if (AnchorPosition < AnchorBottom)
            photonView.RPC("DroppedAnchor", PhotonTargets.AllBufferedViaServer);
        if (AnchorPosition < AnchorBottom)
            photonView.RPC("SetPosition", PhotonTargets.AllBufferedViaServer, AnchorBottom);

        float percUp = (AnchorPosition - AnchorBottom) / (0 - AnchorBottom);
        WindlassGameObject.transform.eulerAngles = new Vector3(0, percUp * -180f + 90f, 0) + transform.rotation.eulerAngles;
    }
    public void DropAnchorRPC()
    {
        photonView.RPC("DropAnchor", PhotonTargets.AllBufferedViaServer);
    }
    public void RaiseAnchorRPC()
    {
        photonView.RPC("RaiseAnchor", PhotonTargets.AllBufferedViaServer);
    }
    public void ChangeWheelDirectionRPCCall(float axisRaw)
    {
        photonView.RPC("ChangeWheelDirectionRPC", PhotonTargets.AllBufferedViaServer, axisRaw);
    }
    public void ChangeDriverIdRPC(int id)
    {
        photonView.RPC("ChangeDriverID", PhotonTargets.AllBufferedViaServer, id);
    }
    public void ChangeBoomRPC(float axis)
    {
        photonView.RPC("ChangeBoom", PhotonTargets.AllBufferedViaServer, axis);
    }
    [PunRPC]
    void ChangeBoom(float direction, PhotonMessageInfo info)
    {
        if (direction > 0)
            BoomAngle += 0.05f;
        else
            BoomAngle -= 0.05f;
        if (BoomAngle > 1)
            BoomAngle = 1;
        if (BoomAngle < -1)
            BoomAngle = -1;
    }
    [PunRPC]
    void SetPosition(float Value, PhotonMessageInfo info)
    {
        AnchorPosition = Value;
    }
    [PunRPC]
    void LowerAnchor(PhotonMessageInfo info)
    {
        AnchorPosition -= Time.deltaTime;
    }
    [PunRPC]
    void RaiseAnchor(PhotonMessageInfo info)
    {
        AnchorPosition += Time.deltaTime * 2;
    }
    [PunRPC]
    void RaisedAnchor(PhotonMessageInfo info)
    {
        Released = false;
        AnchorDown = false;
        AnchorUp = true;
        DestroyAnchor();
    }
    [PunRPC]
    void DropAnchor(PhotonMessageInfo info)
    {
        Released = true;
    }
    [PunRPC]
    void DroppedAnchor(PhotonMessageInfo info)
    {
        AnchorUp = false;
        AnchorDown = true;
        SpawnAnchor();
    }
    [PunRPC]
    void ChangeDriverID(int id, PhotonMessageInfo info)
    {
        Driver_view_id = id;
    }
    [PunRPC]
    void ChangeWheelDirectionRPC(float direction, PhotonMessageInfo info)
    {
        if (direction > 0)
            WheelDirection += 0.05f;
        else
            WheelDirection -= 0.05f;
        if (WheelDirection > 1)
            WheelDirection = 1;
        if (WheelDirection < -1)
            WheelDirection = -1;
    }
}
