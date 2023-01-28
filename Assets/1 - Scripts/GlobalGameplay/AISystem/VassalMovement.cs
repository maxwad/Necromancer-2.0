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
        //iAmMoving = true;
        Vector3 previousPosition = pathPoints[0];
        Vector3 currentPosition = pathPoints[0];

        for(int i = 1; i < pathPoints.Count; i++)
        {
            animationScript.FlipSprite(previousPosition.x - pathPoints[i].x < 0);
            //sprite.flipX = previousPosition.x - pathPoints[i].x < 0 ? true : false;

            if(currentMovementPoints == 0)
            {                
                break;
            }

            //gmPathFinder.ClearRoadTile(pathPoints[i - 1]);
            //gmPathFinder.CheckFog(isFogNeeded, viewRadius);

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
            //gmPathFinder.RefreshSteps(pathPoints[i]);

            //if(cancelMovement == true) break;

            //if(i + 1 < pathPoints.Count)
            //{
            //    if(CheckEnemy(pathPoints[i + 1]) == true) break;
            //}
        }

        //if(cancelMovement == false && currentMovementPoints != 0)
        //{
        //    if((Vector2)transform.position == pathPoints[pathPoints.Length - 1])
        //    {
        //        gmPathFinder.ClearRoadTile(pathPoints[pathPoints.Length - 1]);
        //    }
        //}

        //if(currentMovementPoints == 0) CheckExtraMovement();

        //iAmMoving = false;
        //cancelMovement = false;

        //CheckPosition();
    }
}
