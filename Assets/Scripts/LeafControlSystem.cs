using UnityEngine;
using System.Collections;

public class LeafControlSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leafControlPanel;
    [SerializeField] private Transform sunBar;
    [SerializeField] private Transform leafBar;

    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer leafRenderer;
    [SerializeField] private Sprite shadeLeafSprite;
    [SerializeField] private Sprite sunlightLeafSprite;
    [SerializeField] private Sprite dryLeafSprite;

    [Header("Movement Settings")]
    [SerializeField] private float moveRange = 150f;
    private float sunMoveSpeed;
    [SerializeField] private float leafMoveSpeed = 200f;

    [Header("Leaf Positions")]
    [SerializeField] private Transform[] leafPositions;
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

    private void Start()
    {
        leafControlPanel.SetActive(false);
        startX = sunBar.position.x;
        player1 = GameObject.Find("player1");

        leafRenderer.sprite = shadeLeafSprite;
        sunMoveSpeed = (moveRange * 2) / 15f;
        SetLeafControlPanelPosition(currentLeafIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
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
        // 햇빛 영역은 항상 움직이도록 설정
        MoveSunBar();
        CheckSunlightStatus();

        CheckPlayerRange();

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleControlPanel();
        }

        // LeafControlPanel이 활성화되었을 때만 잎사귀 막대를 움직일 수 있도록
        if (isControlActive)
        {
            MoveLeafBar();
            UpdateLeafState();
        }
    }

    private void CheckPlayerRange()
    {
        if (player1 != null)
        {
            float distance = Vector2.Distance(transform.position, player1.transform.position);
            isPlayerInRange = distance < 1f;

            if (!isPlayerInRange && isControlActive)
            {
                ToggleControl();
            }
        }
    }

    private void ToggleControl()
    {
        isControlActive = !isControlActive;
        leafControlPanel.SetActive(isControlActive);

        if (!isControlActive)
        {
            StopAllCoroutines();
            isTimerActive = false;
            leafRenderer.sprite = shadeLeafSprite;
        }
    }

    private void ToggleControlPanel()
    {
        isControlActive = !isControlActive;
        leafControlPanel.SetActive(isControlActive);
        SetLeafControlPanelPosition(currentLeafIndex);
    }

    private void SetLeafControlPanelPosition(int leafIndex)
    {
        if (leafIndex >= 0 && leafIndex < leafPositions.Length)
        {
            Vector3 leafPosition = leafPositions[leafIndex].position;
            Vector3 panelPosition = leafPosition + new Vector3(1f, 0f, 0f); // X축으로 1 단위 앞에 배치
            leafControlPanel.transform.position = panelPosition;
        }
    }

    private void UpdateLeafState()
    {
        if (!isInSunlight)
        {
            dryTimer += Time.deltaTime;
            if (dryTimer >= 4f)
            {
                leafRenderer.sprite = dryLeafSprite;
                GameManager.DecreaseLife(3); // 생명력 감소
            }
        }
        else
        {
            dryTimer = 0f;
        }

        if (isInSunlight && leafRenderer.sprite == sunlightLeafSprite)
        {
            sunlightTimer += Time.deltaTime;
            if (sunlightTimer >= 2f)
            {
                GameManager.IncreaseGrowth(1); // 성장 증가
            }
        }
        else
        {
            sunlightTimer = 0f;
        }
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

    private void MoveLeafBar()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        Vector3 position = leafBar.position;
        position.x += moveInput * leafMoveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, startX - moveRange, startX + moveRange);

        leafBar.position = position;
    }

    private void CheckSunlightStatus()
    {
        float distance = Mathf.Abs(leafBar.position.x - sunBar.position.x);
        bool currentlyInSunlight = distance < 30f;

        if (currentlyInSunlight != isInSunlight)
        {
            isInSunlight = currentlyInSunlight;
            leafRenderer.sprite = isInSunlight ? sunlightLeafSprite : dryLeafSprite;
            sunlightTimer = 0f;
            dryTimer = 0f;

            if (isTimerActive)
            {
                StopAllCoroutines();
                isTimerActive = false;
            }
        }

        if (isInSunlight)
        {
            sunlightTimer += Time.deltaTime;
            if (sunlightTimer >= 2f && !isTimerActive)
            {
                StartCoroutine(IncreasePlantGrowth());
                isTimerActive = true;
            }
        }
        else
        {
            dryTimer += Time.deltaTime;
            if (dryTimer >= 4f && !isTimerActive)
            {
                StartCoroutine(DecreasePlantLife());
                isTimerActive = true;
            }
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
            GameManager.DecreaseLife(3);
            yield return new WaitForSeconds(1f);
        }
    }
}
