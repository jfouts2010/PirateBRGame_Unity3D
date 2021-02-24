using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class FPSPlayerConnectionObject : NetworkBehaviour
{

    public GameObject PlayerPrefab;
    // Use this for initialization
    void Start()
    {
        CmdSpawn();
    }

    // Update is called once per frame
    void Update()
    {

    }
    [Command]
    public void CmdSpawn()
    {
        GameObject go = Instantiate(PlayerPrefab);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }
}
