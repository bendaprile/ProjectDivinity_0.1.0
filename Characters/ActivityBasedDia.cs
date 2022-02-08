using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityBasedDia : MonoBehaviour
{
    public NPCActivityFlag npcAF;
    [SerializeField] [TextArea(3, 10)] private List<string> ActivityDia = new List<string>();

    private int iter = 0;

    public string Return_Line()
    {
        string final_line = ActivityDia[iter];
        iter = (iter + 1) % ActivityDia.Count;
        return final_line;
    }
}
