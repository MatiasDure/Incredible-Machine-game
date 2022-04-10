using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public class Ball : EasyDraw
{

    Vector2 _position, _oldPosition, _velocity;
    public Vector2 acceleration;

    public static float bounciness = 0.6f;
    public static Vector2 gravity = new Vector2(0, 0.1f);

    public Vector2 Position { get => _position; }

    public readonly bool canMove;
    public readonly int radius;

    public Ball(int pRadius, Vector2 pPosition, Vector2 pVelocity = new Vector2(), Vector2 pAcceleration = new Vector2(),
                bool pCanMove = true) : base(pRadius * 2 + 4, pRadius * 2 + 4)
    {
        _position = pPosition;
        _velocity = pVelocity;
        acceleration = pAcceleration;
        canMove = pCanMove;
        radius = pRadius;
        SetXY(_position.x, _position.y);
        SetOrigin(width / 2, height / 2);

        Draw(250, 0, 0);
    }

    void Draw(byte pR, byte pG, byte pB)
    {
        Fill(pR, pG, pB);
        Ellipse(radius, radius, radius * 2, radius * 2);
        Stroke(pR, pG, pB);
    }

    void UpdateScreenPos()
    {
        x = _position.x;
        y = _position.y;
    }

    bool Approximate(float pA, float margin = 0.001f) => pA <= margin;

    public void Step()
    {
        if (canMove)
        {
            bool firstTime = true;
            _oldPosition = _position;
            _velocity += acceleration;

            for (int i = 0; i < 2; i++)
            {
                _position += _velocity;
                CollisionInfo firstCollision = FindEarliestCollision();
                if (firstCollision != null)
                {
                    ResolveCollision(firstCollision);
                    if (firstTime && Approximate(firstCollision.timeOfImpact)) //rolling
                    {
                        firstTime = false;
                        continue;
                    }
                }
                break;
            }

            UpdateScreenPos();
        }
    }

    CollisionInfo FindEarliestCollision()
    {
        MyGame myGame = (MyGame)game;

        float smallestToi = 100;
        float currentToi = smallestToi;
        bool collisionDetected = false;
        Vector2 firstColNormal = new Vector2();

        for (int i = 0; i < myGame.GetPointsCount(); i++)
        {
            Ball other = myGame.GetPointAtIndex(i);
            smallestToi = ToiPoint(other, currentToi);

            if (smallestToi != currentToi)
            {
                collisionDetected = true;
                firstColNormal = (this._oldPosition + smallestToi * this._velocity) - other._position; // Point of impact - mover.position
                firstColNormal.Normalize();
                currentToi = smallestToi;
            }
        }

        for (int i = 0; i < myGame.GetLinesCount(); i++)
        {
            NLineSegment currentLine = myGame.GetLineAtIndex(i);
            smallestToi = ToiLine(currentLine, currentToi);

            if (smallestToi != currentToi)
            {
                collisionDetected = true;
                firstColNormal = currentLine._normal.vector;
                Console.WriteLine(firstColNormal);
                currentToi = smallestToi;
            }
        }

        if (collisionDetected) return new CollisionInfo(firstColNormal, null, smallestToi);
        return null;
    }

    void ResolveCollision(CollisionInfo pCollision)
    {
        Vector2 desiredPos = _oldPosition + pCollision.timeOfImpact * _velocity;
        _position.SetXY(desiredPos);
        _velocity.Reflect(Ball.bounciness, pCollision.normal);
        _velocity *= 0.995f; //friction
    }

    float ToiPoint(Ball pOther, float pCurrentToi)
    {
        Vector2 oldRelativePos = this._oldPosition - pOther._position;

        float distance = oldRelativePos.Length();
        float velocityLength = this._velocity.Length();
        float sumRadius = this.radius + pOther.radius;
        float a = velocityLength * velocityLength;
        float b = 2 * oldRelativePos.Dot(this._velocity);
        float c = distance * distance - sumRadius * sumRadius;
        float insideSqrt = b * b - 4 * a * c;

        // returns null because a negative number inside the sqrt would give no solution or the velocity is 0 (ball is not moving) 
        if (insideSqrt < 0 || a == 0) return pCurrentToi;

        if (c < 0)
        {
            if (b < 0) return 0;
            else return pCurrentToi;
        }
        else
        {
            float toi;
            float sqrtResult = Mathf.Sqrt(insideSqrt);

            toi = (-b - sqrtResult) / (2 * a);

            if (toi < 0 || toi > 1) return pCurrentToi; //time of impact its outside the possible scope
            if (pCurrentToi > toi) return toi;
            return pCurrentToi;
        }
    }

    float ToiLine(NLineSegment pOther, float pCurrentToi)
    {
        Vector2 lineVector = pOther.start - pOther.end;
        Vector2 diffVecBetweenEndPoint = this._position - pOther.end;
        Vector2 diffVecBetweenStartPoint = this._position - pOther.start;

        float lineVectorLength = lineVector.Length();

        Vector2 unitLineVector = lineVector.Normalized();

        float scalarProjection1 = diffVecBetweenEndPoint.ScalarProjection(unitLineVector, true);
        float scalarProjection2 = diffVecBetweenStartPoint.ScalarProjection(unitLineVector, true);

        // only do this with POI!
        if (Mathf.Abs(scalarProjection1) > lineVectorLength ||
            Mathf.Abs(scalarProjection2) > lineVectorLength) //checks if ball is not between the line segment
        {
            return pCurrentToi;
        }

        Vector2 vectorProjection = scalarProjection1 * unitLineVector;
        vectorProjection += pOther.end;

        float distance = this._position.DistanceBetween(vectorProjection);

        if (distance < this.radius) //if ball collides with lineSegment
        {
            float toi;
            Vector2 lineNormal = pOther._normal.vector; //(currentLine.end - currentLine.start).Normal();
            Vector2 oldDiffVector = this._oldPosition - pOther.end;
            float a = Vector2.Dot(lineNormal, oldDiffVector) - this.radius;
            float b = -Vector2.Dot(lineNormal, _velocity);
            toi = a / b;
            return toi < pCurrentToi ? toi : pCurrentToi;
        }
        return pCurrentToi;
    }
}