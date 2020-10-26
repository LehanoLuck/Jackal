using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts
{

    public class VoxelTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int HorizontalIndex { get; set; }
        public int VerticalIndex { get; set; }

        private float yPos;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"hor - {HorizontalIndex}\n ver - {VerticalIndex}");
            yPos = this.transform.position.y;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
    }
}
