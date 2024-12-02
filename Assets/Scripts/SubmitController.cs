using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SubmitController : MonoBehaviour
{
    private List<GameObject> _unansweredQuestions;
    public List<GameObject> sliders;
    public List<GameObject> slidersLabels;
    public List<GameObject> questionBoxes;
    public Experiment experiment;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _unansweredQuestions = new System.Collections.Generic.List<GameObject>();
    }

    private bool CheckAllAnswered()
    {
        var answered = true;
        foreach (var (slider, label) in Enumerable.Zip(sliders, slidersLabels, (a, b) => (a, b)))
        {
            var changed = slider.transform.GetChild(1).gameObject.activeSelf;

            if (changed) continue;
            answered = false;
            var unansweredLabel = label;
            _unansweredQuestions.Add(unansweredLabel);
        }

        return answered;
    }

    public void next()
    {
        if (CheckAllAnswered())
        {
            var results = new List<int[]> {new int[2], new int[2], new int[2], new int[2]};

            foreach (var attribute in questionBoxes)
            {
                // retrieve values
                var label = attribute.transform.FindChildRecursive("Label").GetComponent<TextMeshProUGUI>().text;
                var sliders = attribute.GetComponentsInChildren<Slider>();

                // save to results
                var arrayIndex = -1;
                switch (label)
                {
                    case "Süß":
                        arrayIndex = 0;
                        break;
                    case "Sauer":
                        arrayIndex = 1;
                        break;
                    case "Bitter":
                        arrayIndex = 2;
                        break;
                    case "Salzig":
                        arrayIndex = 3;
                        break;
                }
                
                results[arrayIndex] = new[]
                {
                    (int)sliders[0].value,
                    (int)sliders[1].value
                };
                
                sliders[0].value = 0;
                sliders[0].transform.GetChild(1).gameObject.SetActive(false);
                sliders[0].transform.GetChild(2).gameObject.SetActive(false);
                sliders[1].value = 0;
                sliders[1].transform.GetChild(1).gameObject.SetActive(false);
                sliders[1].transform.GetChild(2).gameObject.SetActive(false);
            }
            
            // reset value displays
            foreach (var text in GameObject.FindGameObjectsWithTag("SliderValue"))
            {
                text.GetComponent<TextMeshProUGUI>().text = "";
            }

            
            // shuffle groups
            var newOrder = questionBoxes.OrderBy(x => Random.value).ToList();
            for (int i = 0; i < newOrder.Count; i++)
            {
                newOrder[i].transform.SetSiblingIndex(i + 1);
            }
            
            ExecuteEvents.Execute<ISubmitMessageTarget>(experiment.gameObject, null,
                (x, _) => x.OnSubmitMessage(results));
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
            // blink three times
            textMesh.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(3 * timer, 1));
            yield return new WaitForSeconds(increment);
            timer += increment;
        }
    }
}