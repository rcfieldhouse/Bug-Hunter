using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSpawnLocation : NetworkBehaviour
{
    public GameObject EnemyFightPrefab;
    public GameObject EnemySpawnPrefab;
    public GameObject BarrierPrefab;

    public GameObject ArenaSlime;
    public GameObject ArenaTick;
    public GameObject ArenaZeephyr;
    public GameObject ArenaBomber;

    public override void OnNetworkSpawn()
    {
        //transform.position = GameObject.Find("NetworkManager").transform.position;

        if(IsServer && SceneManager.GetActiveScene().name == "SampleScene")
        {

           Vector3 pos = new Vector3(508, 6, 490);
           //Instantiate(EnemyFightPrefab, pos, Quaternion.identity);

           pos = new Vector3(832, 4, 932);
           Instantiate(EnemySpawnPrefab, pos, Quaternion.identity);

           
           pos = new Vector3(444, 45.5f, 542);
           //Instantiate(BarrierPrefab, pos, new Quaternion(-0.707106829f, 0, 0, 0.707106829f));

            

        }
        if (IsServer && SceneManager.GetActiveScene().name == "Arena")
        {
            transform.position = new Vector3(172, 54, 376);
            transform.GetChild(0).localPosition = new Vector3(0, 0, 0);

            //Instantiate(ArenaSlime);
            //Instantiate(ArenaZeephyr);
            //Instantiate(ArenaBomber);
            //Instantiate(ArenaTick);

        }
           
    }
}
