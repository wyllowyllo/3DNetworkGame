using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    
    [SerializeField] Transform[] _spawnPoints;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPoint = (_spawnPoints.Length > 0) ? _spawnPoints[(int)Random.Range(0, _spawnPoints.Length)].position : Vector3.zero;
        PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
