using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public static class  TrueSyncExt {

    public static TSVector MoveTowardsV3(TSVector current, TSVector target, FP maxDistanceDelta)
    {        
        TSVector a = target - current;
        FP magnitude = a.magnitude;

        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }

        return current + a / magnitude * maxDistanceDelta;
    }

    public static TSVector2 MoveTowardsV2(TSVector2 current, TSVector2 target, FP maxDistanceDelta)
    {
        TSVector2 a = target - current;
        FP magnitude = a.magnitude;

        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }

        return current + a / magnitude * maxDistanceDelta;
    }


    public static FP LerpFP(FP a, FP b, FP f)
    {
        return a + f * (b - a);
    }
}
