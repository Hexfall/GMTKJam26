using System.Collections;
using UnityEngine;

public class LoopingElevator : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float moveDistanceY = 3f;

    [SerializeField, Min(0.01f)]
    private float travelTime = 2f;

    [SerializeField]
    private AnimationCurve movementCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Waiting")]
    [SerializeField, Min(0f)]
    private float waitTime = 1f;

    private Vector3 startLocalPosition;
    private bool isActivated;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;
    }

    public void Activate()
    {
        if(isActivated)
            return;

        isActivated = true;
        StartCoroutine(ElevatorLoop());
    }

    private IEnumerator ElevatorLoop()
    {
        Vector3 bottomLocalPosition =
            startLocalPosition +
            Vector3.down * moveDistanceY;

        while(true)
        {
            yield return MoveBetween(
                startLocalPosition,
                bottomLocalPosition
            );

            yield return new WaitForSeconds(waitTime);

            yield return MoveBetween(
                bottomLocalPosition,
                startLocalPosition
            );

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveBetween(
        Vector3 from,
        Vector3 to
    )
    {
        float elapsedTime = 0f;
        float duration = Mathf.Max(travelTime, 0.01f);

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsedTime / duration);

            float curvedProgress =
                movementCurve.Evaluate(progress);

            transform.localPosition =
                Vector3.LerpUnclamped(
                    from,
                    to,
                    curvedProgress
                );

            yield return null;
        }

        transform.localPosition = to;
    }
}
