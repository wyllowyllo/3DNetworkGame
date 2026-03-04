using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UI_Score : MonoBehaviour
{
    private List<UI_ScoreItem> _items;

    private const int DisplayCount = 3;
    private void Start()
    {
        _items = GetComponentsInChildren<UI_ScoreItem>().ToList();
        
        ScoreManager.OnDataChanged += (_, _) => Refresh();
        Refresh();
    }

    private void Refresh()
    {
        var scores = ScoreManager.Instance.Scores;

        // readonly가 아니면 원본을 수정하므로 무결성 문제가 생긴다. 


        foreach (var scoreData in scores)
        {
            List<ScoreData> scoresDatas = scores.Values.OrderByDescending(x => x.Score).ToList();
                                                       
            
            for (int i = 0; i < Mathf.Min(DisplayCount, scoresDatas.Count); i++)
            {
                ScoreData data = scoresDatas[i];
                _items[i].Set(data.NickName, data.Score);
            }
        }
    }
}
