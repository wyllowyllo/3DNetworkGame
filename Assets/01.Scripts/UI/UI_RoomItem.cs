using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _playerCountText;
    [SerializeField] private Button _roomEnterButton;
    
    private RoomInfo _roomInfo;
    
    private void Awake()
    {
        _roomEnterButton = GetComponent<Button>();
        _roomEnterButton.onClick.AddListener(EnterRoom);
    }

    
    public void Init(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;

        _roomNameText.text = roomInfo.Name;
        _nicknameText.text = roomInfo.CustomProperties["ownerName"].ToString();
        _playerCountText.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }

    private void EnterRoom()
    {
        if (_roomInfo == null) return;
        
        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }
}
