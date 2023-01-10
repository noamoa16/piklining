using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MyObject
{
    [SerializeField]
    private float timeBeforeExplosion;
    protected override bool autoDamage => true;

    protected override bool hasInvincibleTime => false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        transform.localEulerAngles = new Vector3(0, 0, Main.random.NextFloat(0, 360));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isFired)
        {
            float alpha = 1 - hp / maxHp;
            GameObject firedBomb = GetChild("FiredBomb");
            Color color = firedBomb.gameObject.GetComponent<SpriteRenderer>().color;
            color.a = alpha;
            firedBomb.GetComponent<SpriteRenderer>().color = color;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if(!Main.isQuitting)
            Create("Explosion", transform.position, Main.random.NextFloat(0, 360));
    }

    public void Fired()
    {
        hp = timeBeforeExplosion * Application.targetFrameRate;
        maxHp = timeBeforeExplosion * Application.targetFrameRate;
        GameObject firedBomb = CreateChild("FiredBomb", transform.position);
        Color color = firedBomb.gameObject.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        firedBomb.GetComponent<SpriteRenderer>().color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        if (!isFired && collider.name == "Seesaw")
        {
            Fired();
        }
    }

    public bool isFired => maxHp >= 0;
}
