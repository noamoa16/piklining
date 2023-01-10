using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObject : MyObject
{
    protected override bool viewHealthBar => false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
