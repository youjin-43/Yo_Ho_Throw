using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    protected Animator playerAnimator; // 플레이어 애니메이터

    protected bool isGround = true; // 현재 isGround 상태

    protected const int HAND_LAYER = 1;

    protected virtual void Start()
    {
        // Animator 가져오기
        playerAnimator = GetComponentInChildren<Animator>();
    }
    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
    }
    #region PlayerController
    protected virtual void SetMoveSpeed(float value) // 이동속도 값 적용 
    {
        // 이동 값 애니메이터에 적용
        playerAnimator.SetFloat(AnimationParameter.Move.ToString(), value);
    }
    public virtual void SetIsCrouch(bool isCrouch) // 앉은 상태 여부 적용 
    {
        playerAnimator.SetBool(AnimationParameter.IsCrouch.ToString(), isCrouch);
    }
    public virtual bool SetJumpTrigger() // 점프 애님이 불가능하면 False, 가능하면 True 반환 후 애니메이션 적용 
    {
        // 현재 쭈구리고 있을 때
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove"))
        {
            // false를 반환
            return false;
        }
        // 만약 서있다면 Jump Trigger 실행
        playerAnimator.SetTrigger(AnimationParameter.Jump.ToString());

        // true를 반환
        return true;
    }
    public virtual void SetIsGround(bool isGround) // IsGround 여부 적용
    {
        // 현재 상태와 동일하다면 반환
        if (this.isGround == isGround) return;

        // 현재 상태에 적용
        this.isGround = isGround;

        // IsGround 적용
        playerAnimator.SetBool("IsGround", isGround);
    }
    public virtual void SetWaistValue(float value)
    {
        playerAnimator.SetFloat(AnimationParameter.Waist.ToString(), value);
    }
    #endregion
    #region PlayerItemHandler
    public virtual void SetItemChangeTrigger(AnimationParameter triggerState) // 아이템 변경 시의 트리거 셋팅
    {
        playerAnimator.SetLayerWeight(HAND_LAYER, triggerState == AnimationParameter.NoItem ? 0f : 1f);

        playerAnimator.SetTrigger(triggerState.ToString());
    }
    public virtual void SetItemUseTrigger(AnimationParameter triggerState)
    {
        playerAnimator.SetTrigger(triggerState.ToString());
    }
    #endregion
    protected virtual void OnEnable()
    {
        // 함수 구독 시켜놓기
        GetComponent<PlayerController>().BindToPlayerAnimator(SetMoveSpeed);
    }
    protected virtual void OnDisable()
    {
        // 함수 구독 해제하기
        GetComponent<PlayerController>().UnbindFromPlayerAnimator(SetMoveSpeed);
    }
    void PlayerDieSetTrigger() => playerAnimator.SetTrigger(AnimationParameter.PlayerDie.ToString());
    void PlayerAliveSetTrigger() => playerAnimator.SetTrigger(AnimationParameter.PlayerAlive.ToString());
}