using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MyObject
{
    protected override bool viewHealthBar => false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        int imageId = 1;
        int period = (int)(Application.targetFrameRate * 1.5f);
        float r = (float)(Main.tick % period) / period;
        if (5f / 8f < r && r < 7f / 8f)
        {
            imageId = 3;
        }
        else if(4f/8f < r)
        {
            imageId = 2;
        }
        ChangeImage("Coin" + imageId);
    }
}
