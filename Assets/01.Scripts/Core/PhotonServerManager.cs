using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonServerManager : MonoBehaviourPunCallbacks
{
   // MonoBahaviour : Unity의 다양항 '이벤트' 콜백 함수를 오버라이드 할 수 있다(Awake, Start, Update..)
   // MonobehaviourPunCallbacks : PUN의 다양한 '서버 이벤트' 콜백 함수를 오버라이드 할 수 있다.
   // - 서버 접속에 성공/실패 했다.
   // - 내가 방 입장에 성공/실패 했다.
   // - 누군가가 내 방에 들어왔다 등등..

   private string _version = "0.0.1";
   private string _nickname = "mingwan";
   
   
   private void Start()
   {
      _nickname += $"_{Random.Range(100, 999)}";
      
      // 시작 시 게임 버전, 닉네임 지정해줘야 함
      PhotonNetwork.GameVersion = _version; // 버전에 따라 유저들이 만날수도,못만날수도 있음(버전에 따른 분기)
      PhotonNetwork.NickName = _nickname;
      
      PhotonNetwork.SendRate = 30; // 얼마나 자주 데이터를 송수신할 것인가 (실제 송수신)
      PhotonNetwork.SerializationRate = 30; // 얼마나 자주 데이터를 직렬화할 것인지 (송수신 준비)
      
      // 방장이 로드한 씬 게임에 다른 유저들도 똑같이 그 씬을 로드하도록 동기화해준다.
      // 방장(마스터 클라이언트) : 방을 만든 '소유자' (방에는 하나의 마스터 클라이언트가 존재)
      // 방장이 씬을 옮기면 다른 사람들도 자동으로 옮겨진다.
      PhotonNetwork.AutomaticallySyncScene = true;
      
      // 위에서 설정한 값들을 이용해서 서버로 접속 시도
      PhotonNetwork.ConnectUsingSettings();
   }

   // 포톤 서버에 접속이 성공하면 호출되는 콜백 함수
   public override void OnConnected()
   {
      Debug.Log("네임서버 접속 완료!");
      // 네임 서버(AppId, GamerVersion 등으로 구분되는 서버)
      
      
      Debug.Log(PhotonNetwork.CloudRegion);
      // 현재 어느 지역의 서버에 연결됐나?
      // ping 테스트를 통해서 가장 빠른 region으로 자동 연결된다(kr : korea)
   }

   public override void OnConnectedToMaster()
   {
      // 포톤 서버는 로비(=채널)이라는 개념이 있다.
      //TypedLobby lobby = new TypedLobby("3Channel", LobbyType.Default);
      
      PhotonNetwork.JoinLobby(); // Default 로비 입장 시도
   }

   // 로비 입장에 성공하면 자동으로 호출되는 콜백 함수
   public override void OnJoinedLobby()
   {
      Debug.Log("로비 접속 완료!");
      Debug.Log(PhotonNetwork.InLobby);
      
      // 랜덤 방 입장 시도
      PhotonNetwork.JoinRandomRoom();
   }
   
  

   // 랜덤 방 입장에 실패하면 자동으로 호출되는 콜백 함수
   public override void OnJoinRandomFailed(short returnCode, string message)
   {
      // 랜덤 룸 입장에 실패하면.. 룸이 하나도 없는 것이니.. 룸을 만들자!~
      
      Debug.Log($"방 입장에 실패했습니다 : {returnCode}, {message}");
      
      // 룸 옵션 정의
      RoomOptions roomOptions = new RoomOptions();
      roomOptions.MaxPlayers = 20; // 룸 최대 접속자 수
      roomOptions.IsVisible = true; // 로비에서 룸을 보여줄 것인지
      roomOptions.IsOpen = true;
      
      // 룸 만들기
      PhotonNetwork.CreateRoom("test", roomOptions);
   }
   
   
   // 방 입장에 실패하면 자동으로 호출되는 콜백 함수
   public override void OnJoinRoomFailed(short returnCode, string message)
   {
      Debug.Log($"방 입장에 실패했습니다 : {returnCode}, {message}");
   }
}
