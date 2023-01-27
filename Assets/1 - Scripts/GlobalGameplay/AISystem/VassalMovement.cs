using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class VassalMovement : MonoBehaviour
{
    //private IEnumerator Movement(Vector2[] pathPoints)
    //{
        //iAmMoving = true;
        //currentPosition = pathPoints[0];

        //for(int i = 1; i < pathPoints.Length; i++)
        //{
        //    if(i == 1)
        //    {
        //        previousPosition = pathPoints[0];
        //        if(CheckEnemy(pathPoints[i]) == true) break;
        //    }

        //    sprite.flipX = previousPosition.x - pathPoints[i].x < 0 ? true : false;

        //    if(currentMovementPoints == 0)
        //    {
        //        CheckExtraMovement();
        //        break;
        //    }


        //    gmPathFinder.ClearRoadTile(pathPoints[i - 1]);
        //    gmPathFinder.CheckFog(isFogNeeded, viewRadius);

        //    Vector2 distance = pathPoints[i] - (Vector2)transform.position;
        //    Vector2 step = distance / (defaultCountSteps / speed);

        //    for(float t = 0; t < defaultCountSteps / speed; t++)
        //    {
        //        transform.position += (Vector3)step;
        //        yield return new WaitForSeconds(0.01f);
        //    }
        //    ChangeMovementPoints(-1);

        //    transform.position = pathPoints[i];
        //    previousPosition = pathPoints[i - 1];
        //    currentPosition = pathPoints[i];
        //    gmPathFinder.RefreshSteps(pathPoints[i]);

        //    if(cancelMovement == true) break;

        //    if(i + 1 < pathPoints.Length)
        //    {
        //        if(CheckEnemy(pathPoints[i + 1]) == true) break;
        //    }
        //}

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
    //}
}
