
using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemObjectFactory : MonoBehaviour
{
    public static ItemObjectFactory Instance {get; private set;}
    private PhotonView _photonView;

    private void Awake()
    {
        Instance = this;
        
        _photonView = GetComponent<PhotonView>();
    }

    // 우리의 약속 : 방장에게 룸 관련해서 뭔가 요청을 할 때는 메서드 명이 Request 로 시작하는게 유지보수적으로 편하다
    public void RequestMakeScoreItem(Vector3 makePosition)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 방장이라면 그냥 함수 호출
            MakeScoreItems(makePosition);
        }
        else
        {
            // 아니라면 방장의 함수를 호출
            _photonView.RPC(nameof(MakeScoreItems), RpcTarget.MasterClient, makePosition);
        }
    }
    
    [PunRPC]
    private void MakeScoreItems(Vector3 makePosition)
    {
        int randomCount = Random.Range(3, 5);

        for (int i = 0; i < randomCount; i++)
        {
            // 소유자가 나가면 해당 네트워크 게임 오브젝트도 삭제된다.
            // 플레이어가 룸을 나가면 그 플레이어가 생성/소유한 모든 네트워크 게임 오브젝트는 삭제된다..
            // 즉, 플레이어 생명 주기를 가지고 있다
            
            // 그래서 플레이어 생명 주기가 아닌 룸 생명 주기로 만들어야 한다.
            PhotonNetwork.InstantiateRoomObject("ScoreItem", makePosition, Quaternion.identity);
            
            // 포톤에는 룸 안에 방장(Master Cilient)가 있다
            // 방을 만든 사람이 방장
            // ㄴ 방장을 양도할 수 있다.
            // ㄴ 방장이 게임을 나가면 자동으로 그 다음으로 들어온 사람이 방장이 된다.
        }
    }

    public void RequestDelete(int viewId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewId);
        }
        else
        {
            _photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewId);
        }
    }

    [PunRPC]
    private void Delete(int viewId)
    {
        GameObject objectToDelete = PhotonView.Find(viewId)?.gameObject;
        if (objectToDelete == null) return;
        
        PhotonNetwork.Destroy(objectToDelete);
    }
}
