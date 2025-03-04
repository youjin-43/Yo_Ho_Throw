using Photon.Pun;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(EditPlayerState))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PlayerKnifeController))]
public class EditPlayerController : MonoBehaviourPun
{
    [Header("Move Attribute")]                  // Header 정리
    [SerializeField] float acceleration = 3f; // 이동속도 증가속도
    [SerializeField] float deceleration = 3f; // 이동속도 감소속도
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float moveSpeedLimit;

    PlayerGroundChecker playerGroundChecker;

    PlayerAnimator playerAnimator = null; // PlayerAnimator

    CharacterController characterController = null; // 플레이어 캐릭터 컨트롤러

    PhotonTransformView photonTransformView;

    Transform playerTransform = null; // 플레이어 트랜스폼

    Vector2 moveDelta = Vector2.zero; // 키보드 이동 입력 값 저장 변수

    float velocityX = 0f; // X축 Velocity
    float velocityY = 0f; // Y축 Velocity
    float velocityZ = 0f; // Z축 Velocity

    Vector3 velocity = Vector3.zero; // 이동에 적용하는 velocity

    [HideInInspector] public bool canMove = true;
    bool characterEnabled = true;
    bool isDash = false;

    float coyoteTime = 0.2f;
    float coyoteTimeCounter = 0;

    public static EditPlayerController Instance { get; private set; } = null;

    private void Awake()
    {
        if (!photonView.IsMine) return;

        Instance = this;
    }
    private void Start()
    {
        canMove = true;

        // GetComponent로 필요 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerGroundChecker = GetComponentInChildren<PlayerGroundChecker>();
        photonTransformView = GetComponent<PhotonTransformView>();

        // 플레이어 Transform 초기화
        playerTransform = transform;
    }
    private void Update()
    {
        if (!photonView.IsMine) return;

        if (!characterEnabled) return;

        if (isDash) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }

        #region Velocity X,Z 값 설정

        moveDelta.x = Input.GetAxisRaw("Horizontal");
        moveDelta.y = Input.GetAxisRaw("Vertical");

        if (!canMove) moveDelta = Vector2.zero;

        // X축의 입력이 있을 때
        if (Mathf.Abs(moveDelta.x) > 0.1f)
        {
            // 해당 방향으로 주는 힘을 증가시킨다
            velocityX += moveDelta.x * Time.deltaTime * acceleration;

            // 이동속도만큼 제한을 둔다
            if (velocityX > moveSpeedLimit) velocityX = moveSpeedLimit;
            if (velocityX < -moveSpeedLimit) velocityX = -moveSpeedLimit;
        }

        // X축의 입력이 없다면 X축의 velocity가 0f가 아니라면
        else if (velocityX != 0f)
        {
            // velocity.x가 음수라면 1, 양수라면 -1을 sign 값에 넣는다
            float sign = -Mathf.Sign(velocityX);

            // sign 값을 이용해서 양수라면 음수 값으로, 음수라면 양수 값으로 변화시킨다
            velocityX += Time.deltaTime * deceleration * sign;

            // 만약 velocity.x가 0을 지나 부호가 달라졌다면 velocity.x의 값을 0f로 초기화 한다
            if (Mathf.Sign(velocityX) == sign) velocityX = 0f;
        }

        // Y축의 입력이 있을 때
        if (Mathf.Abs(moveDelta.y) > 0.1f)
        {
            // 해당 방향으로 주는 힘을 증가시킨다
            velocityZ += moveDelta.y * Time.deltaTime * acceleration;

            // 이동속도만큼 제한을 둔다
            if (velocityZ > moveSpeedLimit) velocityZ = moveSpeedLimit;
            if (velocityZ < -moveSpeedLimit) velocityZ = -moveSpeedLimit;
        }

        // Y축의 입력이 없다면
        else if (velocityZ != 0f)
        {
            // velocity.y가 음수라면 1, 양수라면 -1을 sign 값에 넣는다
            float sign = -Mathf.Sign(velocityZ);

            // sign 값을 이용해서 양수라면 음수 값으로, 음수라면 양수 값으로 변화시킨다
            velocityZ += Time.deltaTime * deceleration * sign;

            // 만약 velocity.z가 0을 지나 부호가 달라졌다면 velocity.z의 값을 0f로 초기화 한다
            if (Mathf.Sign(velocityZ) == sign) velocityZ = 0f;
        }

