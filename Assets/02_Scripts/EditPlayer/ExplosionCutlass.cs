using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCutlass : Cutlass
{
    [SerializeField] float radius;
    [SerializeField] LayerMask playerMask;

    bool isExplosion = false;
    protected override void OnEnable()
    {
        base.OnEnable();

        isExplosion = false;
    }
    protected override void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            // 자신에 대한 공격일 경우 제외
            if (attackerActorNr == playerPhotonView.OwnerActorNr) return;
        }

        if (transform.GetChild(0).gameObject.GetInstanceID() ==
            other.gameObject.GetInstanceID()) return;

        Debug.Log("충돌 OBj : " + other.gameObject.name);
        Debug.Log("현재 칼 Position : " + transform.position.ToString());

        base.OnTriggerEnter(other);

        Explosion();
    }
    public void Explosion()
    {
        if (isExplosion) return;

        isExplosion = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerMask);

        HashSet<int> hashSet = new HashSet<int>();

        foreach (Collider cd in colliders)
        {
            PhotonView photonView = cd.GetComponent<PhotonView>();

            if (hashSet.Add(photonView.OwnerActorNr))
            {
                if (attackerActorNr == photonView.OwnerActorNr) continue;

                photonView.RPC("ReceiveDamage", RpcTarget.All, attackerActorNr, 1);
            }
        }
        Push();
    }
}
