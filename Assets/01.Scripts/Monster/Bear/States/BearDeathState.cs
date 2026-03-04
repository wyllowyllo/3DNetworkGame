using Photon.Pun;

/// <summary>
/// 죽음 상태. 코루틴으로 리스폰을 처리한다.
/// 추가 피격 무시는 BearController.TakeDamage의 IsDead 체크로 보장된다.
/// </summary>
public class BearDeathState : BearStateBase
{
    public override int StateId => (int)EBearState.Death;

    public BearDeathState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.nav.isStopped = true;
        BearController.nav.enabled   = false;
        BearController.photonView.RPC(nameof(BearController.PlayDeathAnimation), RpcTarget.All);
        BearController.StartCoroutine(BearController.DeathRespawn_Coroutine());
    }

    public override void Update() { }
}
