using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviourPunCallbacks
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject _maleCharacter;
    [SerializeField] private GameObject _femaleCharacter;

    [Header("Room Create Info")]
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private Button _createRoomButton;
    
    private ECharacterType _characterType;

    public void OnClickMale() => OnClickCharacterButton(ECharacterType.Male);
    public void OnClickFemale() => OnClickCharacterButton(ECharacterType.Female);

    private void Start()
    {
        _createRoomButton.onClick.AddListener(MakeRoom);
    }
    
    private void MakeRoom()
    {
        string nickname = _nicknameInputField.text;
        string roomName = _roomNameInputField.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomName))
        {
            return;
        }
        
        PhotonNetwork.NickName = nickname;

        // 선택한 캐릭터 타입 저장
        var props = new Hashtable { { "characterType", (int)_characterType } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // 룸 옵션 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20; // 룸 최대 접속자 수
        roomOptions.IsVisible = true; // 로비에서 룸을 보여줄 것인지
        roomOptions.IsOpen = true;
        roomOptions.CustomRoomProperties = new Hashtable { { "ownerName", nickname } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "ownerName" };
      
        // 룸 만들기
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        
       
    }
    
    private void OnClickCharacterButton(ECharacterType characterType)
    {
        _characterType = characterType;
        
        _maleCharacter.SetActive(characterType == ECharacterType.Male);
        _femaleCharacter.SetActive(characterType == ECharacterType.Female);
    }
}