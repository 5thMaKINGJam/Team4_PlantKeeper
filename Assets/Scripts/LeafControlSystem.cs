using UnityEngine;
using System.Collections;

public class LeafControlSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leafControlPanel;
    [SerializeField] private Transform sunBar;

    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer leafRenderer;
    [SerializeField] private Sprite shadeLeafSprite;
    [SerializeField] private Sprite sunlightLeafSprite;
    [SerializeField] private Sprite dryLeafSprite;

    [Header("Movement Settings")]
    [SerializeField] private float moveRange = 150f;
    private float sunMoveSpeed;
    [SerializeField] private float leafMoveSpeed = 200f;

    [Header("Leaf Settings")]
    [SerializeField] private Transform[] leafPositions;
    [SerializeField] private Transform[] leafBars;
    private int currentLeafIndex = 0;

    private bool isControlActive = false;
    private bool isMovingRight = true;
    private float startX;

    private float sunlightTimer = 0f;
    private float dryTimer = 0f;
    private bool isInSunlight = false;
    private bool isTimerActive = false;

    private GameObject player1;
    private bool isPlayerInRange = false;
    private PlayerMove playerMove;

    private void Start()
    {
        leafControlPanel.SetActive(false);
        startX = sunBar.position.x;
        player1 = GameObject.Find("player1");

        playerMove = player1.GetComponent<PlayerMove>();
        if (playerMove == null)
        {
            Debug.LogWarning("PlayerMove 스크립트를 찾을 수 없습니다.");
        }

        leafRenderer.sprite = shadeLeafSprite;
        sunMoveSpeed = (moveRange * 2) / 15f;
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.name == "player1")
    {
        isPlayerInRange = true;
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    if (other.gameObject.name == "player1")
    {
        isPlayerInRange = false;
        if (isControlActive)
        {
            ToggleControlPanel();
        }
    }
}


    private void Update()
    {
        MoveSunBar();

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleControlPanel();
        }

        if (isControlActive)
        {
            if (playerMove != null)
            {
                playerMove.enabled = false;
            }
            MoveLeafBar();
        }
        else
        {
            if (playerMove != null)
            {
                playerMove.enabled = true;
            }
        }

        CheckSunlightStatus();
        UpdateLeafState();
    }

   private void ToggleControlPanel()
{
    isControlActive = !isControlActive;
    leafControlPanel.SetActive(isControlActive);
    
    if (isControlActive)
    {
        RectTransform panelRect = leafControlPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchoredPosition = Vector2.zero; // Canvas의 중앙에 위치
        }
    }

    if (!isControlActive && playerMove != null)
    {
        playerMove.enabled = true;
    }
}

    private void MoveLeafBar()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        Vector3 position = leafBars[currentLeafIndex].position;
        position.x += moveInput * leafMoveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, startX - moveRange, startX + moveRange);

        leafBars[currentLeafIndex].position = position;
    }

    private void MoveSunBar()
    {
        Vector3 position = sunBar.position;

        if (isMovingRight)
        {
            position.x += sunMoveSpeed * Time.deltaTime;
            if (position.x >= startX + moveRange)
                isMovingRight = false;
        }
        else
        {
            position.x -= sunMoveSpeed * Time.deltaTime;
            if (position.x <= startX - moveRange)
                isMovingRight = true;
        }

        sunBar.position = position;
    }

    private void CheckSunlightStatus()
    {
        if (currentLeafIndex < 0 || currentLeafIndex >= leafBars.Length)
        {
            Debug.LogWarning("currentLeafIndex가 leafBars 배열의 범위를 벗어났습니다.");
            return;
        }

        float distance = Mathf.Abs(leafBars[currentLeafIndex].position.x - sunBar.position.x);
        bool currentlyInSunlight = distance < 30f;

        if (currentlyInSunlight != isInSunlight)
        {
            isInSunlight = currentlyInSunlight;
            UpdateLeafSprite();
            sunlightTimer = 0f;
            dryTimer = 0f;

            StopAllCoroutines();
            if (isInSunlight)
            {
                StartCoroutine(IncreasePlantGrowth());
            }
        }
    }

    private void UpdateLeafSprite()
    {
        if (isInSunlight)
        {
            leafRenderer.sprite = sunlightLeafSprite;
        }
        else
        {
            leafRenderer.sprite = shadeLeafSprite;
            StartCoroutine(CheckDryStatus());
        }
    }

    private void UpdateLeafState()
    {
        if (!isInSunlight)
        {
            dryTimer += Time.deltaTime;
            if (dryTimer >= 4f && leafRenderer.sprite != dryLeafSprite)
            {
                leafRenderer.sprite = dryLeafSprite;
                StartCoroutine(DecreasePlantLife());
            }
        }
        else
        {
            dryTimer = 0f;
            StopCoroutine(DecreasePlantLife());
        }
    }

    private IEnumerator CheckDryStatus()
    {
        yield return new WaitForSeconds(4f);
        if (!isInSunlight)
        {
            leafRenderer.sprite = dryLeafSprite;
            StartCoroutine(DecreasePlantLife());
        }
    }

    private IEnumerator IncreasePlantGrowth()
    {
        while (isInSunlight)
        {
            GameManager.IncreaseGrowth(1);
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator DecreasePlantLife()
    {
        while (!isInSunlight)
        {
            GameManager.DecreaseLife(1);
            yield return new WaitForSeconds(1f);
        }
    }
}