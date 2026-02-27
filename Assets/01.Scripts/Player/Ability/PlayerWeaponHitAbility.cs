using Photon.Pun;
using UnityEngine;


public class PlayerWeaponHitAbility : PlayerAbility
{
    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.PhotonView.IsMine) return;
        if (other.transform == _owner.transform) return;

        if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            Debug.Log("공격!");

            // PhotonView 기반으로 TakeDamage RPC 호출 (Player/Bear 공통)
            PhotonView targetView = other.GetComponent<PhotonView>();
            if (targetView != null)
                targetView.RPC("TakeDamage", RpcTarget.All,
                    _owner.PlayerStat.Damage.Value,
                    _owner.PhotonView.Owner.ActorNumber);

            _owner.GetAbility<PlayerWeaponColliderAbility>().DeactiveCollider();
        }
        
    }
}
