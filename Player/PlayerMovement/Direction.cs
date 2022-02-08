using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{

    public float mouseAngle = 0;
    private float movingAngle = 0;


    private void FixedUpdate()
    {
        CalculateMouseDirection();
        CalculateMovingDirection();
    }

    /*
     * Returns the angle of the mouse from -180 to 180
     * TODO: Handle for controllers
     */
    private void CalculateMouseDirection()
    {
        Vector3 v3Pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        v3Pos = Input.mousePosition - v3Pos;
        float angle = -(Vector2.SignedAngle(Vector2.up, v3Pos));
        if (angle < 0)
        {
            angle += 360f;
        }
        mouseAngle = angle;
    }

    // TODO: Handle for controllers
    private void CalculateMovingDirection()
    {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 normDir = direction.normalized;
        float angle = -(Vector2.SignedAngle(Vector2.up, normDir));
        if (angle < 0)
        {
            angle += 360f;
        }
        movingAngle = angle;
    }

    public MoveDir GetMoveDirection()
    {
        MoveDir moveDirection = MoveDir.Forward;

        float angleDifferenceAbs = Mathf.Abs(mouseAngle - movingAngle);
        float angleDifference = mouseAngle - movingAngle;

        if (angleDifferenceAbs < 45 || angleDifferenceAbs > 315f)
        {
            moveDirection = MoveDir.Forward;
        }
        else if (angleDifferenceAbs < 135f || angleDifferenceAbs > 225f)
        {

            if (angleDifference > 180f || (angleDifference <= 0f && angleDifference >= -180f))
            {
                moveDirection = MoveDir.Right;
            }
            else if (angleDifference < -180f || (angleDifference > 0f && angleDifference <= 180f))
            {
                moveDirection = MoveDir.Left;
            }
            else
            {
                Debug.LogError("Move Direction is Broken");
            }
        }
        else
        {
            moveDirection = MoveDir.Backward;
        }

        return moveDirection;
    }
}
