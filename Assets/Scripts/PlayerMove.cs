using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int playerId;

    public readonly float maxSpeed = 5;
    public readonly float jumpPower = 15;
    private bool isJumpPending = false;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    private KeyCode leftKey, rightKey, upKey;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        leftKey = playerId == 0 ? KeyCode.A : KeyCode.LeftArrow;
        rightKey = playerId == 0 ? KeyCode.D : KeyCode.RightArrow;
        upKey = playerId == 0 ? KeyCode.W : KeyCode.UpArrow;
    }

    void Update()
    {
        if (Input.GetKeyDown(upKey))
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1, LayerMask.GetMask("Ground"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    isJumpPending = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // UP
        if (isJumpPending)
        {
            isJumpPending = false;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        float dir = 0.0f;
        // LEFT
        if (Input.GetKey(leftKey))
        {
            dir = -1.0f;
            spriteRenderer.flipX = true;
        }

        // RIGHT
        if (Input.GetKey(rightKey))
        {
            dir = 1.0f;
            spriteRenderer.flipX = false;
        }
        // Move horizontally
        rigid.AddForce(Vector2.right * dir, ForceMode2D.Impulse);
        // MaxSpeed Limit
        if (rigid.velocity.x > maxSpeed)// right
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) // Left Maxspeed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
    }


}
