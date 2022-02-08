using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public EventTypeEnum eventType;
    //public string MainText;
    public string SecondaryText;

    public void Setup(EventTypeEnum et, string st)
    {
        eventType = et;
        //MainText = mt;
        SecondaryText = st;
    }
}