        #endregion
        #region Velocity Y 값 설정

        // 플레이어가 땅에 있거나 코요테 타임 값이 남아있거나, 바로 밑에 오브젝트가 있을 때
        if (playerGroundChecker.IsGrounded || coyoteTimeCounter > 0f)
        {
            // 점프 중이 아닐 때, 앉고 있지 않을 때, 점프 키를 눌렀다면
            if (velocityY <= float.Epsilon && Input.GetButtonDown("Jump") && canMove && playerAnimator.IsMoveState())
            {
                // 코요테 타임 초기화
                coyoteTimeCounter = 0;

                // 중력을 현재 jumpSpeed로 변경하여 튀어오르게 한다
                velocityY = jumpSpeed;

                // 점프 애니메이션 트리거 및 플레이어 높이 조절
                playerAnimator.SetTrigger(AnimationParameter.Jump);

                //PlayerAudioController.PlayerAudioPlay(AudioName.PlayerJump);
            }
        }

        // 만약 캐릭터가 무언가의 위에 있지만 점프할 수 있는 Ground가 아닐 경우
        if (playerGroundChecker.IsColliderBelow && !playerGroundChecker.IsGrounded)
        {
            // 중력 적용
            velocityY = Mathf.Clamp(velocityY + Time.deltaTime * gravity, -10f, 1000f);

            // 코요테 타임 감소
            coyoteTimeCounter -= Time.deltaTime;

            if (coyoteTimeCounter <= 0f)
            {
                // 애니메이터에게 땅 위 없다고 알린다
                playerAnimator.SetIsGround(false);
            }
        }
        else if (playerGroundChecker.IsGrounded)
        {
            if (velocityY <= 0f)
            {
                // velocityY를 0으로 초기화
                velocityY = 0;

                // 애니메이터에게 땅 위에 있다고 알린다
                playerAnimator.SetIsGround(true);

                // 코요테 타임 갱신
                coyoteTimeCounter = coyoteTime;
            }
        }
        else if (!playerGroundChecker.IsColliderBelow)
        {
            // 중력 적용
            velocityY += Time.deltaTime * gravity;

            // 코요테 타임 감소
            coyoteTimeCounter -= Time.deltaTime;

            if (coyoteTimeCounter <= 0f)
            {
                // 애니메이터에게 땅 위 없다고 알린다
                playerAnimator.SetIsGround(false);
            }
        }
        

        #endregion
        #region velocity 값 갱신

        // velocity의 x와 z의 벡터 크기 값을 구한다
        // moveSpeed로 제한을 두기 전의 값인 prevMagnitude와
        // 제한을 둔 후인 currMagnitude 값을 만든다
        float prevMagnitude = new Vector2(velocityX, velocityZ).magnitude;
        float currMagnitude = prevMagnitude > moveSpeedLimit ? moveSpeedLimit : prevMagnitude;

        velocity.Set(
            prevMagnitude > float.Epsilon ? (velocityX / prevMagnitude) * currMagnitude : 0f,
            velocityY,
            prevMagnitude > float.Epsilon ? (velocityZ / prevMagnitude) * currMagnitude : 0f);

        #endregion

        #region 이동 값 적용

        animHorizontalSetAction?.Invoke(velocityX == 0 ? 0 : velocityX / moveSpeedLimit);
        animVerticalSetAction?.Invoke(velocityZ == 0 ? 0 : velocityZ / moveSpeedLimit);

        animMotionSpeedSetAction?.Invoke(
            Mathf.Max(Mathf.Abs(velocityX), Mathf.Abs(velocityZ)) == 0 ?
            0 : Mathf.Max(Mathf.Abs(velocityX), Mathf.Abs(velocityZ)) / moveSpeedLimit);

        // Animator 이동속도 값 제공
        animSpeedSetAction?.Invoke(currMagnitude);

        // AudioController 값 제공
        audioMoveValueAddAction?.Invoke(currMagnitude);

        // 플레이어 이동 적용
        characterController.Move(playerTransform.rotation * new Vector3(velocity.x, velocity.y, velocity.z) * Time.deltaTime);

