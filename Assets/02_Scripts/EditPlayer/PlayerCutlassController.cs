using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCutlassController : MonoBehaviourPun
{
    const int MAX_KNIFE_COUNT = 5;

    [SerializeField] GameObject meleeAttackCollider;

    [SerializeField] Transform fireTransform;

    [HideInInspector] public Transform dirTransform;
    [HideInInspector] public Transform cameraTransform;

    [SerializeField] GameObject knifePrefab;

    [SerializeField] float throwKnifeSpeed;
    [SerializeField] float throwKnifeRange = 10f;
    [SerializeField] float gravityTime = 1f;

    [SerializeField] float meleeAttackDuration;

    EditPlayerController playerController = null;

    PlayerAnimator playerAnimator = null;

    bool CanMove => playerController.canMove;

    int KnifeCount
    {
        get => knifeCount;
        set
        {
            if (knifeCount > value)
            {
                //for (; knifeCount == value; knifeCount--)
                //    InGameUIManager.Instance.SkillIndicator.RemoveDagger();
            }
            else if (knifeCount < value)
            {
                InGameUIManager.Instance.SkillIndicator.AddDagger(value - knifeCount);

                knifeCount = value;
            }
        }
    }

    bool isAttackAction = false;
    bool isInLobby = false;

    int knifeCount = MAX_KNIFE_COUNT;

    private void Start()
    {
        meleeAttackCollider.SetActive(false);

        playerController = GetComponent<EditPlayerController>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (!CanMove) return;

        #region 단검 공격

        // 단검 투척 수행
        if (Input.GetMouseButtonDown(0)) ThrowKnife();

        // 근접 공격 수행
        if (Input.GetMouseButtonDown(1)) KnifeSwing();

        #endregion
    }
    void ThrowKnife()
    {
        if (isAttackAction) return;

        if (knifeCount == 0) return;

        isAttackAction = true;

        if (!isInLobby) InGameUIManager.Instance.SkillIndicator.StartCooldownEffect(1, 1f);

        playerAnimator.SetTrigger(AnimationParameter.Shoot);
    }
    void KnifeSwing()
    {
        if (isAttackAction) return;

        if (knifeCount == 0) return;

        isAttackAction = true;

        if (!isInLobby) InGameUIManager.Instance.SkillIndicator.StartCooldownEffect(0, 1f);

        playerAnimator.SetTrigger(AnimationParameter.MeleeAttack);
    }
    public void ThrowKnifeAction()
    {
        if (!photonView.IsMine) return;

        if (!isInLobby) KnifeCount--;

        photonView.RPC(
            "ThrowKnifeRPC",
            RpcTarget.All,
            fireTransform.position,
            ((cameraTransform.forward * throwKnifeRange + cameraTransform.position) - fireTransform.position).normalized,
            PhotonNetwork.LocalPlayer.ActorNumber);
    }
    public void KnifeSwingAction()
    {
        meleeAttackCollider.SetActive(true);

        StartCoroutine(DeactiveGameObjectCoroutine(meleeAttackDuration, meleeAttackCollider));
    }
    IEnumerator DeactiveGameObjectCoroutine(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(false);
    }
    [PunRPC]
    public void ThrowKnifeRPC(Vector3 pos, Vector3 dir, int attackerActorNr)
    {
        // 칼 오브젝트 생성 
        GameObject knifeGameObject = PoolManager.Instance.Pop(knifePrefab);

        if (knifeGameObject == null) 
        {
            Debug.LogWarning("knife null");

            return;
        }

        knifeGameObject.transform.position = pos;
        knifeGameObject.transform.forward = dir;

        knifeGameObject.transform.Rotate(-35f, 0, 0);

        knifeGameObject.GetComponentInChildren<Cutlass>().attackerActorNr = attackerActorNr;
        //knifeGameObject.GetComponent<Knife>().gravityTime = gravityTime;
        
        Rigidbody rb = knifeGameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;

            rb.linearVelocity = dir * throwKnifeSpeed;
        }
    }
    public void OnIdleState()
    {
        isAttackAction = false;
    }
    public void OnInLobby() => isInLobby = true;
    public void OnOutLobby() => isInLobby = false;
}