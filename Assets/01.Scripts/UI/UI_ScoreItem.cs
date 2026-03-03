using TMPro;
using UnityEngine;


public class UI_ScoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nickNameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Set(string nickname, int score)
    {
        _nickNameText.text = nickname;
        _scoreText.text = $"{score}";
    }
}
