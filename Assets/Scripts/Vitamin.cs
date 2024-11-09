using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vitamin : MonoBehaviour
{
    private AudioSource soundEffect;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int amount;

    void Awake()
    {
        soundEffect = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.IncreaseVitamin(amount);
        soundEffect.Play();
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1f);
    }
}
