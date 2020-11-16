using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts
{

    public class VoxelTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public List<Pirate> Pirates { get; set; } = new List<Pirate>();

        public VoxelTile HiddenTile;

        public VoxelTile[][] Map { get; set; }
        public int HorizontalIndex { get; set; }
        public int VerticalIndex { get; set; }

        public int maxSize = 5;

        private float yPos;

        public bool isHidden;

        public void OnPointerEnter(PointerEventData eventData)
        {
            yPos = this.transform.position.y;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }

        public void AddPirate(Pirate pirate)
        {
            this.Pirates.Add(pirate);

            UpdateTile();

            if (!isHidden)
            {
                pirate.CurrentTile = OpenTile();
            }
            else
            {
                pirate.CurrentTile = this;
            }
        }

        public void LeavePirate(Pirate pirate)
        {
            this.Pirates.Remove(pirate);

            UpdateTile();
        }

        public void UpdateTile()
        {
            int count = Pirates.Count;

            //Коэффициент для изменения размера пирата
            float factor = (float)(1 / Math.Pow(Math.Log(Math.E * count), 0.4));
            var localScale = new Vector3(factor, factor, factor);

            float radius = count == 1 ? 0 : this.transform.localScale.x * 0.7f;

            for (int i = 0; i < Pirates.Count; i++)
            {
                Pirates[i].transform.localScale = localScale;

                float value = i * 2f * Mathf.PI / count;

                float x = this.transform.position.x + radius * Mathf.Sin(value);
                float z = this.transform.position.z + radius * Mathf.Cos(value);

                Pirates[i].transform.position = new Vector3(x, this.transform.position.y - 1.25f, z);
            }
        }

        public VoxelTile OpenTile()
        {
            var hidden = Instantiate(HiddenTile, this.transform.parent);
            hidden.transform.position = this.transform.position;
            hidden.HorizontalIndex = this.HorizontalIndex;
            hidden.VerticalIndex = this.VerticalIndex;
            Map[HorizontalIndex][VerticalIndex] = hidden;
            hidden.Pirates = this.Pirates;

            Destroy(gameObject);

            return hidden;
        }
    }
}
