using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

public class Complex_Flag : MonoBehaviour
{
    [SerializeField] private List<GameObject> flags = new List<GameObject>();
    [SerializeField] private string LogicFunc = "";

    private Dictionary<int, GATE> dict = new Dictionary<int, GATE>();
    private TRI[] valuePile;
    private int root;

    private Zone_Flags ZF;

    public void Setup()
    {
        ZF = transform.GetComponentInParent<Zone_Flags>();
        string internal_str = LogicFunc.Replace(" ", "");
        LogicParser(internal_str);
    }

    private void LogicParser(string str)
    {
        int start = -1;
        int gate_iter = flags.Count;
        while (str.Contains("(")) //Go through multiple loops
        {
            for (int i = 0; i < str.Length; ++i) //Iter through
            {
                //Debug.Log(str);
                if (str[i] == '(')
                {
                    start = i;
                }
                else if (str[i] == ')' && start != -1)
                {
                    Assert.IsTrue(str[start + 1] == '!' || str[start + 1] == '*' || str[start + 1] == '+');
                    GATE gate = new GATE();
                    gate.inputs = new List<int>();
                    gate.func = str[start + 1];

                    int tempIter = start + 2;
                    string num = "";
                    while (tempIter <= i) //backtrack to replace logic with a gate
                    {
                        if (Char.IsNumber(str[tempIter]))
                        {
                            num += str[tempIter];
                        }
                        else //Seperator or ')'
                        {
                            int value;
                            if (int.TryParse(num, out value))
                            {
                                gate.inputs.Add(value);
                            }
                            num = "";
                        }
                        tempIter += 1;
                    }

                    dict.Add(gate_iter, gate);
                    str = str.Remove(start, i - start + 1);
                    str = str.Insert(start, gate_iter.ToString());
                    start = -1;
                    gate_iter += 1;
                }
            }
        }

        root = gate_iter - 1;
    }

    public bool LogicTest()
    {
        valuePile = new TRI[root + 1];
        for(int i = 0; i < root + 1; ++i)
        {
            if (i < flags.Count)
            { 
                if (ZF.CheckFlag(flags[i].name))
                {
                    valuePile[i] = TRI.TRUE;
                }
                else
                {
                    valuePile[i] = TRI.FALSE;
                }
            }
            else
            {
                valuePile[i] = TRI.UNSET;
            }
        }

        foreach(int v in valuePile)
        {
            //Debug.Log((TRI)v);
        }

        return LogicSolver(dict[root]);
    }

    private bool LogicSolver(GATE gate)
    {
        for(int i = 0; i < gate.inputs.Count; ++i)
        {
            if(valuePile[i] == TRI.UNSET)
            {
                if (LogicSolver(dict[i]))
                {
                    valuePile[i] = TRI.TRUE;
                }
                else
                {
                    valuePile[i] = TRI.FALSE;
                }
            }
        }

        if(gate.func == '*')
        {
            foreach(int input in gate.inputs)
            {
                if (valuePile[input] == TRI.FALSE)
                {
                    return false;
                }
            }
            return true;
        }
        else if(gate.func == '+')
        {
            foreach (int input in gate.inputs)
            {
                if (valuePile[input] == TRI.TRUE)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return (valuePile[gate.inputs[0]] == TRI.FALSE);
        }
    }

    private struct GATE
    {
        public char func;
        public List<int> inputs;
    }

    private enum TRI { UNSET, TRUE, FALSE }
}


