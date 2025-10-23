using UnityEngine;

public class WeldingSparkController : MonoBehaviour
{
    public ParticleSystem weldingSparks;
    public AudioSource weldingAudioSource;

    [Header("스파크 크기 설정")]
    [Tooltip("스파크 파티클 크기 배율 (기본 1.0, 작을수록 작아짐)")]
    public float sparkSizeMultiplier = 0.15f;

    void Start()
    {
        // 스파크 크기 조정
        if (weldingSparks != null)
        {
            var main = weldingSparks.main;
            main.startSizeMultiplier *= sparkSizeMultiplier;
            main.loop = true; // 계속 반복되도록 설정

            // 스파크가 카메라를 뚫고 들어오지 않도록 Collision 활성화
            var collision = weldingSparks.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;

            Debug.Log($"스파크 크기를 {sparkSizeMultiplier * 100}%로 조정했습니다.");
        }

        // 오디오 소스도 반복 재생되도록 설정
        if (weldingAudioSource != null)
        {
            weldingAudioSource.loop = true;
        }

        // 용접봉이 프리팹을 뚫지 못하도록 Rigidbody 설정
        EnsureWeldingRodPhysics();
    }

    void EnsureWeldingRodPhysics()
    {
        // Rigidbody가 없으면 추가
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Rigidbody 설정
        rb.useGravity = false; // 중력 끄기 (VR 컨트롤러로 제어)
        rb.isKinematic = false; // Kinematic 끄기 (물리 충돌 활성화)
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 빠른 움직임에도 충돌 감지

        Debug.Log("용접봉 물리 설정 완료 (프리팹 뚫림 방지)");
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 용접봉에 닿은 객체가 가이드라인인지 확인
        if (other.CompareTag("WeldingGuide"))
        {
            WeldingGuideColorChanger guide = other.GetComponent<WeldingGuideColorChanger>();
            if (guide != null && !guide.IsWelded)
            {
                // 스파크 발생
                weldingSparks.Play();

                // 용접 소리 재생
                if (weldingAudioSource != null && !weldingAudioSource.isPlaying)
                {
                    weldingAudioSource.Play();
                }

                // 용접 시작 (시간 측정 시작)
                guide.StartWelding();
            }
        }
        // 메탈플레이트인지 확인
        else if (other.CompareTag("MetalPlate"))
        {
            // 스파크 발생 및 게임 관리자에 알림 (감점)
            weldingSparks.Play();

            // 용접 소리 재생
            if (weldingAudioSource != null && !weldingAudioSource.isPlaying)
            {
                weldingAudioSource.Play();
            }

            GameManager.Instance.OnWrongContact();
        }
        // 백보드인지 확인 (너무 깊이 들어간 경우)
        else if (other.CompareTag("Backboard"))
        {
            // 스파크 발생 및 깊이 점수 감점
            weldingSparks.Play();

            // 용접 소리 재생
            if (weldingAudioSource != null && !weldingAudioSource.isPlaying)
            {
                weldingAudioSource.Play();
            }

            GameManager.Instance.OnBackboardContact();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 용접봉이 가이드라인에서 떠날 때
        if (other.CompareTag("WeldingGuide"))
        {
            WeldingGuideColorChanger guide = other.GetComponent<WeldingGuideColorChanger>();
            if (guide != null)
            {
                // 접촉 종료
                guide.EndWelding();

                // GameManager 알림은 WeldingGuideColorChanger.UpdateWeldingProgress()에서 처리하므로 여기서는 제거
            }

            // 스파크 정지
            if (weldingSparks != null && weldingSparks.isPlaying)
            {
                weldingSparks.Stop();
            }

            // 용접 소리 정지
            if (weldingAudioSource != null && weldingAudioSource.isPlaying)
            {
                weldingAudioSource.Stop();
            }
        }
        // 메탈플레이트나 백보드에서 떠날 때도 소리와 스파크 정지
        else if (other.CompareTag("MetalPlate") || other.CompareTag("Backboard"))
        {
            // 스파크 정지
            if (weldingSparks != null && weldingSparks.isPlaying)
            {
                weldingSparks.Stop();
            }

            // 용접 소리 정지
            if (weldingAudioSource != null && weldingAudioSource.isPlaying)
            {
                weldingAudioSource.Stop();
            }
        }
    }
}