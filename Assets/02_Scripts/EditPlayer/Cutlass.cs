using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Cutlass : MonoBehaviour
{
    const int CUTLASS_THROW_DAMAGE = 1;

    [SerializeField] ReturnCutlass returnCutlass;
    [SerializeField] ExplosionCutlass explosionCutlass;

    [SerializeField] float rotateSpeed = 180f;

    [SerializeField] Vector3 dir;

    [HideInInspector] public float gravityTime = 1f;

    [HideInInspector] public int attackerActorNr = -1;

    Rigidbody rb = null;

    Coroutine gravityTimeCoroutine = null;

    private void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }
    protected virtual void OnEnable()
    {
        if (rb == null) rb = transform.parent.GetComponent<Rigidbody>();

        rb.useGravity = false;

        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = Vector3.zero;

        gravityTimeCoroutine = StartCoroutine(ApplyGravity());
    }
    public void Setting(int attackerActorNr, Vector3 position)
    {
        this.attackerActorNr = attackerActorNr;
        transform.parent.position = position;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        // 플레이어에게 충돌했을 때
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            //데미지 받을 때 애니메이션 출력
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.OnDamagedAnim();
            // 자신에 대한 공격일 경우 제외
            if (attackerActorNr == playerPhotonView.OwnerActorNr) return;

            // 칼 비활성화
            Push();

            // TODO 찬규 : 플레이어가 칼에 부딪혔을 때 낼 효과 추가 부분

            // TODO 찬규 : 공격 클라이언트를 누구 기준으로 할지

            // 1번 호스트가 아닐 경우 제외
            if (!PhotonNetwork.IsMasterClient) return;

            // 2번 칼을 던진 사람이 아닐 경우 제외
            // if (PhotonNetwork.LocalPlayer.ActorNumber != attackerActorNr) return;

            explosionCutlass?.Explosion();

            // EditPlayerState 에 있는 ReceiveDamage 함수 호출
            playerPhotonView.RPC("ReceiveDamage", RpcTarget.All, attackerActorNr, CUTLASS_THROW_DAMAGE);
        }

        // �������ڿ� �浹���� ��
        else if (other.CompareTag("TreasureChest"))
        {
            PhotonView chestPhotonView = other.GetComponent<PhotonView>();
            // TreasuerChest �� �ִ� Attack �Լ� ȣ��
            chestPhotonView.RPC("Attack", RpcTarget.All);
        }
    }
    IEnumerator ApplyGravity()
    {
        yield return null;

        yield return new WaitForSeconds(gravityTime);

        transform.parent.GetComponent<Rigidbody>().useGravity = true;

        gravityTimeCoroutine = null;
    }
    private void LateUpdate()
    {
        transform.parent.Rotate(Time.deltaTime * rotateSpeed * dir);
    }
    private void OnDisable()
    {
        if (gravityTimeCoroutine != null)
        {
            StopCoroutine(gravityTimeCoroutine);

            gravityTimeCoroutine = null;
        }
    }
    public void Push()
    {
        returnCutlass.DeactivateKnife();
    }
}
