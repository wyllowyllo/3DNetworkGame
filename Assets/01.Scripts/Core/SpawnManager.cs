using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
   
    
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] float _respawnDelay;

    [SerializeField] Transform[] _bearSpawnPoints;
    [SerializeField] string _bearPrefabName = "BearMonster";
    
   
    
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
        Vector3 spawnPoint = GetRandomSpawnPosition();
        PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }

    
    public Vector3 GetRandomSpawnPosition()
    {
        return (_spawnPoints.Length > 0) ? _spawnPoints[(int)Random.Range(0, _spawnPoints.Length)].position : Vector3.zero;
    }

    public void SpawnBear()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        foreach (var point in _bearSpawnPoints)
            PhotonNetwork.InstantiateRoomObject(_bearPrefabName, point.position, point.rotation);
    }
}
