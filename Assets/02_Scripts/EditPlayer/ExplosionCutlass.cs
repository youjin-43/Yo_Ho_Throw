using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCutlass : MonoBehaviour
{
    [SerializeField] Cutlass cutlass;
    [SerializeField] float radius;
    [SerializeField] LayerMask playerMask;

    bool isExplosion = false;
    public void OnEnable()
    {
        isExplosion = false;
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
                if (cutlass.attackerActorNr == photonView.OwnerActorNr) continue;

                photonView.RPC("ReceiveDamage", RpcTarget.All, cutlass.attackerActorNr, 1);
            }
        }

        cutlass.Push();
    }
    public void ExplosionVFX()
    {
        EffectManager.Instance.Play(transform.position, EffectType.CutlassExplosion);
    }
}
