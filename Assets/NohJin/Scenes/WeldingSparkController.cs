using UnityEngine;

public class WeldingSparkController : MonoBehaviour
{
    public ParticleSystem weldingSparks;

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
                // 스파크 발생 및 게임 관리자에 알림
                weldingSparks.Play();
                GameManager.Instance.OnSparkFired();
                
                // 가이드라인에게 색상 변경을 지시
                guide.WeldGuide();
            }
        }
        // 메탈플레이트인지 확인
        else if (other.CompareTag("MetalPlate"))
        {
            // 스파크 발생 및 게임 관리자에 알림 (감점)
            weldingSparks.Play();
            GameManager.Instance.OnWrongContact();
            GameManager.Instance.OnSparkFired();
        }
    }
}