/// <summary>
/// 죽음 상태. 코루틴으로 리스폰을 처리하며, OnTakeDamage는 무시한다.
/// </summary>
public class BearDeathState : BearStateBase
{
    public override int StateId => (int)EBearState.Death;

    public BearDeathState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.Agent.isStopped = true;
        BearController.Agent.enabled   = false;
        BearController.TriggerDeathAnim();
        BearController.StartCoroutine(BearController.DeathRespawn_Coroutine());
    }

    public override void Update() { }

    // 죽음 상태에서는 추가 피격 무시
    public override void OnTakeDamage(float damage, int attackerActorNumber) { }
}
