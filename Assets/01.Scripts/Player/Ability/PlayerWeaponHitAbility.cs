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
            
            // 상대방의 TakeDamage를 RPC로 호출한다
            PlayerController otherPlayer = other.GetComponent<PlayerController>();
            otherPlayer.PhotonView.RPC(nameof(PlayerController.Takedamage), RpcTarget.All, _owner.PlayerStat.Damage.Value);
            
           _owner.GetAbility<PlayerWeaponColliderAbility>().DeactiveCollider();
        }
        
    }
}
