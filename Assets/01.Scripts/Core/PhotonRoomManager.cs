using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoomManager :  MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance { get; private set; }
    
    // 이벤트
    public event Action OnDataChanged; // 룸 정보가 바뀌었을 떄
    public event Action<Player> OnPlayerEnter; // 플레이어 입장
    public event Action<Player> OnPlayerLeave; // 플레이어 퇴장
    public event Action<string, string> OnPlayerDeathed; // 플레이어 사망
    
    private Room _room;

    // 프로퍼티
    public Room Room => _room;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    // 방 입장에 성공하면 자동으로 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        
        //SceneManager.LoadScene("GameScene");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");   
        }
        else
        {
            // 아무것도 하지 않아도.. 자동으로 방장이 있는 씬으로 옮겨진다
        }
        
        OnDataChanged?.Invoke();
        
        
        /*if (PhotonNetwork.IsMasterClient)
            SpawnManager.Instance.SpawnBear();*/
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnDataChanged?.Invoke();
        OnPlayerEnter?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        OnDataChanged?.Invoke();
        OnPlayerLeave?.Invoke(player);
    }

    public void OnPlayerDeath(int attackerActorNumber, int victimActorNumber)
    {
        string attackerNickname = _room.Players[attackerActorNumber].NickName;
        string victimNickname = _room.Players[victimActorNumber].NickName;
        
        OnPlayerDeathed?.Invoke(attackerNickname, victimNickname);
    }
}
