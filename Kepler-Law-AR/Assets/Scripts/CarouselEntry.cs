using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carousel.UI
{
    [CreateAssetMenu(fileName = "New Carousel Entry", menuName = "UI/CarouselEntry", order = 0)]
    public class CarouselEntry : ScriptableObject
    {
        [field: SerializeField] public Sprite EntryGraphic { get; private set; }
        [field: SerializeField, Multiline(10)] public string Description { get; private set; }
    }
}

