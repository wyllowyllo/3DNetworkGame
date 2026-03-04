using Photon.Pun;
using UnityEngine;

public class PlayerWeaponScaleAbility : PlayerAbility
{
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private int _scorePerLevel = 1000;

    private int _currentScaleLevel;

    private void Start()
    {
        ScoreManager.OnDataChanged += OnScoreChanged;
    }

    private void OnDestroy()
    {
        ScoreManager.OnDataChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int actorNumber, int score)
    {
        if (!_owner.PhotonView.IsMine) return;
        if (actorNumber != _owner.PhotonView.Owner.ActorNumber) return;

        int newLevel = score / _scorePerLevel;
        if (newLevel == _currentScaleLevel) return;

        _currentScaleLevel = newLevel;
        float scale = 1f + _currentScaleLevel * 0.1f;
        _owner.PhotonView.RPC(nameof(RPC_SetWeaponScale), RpcTarget.AllBuffered, scale);
    }

    [PunRPC]
    private void RPC_SetWeaponScale(float scale)
    {
        _weaponTransform.localScale = Vector3.one * scale;
    }
}
