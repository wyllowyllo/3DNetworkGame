using System;
using Photon.Pun;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Start()
    {
        var charType = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("characterType")
            ? (ECharacterType)(int)PhotonNetwork.LocalPlayer.CustomProperties["characterType"]
            : ECharacterType.Male;

        PhotonNetwork.Instantiate($"Player{charType}", Vector3.zero, Quaternion.identity);

        if (PhotonNetwork.IsMasterClient)
          SpawnManager.Instance.SpawnBear();
    }
}
