using System;
using System.Collections;
using UnityEngine;

public class EditPlayerController : MonoBehaviour
{
    [Header("Need Reference")] // Header 정리
    [SerializeField] Transform cameraTransform; // 카메라의 Transform
    [SerializeField] Transform playerPivotTransform; // 플레이어 피벗 Transform
    [SerializeField] Transform initPosition;
    [SerializeField] PlayerGroundChecker playerGroundChecker;

    [Header("Move Attribute")]                  // Header 정리
    [SerializeField] float walkMoveSpeed = 3.5f; // 걷기 이동속도
    [SerializeField] float runMoveSpeed = 7f; // 뛰기 이동속도
    [SerializeField] float acceleration = 3f; // 이동속도 증가속도
    [SerializeField] float deceleration = 3f; // 이동속도 감소속도
    [SerializeField] float crouchRatioSpeed = 0.5f; // 앉을 때의 이속 비율
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float oxygenBurnRate = 0.5f;
    [SerializeField] float moveSpeedLimit;

    [Header("Rotate Attribute")] // Header 정리
    [SerializeField] float pitchClampMax = 70f; // 카메라 각도 최대값
    [SerializeField] float pitchClampMin = -70f; // 카메라 각도 최대값
    [SerializeField] float rotateSpeed = 10f; // 회전 속도
    [SerializeField] float cameraCrouchHeight = 0.891f; // 앉아있을 때의 카메라 높이

    [Header("JumpPivot Attribute")] // Header 정리
    [SerializeField] float maxJumpHeightTime = 0.3f; // 점프 했을 시 최대로 높이 올라가는 시간

    PlayerAnimator playerAnimator = null; // PlayerAnimator
    CharacterController characterController = null; // 플레이어 캐릭터 컨트롤러
    Transform playerTransform = null; // 플레이어 트랜스폼

    Vector2 moveDelta = Vector2.zero; // 키보드 이동 입력 값 저장 변수
    Vector2 rotateDelta = Vector2.zero; // 마우스 이동 입력 값 저장 변수

    float velocityX = 0f; // X축 Velocity
    float velocityY = 0f; // Y축 Velocity
    float velocityZ = 0f; // Z축 Velocity

    float shoesSpeed = 1f; // 신발 속도 (강화된 신발 shoesSpeed = 1.15f)

    Vector3 velocity = Vector3.zero; // 이동에 적용하는 velocity

    float currCameraAngle = 0f; // 카메라 x축 회전 값
    float cameraStandHeight; // 서있을 때의 카메라 높이
    float currCameraHeight; // 현재 카메라 높이

    Coroutine cameraHeightCoroutine = null;

    bool isCrouch = false; // 앉은 상태 여부
    bool canMove = false;
    bool characterEnabled = true;

    event Action<float> animMoveSpeedSetAction = null; // 애니메이터 이동속도 설정 Action
    event Action<float> audioMoveValueAddAction = null; // 발걸음 소리를 위한 Action

    float coyoteTime = 0.2f;
    float coyoteTimeCounter = 0;
    float jumpPivotHeight = -0.3f; // 점프 했을 시의 높이
    float standPivotHeight = -0.024f; // 서있을 때의 높이

    Vector3 pivotLocalPosition = Vector3.zero; // 플레이어 로컬 피벗 Y축 적용 Vector3

    void InitPosition()
    {
        StartCoroutine(InitPositionCoroutine());
    }
    IEnumerator InitPositionCoroutine()
    {
        characterEnabled = false;

        characterController.enabled = false;

        transform.position = initPosition.position;
        transform.rotation = initPosition.rotation;

        yield return null;

        characterEnabled = true;

        characterController.enabled = true;
    }

    private void Start()
    {
        // GetComponent로 필요 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<PlayerAnimator>();

        // 플레이어 Transform 초기화
        playerTransform = transform;

        // 서있을 때의 카메라 높이
        cameraStandHeight = 1.657f;
        //cameraStandHeight = cameraTransform.localPosition.y;

        // 현재 카메라 높이 설정
        currCameraHeight = cameraStandHeight;
    }
    private void Update()
    {
        if (!characterEnabled) return;

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
            if (velocityY <= float.Epsilon && Input.GetButtonDown("Jump") && canMove && !isCrouch)
            {
                // 코요테 타임 초기화
                coyoteTimeCounter = 0;

                // 중력을 현재 jumpSpeed로 변경하여 튀어오르게 한다
                velocityY = jumpSpeed;

                // 점프 애니메이션 트리거 및 플레이어 높이 조절
                PlayerJumpHeightAnim(playerAnimator.SetJumpTrigger());

                //PlayerAudioController.PlayerAudioPlay(AudioName.PlayerJump);
            }
        }

