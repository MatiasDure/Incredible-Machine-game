using System;
using GXPEngine;

public struct Vector2
{
    public float x, y;

    public Vector2(float pX, float pY)
    {
        this.x = pX;
        this.y = pY;
    }

    public void SetXY(Vector2 pOther)
    {
        SetXY(pOther.x, pOther.y);
    }

    public void SetXY(float pX, float pY)
    {
        this.x = pX;
        this.y = pY;
    }

    public void Mult(float pScale)
    {
        this.x *= pScale;
        this.y *= pScale;
    }

    public void Div(float pNum)
    {
        if (pNum == 0) return;
        this.x /= pNum;
        this.y /= pNum;
    }

    public float Length() => Mathf.Sqrt(this.x * this.x + this.y * this.y);
    
    public void Normalize()
    {
        float length = this.Length();
        if (length == 0) return;
        this.Div(length);
    }

    public void SetLength(float pScale)
    {
        this.Normalize();
        this.Mult(pScale);
    }

    public void LimitLength(float pLimit)
    {
        float length = this.Length();
        if (length > pLimit) SetLength(pLimit);
    }

    public float GetAngleDegrees() => Rad2Deg(GetAngleRadians());

    public float GetAngleRadians() => Mathf.Atan2(this.y, this.x);

    public void SetAngleDegrees(float pDegrees)
    {
        this.SetAngleRadians(Deg2Rad(pDegrees));
    }

    public void SetAngleRadians(float pRadians)
    {
        float length = this.Length();
        this.SetXY(GetUnitVectorRad(pRadians));
        this.Mult(length);
    }

    public Vector2 Normalized()
    {
        float length = this.Length();
        if (length == 0) return new Vector2(0, 0);
        return new Vector2(this.x / length, this.y / length);
    }

    public static Vector2 COM(Vector2 pVelocity1, Vector2 pVelocity2, float pMass1, float pMass2) => (pVelocity1 * pMass1 + pVelocity2 * pMass2) / (pMass1 + pMass2);

    public void Reflect(float pBounciness, Vector2 pNormal, Vector2 pCom = new Vector2())
    {
        Vector2 vectorProjection = Vector2.VectorProjection(this - pCom, pNormal);
        this.SetXY(this - (1 + pBounciness) * vectorProjection);
    }

    public float Dot(Vector2 pOther) => Dot(this, pOther);

    public static float Dot(Vector2 pLeft, Vector2 pRight) => pLeft.x * pRight.x + pLeft.y * pRight.y;

    public float ScalarProjection(Vector2 pOther, bool pAlreadyUnitVec = false) => ScalarProjection(this, pOther, pAlreadyUnitVec);

    public static float ScalarProjection(Vector2 pLeft, Vector2 pRight, bool pAlreadyUnitVec = false)
    {
        return pAlreadyUnitVec ? Dot(pLeft, pRight) : Dot(pLeft, pRight.Normalized());
    }

    public Vector2 Normal() => new Vector2(-this.y, this.x).Normalized();

    public static Vector2 VectorProjection(Vector2 pLeft, Vector2 pRight, bool pAlreadyUnitVec = false)
    {
        float sp = ScalarProjection(pLeft, pRight, pAlreadyUnitVec);
        return pAlreadyUnitVec ? pRight * sp : pRight.Normalized() * sp;
    }

    public static float Deg2Rad(float pDegrees) => pDegrees * Mathf.PI / 180;

    public static float Rad2Deg(float pRadians) => pRadians * 180 / Mathf.PI;

    public static Vector2 GetUnitVectorDeg(float pDegrees)
    {
        return GetUnitVectorRad(Deg2Rad(pDegrees));
    }

    public static Vector2 GetUnitVectorRad(float pRad)
    {
        float x = Mathf.Cos(pRad);
        float y = Mathf.Sin(pRad);

        return new Vector2(x, y);
    }

    public static Vector2 RandomUnitVector()
    {
        float randomRadian = Utils.Random(0, Mathf.PI * 2);
        return GetUnitVectorRad(randomRadian);
    }

    public void RotateDegrees(float pDegrees)
    {
        RotateRadians(Deg2Rad(pDegrees));
    }

    public void RotateRadians(float pRadians)
    {
        float sin = Mathf.Sin(pRadians);
        float cos = Mathf.Cos(pRadians);
        SetXY(cos * this.x - sin * this.y, cos * this.y + sin * this.x);
    }

    public void RotateAroundDegrees(Vector2 pTarget, float pDegrees)
    {
        RotateAroundRadians(pTarget, Deg2Rad(pDegrees));
    }

    public void RotateAroundRadians(Vector2 pTarget, float pRadians)
    {
        this -= pTarget;
        RotateRadians(pRadians);
        this += pTarget;
    }

    public float DistanceBetween(Vector2 pOther)
    {
        return Mathf.Sqrt((pOther.x - this.x) * (pOther.x - this.x) + (pOther.y - this.y) * (pOther.y - this.y));
    }

    public static Vector2 operator -(Vector2 pFirst, Vector2 pSecond) => new Vector2(pFirst.x - pSecond.x, pFirst.y - pSecond.y);
    public static Vector2 operator *(Vector2 pVecToScale, float pScalar) => new Vector2(pVecToScale.x * pScalar, pVecToScale.y * pScalar);
    public static Vector2 operator *(float pScalar, Vector2 pVecToScale) => pVecToScale * pScalar;
    public static Vector2 operator /(Vector2 pVecToDiv, float pDivBy) => new Vector2(pVecToDiv.x / pDivBy, pVecToDiv.y / pDivBy);
    public static Vector2 operator +(Vector2 pFirst, Vector2 pSecond) => new Vector2(pFirst.x + pSecond.x, pFirst.y + pSecond.y);

    public override string ToString()
    {
        return string.Format("{0}, {1}", x, y);
    }

    public static void PerformUnitTest()
    {
        Vector2 v1, v2, result;

        v1 = new Vector2(2, 3);

        v2 = v1 * 3;
        Console.WriteLine("Right multiplication ok?: {0}", v2.x == 6 && v2.y == 9);

        v2 = 4 * v1;
        Console.WriteLine("Left multiplication ok?: {0}", v2.x == 8 && v2.y == 12);

        result = v2 - v1;
        Console.WriteLine("Left subtraction ok?: {0}", result.x == 6 && result.y == 9);

        result = v1 - v2;
        Console.WriteLine("Right subtraction ok?: {0}", result.x == -6 && result.y == -9);

        result = v1 + v2;
        Console.WriteLine("Addition ok?: {0}", result.x == 10 && result.y == 15);

        Console.WriteLine("V1 length ok?: {0}", Approximate(v1.Length(), 3, 87f));

        Console.WriteLine("V2 length ok?: {0}", Approximate(v2.Length(), 14, 42f));

        result = v1.Normalized();
        Console.WriteLine("result normalized ok?: {0}", Approximate(result.x, 0.55f) && Approximate(result.y, 0.83f));

        Console.WriteLine("V1 after using normalized ok?: {0}", v1.x == 2 && v1.y == 3);

        v1.SetLength(8);
        Console.WriteLine("Set lenght ok?: {0}", v1.Length() == 8);

        v1.LimitLength(5);
        Console.WriteLine("Limit length ok?: {0}", v1.Length() == 5);

        v1.SetXY(2, -1);
        Console.WriteLine("Set X & Y ok?: {0}", v1.x == 2 && v1.y == -1);
    }

    static bool Approximate(float a, float b, float errorMargin = 0.01f) => (Mathf.Abs(a - b) < errorMargin);

}

