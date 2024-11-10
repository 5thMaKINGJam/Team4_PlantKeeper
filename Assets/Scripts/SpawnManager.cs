using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SpawnSetting
{
    public GameObject prefab;
    public float minSpawnInterval;
    public float maxSpawnInterval;
    public float lockSeconds;

    [HideInInspector]
    public Vector2 prefabSize;
}
public class RandomSection
{
    public float y;
    public float minX, maxX;

    public bool isLocked = false;

    public RandomSection(float minX, float maxX, float y)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.y = y;
    }

    public RandomSection(Collider2D collider) :
        this(collider.bounds.min.x, collider.bounds.max.x, collider.bounds.max.y)
    {
    }

    public float GetWidth()
    {
        return maxX - minX;
    }
}

public struct RandomResult
{
    public Vector3 position;
    public int sectionIdx;

    public RandomResult(Vector3 pos, int idx)
    {
        position = pos;
        sectionIdx = idx;
    }
}

public class RandomArea
{
    const float minSectionWidth = 2f;
    public List<RandomSection> sections = new List<RandomSection>();

    private int retryCount = 5;
    public RandomSection baseArea;

    public RandomArea(RandomSection baseArea)
    {
        this.baseArea = baseArea;
        _SplitArea();
    }

    private void _SplitArea()
    {
        float x = baseArea.minX;
        // split with minSectionWidth
        while (x + minSectionWidth <= baseArea.maxX)
        {
            sections.Add(new RandomSection(x, x + minSectionWidth, baseArea.y));
            x += minSectionWidth;
        }

        if (x < baseArea.maxX)
        {
            sections.Add(new RandomSection(x, baseArea.maxX, baseArea.y));
        }
    }


    public RandomResult? GetRandomPosition(SpawnSetting spawnSetting)
    {
        for (int count = 0; count < retryCount; count++)
        {
            int sectionIdx = Random.Range(0, this.sections.Count);
            if (sections[sectionIdx].isLocked)
            {
                continue;
            }

            float posX = Random.Range(sections[sectionIdx].minX, sections[sectionIdx].maxX);
            if (posX - spawnSetting.prefabSize.x / 2 < baseArea.minX || posX + spawnSetting.prefabSize.x / 2 > baseArea.maxX) // 공간 부족
            {
                continue;
            }
            ToggleSections(sectionIdx, true);
            // StartCoroutine(ExecuteAfterDelay(1f)); -> Manager에서 호출하기!!!!
            // y 좌표도 collider에 맞게 수정하기
            return new RandomResult(new Vector3(posX - spawnSetting.prefabSize.x / 2, sections[sectionIdx].y + spawnSetting.prefabSize.y / 2, 0f), sectionIdx);
        }

        return null;
    }

    public void ToggleSections(int center, bool flag)
    {
        sections[center].isLocked = flag;
        if (center > 0)
        {
            sections[center - 1].isLocked = flag;
        }
        if (center + 1 < sections.Count)
        {
            sections[center + 1].isLocked = flag;
        }
    }
}


public class SpawnManager : MonoBehaviour
{
    public List<SpawnSetting> settings = new List<SpawnSetting>();

    public GameObject spawnArea;

    private List<RandomArea> randomAreas;

    private int retryCount = 5;

    private void Awake()
    {
        GetRandomAreaFromSpawnArea();
        GetBoundsFromPrefabs();
    }

    private void GetRandomAreaFromSpawnArea()
    {
        randomAreas = spawnArea.GetComponentsInChildren<Collider2D>()
            .Where(collider => collider.gameObject != spawnArea)
            .Select(collider => new RandomArea(new RandomSection(collider)))
            .ToList();
    }

    private void GetBoundsFromPrefabs()
    {
        foreach (SpawnSetting item in settings)
        {
            Collider2D collider = item.prefab.GetComponent<Collider2D>();
            if (collider != null)
            {
                // CircleCollider2D의 경우
                if (collider is CircleCollider2D)
                {
                    CircleCollider2D circleCollider = (CircleCollider2D)collider;
                    item.prefabSize = new Vector2(circleCollider.radius * 2, circleCollider.radius * 2); // 반지름을 두 배로
                }
                // BoxCollider2D의 경우
                else if (collider is BoxCollider2D)
                {
                    BoxCollider2D boxCollider = (BoxCollider2D)collider;
                    item.prefabSize = boxCollider.size; // BoxCollider2D의 size
                }
                // CapsuleCollider2D의 경우
                else if (collider is CapsuleCollider2D)
                {
                    CapsuleCollider2D capsuleCollider = (CapsuleCollider2D)collider;
                    item.prefabSize = new Vector2(capsuleCollider.size.x, capsuleCollider.size.y); // CapsuleCollider2D의 size
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 각 setting 별로 코루틴 호출하기
        // 코루틴 내부: areaIdx 추첨, o
        // vector 받았으면 lock 해제 예약걸기 o
        // 못 받았으면 다시 추첨 o
        // 이상 반복하기

        for (int i = 0; i < settings.Count; i++)
        {
            StartCoroutine(StartRandomSpawn(i));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PrintStatus()
    {
        System.String str = "";
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < randomAreas[i].sections.Count; j++)
            {
                str += randomAreas[i].sections[j].isLocked;
                str += " ";
            }
            str += "\n";
        }
        Debug.Log(str);
    }

    void Spawn(int settingIdx)
    {
        int areaIdx = Random.Range(0, randomAreas.Count);

        for (int i = 0; i < retryCount; i++)
        {
            RandomResult? result = randomAreas[areaIdx].GetRandomPosition(settings[settingIdx]);
            if (result.HasValue)
            {
                Instantiate(settings[settingIdx].prefab, result.Value.position, Quaternion.identity);
                // release 예약
                StartCoroutine(ReleaseSections(settings[settingIdx].lockSeconds, areaIdx, result.Value.sectionIdx));
                break;
            }
        }
        // PrintStatus();
    }

    IEnumerator StartRandomSpawn(int settingIdx)
    {
        while (true)
        {
            Spawn(settingIdx);
            // wait for random seconds
            yield return new WaitForSeconds(Random.Range(settings[settingIdx].minSpawnInterval, settings[settingIdx].maxSpawnInterval));
        }
    }

    IEnumerator ReleaseSections(float delay, int areaIdx, int sectionIdx)
    {
        yield return new WaitForSeconds(delay);
        randomAreas[areaIdx].ToggleSections(sectionIdx, false);
    }
}