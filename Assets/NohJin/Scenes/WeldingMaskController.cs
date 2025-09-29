using UnityEngine;

public class WeldingMaskController : MonoBehaviour
{
    //  Inspector에서 연결할 필요가 없으므로 private으로 변경
    private Transform cameraTransform;

    [Header("마스크 설정")]
    [Tooltip("카메라 앞에 마스크가 고정될 위치 오프셋입니다.")]
    public Vector3 maskOffset = new Vector3(0, 0, 0.3f);

    [Tooltip("마스크를 해제할 때 사용할 컨트롤러 버튼입니다.")]
    public OVRInput.Button detachButton = OVRInput.Button.One;

    private bool isAttached = false;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    //  게임이 시작될 때 단 한번, 스스로 카메라를 찾기 위해 Awake 함수 추가
    void Awake()
    {
        // "PlayerHead"라는 태그를 가진 게임 오브젝트를 씬에서 찾습니다.
        GameObject playerHead = GameObject.FindGameObjectWithTag("PlayerHead");

        // 만약 성공적으로 찾았다면
        if (playerHead != null)
        {
            // 찾은 오브젝트의 Transform 정보를 cameraTransform 변수에 할당합니다.
            cameraTransform = playerHead.transform;
        }
        else
        {
            // 만약 못찾았다면, 에러 메시지를 콘솔에 출력해서 문제 해결을 돕습니다.
            Debug.LogError("오류: 씬에 'PlayerHead' 태그를 가진 오브젝트가 없습니다. CenterEyeAnchor에 태그를 설정했는지 확인해주세요.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //  cameraTransform이 할당되지 않았다면 함수를 실행하지 않도록 방어 코드 추가
        if (cameraTransform == null) return;

        if (!isAttached && other.CompareTag("PlayerHead"))
        {
            AttachMask();
        }
    }

    // (Update, AttachMask, DetachMask 함수는 이전과 동일)

    void Update()
    {
        if (isAttached && OVRInput.GetDown(detachButton))
        {
            DetachMask();
        }
    }

    void AttachMask()
    {
        isAttached = true;

        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        transform.SetParent(cameraTransform);
        transform.localPosition = maskOffset;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    void DetachMask()
    {
        isAttached = false;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }
}