using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insect : MonoBehaviour
{
    int insectLevel;
    float timer;
    SpriteRenderer spriter;
    public Sprite[] sprites;

    private void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
    }
    void Init()
    {
        insectLevel = 0;
        timer = 0f;
        spriter.sprite = sprites[0];
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (insectLevel==0&&timer > 4.0f)
        {
            insectLevel++;
            spriter.sprite = sprites[1];
            timer = 0;
        }
        if (insectLevel == 1 && timer > 2.0f)
        {
            insectLevel++;
            spriter.sprite = sprites[2];
            timer = 0;
        }
        if (insectLevel == 2 && timer > 1.0f)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
        //GameManager.DecreaseLife(8);
    }

    void Kill()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerMove>().playerId == 0)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) Kill();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.RightShift)) Kill();
            }
        }
    }
}
