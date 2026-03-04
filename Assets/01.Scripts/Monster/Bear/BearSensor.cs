using UnityEngine;

/// <summary>
/// Bear의 플레이어 탐지 전담 컴포넌트.
/// BearController와 같은 GameObject에 부착한다.
/// 상태 클래스는 BearController.DetectTarget() / BearController.Target 으로 접근한다.
/// </summary>
public class BearSensor : MonoBehaviour
{
    private float _detectRange;

    public Transform Target { get; private set; }

    public void Initialize(float detectRange)
    {
        _detectRange = detectRange;
    }

    
    public void Detect()
    {
        Collider[] hits = Physics.OverlapSphere(                              
                        transform.position, _detectRange,                                 
                        LayerMask.GetMask("Player"));      

        Target = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            
            float d = Vector3.Distance(transform.position, col.transform.position);
            if (d < minDist)
            {
                minDist = d;
                Target  = col.transform;
            }
        }
    }

    public void ClearTarget() => Target = null;
}
