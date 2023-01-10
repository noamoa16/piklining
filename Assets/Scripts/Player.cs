using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MyObject
{
    [SerializeField]
    private float maxSpeed = 4;

    public LayerMask collisionLayer;

    private Dictionary<KeyCode, bool> lastKeyState = new Dictionary<KeyCode, bool>();
    private Dictionary<KeyCode, bool> currentKeyState = new Dictionary<KeyCode, bool>();

    protected override int paintedOrder => 2;

    protected override bool viewHealthBar => false;

    private Assets.Scripts.Button leftButton, rightButton, enterButton;
    private int jumpCount = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        lastKeyState[KeyCode.Return] = false;
        currentKeyState[KeyCode.Return] = false;

        GameObject canvasObject = GameObject.Find("Canvas");
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        for (int i = 0; i < canvasObject.transform.childCount; i++)
        {
            GameObject child = canvasObject.transform.GetChild(i).gameObject;
            Assets.Scripts.Button button = child.GetComponent<Assets.Scripts.Button>();
            if (child.name == "LeftButton")
            {
                leftButton = button;
            }
            else if (child.name == "RightButton")
            {
                rightButton = button;
            }
            else if (child.name == "EnterButton")
            {
                enterButton = button;
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        lastKeyState = new Dictionary<KeyCode, bool>(currentKeyState);
        currentKeyState[KeyCode.Return]
            = Input.GetKey(KeyCode.Return)
            | Input.GetKey(KeyCode.Space)
            | enterButton.isPressed;

        // 移動
        float h = Input.GetAxis("Horizontal");
        h -= leftButton.isPressed ? 1 : 0;
        h += rightButton.isPressed ? 1 : 0;
        h = Mathf.Clamp(h, -1, 1);
        velocity = new Vector2(h * maxSpeed, velocity.y);

        // Enterが押された瞬間
        if (
            !lastKeyState[KeyCode.Return] &&
            currentKeyState[KeyCode.Return] &&
            IsGround()
            )
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 2500);
            jumpCount = 0;
        }
        else
        {
            jumpCount++;
        }

        // 押しっぱなしでジャンプ継続
        float maxJumpCount = (int)(Application.targetFrameRate / 5.0f);
        if(
            lastKeyState[KeyCode.Return] &&
            currentKeyState[KeyCode.Return] &&
            0 < jumpCount && jumpCount <= maxJumpCount
            )
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 2500 / maxJumpCount);
        }

        // シーソーが落ちたら死亡
        if(GameObject.Find("Seesaw") == null)
        {
            Killed();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Main.isQuitting)
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
    }

    private void OnTriggerStay2D(Collider2D collider)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        // コインと衝突した場合
        Coin coin = collider.gameObject.GetComponent<Coin>();
        if(coin != null)
        {
            Create("GottenCoin", collider.gameObject.transform.position);
            Destroy(collider.gameObject);
            ScoreText scoreText
                = Main.GetScore(collider.gameObject.transform.position, 100);
        }

        // トゲと衝突した場合
        if (collider.GetComponent<SpriteRenderer>().sprite.name == "Thorn")
        {
            Damaged(1f);
        }

        // ハートと衝突した場合
        if (collider.GetComponent<SpriteRenderer>().sprite.name == "Heart")
        {
            Destroy(collider.gameObject);
            Healed(1f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

    }
}