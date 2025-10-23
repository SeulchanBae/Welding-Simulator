using UnityEngine;

public class WeldingMaskController : MonoBehaviour
{
    private Transform cameraTransform;

    [Header("마스크 설정")]
    [Tooltip("카메라 앞에 마스크를 배치할 위치 오프셋입니다.")]
    public Vector3 maskOffset = new Vector3(0, 0, 0.3f);

    [Tooltip("마스크를 떼어낼 때 사용할 컨트롤러 버튼입니다.")]
    public OVRInput.Button detachButton = OVRInput.Button.One;

    private GameObject sparkBlocker; // 스파크 차단용 오브젝트

    private bool isAttached = false;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    void Awake()
    {
        GameObject playerHead = GameObject.FindGameObjectWithTag("PlayerHead");

        if (playerHead != null)
        {
            cameraTransform = playerHead.transform;
        }
        else
        {
            Debug.LogError("경고: 씬에 'PlayerHead' 태그를 가진 오브젝트가 없습니다.");
        }

        // 마스크에 Collider 추가 (스파크 파티클이 충돌하도록)
        EnsureMaskHasCollider();
    }

    void EnsureMaskHasCollider()
    {
        // 마스크에 MeshCollider나 BoxCollider가 없으면 자동으로 추가
        Collider existingCollider = GetComponent<Collider>();
        if (existingCollider == null)
        {
            // MeshFilter가 있으면 MeshCollider 추가
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = false;
                Debug.Log("마스크에 MeshCollider를 추가했습니다 (스파크 차단용)");
            }
            else
            {
                // MeshFilter가 없으면 BoxCollider 추가
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                Debug.Log("마스크에 BoxCollider를 추가했습니다 (스파크 차단용)");
            }
        }
        else
        {
            Debug.Log("마스크에 이미 Collider가 있습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cameraTransform == null) return;

        if (!isAttached && other.CompareTag("PlayerHead"))
        {
            AttachMask();
        }
    }

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

        // 스파크 차단용 투명 벽 생성 (카메라 바로 앞)
        CreateSparkBlocker();
    }

    void CreateSparkBlocker()
    {
        if (sparkBlocker == null && cameraTransform != null)
        {
            // 카메라 앞에 투명한 쿼드 생성 (스파크 차단용)
            sparkBlocker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            sparkBlocker.name = "SparkBlocker";
            sparkBlocker.transform.SetParent(cameraTransform);
            sparkBlocker.transform.localPosition = new Vector3(0, 0, 0.05f); // 카메라 바로 앞
            sparkBlocker.transform.localRotation = Quaternion.identity;
            sparkBlocker.transform.localScale = new Vector3(2f, 2f, 1f); // 충분히 큰 크기

            // 완전 투명한 Material 적용 (보이지는 않지만 파티클은 막음)
            Renderer renderer = sparkBlocker.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material transparentMat = new Material(Shader.Find("Unlit/Transparent"));
                transparentMat.color = new Color(0, 0, 0, 0); // 완전 투명
                renderer.material = transparentMat;
            }

            // Collider는 제거 (물리 충돌은 필요 없음)
            Collider blockerCollider = sparkBlocker.GetComponent<Collider>();
            if (blockerCollider != null)
            {
                Destroy(blockerCollider);
            }

            Debug.Log("스파크 차단 벽 생성 완료");
        }
    }

    void DetachMask()
    {
        isAttached = false;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        // 스파크 차단 벽 제거
        if (sparkBlocker != null)
        {
            Destroy(sparkBlocker);
            sparkBlocker = null;
            Debug.Log("스파크 차단 벽 제거 완료");
        }
    }
}
