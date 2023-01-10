using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GottenCoin : MyObject
{
    protected override bool viewHealthBar => false;
    protected override bool autoDamage => true;
    protected override bool hasInvincibleTime => false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 375f));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        int id = (Main.tick % 8) / 2; // [0, 3]
        ChangeImage("GottenCoin" + (id == 1 || id == 2 ? 2 : 1));
        GetComponent<SpriteRenderer>().flipX = id == 2 || id == 3;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Main.isQuitting)
        {
            // 
            //
        }
    }
}
