using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeValueSlider : MonoBehaviour
{
    [SerializeField]
    private bool IsLinkedSlider;

    [SerializeField]
    private Slider LinkedSlider;

    [SerializeField]
    private Slider SelfSlider;

    public Text SliderValueText;

    public void ChangeValue(float value)
    {
        if (IsLinkedSlider)
        {
            SliderValueText.text = Mathf.RoundToInt(value).ToString() + "%";

            var distinction = LinkedSlider.value + SelfSlider.value - SelfSlider.maxValue;
            LinkedSlider.value -= distinction;
        }
        else
        {
            SliderValueText.text = Mathf.RoundToInt(value).ToString();
        }
    }
}
