using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;


public class Knife : MonoBehaviour
{
    const int KNIFE_THROW_DAMAGE = 1;

    [SerializeField] float lifeTime = 5f;

    [HideInInspector] public float gravityTime = 1f;

    [HideInInspector] public int attackerActorNr = -1;

    Rigidbody rb = null;

    Coroutine lifeTimeCoroutine = null;
    Coroutine gravityTimeCoroutine = null;

    bool inPool = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        inPool = false;

        rb.useGravity = false;

        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = Vector3.zero;

        lifeTimeCoroutine = StartCoroutine(AutoDisable());
        gravityTimeCoroutine = StartCoroutine(ApplyGravity());
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어에게 충돌했을 때
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            // 내 공격이였을 경우 제외
            if (attackerActorNr == playerPhotonView.OwnerActorNr) return;

            // 칼 비활성화
            DeactivateKnife();

            // TODO 찬규 : 플레이어가 칼에 부딪혔을 때 낼 효과 추가 부분


            // TODO 찬규 : 공격 클라이언트를 누구 기준으로 할지

            // 1번 호스트가 아닐 경우 제외
            if (!PhotonNetwork.IsMasterClient) return;

            // 2번 칼을 던진 사람이 아닐 경우 제외
            // if (PhotonNetwork.LocalPlayer.ActorNumber != attackerActorNr) return;

            // EditPlayerState 에 있는 ReceiveDamage 함수 호출
            playerPhotonView.RPC("ReceiveDamage", RpcTarget.All, attackerActorNr, KNIFE_THROW_DAMAGE);
        }
        else
        {
            // 칼 비활성화
            DeactivateKnife();
        }
    }
    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(lifeTime);

        DeactivateKnife();
    }
    IEnumerator ApplyGravity()
    {
        yield return null;

        yield return new WaitForSeconds(gravityTime);

        GetComponent<Rigidbody>().useGravity = true;

        gravityTimeCoroutine = null;
    }
    private void DeactivateKnife()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);

            lifeTimeCoroutine = null;
        }
        if (gravityTimeCoroutine != null)
        {
            StopCoroutine(gravityTimeCoroutine);

            gravityTimeCoroutine = null;
        }
        Push();
    }
    private void LateUpdate()
    {
        // 중력을 적용하고 있을 때 (해당 코루틴이 비어있다는 뜻은 곧 중력을 적용했다는 의미)
        if (gravityTimeCoroutine == null)
        {
            transform.forward = rb.linearVelocity.normalized;

            transform.Rotate(90f, 0, 0);
        }
    }
    private void OnDisable()
    {
        Push();

        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);

            lifeTimeCoroutine = null;
        }
        if (gravityTimeCoroutine != null)
        {
            StopCoroutine(gravityTimeCoroutine);

            gravityTimeCoroutine = null;
        }
    }
    void Push()
    {
        if (inPool == false)
        {
            inPool = true;

            PoolManager.Instance.Push(gameObject);
        }
    }
}
