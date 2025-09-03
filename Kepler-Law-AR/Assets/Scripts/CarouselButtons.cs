using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarouselButtons : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Button nextButton;
    public Button previousButton;

    private int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        nextButton.onClick.AddListener(NextSlide);
        previousButton.onClick.AddListener(PrevSlide);
    }

    void NextSlide()
    {
        if (currentIndex < scrollRect.content.childCount - 1)
        {
            currentIndex++;
            ScrollTo(currentIndex);
        }
    }

    void PrevSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ScrollTo(currentIndex);
        }
    }

     void ScrollTo(int index)
    {
        float target = (float)index / (scrollRect.content.childCount - 1);
        StartCoroutine(SmoothScroll(target));
    }

    IEnumerator SmoothScroll(float target)
    {
        float start = scrollRect.horizontalNormalizedPosition;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = target;
    }
}
