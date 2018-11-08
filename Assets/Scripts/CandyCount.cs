using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Candy
{
    Candies,
    CandyOnBowler,
    CandyOnBucket,
    CandyOnDump,
    CandyOnPum
}


public class CandyCount : Ammunition
{
    public Candy candyType;
    public IHaveCandy candy;
    
    public override void Adder()
    {
        candy.AddCandy(candyType, sound);    
    }
}
