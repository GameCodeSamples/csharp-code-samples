using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInfo : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    private float currentValue;

    public float CurrentValue => currentValue;

    public event Action<float> OnValueChange;

    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
    }
    
    public void SetValuesWithoutNotify(float value)
    {
        var clampedValue = Mathf.Clamp(value, minValue, maxValue);

        currentValue = clampedValue;
        slider.SetValueWithoutNotify(clampedValue);
        inputField.SetTextWithoutNotify(clampedValue.ToString("0.0000"));
    }

    private void OnInputFieldValueChanged(string text)
    {
        float value = float.Parse(text);
        slider.value = value;
        SetValuesWithoutNotify(value);
        OnValueChange?.Invoke(value);
    }

    private void OnSliderValueChanged(float value)
    {
        inputField.text = value.ToString("0.0000");
        SetValuesWithoutNotify(value);
        OnValueChange?.Invoke(value);
    }
}