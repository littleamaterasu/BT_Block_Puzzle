using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class GameProgress
{

    public bool haveTut;
    public int length = 0;
    public List<string> elements;
    public List<Step> steps;

    public GameProgress()
    {
        elements = new List<string>();
        steps = new List<Step>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reset()
    {
        haveTut = false;
        elements.Clear();
        steps.Clear();
    }
}

[System.Serializable]
public class Step
{
    public int p;
    public int x;
    public int y;

    public Step() { }

    public Step(BoardHolderPosition pos)
    {
        this.p = (int) pos;
    }

    public Step(BoardHolderPosition pos, Vector2Int destination)
    {
        this.p = (int) pos;
        this.x = destination.x;
        this.y = destination.y;
    }
}