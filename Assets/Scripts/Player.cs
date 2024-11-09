using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerId;
    public Vector3 inputVec1;
    public Vector3 inputVec2;
    Rigidbody2D rigid;
    public float runSpeed;
    public float jumpForce;
    public bool isGround = true;
    bool leafOpen;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputVec1 = Vector3.zero;
        inputVec2 = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            inputVec1 = -Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVec1 = Vector3.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (playerId == 0) Jump();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputVec2 = -Vector3.right;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVec2 = Vector3.right;
        }

        if (playerId == 0)
        {
            Vector2 nextVec = inputVec1.normalized * runSpeed * Time.deltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }
        else
        {
            Vector2 nextVec = inputVec2.normalized * runSpeed * Time.deltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }
    }

    void FixedUpdate()
    {
        /*
        if (playerId == 0)
        {
            Vector2 nextVec = inputVec1.normalized * runSpeed * Time.deltaTime;
            //rigid.MovePosition(rigid.position + nextVec);
        }
        else
        {
            Vector2 nextVec = inputVec2.normalized * runSpeed * Time.deltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }*/
    }

    void Jump()
    {
        Debug.Log(Vector2.up * jumpForce);
        rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGround = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
}
