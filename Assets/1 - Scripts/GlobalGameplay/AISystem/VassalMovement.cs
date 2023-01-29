using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class VassalMovement : MonoBehaviour
{
    private VassalAnimation animationScript;

    [SerializeField] private int movementPoints = 20;
    private int currentMovementPoints;

    private float speed = 50f; //50 for build
    private float defaultCountSteps = 1000;

    public void Init(VassalAnimation animation)
    {
        animationScript = animation;
    }

    public int GetMovementPointsAmoumt()
    {
        return movementPoints;
    }

    public void ResetMovementPoints()
    {
        currentMovementPoints = movementPoints;
    }

    public void Movement(List<Vector3> path)
    {
        ResetMovementPoints();

        StartCoroutine(Moving(path));
    }

    private IEnumerator Moving(List<Vector3> pathPoints)
    {
        Vector3 previousPosition = pathPoints[0];
        Vector3 currentPosition = pathPoints[0];

        for(int i = 1; i < pathPoints.Count; i++)
        {
            animationScript.FlipSprite(previousPosition.x - pathPoints[i].x < 0);

            if(currentMovementPoints == 0) break;

            Vector3 distance = pathPoints[i] - transform.position;
            Vector3 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += step;
                yield return new WaitForSeconds(0.01f);
            }

            currentMovementPoints--;

            transform.position = pathPoints[i];
            previousPosition = pathPoints[i - 1];
            currentPosition = pathPoints[i];
        }
    }
}
