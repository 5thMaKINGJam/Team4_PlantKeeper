using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private AudioSource lifeDecreasingSound;
    [Header("Ranking Version")]
    [SerializeField] string rankingVersion;

    [Header("Bar > Background > Slider")]
    [SerializeField] Image lifeBar; // bar prefab 내 Cover
    [SerializeField] Image growthBar;
    [SerializeField] Image waterBar;
    [SerializeField] Image vitaminBar;

    private BarState life;
    private BarState growth;
    private BoundedBarState water;
    private BoundedBarState vitamin;

    // Edit in Unity Editor //
    // !!!!DO NOT INITIALIZE HERE!!!! //
    [Header("Settings - Life")]
    [SerializeField] int maxLife;
    [SerializeField] int initial_life;

    [Header("Settings - Growth")]
    [SerializeField] int maxGrowth;
    [SerializeField] int initialGrowth;

    [Header("Settings - Water")]
    [SerializeField] int maxWater;
    [SerializeField] int initialWater;

    [SerializeField] int waterMinBound;
    [SerializeField] int waterMaxBound;

    [Header("Settings - Vitamin")]
    [SerializeField] int maxVitamin;
    [SerializeField] int initialVitamin;
    [SerializeField] int vitaminMinBound;
    [SerializeField] int vitaminMaxBound;

    [Header("Settings - Decrease/Increase Amount Per Second (Absolute Value)")]
    [SerializeField] int lifeDecreaseAmount;
    [SerializeField] int growthIncreaseAmount;

    private float t = 0;
    private string recordId;

    private bool isEnded = false;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        life = new BarState(initial_life, maxLife, lifeBar);
        growth = new BarState(initialGrowth, maxGrowth, growthBar);
        water = new BoundedBarState(initialWater, maxWater, waterBar, waterMinBound, waterMaxBound);
        vitamin = new BoundedBarState(initialVitamin, maxVitamin, vitaminBar, vitaminMinBound, vitaminMaxBound);

        _instance.lifeDecreasingSound = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateStateForEverySecond());
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnded)
        {
            return;
        }
        t += Time.deltaTime; // 시간 누적
        // int minutes = (int)(t / 60);
        // int seconds = (int)(t % 60);
        // timeT.text = $"{minutes:D2}:{seconds:D2}"; // "MM:SS" 형식으로 표시
    }

    public static void DecreaseLife(int x)
    {
        _instance.lifeDecreasingSound.Play();
        _instance.life.UpdateValue(-x);
        _instance.CheckExitCondition();
    }

    public static void IncreaseGrowth(int x)
    {
        _instance.growth.UpdateValue(x);
        _instance.CheckExitCondition();
    }

    public static void IncreaseWater(int x)
    {
        _instance.water.UpdateValue(x);
    }

    public static void IncreaseVitamin(int x)
    {
        _instance.vitamin.UpdateValue(x);
    }

    private async void CheckExitCondition()
    {
        if (isEnded)
        {
            return;
        }
        if (_instance.growth.GetValue() == maxGrowth)
        {
            isEnded = true;
            _instance.recordId = await FirebaseService.InsertNewData(new Ranking(Nickname.nickname, (int)(t * 1000)));
            SceneManager.LoadScene("GameClear");
        }
        if (_instance.life.GetValue() == 0)
        {
            isEnded = true;
            SceneManager.LoadScene("GameOver");
        }
    }


    IEnumerator UpdateStateForEverySecond()
    {
        while (true)
        {
            // decrease water&vitamin by 1 for every second
            _instance.water.UpdateValue(-1);
            _instance.vitamin.UpdateValue(-1);

            // update life & growth
            bool isWaterInBound = _instance.water.isInBound();
            bool isVitaminInBound = _instance.vitamin.isInBound();

            if (!isWaterInBound)
            {
                _instance.life.UpdateValue(-lifeDecreaseAmount);
            }
            if (!isVitaminInBound)
            {
                _instance.life.UpdateValue(-lifeDecreaseAmount);
            }
            if (isWaterInBound && isVitaminInBound)
            {
                _instance.growth.UpdateValue(growthIncreaseAmount);
            }

            _instance.CheckExitCondition();

            yield return new WaitForSeconds(1f);
        }
    }

    public static string GetVersion()
    {
        return _instance.rankingVersion;
    }

    public static string GetRecordId()
    {
        return _instance.recordId;
    }
}
