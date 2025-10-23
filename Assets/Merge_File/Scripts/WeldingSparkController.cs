using UnityEngine;

public class WeldingSparkController : MonoBehaviour
{
    public ParticleSystem weldingSparks;
    public AudioSource weldingAudioSource;

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

                // 용접이 완료되었으면 GameManager에 알림
                if (guide.IsWelded)
                {
                    GameManager.Instance.OnGuideWelded();
                }
            }

            // 용접 소리 정지
            if (weldingAudioSource != null && weldingAudioSource.isPlaying)
            {
                weldingAudioSource.Stop();
            }
        }
        // 메탈플레이트나 백보드에서 떠날 때도 소리 정지
        else if (other.CompareTag("MetalPlate") || other.CompareTag("Backboard"))
        {
            // 용접 소리 정지
            if (weldingAudioSource != null && weldingAudioSource.isPlaying)
            {
                weldingAudioSource.Stop();
            }
        }
    }
}