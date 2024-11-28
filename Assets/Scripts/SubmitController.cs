using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmitController : MonoBehaviour
{
    public List<GameObject> questions;
    private List<GameObject> _unansweredQuestions;
    public Experiment experiment;

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
            var results = new List<int[]>(4);

            foreach (var attribute in questions)
            {
                // retrieve values
                var label = attribute.transform.FindChildRecursive("Label").GetComponent<TextMeshProUGUI>().text;
                var toggleGroup = attribute.GetComponentInChildren<ToggleGroup>();
                var slider = attribute.GetComponentInChildren<Slider>();

                // save to results
                switch (label)
                {
                    case "Sweet":
                        results.Insert(0,
                            new[]
                            {
                                int.Parse(
                                    toggleGroup.GetFirstActiveToggle().GetComponentInChildren<TextMeshProUGUI>().text
                                ),
                                (int)slider.value
                            });
                        break;
                    case "Sour":
                        results.Insert(1,
                            new[]
                            {
                                int.Parse(
                                    toggleGroup.GetFirstActiveToggle().GetComponentInChildren<TextMeshProUGUI>().text
                                ),
                                (int)slider.value
                            });
                        break;
                    case "Bitter":
                        results.Insert(2,
                            new[]
                            {
                                int.Parse(
                                    toggleGroup.GetFirstActiveToggle().GetComponentInChildren<TextMeshProUGUI>().text
                                ),
                                (int)slider.value
                            });
                        break;
                    case "Salty":
                        results.Insert(3,
                            new[]
                            {
                                int.Parse(
                                    toggleGroup.GetFirstActiveToggle().GetComponentInChildren<TextMeshProUGUI>().text
                                ),
                                (int)slider.value
                            });
                        break;
                }


                toggleGroup.SetAllTogglesOff();
                slider.value = 50;
            }

            // shuffle groups
            var newOrder = questions.OrderBy(x => Random.value).ToList();
            for (int i = 0; i < newOrder.Count; i++)
            {
                newOrder[i].transform.SetSiblingIndex(i + 2);
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