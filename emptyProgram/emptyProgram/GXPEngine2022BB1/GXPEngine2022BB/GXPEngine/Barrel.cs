using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public class Barrel : Sprite
{
    Vector2 targetPos;
    Tank owner;

    public Barrel(Tank pOwner):base("barrel.png",false,false)
    {
        targetPos = new Vector2();
        owner = pOwner;
        SetOrigin(width / 2, height / 2);
    }

    void Aiming()
    {
        rotation = (targetPos - owner.Position).GetAngleDegrees();
    }

    public void Step(Vector2 pTargetPos)
    {
        targetPos = pTargetPos;
        Aiming();
    }

}