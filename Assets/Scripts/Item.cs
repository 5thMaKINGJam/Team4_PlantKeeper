using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Water,       // 0
    Vitamin,     // 1
}

public class Item : MonoBehaviour
{
    private AudioSource soundEffect;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private ItemType itemType;
    [SerializeField] private int amount;
    [SerializeField] private float timeToDestroy;

    void Awake()
    {
        soundEffect = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        switch (itemType)
        {
            case ItemType.Water:
                GameManager.IncreaseWater(amount);
                break;
            case ItemType.Vitamin:
                GameManager.IncreaseVitamin(amount);
                break;
        }
        soundEffect.Play();
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1f);
    }
}
