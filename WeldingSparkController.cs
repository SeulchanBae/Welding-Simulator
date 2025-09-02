using UnityEngine;

public class WeldingSparkController : MonoBehaviour
{
    public ParticleSystem sparkParticleSystem;
    private bool isWelding = false;

    void Start()
    {
        if (sparkParticleSystem != null)
        {
            sparkParticleSystem.Stop();
        }
    }

    // 다른 오브젝트와 충돌하기 시작했을 때
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MetalPlate")) 
        {
            StartWelding();
        }
    }

    // 다른 오브젝트와 충돌이 끝났을 때
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MetalPlate"))
        {
            StopWelding();
        }
    }

    public void StartWelding()
    {
        if (!isWelding && sparkParticleSystem != null)
        {
            sparkParticleSystem.Play();
            isWelding = true;
            Debug.Log("용접 시작! 불꽃 발생.");
        }
    }

    public void StopWelding()
    {
        if (isWelding && sparkParticleSystem != null)
        {
            sparkParticleSystem.Stop();
            isWelding = false;
            Debug.Log("용접 종료! 불꽃 멈춤.");
        }
    }
}