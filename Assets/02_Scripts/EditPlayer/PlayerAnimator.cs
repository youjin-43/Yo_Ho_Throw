using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    protected Animator playerAnimator; // 플레이어 애니메이터

    protected bool isGround = true; // 현재 isGround 상태

    protected const int HAND_LAYER = 1;

    void Start()
    {
        // Animator 가져오기
        playerAnimator = GetComponentInChildren<Animator>();

        // IsGround 적용
        playerAnimator.SetBool(AnimationParameter.IsGround.ToString(), isGround);
    }
    void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
    }
    void SetMotionSpeed(float value) => playerAnimator.SetFloat(AnimationParameter.MotionSpeed.ToString(), value);
    void SetSpeed(float value) => playerAnimator.SetFloat(AnimationParameter.Speed.ToString(), value);
    void SetHorizontal(float value) => playerAnimator.SetFloat(AnimationParameter.Horizontal.ToString(), value);
    void SetVertical(float value) => playerAnimator.SetFloat(AnimationParameter.Vertical.ToString(), value);
    void SetHorizontalRaw(float value) => playerAnimator.SetFloat(AnimationParameter.HorizontalRaw.ToString(), value);
    void SetVerticalRaw(float value) => playerAnimator.SetFloat(AnimationParameter.VerticalRaw.ToString(), value);
    public void SetTrigger(AnimationParameter animationParameter) => playerAnimator.SetTrigger(animationParameter.ToString());
    public void SetIsGround(bool isGround) // IsGround 여부 적용
    {
        // 현재 상태와 동일하다면 반환
        if (this.isGround == isGround) return;

        // 현재 상태에 적용
        this.isGround = isGround;

        // IsGround 적용
        playerAnimator.SetBool(AnimationParameter.IsGround.ToString(), isGround);
    }
    protected void OnEnable()
    {
        EditPlayerController editPlayerController = GetComponent<EditPlayerController>();

        editPlayerController.BindToSetActionMotionSpeed(SetMotionSpeed);
        editPlayerController.BindToSetActionSpeed(SetSpeed);
        editPlayerController.BindToSetActionHorizontal(SetHorizontal);
        editPlayerController.BindToSetActionHorizontalRaw(SetHorizontalRaw);
        editPlayerController.BindToSetActionVertical(SetVertical);
        editPlayerController.BindToSetActionVerticalRaw(SetVerticalRaw);
    }
    protected void OnDisable()
    {
        // 함수 구독 해제하기
        GetComponent<EditPlayerController>().UnbindFromSetActionMotionSpeed(SetMotionSpeed);
    }
    public void PlayerDieSetTrigger() => playerAnimator.SetTrigger(AnimationParameter.PlayerDie.ToString());
    public void PlayerAliveSetTrigger() => playerAnimator.SetTrigger(AnimationParameter.PlayerAlive.ToString());
    public bool IsMoveState() => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleWalkRunBlend");
}