using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAnimation : MonoBehaviour
{
    public GameObject target;
    public float lifetime = 1.0f;
    public AnimationCurve easeOutCurve;

    void Start()
    {
        transform.position = target.transform.position;
        StartCoroutine(MoveUp());
    }

    IEnumerator MoveUp() {
        float duration = 1.0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float curveValue = easeOutCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}