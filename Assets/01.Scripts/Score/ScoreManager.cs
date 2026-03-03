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
    
    public static event Action OnDataChanged;

    private void Awake()
    {
        Instance = this;
    }
    
    public void AddScore(int score)
    {
       
        _score += score;
        
       Refresh();
    }

    public void DiscountScore()
    {
        if(_score > 0)
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

        OnDataChanged?.Invoke();
    }
    
    private void Refresh()
    {
        // 해시테이블은 딕셔너리와 같은 키 - 값 형태로 저장하는데
        // 키 - 값에 있어서 자료형이 object다 (즉 모든 값을 저장할 수 있다)
        Hashtable hashtable = new Hashtable();
        hashtable.Add("score", _score);
        
        // 프로퍼티 등록
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }
}
