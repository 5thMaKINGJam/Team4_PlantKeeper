using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int playerId;
    public float maxSpeed;
    public float jumpPower;
    public bool isGround = true;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.W)&&playerId==0)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && playerId == 1)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        // Stop Speed 
        if ((Input.GetKeyUp(KeyCode.A)||Input.GetKeyUp(KeyCode.D))&&playerId==0)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        }
        if ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) && playerId == 1)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        }

        if (Input.GetKeyDown(KeyCode.A)&&playerId==0) spriteRenderer.flipX = true;
        if (Input.GetKeyDown(KeyCode.D) && playerId == 0) spriteRenderer.flipX = false;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && playerId == 1) spriteRenderer.flipX = true;
        if (Input.GetKeyDown(KeyCode.RightArrow) && playerId == 1) spriteRenderer.flipX = false;

    }


    void FixedUpdate()
    {
        // Move by Control
        float h = 0.0f;
        if (Input.GetKey(KeyCode.A)&&playerId==0) h = -1.0f;
        if (Input.GetKey(KeyCode.D) && playerId == 0) h = 1.0f;
        if (Input.GetKey(KeyCode.LeftArrow) && playerId == 1) h = -1.0f;
        if (Input.GetKey(KeyCode.RightArrow) && playerId == 1) h = 1.0f;
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);


        // MaxSpeed Limit
        if (rigid.velocity.x > maxSpeed)// right
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) // Left Maxspeed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        // Lending Platform
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                }
            }
        }

    }
}
