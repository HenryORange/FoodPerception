using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderText : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;

    public void Start()
    {
        slider.onValueChanged.AddListener(delegate {ValueChangedCheck();});
    }

    public void ValueChangedCheck()
    {
        slider.transform.GetChild(1).gameObject.SetActive(true);
        slider.transform.GetChild(2).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!slider.transform.GetChild(1).gameObject.activeSelf) return;
        text.SetText(slider.value.ToString(CultureInfo.CurrentCulture));
    }
}
