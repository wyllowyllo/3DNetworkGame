using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 일정 간격으로 하늘에서 ScoreItem을 랜덤 위치에 드롭합니다.
/// MasterClient만 드롭 루틴을 실행하며, 방장 교체 시 자동으로 인계됩니다.
/// </summary>
public class SkyItemDropper : MonoBehaviourPunCallbacks
{
    public static SkyItemDropper Instance { get; private set; }

    [SerializeField] private float _dropInterval = 5f; // 드롭 주기 (초)
    [SerializeField] private float _dropHeight = 20f;  // 드롭 시작 높이 (Y)

    [Header("드롭 범위 피벗")]
    [SerializeField] private Transform _pivotBottomLeft;
    [SerializeField] private Transform _pivotTopRight;

    private Coroutine _dropCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            StartDropping();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
            StartDropping();
    }

    private void StartDropping()
    {
        if (_dropCoroutine != null)
            StopCoroutine(_dropCoroutine);

        _dropCoroutine = StartCoroutine(DropRoutine());
    }

    public void StopDropping()
    {
        if (_dropCoroutine == null) return;

        StopCoroutine(_dropCoroutine);
        _dropCoroutine = null;
    }

    private IEnumerator DropRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_dropInterval);
            Debug.Log("아이템 드롭");
            DropItems();
        }
    }

    private void DropItems()
    {
        // 직사각형 배치이므로 대각선 두 피벗으로 XZ 범위 결정
        float x = Random.Range(_pivotBottomLeft.position.x, _pivotTopRight.position.x);
        float z = Random.Range(_pivotBottomLeft.position.z, _pivotTopRight.position.z);
        Vector3 dropPosition = new Vector3(x, _dropHeight, z);

        ItemObjectFactory.Instance.RequestMakeScoreItem(dropPosition);
    }
}
