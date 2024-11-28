using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubmitController : MonoBehaviour
{
    public List<GameObject> questions;
    private List<GameObject> _unansweredQuestions;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _unansweredQuestions = new List<GameObject>();
    }

    private bool CheckAllAnswered()
    {
        var answered = true;
        foreach (var questionGroup in questions)
        {
            var toggleGroup = questionGroup.GetComponentInChildren<ToggleGroup>();

            if (toggleGroup.AnyTogglesOn()) continue;
            answered = false;
            var unansweredLabel = questionGroup.transform.FindChildRecursive("Label").gameObject;
            _unansweredQuestions.Add(unansweredLabel);
        }

        return answered;
    }

    public void next()
    {
        if (CheckAllAnswered())
        {
            foreach(var attribute in questions)
            {
                // retrieve values
                var label = attribute.transform.FindChildRecursive("Label").GetComponent<TextMeshProUGUI>().text;
                var toggleGroup = attribute.GetComponentInChildren<ToggleGroup>();
                var slider = attribute.GetComponentInChildren<Slider>();
                
                // export to csv

                toggleGroup.SetAllTogglesOff();
                slider.value = 50;
            }
            // shuffle groups
            var newOrder = questions.OrderBy(x => Random.value).ToList();
            for (int i = 0; i < newOrder.Count; i++)
            {
                newOrder[i].transform.SetSiblingIndex(i+2);
            }
        }
        else
        {
            foreach (var obj in _unansweredQuestions)
            {
                StartCoroutine(ChangeTextColor(obj));
            }
            _unansweredQuestions.Clear();
        }
    }

    IEnumerator ChangeTextColor(GameObject textObj)
    {
        var textMesh = textObj.GetComponent<TextMeshProUGUI>();
        const float increment = 0.05f;
        float timer = 0;
        const int length = 2;
        while (timer <= length)
        {
            timer += increment;
            // blink three times
            textMesh.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(timer/length, 1));
            yield return new WaitForSeconds(increment);
        }
    }
}
