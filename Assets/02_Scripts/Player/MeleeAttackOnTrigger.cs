using UnityEngine;

public class MeleeAttackOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // 중력 활성화
                rb.useGravity = true;
                rb.isKinematic = false; // 만약 Kinematic이 켜져 있다면 비활성화

                // 랜덤한 방향으로 튕겨나가게 함
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f), // 살짝 위쪽으로 튕기게 설정
                    Random.Range(-1f, 1f)
                ).normalized;

                float forceAmount = Random.Range(5f, 10f); // 튕기는 힘 설정
                rb.AddForce(randomDirection * forceAmount, ForceMode.Impulse);

                // 랜덤 회전 추가
                Vector3 randomTorque = new Vector3(
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f)
                );

                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }

            // TODO : 패링 성공 사운드 추가
            Debug.Log("패링 성공");
        }

        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.OnDamaged(1);
            }
        }
    }
}
