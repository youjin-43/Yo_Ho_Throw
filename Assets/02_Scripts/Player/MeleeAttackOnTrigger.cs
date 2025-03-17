using Photon.Pun;
using UnityEngine;

public class MeleeAttackOnTrigger : MonoBehaviour
{
    const int MELEE_ATTACK_DAMAGE = 2;

    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponentInParent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Bullet"))
        {
            other.transform.GetChild(0).GetComponent<Collider>().enabled = false;
            Debug.Log("패링 성공");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            other.GetComponent<Cutlass>().isParry = true;
            other.transform.position = rb.transform.position+new Vector3(0f,0f,1f);
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.useGravity = true;
                rb.isKinematic = false; 

                // 랜덤한 방향으로 튕겨나가게 함
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    1f, 
                    1f
                ).normalized;

                float forceAmount = 10; // 튕기는 힘 설정
                rb.AddForce(randomDirection * forceAmount, ForceMode.Impulse);

                // 랜덤 회전 추가
                Vector3 randomTorque = new Vector3(
                    Random.Range(15f, 25f),
                    Random.Range(15f, 25f),
                    Random.Range(15f, 25f)
                );

                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }

            // TODO 석진 패링 성공 사운드 추가 사운드 - 확인해야함
            AudioManager.Instance.PlaySfxAtPosition(AudioManager.Sfx.Block, transform.position);
            Debug.Log("패링 성공");
        }
        */
        if (other.CompareTag("Player"))
        {
            //TODO 석진 근접공격 성공 사운드
            

            PhotonView playerPhotonView = other.GetComponent<PhotonView>();


            // 자신에 대한 공격일 경우 제외
            if (photonView.OwnerActorNr == playerPhotonView.OwnerActorNr) return;


            // TODO 찬규 : 플레이어가 칼에 부딪혔을 때 낼 효과 추가 부분
            // TODO 찬규 : 공격 클라이언트를 누구 기준으로 할지


            // 1번 호스트가 아닐 경우 제외
            if (!PhotonNetwork.IsMasterClient) return;

            // 2번 칼을 던진 사람이 아닐 경우 제외
            //if (PhotonNetwork.LocalPlayer.ActorNumber != attackerActorNr) return;

            // EditPlayerState 에 있는 ReceiveDamage 함수 호출
            playerPhotonView.RPC("ReceiveDamage", RpcTarget.All, photonView.OwnerActorNr, MELEE_ATTACK_DAMAGE);
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.OnDamagedAnim();
            }
        }

        else if (other.CompareTag("TreasureChest"))
        {
            PhotonView chestPhotonView = other.GetComponent<PhotonView>();
            // TreasureChest에 있는 Attack 함수 호출
            chestPhotonView.RPC("Attack", RpcTarget.All);
        }
    }
    
}
