using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insect : MonoBehaviour
{
    int insectLevel;
    float timer;
    SpriteRenderer spriter;
    Sprite[] sprites;

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

        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
