using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviourPun
{
   [SerializeField] private int _itemScore = 10;
   
  

   private void OnTriggerEnter(Collider other)
   {
      Debug.Log("충돌!");
      
      if (other.CompareTag("Player"))
      {
         
         other.GetComponent<PlayerController>().Score += _itemScore;
         
         ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
      }
   }
}
