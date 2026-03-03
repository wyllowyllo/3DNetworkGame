using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UI_Score : MonoBehaviour
{
    private List<UI_ScoreItem> _items;

    private void Start()
    {
        _items = GetComponentsInChildren<UI_ScoreItem>().ToList();
        
        ScoreManager.OnDataChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        var scores = ScoreManager.Instance.Scores;

        // readonly가 아니면 원본을 수정하므로 무결성 문제가 생긴다. 


        foreach (var scoreData in scores)
        {
            List<ScoreData> scoresDatas = scores.Values.ToList();
            
            // 1. TODO : 1등부터 3등까지 정렬
            // 2.  TODO : 3명 있는지 적절하게 반복문
            for (int i = 0; i < scoresDatas.Count; i++)
            {
                ScoreData data = scoresDatas[i];
                _items[i].Set(data.NickName, data.Score);
            }
        }
    }
}
