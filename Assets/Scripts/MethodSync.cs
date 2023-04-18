using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodSync
{
    private static bool isLocked = false;

    public static void Enter()
    {
        while (isLocked) { }
        isLocked = true;
    }

    public static void Exit() => isLocked = false;
}
