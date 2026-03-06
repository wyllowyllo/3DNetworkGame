using System;
using Photon.Pun;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Start()
    {
       SpawnManager.Instance.SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
          SpawnManager.Instance.SpawnBear();
    }
}
