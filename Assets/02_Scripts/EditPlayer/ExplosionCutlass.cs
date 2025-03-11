using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCutlass : Cutlass
{
    [SerializeField] float radius;
    [SerializeField] LayerMask playerMask;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerMask);

        HashSet<int> hashSet = new HashSet<int>();

        foreach (Collider cd in colliders)
        {
            PhotonView photonView = cd.GetComponent<PhotonView>();

            if (hashSet.Add(photonView.OwnerActorNr))
            {
                photonView.RPC("ReceiveDamage", RpcTarget.All, attackerActorNr, 1);
            }
        }
    }
}
