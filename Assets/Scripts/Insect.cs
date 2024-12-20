using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insect : MonoBehaviour
{
    int insectLevel;
    float timer;
    float audioTimer;
    bool isKill;
    SpriteRenderer spriter;
    AudioSource audioPlayer;
    public AudioClip[] clips;
    public Sprite[] sprites;
    public float basicLevelTime;
    public float firstLevelTime;
    public int index=0;
    // public GameObject insectSpawner;

    private void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        audioPlayer = GetComponent<AudioSource>();
    }
    void Init()
    {
        insectLevel = 0;
        timer = 0f;
        audioTimer = 0f;
        spriter.sprite = sprites[0];
        spriter.enabled = true;
        isKill= false;
        audioPlayer.clip= clips[0];
        audioPlayer.loop = false;
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        audioTimer += Time.deltaTime;
        if (audioTimer > 1.0f&&insectLevel==1&&spriter.enabled)
        {
            audioPlayer.Play();
            audioTimer = 0;
        }

        timer += Time.deltaTime;
        if (insectLevel==0&&timer > basicLevelTime)
        {
            insectLevel++;
            spriter.sprite = sprites[1];
            timer = 0;
        }
        if (insectLevel == 1 && timer > firstLevelTime)
        {
            insectLevel++;
            spriter.sprite = sprites[2];
            timer = 0;
            audioPlayer.clip = clips[1];
            audioPlayer.loop = false;
            audioPlayer.Play();

        }
        if (insectLevel == 2 && timer > 1.0f)
        {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.Return) && isKill && insectLevel != 2)
        {
            audioPlayer.clip = clips[2];
            audioPlayer.loop = false;
            audioPlayer.Play();
            spriter.enabled = false;
            Invoke("Kill",1);
        }
    }

    void Die()
    {
        // insectSpawner.GetComponent<InsectSpawner>().isSpawn[index] = false;
        GameManager.DecreaseLife(6);
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void Kill()
    {
        // insectSpawner.GetComponent<InsectSpawner>().isSpawn[index] = false;
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerMove>().playerId == 1)
        {
            isKill = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerMove>().playerId == 1)
        {
            isKill = false;
        }
    }
}
