using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

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
        
        OnDataChanged?.Invoke();
        
        /*Debug.Log("룸 입장 완료!");
      
        Debug.Log($"룸 : {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"플레이어 인원 : {PhotonNetwork.CurrentRoom.PlayerCount}");
      
        // 룸에 입장한 플레이어 정보
        /*Dictionary<int, Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Player> player in roomPlayers)
        {
           Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
        }#1#
      
        // 리소스 폴도에서 "Player" 이름을 가진 프리팹을 생성(인스턴스화)하고, 서버에 등록도 한다
        // ㄴ 그러나 리소스 폴더는 나쁜것이기에, 다른 방법을 알아보자
        //PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);*/
        
        SpawnManager.Instance.SpawnPlayer();
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

    public void OnPlayerDeath(int attackerActorNumber)
    {
        string attackerNickname = _room.Players[attackerActorNumber].NickName;
        string victimNickname = PhotonNetwork.LocalPlayer.NickName;
        
        OnPlayerDeathed?.Invoke(attackerNickname, victimNickname);
    }
}
