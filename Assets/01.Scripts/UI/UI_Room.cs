using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class UI_Room : MonoBehaviourPunCallbacks
{
    private List<UI_RoomItem> _roomItems;
    private Dictionary<string, RoomInfo> _rooms = new();
    
    
    private void Awake()
    {
        _roomItems = GetComponentsInChildren<UI_RoomItem>().ToList();
        
        HideAllRoomUI();
    }
    
    // 로비에 입장 후 방 내용(개수, 이름 등등) 이 바뀌면 자동으로 호출되는 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 모든 UI를 비활성화하고, 
        HideAllRoomUI();

        // 딕셔너리 업데이트(인자로 전달받는 roomList는 전체 room 목록이 아닌, 변경된 룸(추가,삭제)들 목록이다)
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                _rooms.Remove(room.Name); // Delete
            }
            else
            {
                _rooms[room.Name] = room; // Add or Update
            }
        }
        
        int roomCount = _rooms.Count;
        List<RoomInfo> rooms = _rooms.Values.ToList();
        for (int i = 0; i < roomCount; i++)
        {
            // 방 개수만큼만 UI를 활성화한다. 
            _roomItems[i].Init(rooms[i]);
            _roomItems[i].gameObject.SetActive(true);
        }
        
    }
    
    private void HideAllRoomUI()
    {
        foreach (UI_RoomItem roomItem in _roomItems)
        {
            roomItem.gameObject.SetActive(false);
        }
    }
}
