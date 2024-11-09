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
    [SerializeField] private Transform leafBar;

    private static bool isAnyPanelActive = false;
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
        
        // 게임 시작 시 DryTimer 시작
        StartCoroutine(CheckDryStatus());
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

        if (isAnyPanelActive)
        {
            if (playerMove != null)
            {
                playerMove.enabled = false;
            }
        }
        else
        {
            if (playerMove != null)
            {
                playerMove.enabled = true;
            }
        }

        if (isControlActive)
        {
            MoveLeafBar();
        }

        CheckSunlightStatus();
        UpdateLeafState();
    }

    private void ToggleControlPanel()
    {
        isControlActive = !isControlActive;
        isAnyPanelActive = isControlActive;
        leafControlPanel.SetActive(isControlActive);

        if (isControlActive)
        {
            RectTransform panelRect = leafControlPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    private void MoveLeafBar()
    {
        Debug.Log("moveleafbar called");
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            Debug.Log("AAAAA");
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            Debug.Log("BBBBB");
        }

        if (leafBar == null)
        {
            Debug.LogError("leafBar가 null입니다. 인스펙터에서 설정해 주세요.");
            return;
        }

        Vector3 position = leafBar.position;
        position.x += moveInput * leafMoveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, startX - moveRange, startX + moveRange);

        leafBar.position = position;
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
        if (leafBar == null)
        {
            Debug.LogError("leafBar가 null입니다.");
            return;
        }

        float distance = Mathf.Abs(leafBar.position.x - sunBar.position.x);
        bool currentlyInSunlight = distance < 30f;

        if (currentlyInSunlight != isInSunlight)
        {
            isInSunlight = currentlyInSunlight;
            
            StopAllCoroutines();
            if (isInSunlight)
            {
                leafRenderer.sprite = sunlightLeafSprite;
                StartCoroutine(IncreasePlantGrowth());
            }
            else
            {
                leafRenderer.sprite = shadeLeafSprite;
                StartCoroutine(CheckDryStatus());
            }
        }
    }

    private void UpdateLeafState()
    {
    }

    private IEnumerator CheckDryStatus()
    {
        yield return new WaitForSeconds(4f);
        
        if (!isInSunlight && leafRenderer.sprite == shadeLeafSprite)
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
        while (!isInSunlight && leafRenderer.sprite == dryLeafSprite)
        {
            GameManager.DecreaseLife(1);
            yield return new WaitForSeconds(1f);
        }
    }
}