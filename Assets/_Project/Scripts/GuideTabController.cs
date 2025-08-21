using UnityEngine;
using TMPro; // TextMeshPro 사용 시 필요

public class GuideTabController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    public void ShowUsage()
    {
        titleText.text = "사용법";
        contentText.text =
            "1. Start 버튼을 눌러 시뮬레이터를 시작하세요.\n" +
            "2. 작업 공간을 인식하고 객체를 선택하여 배치합니다.\n" +
            "3. 필요한 설정을 완료한 후 시작할 수 있습니다.";
    }

    public void ShowControls()
    {
        titleText.text = "조작 방법";
        contentText.text =
            "1. 조이스틱으로 이동\n" +
            "2. 트리거 버튼으로 선택/확인\n" +
            "3. Grip 버튼으로 잡기 동작\n" +
            "4. 메뉴 버튼으로 UI 열기/닫기";
    }
}
