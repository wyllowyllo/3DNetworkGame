/// <summary>
/// 몬스터 상태 계약. 모든 Bear 상태 클래스가 구현한다.
/// StateId는 OnPhotonSerializeView 직렬화 및 MasterClient 전환 시 상태 복원에 사용된다.
/// </summary>
public interface IMonsterState
{
    int  StateId { get; }
    void Enter();
    void Update();
    void Exit();
    void OnTakeDamage(float damage, int attackerActorNumber);
}