        // 만약 캐릭터가 무언가 위에 있지만 점프할 수 있는 Ground가 아닐 경우
        if (playerGroundChecker.IsColliderBelow && !playerGroundChecker.IsGrounded)
        {
            // 중력 적용
            velocityY = Mathf.Clamp(velocityY + Time.deltaTime * gravity, -10f, 1000f);

            // 애니메이터에게 땅 위 없다고 알린다
            playerAnimator.SetIsGround(false);

            // 코요테 타임 감소
            coyoteTimeCounter -= Time.deltaTime;
        }
        else if (!playerGroundChecker.IsColliderBelow)
        {
            // 중력 적용
            velocityY += Time.deltaTime * gravity;

            // 애니메이터에게 땅 위 없다고 알린다
            playerAnimator.SetIsGround(false);

            // 코요테 타임 감소
            coyoteTimeCounter -= Time.deltaTime;
        }
        else if (playerGroundChecker.IsGrounded)
        {
            if (velocityY <= 0f)
            {
                // velocityY를 0으로 초기화
                velocityY = 0;

                // 애니메이터에게 땅 위에 있다고 알린다
                playerAnimator.SetIsGround(true);

                // 땅 위에 있으므로 피벗을 standPivotHeight 위치로 적용한다
                pivotLocalPosition.y = standPivotHeight;
                playerPivotTransform.localPosition = pivotLocalPosition;

                // 코요테 타임 갱신
                coyoteTimeCounter = coyoteTime;
            }
        }

        /*
        // 만약 캐릭터가 땅 위에 없다면
        else
        {
            // 코요테 타임 감소
            coyoteTimeCounter -= Time.deltaTime;

            // 중력 적용
            velocityY += Time.deltaTime * gravity;

            // 애니메이터에게 땅 위에 없다고 알림
            playerAnimator.SetIsGround(false);

            // 만약 아래에 맞는 오브젝트가 없다면
            if (playerGroundChecker.IsColliderBelow)
            {
                // 중력 적용
                velocityY += Time.deltaTime * gravity;

                // 애니메이터에게 땅 위에 없다고 알림
                playerAnimator.SetIsGround(false);
            }
        }

        // 캐릭터가 떨어지고 있거나 가만히 있을 때
        if (velocityY <= 0f)
        {
            // velocityY를 0으로 초기화
            velocityY = 0;

            // 애니메이터에게 땅 위에 있다고 알린다
            playerAnimator.SetIsGround(true);

            // 땅 위에 있으므로 피벗을 standPivotHeight 위치로 적용한다
            pivotLocalPosition.y = standPivotHeight;
            playerPivotTransform.localPosition = pivotLocalPosition;
        }

        // 코요테 타임 갱신
        coyoteTimeCounter = coyoteTime;
*/

        #endregion
        #region velocity 값 갱신

        // velocity의 x와 z의 벡터 크기 값을 구한다
        // moveSpeed로 제한을 두기 전의 값인 prevMagnitude와
        // 제한을 둔 후인 currMagnitude 값을 만든다
        float prevMagnitude = new Vector2(velocityX, velocityZ).magnitude;
        float currMagnitude = prevMagnitude > moveSpeedLimit ? moveSpeedLimit : prevMagnitude;

        velocity.Set(
            prevMagnitude > float.Epsilon ? (velocityX / prevMagnitude) * currMagnitude * shoesSpeed : 0f,
            velocityY,
            prevMagnitude > float.Epsilon ? (velocityZ / prevMagnitude) * currMagnitude * shoesSpeed : 0f);

        #endregion
        #region 카메라 회전

        // 마우스 이동 값 가져오기
        rotateDelta.x = Input.GetAxisRaw("Mouse X");
        rotateDelta.y = Input.GetAxisRaw("Mouse Y");

        if (!canMove) rotateDelta = Vector2.zero;

        // 마우스 이동 값이 있을 때
        if (rotateDelta != Vector2.zero)
        {
            // 마우스 상하 이동 값은 up:1, down:-1이라 생각하면 되는데
            // 실제로 카메라의 x축 회전을 적용했을 때는 up:-1, down:1 값을 해야하기 때문에 
            // mouseMoveValue.y에 -1f을 곱해줌으로써 값을 맞춰준다
            rotateDelta.y *= -1f;

            // 카메라의 회전은 Clamp를 이용해서 제한을 한다
            currCameraAngle = Mathf.Clamp(
                currCameraAngle + (rotateDelta.y * Time.deltaTime * rotateSpeed),
                pitchClampMin,
                pitchClampMax);

            playerAnimator.SetWaistValue(currCameraAngle);

            // 카메라의 상하 회전은 Transform eulerAngle X에 적용
            // 좌우 회전은 playerTransform.eulerAngles.y를 가져옴
            cameraTransform.rotation = Quaternion.Euler(currCameraAngle, playerTransform.eulerAngles.y, 0f);

            // 플레이어 트랜스폼에는 x축의 마우스 회전 값만 적용한다
            playerTransform.Rotate(0, rotateDelta.x * rotateSpeed * Time.deltaTime, 0);
        }
        #endregion

        #region 이동 값 적용

        // Animator 이동속도 값 제공
        animMoveSpeedSetAction?.Invoke(currMagnitude);

        // AudioController 값 제공
        audioMoveValueAddAction?.Invoke(!isCrouch ? currMagnitude : currMagnitude * crouchRatioSpeed);

