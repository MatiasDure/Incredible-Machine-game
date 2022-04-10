using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public abstract class Rec:Sprite
{
    protected Vector2 _position;

    protected Ball ball;

    public Vector2 Position { get => _position; }

    public Rec(string pImg, Vector2 pPos):base(pImg, false,false)
    {
        _position = pPos;
        SetOrigin(width / 2, height / 2);
        UpdateScreenPos();
    }

    protected bool CollisionWithBall(Ball pBall)
    {
        Vector2 distance = new Vector2(Mathf.Abs(pBall.x - Position.x), Mathf.Abs(pBall.y - Position.y));

        if (distance.x > (width / 2 + pBall.radius) ||
            distance.y > (height / 2 + pBall.radius)) return false;

        if (distance.x <= (width / 2) ||
            distance.y <= (height / 2)) return true;

        //corner edge collision
        float diffFromBallToEdgeX = distance.x - width / 2;
        float diffFromBallToEdgeY = distance.y - height / 2;

        float cornerDistance_sq = diffFromBallToEdgeX * diffFromBallToEdgeX + diffFromBallToEdgeY * diffFromBallToEdgeY;

        return cornerDistance_sq <= pBall.radius * pBall.radius;
    }

    protected void UpdateScreenPos()
    {
        x = Position.x;
        y = Position.y;
    }

    public void GetBall(Ball pBall)
    {
        ball = pBall;
    }

}