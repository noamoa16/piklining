using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MyObject
{
    [SerializeField]
    private float timeToExplosion;
    protected override bool autoDamage => true;
    protected override bool viewHealthBar => false;

    protected override bool hasInvincibleTime => false;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hp = timeToExplosion * Application.targetFrameRate;
        maxHp = timeToExplosion * Application.targetFrameRate;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0;
        GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        float rate = 1 - hp / maxHp;
        float alpha = Mathf.Sin(Mathf.PI / 4.0f + rate * Mathf.PI * 3.0f / 4.0f);
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = alpha;
        GetComponent<SpriteRenderer>().color = color;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        MyObject myObject = collision.gameObject.GetComponent<MyObject>();
        if (myObject != null)
        {
            myObject.Damaged(1f);
        }

        // ”š’e‚Éˆø‰Î‚µ‚Ä—U”š
        if (collision.gameObject.name == "Bomb")
        {
            Bomb bomb = collision.gameObject.GetComponent<Bomb>();
            if (!bomb.isFired)
            {
                bomb.Fired();
            }
            bomb.Killed();
        }
    }
}
