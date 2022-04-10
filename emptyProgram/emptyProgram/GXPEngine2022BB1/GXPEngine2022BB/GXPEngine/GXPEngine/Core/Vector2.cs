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

    //extra method
    public void SetLength(float pScale)
    {
        this.Normalize();
        this.Mult(pScale);
    }

    //extra method
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

    //extra method
    public static Vector2 COM(Vector2 pVelocity1, Vector2 pVelocity2, float pMass1, float pMass2) => (pVelocity1 * pMass1 + pVelocity2 * pMass2) / (pMass1 + pMass2);

    public void Reflect(float pBounciness, Vector2 pNormal, Vector2 pCom = new Vector2())
    {
        Vector2 vectorProjection = Vector2.VectorProjection(this - pCom, pNormal);
        this.SetXY(this - (1 + pBounciness) * vectorProjection);
    }

    public float Dot(Vector2 pOther) => Dot(this, pOther);

    public static float Dot(Vector2 pLeft, Vector2 pRight) => pLeft.x * pRight.x + pLeft.y * pRight.y;

    //extra method
    public float ScalarProjection(Vector2 pOther, bool pAlreadyUnitVec = false) => ScalarProjection(this, pOther, pAlreadyUnitVec);

    public static float ScalarProjection(Vector2 pLeft, Vector2 pRight, bool pAlreadyUnitVec = false)
    {
        return pAlreadyUnitVec ? Dot(pLeft, pRight) : Dot(pLeft, pRight.Normalized());
    }

    public Vector2 Normal() => new Vector2(-this.y, this.x).Normalized();

    //extra method
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

    //extra method
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

        v1 = new Vector2(2, 4);
        Vector2 v3 = new Vector2(4, 6);
        Vector2 v4 = new Vector2(2, 1);

        Console.WriteLine("Degrees to Radians conversion ok?: {0}", Approximate(Vector2.Deg2Rad(360), Mathf.PI * 2));
        Console.WriteLine("Radians to Degrees conversion ok?: {0}", Approximate(Vector2.Rad2Deg(Mathf.PI), 180));
        Console.WriteLine("Get angles degrees ok?: {0}", Approximate(v1.GetAngleDegrees(), 63.43f));
        Console.WriteLine("Get angles radians ok?: {0}", Approximate(v1.GetAngleRadians(), 1.10f));
        v1.SetAngleDegrees(270);
        Console.WriteLine("Set angles degrees ok?: {0}", Approximate(v1.x, 0) && Approximate(v1.y, -4.47f));
        v1.SetAngleRadians(Mathf.PI);
        Console.WriteLine("Set angles radians ok?: {0}", Approximate(v1.x, -4, 47f) && Approximate(v1.y, 0));
        v2 = Vector2.GetUnitVectorDeg(270);
        Console.WriteLine("Get unit vector degrees ok?: {0}", Approximate(v2.x, 0) && Approximate(v2.y, -1) && Approximate(v2.Length(), 1));
        v2 = Vector2.GetUnitVectorRad(5f);
        Console.WriteLine("Get unit vector radians ok?: {0}", Approximate(v2.x, 0.28f) && Approximate(v2.y, -0.96f) && Approximate(v2.Length(), 1));
        for (int i = 0; i < 4; i++)
        {
            Vector2 v5 = Vector2.RandomUnitVector();
            Console.WriteLine("Vector number " + i + ": " + v5 + "\n length is okay?: " + Approximate(v5.Length(), 1));
        }
        v1.SetXY(2, 4);
        v1.RotateDegrees(45);
        Console.WriteLine("Rotate degrees ok?: {0}", Approximate(v1.GetAngleDegrees(), 108.43f));
        v1.SetXY(2, 4);
        v1.RotateRadians(Mathf.PI / 4);
        Console.WriteLine("Rotate radians ok?: {0}", Approximate(v1.GetAngleRadians(), 1.90f));
        v3.RotateAroundDegrees(v4, 90);
        Console.WriteLine("Rotate around degrees ok?: {0}", Approximate(v3.x, -3) && Approximate(v3.y, 3));
        v3.SetXY(4, 6);
        v3.RotateAroundRadians(v4, 5f);
        Console.WriteLine("Rotate around radians ok?: {0}", Approximate(v3.x, 7.36f) && Approximate(v3.y, 0.50f));

        v1.SetXY(3,4);
        v2.SetXY(4,6);
        Console.WriteLine("Dot product ok?: {0}",v1.Dot(v2) == 36);

        float mass1 = 4;
        float mass2 = 5;

        Vector2 com = Vector2.COM(v1, v2, mass1, mass2);
        Console.WriteLine("Center of Mass velocity ok?: {0}", Vector2.Approximate(com.x, 3.55f) && Vector2.Approximate(com.y, 5.11f));
        v1.Reflect(1, v2.Normal(), com);
        Console.WriteLine("Reflect ok?: {0}", Vector2.Approximate(v1.x, 2.743f) && Vector2.Approximate(v1.y,4.17f));
        v1.SetXY(3, 4);
        Console.WriteLine("ScalarProjection ok?: {0}", Vector2.Approximate(v1.ScalarProjection(v2),4.992f));
        Vector2 vp = Vector2.VectorProjection(v1, v2);
        Console.WriteLine("VectorProjection ok?: {0}", Vector2.Approximate(vp.x,2.76f),Vector2.Approximate(vp.y,4.15f));
        Vector2 norm = v1.Normal();
        Console.WriteLine("Normal ok?: {0}",norm.x == -0.8f && norm.y == 0.6f);
        Console.WriteLine("distance between ok?: {0}", Vector2.Approximate(v1.DistanceBetween(v2), 2.23f));
    }

    static bool Approximate(float a, float b, float errorMargin = 0.01f) => (Mathf.Abs(a - b) < errorMargin);

}

