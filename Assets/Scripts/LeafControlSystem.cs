using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeafControlSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leafControlPanel;
    [SerializeField] private RectTransform sunBar;
    [SerializeField] private RectTransform leafBar;
    
    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer leafRenderer;
    [SerializeField] private Sprite shadeLeafSprite;
    [SerializeField] private Sprite sunlightLeafSprite;
    [SerializeField] private Sprite dryLeafSprite;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveRange = 150f;
    private float sunMoveSpeed;
    [SerializeField] private float leafMoveSpeed = 200f;
    
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
        startX = sunBar.anchoredPosition.x;
        player1 = GameObject.Find("player1");
        
        leafRenderer.sprite = shadeLeafSprite;
        sunMoveSpeed = (moveRange * 2) / 15f;
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
        CheckPlayerRange();
        
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleControlPanel();
        }
        
        if (isControlActive)
        {
            MoveSunBar();
            MoveLeafBar();
            UpdateLeafState();
        }

        if (!isControlActive && leafRenderer.sprite != shadeLeafSprite)
        {
            leafRenderer.sprite = shadeLeafSprite;
        }
    }
    
    private void CheckPlayerRange()
    {
        if (player1 != null)
        {
            float distance = Vector2.Distance(transform.position, player1.transform.position);
            isPlayerInRange = distance < 2f;
            
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
    }

    private void UpdateLeafState()
{
    // 그늘 상태로 4초 경과 후 시든 상태로 변경
    if (!isInSunlight)
    {
        dryTimer += Time.deltaTime;
        if (dryTimer >= 4f)
        {
            leafRenderer.sprite = dryLeafSprite; // 시든 상태로 변경
            //TODO: DecreaseLifeOverTime(); // 생명력 감소 로직 호출
        }
    }
    else
    {
        dryTimer = 0f; // 타이머 초기화
    }

    // 햇빛 상태로 2초 경과 후 성장 증가
    if (isInSunlight)
    {
        sunlightTimer += Time.deltaTime;
        if (sunlightTimer >= 2f)
        {
            //TODO: IncreaseGrowthOverTime(); // 성장 증가 로직 호출
        }
    }
    else
    {
        sunlightTimer = 0f; // 타이머 초기화
    }
}

    
    private void MoveSunBar()
    {
        Vector2 position = sunBar.anchoredPosition;
        
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
        
        sunBar.anchoredPosition = position;
    }
    
    private void MoveLeafBar()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        
        Vector2 position = leafBar.anchoredPosition;
        position.x += moveInput * leafMoveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, startX - moveRange, startX + moveRange);
        
        leafBar.anchoredPosition = position;
    }
    
    private void CheckSunlightStatus()
    {
        float distance = Mathf.Abs(leafBar.anchoredPosition.x - sunBar.anchoredPosition.x);
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
            // TODO: GameManager의 성장 증가 함수 호출
            // GameManager.Instance.increaseGrowth(1);
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator DecreasePlantLife()
    {
        while (!isInSunlight)
        {
            // TODO: GameManager의 생명력 감소 함수 호출
            // GameManager.Instance.decreaseLife(3);
            yield return new WaitForSeconds(1f);
        }
    }
}
