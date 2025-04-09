using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeldingInteraction : MonoBehaviour
{
    public Transform leftControllerTransform; // 왼손 컨트롤러
    public Transform rightControllerTransform; // 오른손 컨트롤러
    public Text distanceText; // 거리 텍스트 UI
    public LayerMask weldableLayer; // 용접할 수 있는 물체의 레이어

    private GameObject selectedObject; // 선택된 물체
    private bool isWelding = false; // 용접 상태 확인
    private bool isTriggerPressed = false;  // 트리거 버튼이 눌렸는지 확인

    void Update()
    {
        // 레이캐스트가 물체를 향하는지 체크 (왼손 컨트롤러)
        RaycastHit hit;
        Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.forward);

        // 레이캐스트가 물체에 충돌했을 때
        if (Physics.Raycast(ray, out hit, 10f, weldableLayer))
        {
            // 트리거 버튼을 눌렀을 때
            if (Input.GetButtonDown("Fire1") && !isTriggerPressed)  // 트리거 버튼을 눌렀을 때
            {
                SelectObject(hit.collider.gameObject);  // 물체 선택
            }
        }

        // 물체가 선택된 상태라면 오른손과 물체 간의 거리를 계속 측정
        if (isWelding && selectedObject != null)
        {
            // 오른손과 선택된 물체 간의 거리 계산
            float distance = Vector3.Distance(rightControllerTransform.position, selectedObject.transform.position);
            UpdateDistanceUI(distance);
        }

        // 트리거 버튼을 떴을 때
        if (Input.GetButtonUp("Fire1") && isTriggerPressed)
        {
            isTriggerPressed = false;  // 트리거 버튼 떼었을 때 상태 업데이트
        }
    }

    // 물체를 선택하고 용접 가능 상태로 변경
    void SelectObject(GameObject targetObject)
    {
        // 이미 선택된 물체가 있으면 선택 해제
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;
        }

        // 물체를 선택하고 용접 가능 상태로 설정
        selectedObject = targetObject;
        selectedObject.GetComponent<Renderer>().material.color = Color.green;  // 하이라이트 효과
        isWelding = true;  // 용접 가능 상태로 설정

        // 트리거 버튼을 눌렀을 때 상태 표시
        isTriggerPressed = true;  // 트리거가 눌린 상태로 설정

        // 거리 UI 활성화
        distanceText.gameObject.SetActive(true);
    }

    // 거리 UI 업데이트
    void UpdateDistanceUI(float distance)
    {
        distanceText.text = "Distance: " + distance.ToString("F2") + "m";

        // UI 텍스트 크기에 맞춰 크기 동적 조정
        RectTransform textRect = distanceText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(distanceText.preferredWidth + 20, distanceText.preferredHeight + 10);
    }

    // 물체 선택 해제
    public void DeselectObject()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;  // 하이라이트 해제
        }

        selectedObject = null;  // 선택 해제
        isWelding = false;  // 용접 상태 해제
        distanceText.gameObject.SetActive(false);  // 거리 UI 비활성화
    }
}
