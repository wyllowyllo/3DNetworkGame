using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }

    private int _score;
    private Dictionary<int, ScoreData> _scores = new();

    public ReadOnlyDictionary<int, ScoreData> Scores => new ReadOnlyDictionary<int, ScoreData>(_scores);

    public static event Action<int, int> OnDataChanged; // actorNumber, score

    private void Awake()
    {
        Instance = this;
    }
    
    public override void OnJoinedRoom()
    {
        // 방에 들어가면 '내 점수가 0이다' 라는 내용으로 
        // 커스텀 프로퍼티를 초기화해준다.
        Refresh();
    }

    public void AddScore(int score)
    {
        _score += score;
        Refresh();
    }

    public void DiscountScore()
    {
        if (_score > 0)
            _score /= 2;

        Refresh();
    }

    // 플레이어의 커스텀 프로퍼티가 변경되면 자동으로 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.ContainsKey("score")) return;

        ScoreData scoreData = new ScoreData()
        {
            NickName = targetPlayer.NickName,
            Score = (int)changedProps["score"]
        };

        _scores[targetPlayer.ActorNumber] = scoreData;

        OnDataChanged?.Invoke(targetPlayer.ActorNumber, scoreData.Score);
    }

    private void Refresh()
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("score", _score);

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }
}
