using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public class Tank : Rec
{
    Vector2 targetPos;

    Vector2 barrelRotationUnitVector;

    Barrel barrel;

    bool ballFired;
    bool selected;
    bool visualAim;

    List<Ball> balls;

    public Tank(Vector2 pPosition,bool pSelected = false) : base("tank.png", pPosition)
    {
        targetPos = new Vector2();
        selected = pSelected;
        barrel = new Barrel(this);
        AddChild(barrel);
        balls = new List<Ball>(8);
        barrelRotationUnitVector = Vector2.GetUnitVectorDeg(barrel.rotation);
    }

    void Update()
    {
        CannonSelection();
        UpdateTargetPos();

        if (selected)
        {
            barrelRotationUnitVector = Vector2.GetUnitVectorDeg(barrel.rotation);
            barrel.Step(targetPos);
            if (!visualAim)
            {
                UpdateAimVisualization();
                visualAim = true;
            }
            else UpdateAimVisualization();
        }
        else RemoveAimVisualization();
        
    }

    void UpdateTargetPos()
    {
        targetPos = new Vector2(Input.mouseX, Input.mouseY);
    }

    public Ball Shooting()
    {
        if (!ballFired && CollisionWithBall(ball))
        {
            ball.LateDestroy();
            ballFired = true;
            Ball firedBall = new Ball(20, this._position + barrelRotationUnitVector * 80, barrelRotationUnitVector * 4, Ball.gravity);
            parent.AddChild(firedBall);
            return firedBall;
        }
        return ball;      
    }

    public void IterationStarted()
    {
        selected = false;
    }

    void UpdateAimVisualization()
    {
        Vector2 velo = barrelRotationUnitVector * 4;
        Vector2 pos = this._position + barrelRotationUnitVector * 80;

        for (int i = 0; i < 8; i++)
        {
            velo += Ball.gravity * 7;
            pos += velo * 7;

            if (!visualAim)
            {
                Ball b = new Ball(8 - i, pos, new Vector2(), new Vector2(), false);
                parent.AddChild(b);
                balls.Add(b);
                continue;
            }
            balls[i].SetXY(pos.x, pos.y);
        }
    }

    void RemoveAimVisualization()
    {
        if (balls.Count < 1) return;
        for(int i = balls.Count - 1; i >= 0; i--)
        {
            balls[i].Destroy();
        }
        balls.Clear();
        visualAim = false;
    }

    void CannonSelection()
    {
        bool leftMouseDown = Input.GetMouseButtonDown(0);
        if(selected)
        {
            barrel.SetColor(255, 0, 0);
            if (leftMouseDown) selected = false;

        }
        else
        {
            if (targetPos.x > _position.x - width / 2 &&
            targetPos.x < _position.x + width / 2 &&
            targetPos.y > _position.y - height / 2 &&
            targetPos.y < _position.y + height / 2)
            {
                barrel.SetColor(0, 255, 0);
                if(leftMouseDown) selected = true;
            }
            else
            {
                barrel.SetColor(255, 255, 255);
            }
        }
    }

}