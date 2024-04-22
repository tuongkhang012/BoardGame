using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RoundDisplay : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI roundText;
    public void ShowRound(int round, Action callback)
    {
        roundText.text = "START ROUND";
        panel.SetActive(true);
        StartCoroutine(HideAfterDelay(2.0f, callback));
    }

    IEnumerator HideAfterDelay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
        callback?.Invoke();
    }
}
