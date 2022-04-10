using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public class Flag:Rec
{

    public Flag(Vector2 pPos):base("square.png", pPos)
    {
    }

    public bool SetLevelStatus()
    {
        if(CollisionWithBall(ball))
        {
            ball.LateDestroy();
            return true;
        }
        return false;
    }
}