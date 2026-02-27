/// <summary>
/// 몬스터 외부 계약. 다양한 몬스터 타입 추가 시 이 인터페이스로 참조한다.
/// IDamagable을 포함하여 피격 가능 + 상태 조회 기능을 제공한다.
/// </summary>
public interface IMonsterController : IDamagable
{
    float CurrentHP { get; }
    float MaxHP     { get; }
    bool  IsDead    { get; }
}
