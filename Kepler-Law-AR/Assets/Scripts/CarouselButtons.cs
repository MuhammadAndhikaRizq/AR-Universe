using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarouselButtons : MonoBehaviour
{
    [Header("Carousel Components")]
    public ScrollRect scrollRect;   // Carousel gambar
    public ScrollRect scrollInfo;   // Carousel teks/info

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button previousButton;

    private int currentIndex = 0;
    private int maxIndex; // jumlah slide maksimum

    [SerializeField] private float scrollDuration = 0.3f;

    void Start()
    {
        // Hitung max slide berdasarkan child terbanyak
        maxIndex = Mathf.Min(scrollRect.content.childCount, scrollInfo.content.childCount) - 1;

        nextButton.onClick.AddListener(NextSlide);
        previousButton.onClick.AddListener(PrevSlide);

        UpdateButtons();
    }

    void NextSlide()
    {
        if (currentIndex < maxIndex)
        {
            currentIndex++;
            ScrollAll(currentIndex);
        }
    }

    void PrevSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ScrollAll(currentIndex);
        }
    }

    void ScrollAll(int index)
    {
        float targetImage = (float)index / (scrollRect.content.childCount - 1);
        float targetInfo  = (float)index / (scrollInfo.content.childCount - 1);

        StopAllCoroutines();
        StartCoroutine(SmoothScroll(scrollRect, targetImage));
        StartCoroutine(SmoothScroll(scrollInfo, targetInfo));

        UpdateButtons();
    }

    IEnumerator SmoothScroll(ScrollRect scroll, float target)
    {
        float start = scroll.horizontalNormalizedPosition;
        float elapsed = 0f;

        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            scroll.horizontalNormalizedPosition = Mathf.Lerp(start, target, elapsed / scrollDuration);
            yield return null;
        }

        scroll.horizontalNormalizedPosition = target;
    }

    void UpdateButtons()
    {
        // Disable button jika sudah di ujung
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < maxIndex;
    }
}
