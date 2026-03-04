/// <summary>
/// 몬스터 상태 계약. Enter/Update/Exit만 정의한다.
/// 데미지 처리는 BearController가 전담하므로 OnTakeDamage는 포함하지 않는다.
/// StateId는 OnPhotonSerializeView 직렬화 및 MasterClient 전환 시 상태 복원에 사용된다.
/// </summary>
public interface IMonsterState
{
    int  StateId { get; }
    void Enter();
    void Update();
    void Exit();
}
