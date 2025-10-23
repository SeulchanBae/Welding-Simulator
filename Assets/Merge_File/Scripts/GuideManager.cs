using UnityEngine;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    public Image guideImage;
    public Sprite[] usageImages;
    public Sprite[] controlImages;

    private Sprite[] currentImages;
    private int currentIndex = 0;

    void OnEnable()
    {
        Debug.Log("GuideManager OnEnable");
        ShowUsage();
    }

    public void ShowUsage()
    {
        Debug.Log("ShowUsage 호출");
        currentImages = usageImages;
        currentIndex = 0;
        UpdateImage();
    }

    public void ShowControls()
    {
        Debug.Log("ShowControls 호출");
        currentImages = controlImages;
        currentIndex = 0;
        UpdateImage();
    }

    public void NextImage()
    {
        Debug.Log($"NextImage 호출됨. currentImages: {currentImages != null}, Length: {currentImages?.Length}");

        if (currentImages == null || currentImages.Length == 0)
        {
            Debug.Log("이미지 배열이 비어있음!");
            return;
        }

        currentIndex++;
        if (currentIndex >= currentImages.Length)
            currentIndex = 0;

        Debug.Log($"새 인덱스: {currentIndex}, 이미지: {currentImages[currentIndex]?.name}");
        UpdateImage();
    }

    public void PreviousImage()
    {
        Debug.Log("PreviousImage 호출됨!");

        if (currentImages == null || currentImages.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = currentImages.Length - 1;

        Debug.Log($"새 인덱스: {currentIndex}");
        UpdateImage();
    }

    void UpdateImage()
    {
        Debug.Log($"UpdateImage 호출. guideImage: {guideImage != null}");

        if (currentImages != null && currentImages.Length > 0)
        {
            guideImage.sprite = currentImages[currentIndex];
            Debug.Log($"이미지 변경 완료: {currentImages[currentIndex].name}");
        }
        else
        {
            Debug.Log("이미지를 업데이트할 수 없음");
        }
    }
}