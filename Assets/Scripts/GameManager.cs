using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    [SerializeField] Image lifeBar; // bar prefab 내 Cover
    [SerializeField] Image growthBar;

    private AudioSource lifeDecreasingSound;

    [SerializeField] private int lifeNum;
    [SerializeField] private int growthNum;

    const int MAX_LIFE = 100;
    const int MAX_GROWTH = 500;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        _instance.growthNum = 0;
        _instance.lifeNum = 100;
        _instance.lifeDecreasingSound = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _instance.lifeBar.fillAmount = 1 - ((float)_instance.lifeNum / MAX_LIFE);
        _instance.growthBar.fillAmount = 1 - ((float)_instance.growthNum / MAX_GROWTH);
    }

    public static void DecreaseLife(int x)
    {
        _instance.lifeDecreasingSound.Play();
        _instance.lifeNum -= x;
        if (_instance.growthNum <= MAX_GROWTH && _instance.lifeNum <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public static void IncreaseGrowth(int x)
    {
        _instance.growthNum += x;
        if (_instance.growthNum >= MAX_GROWTH)
        {
            SceneManager.LoadScene("GameClear");
        }
    }
}
