using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeldingInteraction : MonoBehaviour
{
    public Transform leftControllerTransform; // �޼� ��Ʈ�ѷ�
    public Transform rightControllerTransform; // ������ ��Ʈ�ѷ�
    public Text distanceText; // �Ÿ� �ؽ�Ʈ UI
    public LayerMask weldableLayer; // ������ �� �ִ� ��ü�� ���̾�

    private GameObject selectedObject; // ���õ� ��ü
    private bool isWelding = false; // ���� ���� Ȯ��
    private bool isTriggerPressed = false;  // Ʈ���� ��ư�� ���ȴ��� Ȯ��

    void Update()
    {
        // ����ĳ��Ʈ�� ��ü�� ���ϴ��� üũ (�޼� ��Ʈ�ѷ�)
        RaycastHit hit;
        Ray ray = new Ray(leftControllerTransform.position, leftControllerTransform.forward);

        // ����ĳ��Ʈ�� ��ü�� �浹���� ��
        if (Physics.Raycast(ray, out hit, 10f, weldableLayer))
        {
            // Ʈ���� ��ư�� ������ ��
            if (Input.GetButtonDown("Fire1") && !isTriggerPressed)  // Ʈ���� ��ư�� ������ ��
            {
                SelectObject(hit.collider.gameObject);  // ��ü ����
            }
        }

        // ��ü�� ���õ� ���¶�� �����հ� ��ü ���� �Ÿ��� ��� ����
        if (isWelding && selectedObject != null)
        {
            // �����հ� ���õ� ��ü ���� �Ÿ� ���
            float distance = Vector3.Distance(rightControllerTransform.position, selectedObject.transform.position);
            UpdateDistanceUI(distance);
        }

        // Ʈ���� ��ư�� ���� ��
        if (Input.GetButtonUp("Fire1") && isTriggerPressed)
        {
            isTriggerPressed = false;  // Ʈ���� ��ư ������ �� ���� ������Ʈ
        }
    }

    // ��ü�� �����ϰ� ���� ���� ���·� ����
    void SelectObject(GameObject targetObject)
    {
        // �̹� ���õ� ��ü�� ������ ���� ����
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;
        }

        // ��ü�� �����ϰ� ���� ���� ���·� ����
        selectedObject = targetObject;
        selectedObject.GetComponent<Renderer>().material.color = Color.green;  // ���̶���Ʈ ȿ��
        isWelding = true;  // ���� ���� ���·� ����

        // Ʈ���� ��ư�� ������ �� ���� ǥ��
        isTriggerPressed = true;  // Ʈ���Ű� ���� ���·� ����

        // �Ÿ� UI Ȱ��ȭ
        distanceText.gameObject.SetActive(true);
    }

    // �Ÿ� UI ������Ʈ
    void UpdateDistanceUI(float distance)
    {
        distanceText.text = "Distance: " + distance.ToString("F2") + "m";

        // UI �ؽ�Ʈ ũ�⿡ ���� ũ�� ���� ����
        RectTransform textRect = distanceText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(distanceText.preferredWidth + 20, distanceText.preferredHeight + 10);
    }

    // ��ü ���� ����
    public void DeselectObject()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;  // ���̶���Ʈ ����
        }

        selectedObject = null;  // ���� ����
        isWelding = false;  // ���� ���� ����
        distanceText.gameObject.SetActive(false);  // �Ÿ� UI ��Ȱ��ȭ
    }
}
