using UnityEngine;

public class MeleeAttackOnTrigger : MonoBehaviour
{
    private void Awake()
    {
        Rigidbody rb1 = GetComponent<Rigidbody>();
        rb1.isKinematic = true; // 물리 영향 X
        rb1.detectCollisions = true; // 충돌 감지 활성화
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bullet"))
        {
            Debug.Log("패링 성공");
            Rigidbody rb = other.GetComponent<Rigidbody>();
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

            // TODO 패링 성공 사운드 추가
            Debug.Log("패링 성공");
        }

        if (other.CompareTag("Player"))
        {
            //TODO 근접공격 성공 사운드
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.OnDamagedAnim();
            }
        }
    }
}
