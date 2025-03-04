using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviourPun
{
    [SerializeField] Transform playerTransform;
    public Transform targetTransform;

    [SerializeField] float pitchClampMax = 70f; // 최대 값
    [SerializeField] float pitchClampMin = -70f; // 최소 값
    [SerializeField] float rotateSpeedX = 10f; // 회전 속도
    [SerializeField] float rotateSpeedY = 10f; // 회전 속도

    float angleX = 0f; // 카메라 x축 회전 값

    Vector2 rotateDelta = Vector2.zero; // 마우스 이동 입력 값 저장 변수

    private void Update()
    {
        if (!photonView.IsMine) return;

        #region 카메라 회전

        // 마우스 이동 값 가져오기
        rotateDelta.x = Input.GetAxis("Mouse X");
        rotateDelta.y = Input.GetAxis("Mouse Y");

        // 마우스 이동 값이 있을 때
        if (rotateDelta != Vector2.zero)
        {
            // 마우스 상하 이동 값은 up:1, down:-1이라 생각하면 되는데
            // 실제로 카메라의 x축 회전을 적용했을 때는 up:-1, down:1 값을 해야하기 때문에 
            // mouseMoveValue.y에 -1f을 곱해줌으로써 값을 맞춰준다
            rotateDelta.y *= -1f;

            // 카메라의 회전은 Clamp를 이용해서 제한을 한다
            angleX = Mathf.Clamp(
                angleX + (rotateDelta.y * rotateSpeedX),
                pitchClampMin,
                pitchClampMax);

            //playerAnimator.SetWaistValue(currCameraAngle);

            // 카메라의 상하 회전은 Transform eulerAngle X에 적용
            // 좌우 회전은 playerTransform.eulerAngles.y를 가져옴
            targetTransform.rotation = Quaternion.Euler(angleX, playerTransform.eulerAngles.y, 0f);

            // 플레이어 트랜스폼에는 x축의 마우스 회전 값만 적용한다
            playerTransform.Rotate(0, rotateDelta.x * rotateSpeedY * Time.deltaTime, 0);
        }
        #endregion
    }
}