        #endregion
    }
    public void DisableMovement() => canMove = false;
    public void EnableMovement() => canMove = true; // 움직임 활성화
    public void MovePosition(Transform targetTransform)
    {
        StartCoroutine(MovePositionCoroutine(targetTransform));
    }
    IEnumerator MovePositionCoroutine(Transform targetTransform)
    {
        DisableMovement();

        characterController.enabled = false;

        transform.position = targetTransform.position;

        transform.rotation = targetTransform.rotation;

        characterEnabled = false;

        yield return null;

        characterEnabled = true;

        EnableMovement();

        characterController.enabled = true;
    }

    public void Dash()
    {
        if (photonTransformView != null)
        {
            photonTransformView.enabled = false;
        }

        photonView.RPC("Dash_RPC", RpcTarget.All, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    [PunRPC]
    public void Dash_RPC(float horizontal, float vertical)
    {
        isDash = true;

        playerAnimator.SetTrigger(AnimationParameter.Dash);

        Vector3 dashDirection = transform.forward * vertical + transform.right * horizontal;
        dashDirection.Normalize();

        animHorizontalRawSetAction?.Invoke(horizontal);
        animVerticalRawSetAction?.Invoke(vertical);

        StartCoroutine(DashMovement_RPC(dashDirection));
    }
    IEnumerator DashMovement_RPC(Vector3 direction)
    {
        float dashDistance = 3f;
        float dashTime = 0.4915f;
        float elapsedTime = 0f;

        Vector3 velocity = direction * (dashDistance / dashTime);

        while (elapsedTime < dashTime)
        {
            characterController.Move(velocity * Time.deltaTime);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        photonTransformView.enabled = true; // ✅ 보간 다시 활성화

        isDash = false;
    }

    [PunRPC]
    public void OnInLobby()
    {
        GetComponent<PlayerKnifeController>().OnInLobby();
        GetComponent<EditPlayerState>().OnInLobby();
    }
    [PunRPC]
    public void OnOutLobby()
    {
        GetComponent<PlayerKnifeController>().OnOutLobby();
        GetComponent<EditPlayerState>().OnOutLobby();
    }
    #region 플레이어 애니메이터 연결 Action 바인드

    event Action<float> animMotionSpeedSetAction = null; // 애니메이터 모션 속도 설정 Action
    event Action<float> animSpeedSetAction = null;
    event Action<float> animHorizontalSetAction = null;
    event Action<float> animVerticalSetAction = null;
    event Action<float> animHorizontalRawSetAction = null;
    event Action<float> animVerticalRawSetAction = null;
    event Action<float> audioMoveValueAddAction = null; // 발걸음 소리를 위한 Action

    public void BindToSetActionMotionSpeed(Action<float> action) => animMotionSpeedSetAction += action;
    public void UnbindFromSetActionMotionSpeed(Action<float> action) => animMotionSpeedSetAction -= action;
    public void BindToSetActionSpeed(Action<float> action) => animSpeedSetAction += action;
    public void UnbindFromSetActionSpeed(Action<float> action) => animSpeedSetAction -= action;
    public void BindToSetActionHorizontal(Action<float> action) => animHorizontalSetAction += action;
    public void UnbindFromSetActionHorizontal(Action<float> action) => animHorizontalSetAction -= action;
    public void BindToSetActionVertical(Action<float> action) => animVerticalSetAction += action;
    public void UnbindFromSetActionVertical(Action<float> action) => animVerticalSetAction -= action;
    public void BindToSetActionHorizontalRaw(Action<float> action) => animHorizontalRawSetAction += action;
    public void UnbindFromSetActionHorizontalRaw(Action<float> action) => animHorizontalRawSetAction -= action;
    public void BindToSetActionVerticalRaw(Action<float> action) => animVerticalRawSetAction += action;
    public void UnbindFromSetActionVerticalRaw(Action<float> action) => animVerticalRawSetAction -= action;
    public void BindToPlayerAudioController(Action<float> action) => audioMoveValueAddAction += action;
    public void UnbindFromAudioController(Action<float> action) => audioMoveValueAddAction -= action;
    #endregion
}
