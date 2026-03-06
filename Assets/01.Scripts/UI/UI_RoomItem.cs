using Photon.Realtime;
using TMPro;
using UnityEngine;


public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _playerCountText;

    private RoomInfo _roomInfo;
    public void Init(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;

        _roomNameText.text = roomInfo.Name;
        _nicknameText.text = "";
        _playerCountText.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }

    private void EnterRoom()
    {
        if (_roomInfo == null) return;
    }
}
