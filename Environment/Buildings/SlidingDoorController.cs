using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class SlidingDoorController : DoorController
{
    private enum OpenDirections
    {
        Up,
        Down,
        Right,
        Left
    }

    [SerializeField] float doorSize = 2.5f;
    [SerializeField] OpenDirections openDirection = OpenDirections.Down;
    [SerializeField] bool xDirection = true;

    private Transform doorTransform;
    private Vector3 defaultDoorPosition;

    protected override void Start()
    {
        base.Start();
        doorTransform = GetComponent<MeshRenderer>().transform;
        defaultDoorPosition = doorTransform.transform.localPosition;
    }

    protected override void Update()
    {
        if (openTime < 1)
        {
            openTime += Time.deltaTime * openSpeed;

            switch (openDirection)
            {
                case OpenDirections.Up:
                    doorTransform.localPosition = new Vector3(doorTransform.localPosition.x, Mathf.Lerp(
                        doorTransform.localPosition.y, defaultDoorPosition.y + (ID.DoorOpen ? doorSize : 0), openTime),
                        doorTransform.localPosition.z);
                    break;
                case OpenDirections.Down:
                    doorTransform.localPosition = new Vector3(doorTransform.localPosition.x, Mathf.Lerp(
                        doorTransform.localPosition.y, (defaultDoorPosition.y - (ID.DoorOpen ? doorSize : 0)), openTime),
                        doorTransform.localPosition.z);
                    break;
                case OpenDirections.Right:
                    if (xDirection)
                    {
                        doorTransform.localPosition = new Vector3(Mathf.Lerp(
                            doorTransform.localPosition.x, defaultDoorPosition.x + (ID.DoorOpen ? doorSize : 0), openTime),
                            doorTransform.localPosition.y,
                            doorTransform.localPosition.z);
                    }
                    else
                    {
                        doorTransform.localPosition = new Vector3(doorTransform.localPosition.x, doorTransform.localPosition.y,
                            Mathf.Lerp(doorTransform.localPosition.z, defaultDoorPosition.z + (ID.DoorOpen ? doorSize : 0), openTime));
                    }

                    break;
                case OpenDirections.Left:
                    if (xDirection)
                    {
                        doorTransform.localPosition = new Vector3(Mathf.Lerp(
                            doorTransform.localPosition.x, defaultDoorPosition.x - (ID.DoorOpen ? doorSize : 0), openTime),
                            doorTransform.localPosition.y,
                            doorTransform.localPosition.z);
                    }
                    else
                    {
                        doorTransform.localPosition = new Vector3(doorTransform.localPosition.x, doorTransform.localPosition.y,
                            Mathf.Lerp(doorTransform.localPosition.z, defaultDoorPosition.z - (ID.DoorOpen ? doorSize : 0), openTime));
                    }

                    break;
            }
        }

        
    }
}
