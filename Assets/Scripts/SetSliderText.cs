using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderText : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;
    public string unit;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText((int)slider.value + unit);
    }
}
