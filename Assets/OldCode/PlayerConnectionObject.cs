using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject SpawnMenuPrefab;
    public GameObject FPSUI;
    public GameObject BoatPrefab;
    public GameObject CurrentMenu;
    // Use this for initialization
    public override void OnStartAuthority()
    {
        //spawn menu on start
        GameObject spawnMenu = Instantiate(SpawnMenuPrefab);
        Button button = spawnMenu.transform.Find("New Ship").GetComponent<Button>();
        button.onClick.AddListener(SpawnNewShip);
        Button button2 = spawnMenu.transform.Find("Join Ship").GetComponent<Button>();
        button2.onClick.AddListener(SpawnRandom);
        CurrentMenu = spawnMenu;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SpawnRandom()
    {
        Destroy(CurrentMenu);
        CurrentMenu = null;
        CmdSpawnPlayerRandom();
    }
    public void SpawnNewShip()
    {
        Destroy(CurrentMenu);
        CurrentMenu = null;
        CmdSpawnPlayerNewShip();
        GameObject fpsui = Instantiate(FPSUI);
        CurrentMenu = fpsui;
        
    }
    [Command]
    public void CmdSpawnPlayerRandom()
    {
        GameObject boat = GameObject.Find("boatBase(Clone)");
        GameObject go = Instantiate(playerPrefab, boat.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        go.GetComponent<FirstPersonController>().Parent = boat;
        go.GetComponent<FirstPersonController>().HomeShip = boat;
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

    }
    [Command]
    public void CmdSpawnPlayerNewShip()
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
        if (isServer)
        {
            GameObject boat = Instantiate(BoatPrefab, spawnPoint, Quaternion.identity);
            NetworkServer.Spawn(boat);
            GameObject go2 = Instantiate(playerPrefab, spawnPoint + new Vector3(0, 2, 0), Quaternion.identity);
            go2.GetComponent<FirstPersonController>().Parent = boat;
            go2.GetComponent<FirstPersonController>().HomeShip = boat;
            NetworkServer.SpawnWithClientAuthority(go2, connectionToClient);
        }
    }
}