        // 서있을 때의 이동 적용
        if (!isCrouch)
        {
            // 플레이어 이동 적용
            characterController.Move(playerTransform.rotation * velocity * Time.deltaTime);
        }

        // 앉아 있을 때의 이동 적용 
        else
        {
            // 플레이어 이동 적용
            characterController.Move(playerTransform.rotation * new Vector3(velocity.x * crouchRatioSpeed, velocity.y, velocity.z * crouchRatioSpeed) * Time.deltaTime);
        }
        #endregion
    }
    void CrouchStateUpdate(bool isCrouch) // 앉은 상태 갱신
    {
        // 상태 갱신
        this.isCrouch = isCrouch;

        // 애니메이터에게 알림
        playerAnimator.SetIsCrouch(isCrouch);

        // 변경 중인 카메라 코루틴 중지
        if (cameraHeightCoroutine != null)
        {
            StopCoroutine(cameraHeightCoroutine);
        }

        // 카메라 높이 변경 코루틴 실행
        cameraHeightCoroutine = StartCoroutine(CameraHeightAnimCoroutine(isCrouch));
    }
    void PlayerJumpHeightAnim(bool isJump) // 점프 시의 피벗 애니메이션
    {
        // 점프가 트리거 되지 않았다면 실행하지 않는다
        if (!isJump) return;

        // 점프 시의 위치 조절 Coroutine을 실행
        StartCoroutine(PlayerJumpHeightAnimCoroutine());
    }
    IEnumerator PlayerJumpHeightAnimCoroutine() // 피벗 애니메이션 코루틴
    {
        // 시간 값
        float t = 0;

        // 0초부터 공중에 있는 시간만큼 진행
        while (t < maxJumpHeightTime)
        {
            // 시간 값 누적
            t += Time.deltaTime;

            // 피벗 로컬 y 포지션을 서있는 상태와 점프한 상태의 기준 높이를 Lerp로 공중에 있는 시간
            pivotLocalPosition.y = Mathf.Lerp(standPivotHeight, jumpPivotHeight, Mathf.InverseLerp(0, maxJumpHeightTime, t));

            // 플레이어 피벗 트랜스폼에 로컬 포지션 적용
            playerPivotTransform.localPosition = pivotLocalPosition;

            // 다음 프레임 대기
            yield return null;
        }
    }
    IEnumerator CameraHeightAnimCoroutine(bool isCrouch) // 카메라 높이 변경 코루틴
    {
        Vector3 cameraLocalPosition = cameraTransform.localPosition;

        // 앉아있을 경우
        if (isCrouch)
        {
            for (; currCameraHeight > cameraCrouchHeight; currCameraHeight -= Time.deltaTime * 7f)
            {
                // 카메라의 로컬 좌표 가져오기
                cameraLocalPosition = cameraTransform.localPosition;

                // 카메라의 로컬 y축 값 변경하기
                cameraLocalPosition.y = currCameraHeight;

                // 카메라의 로컬 좌표에 적용하기
                cameraTransform.localPosition = cameraLocalPosition;

                // 다음 프레임까지 대기
                yield return null;
            }
            // 카메라의 로컬 좌표 cameraCrouchHeight로 정확히 값 맞춰놓기
            cameraLocalPosition.y = cameraCrouchHeight;
        }

        // 서있을 경우
        else
        {
            for (; currCameraHeight < cameraStandHeight; currCameraHeight += Time.deltaTime * 7f)
            {
                // 카메라의 로컬 좌표 가져오기
                cameraLocalPosition = cameraTransform.localPosition;

                // 카메라의 로컬 y축 값 변경하기
                cameraLocalPosition.y = currCameraHeight;

                // 카메라의 로컬 좌표에 적용하기
                cameraTransform.localPosition = cameraLocalPosition;

                // 다음 프레임까지 대기
                yield return null;
            }
            // 카메라의 로컬 좌표 cameraStandHeight로 정확히 값 맞춰놓기
            cameraLocalPosition.y = cameraStandHeight;
        }
        // 정확히 맞춰놓은 값으로 카메라의 로컬 좌표에 적용하기
        cameraTransform.localPosition = cameraLocalPosition;
    }
    public void EquipEnhancedShoes() => shoesSpeed = 1.15f;
    public void DisableMovement() // 움직임 비활성화
    {
        canMove = false;
    }
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
    #region 플레이어 이동속도 설정 Action 바인드
    public void BindToPlayerAnimator(Action<float> action)
    {
        animMoveSpeedSetAction += action;
    }
    public void UnbindFromPlayerAnimator(Action<float> action)
    {
        animMoveSpeedSetAction -= action;
    }
    #region 플레이어 오디오 컨트롤러 Action 바인드
    #endregion
    public void BindToPlayerAudioController(Action<float> action)
    {
        audioMoveValueAddAction += action;
    }
    public void UnbindFromAudioController(Action<float> action)
    {
        audioMoveValueAddAction -= action;
    }
    #endregion

}
