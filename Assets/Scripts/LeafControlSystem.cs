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
    [SerializeField] private SpriteRenderer leafRenderer;  // 실제 잎사귀의 SpriteRenderer
    [SerializeField] private Sprite normalLeafSprite;     // 기본 상태
    [SerializeField] private Sprite sunlightLeafSprite;   // 햇빛 상태
    [SerializeField] private Sprite shadeLeafSprite;      // 그늘 상태
    
    [Header("Movement Settings")]
    [SerializeField] private float moveRange = 150f;
    private float sunMoveSpeed;
    [SerializeField] private float leafMoveSpeed = 200f;
    
    // 상태 관리
    private bool isControlActive = false;
    private bool isMovingRight = true;
    private float startX;
    
    // 타이머
    private float sunlightTimer = 0f;
    private float shadeTimer = 0f;
    private bool isInSunlight = false;
    private bool isTimerActive = false;
    
    // 플레이어 참조
    private GameObject player1;
    private bool isPlayerInRange = false;
    
    private void Start()
    {
        leafControlPanel.SetActive(false);
        startX = sunBar.anchoredPosition.x;
        player1 = GameObject.FindGameObjectWithTag("Player1");
        
        // 초기 상태 설정
        leafRenderer.sprite = normalLeafSprite;
        
        // 15초 동안 왕복하도록 속도 계산
        sunMoveSpeed = (moveRange * 2) / 7.5f;
    }
    
    private void Update()
    {
        CheckPlayerRange();
        
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleControl();
        }
        
        if (isControlActive)
        {
            MoveSunBar();
            MoveLeafBar();
            CheckSunlightStatus();
        }
        
        // 컨트롤 UI가 비활성화될 때 기본 상태로 복귀
        if (!isControlActive && leafRenderer.sprite != normalLeafSprite)
        {
            leafRenderer.sprite = normalLeafSprite;
        }
    }
    
    private void CheckPlayerRange()
    {
        if (player1 != null)
        {
            float distance = Vector2.Distance(transform.position, player1.transform.position);
            isPlayerInRange = distance < 2f;
            
            // 플레이어가 범위를 벗어나면 컨트롤 비활성화
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
        
        // 컨트롤이 활성화될 때는 현재 상태 유지
        if (!isControlActive)
        {
            StopAllCoroutines();
            isTimerActive = false;
            leafRenderer.sprite = normalLeafSprite;
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
            
            // 상태에 따른 스프라이트 변경
            leafRenderer.sprite = isInSunlight ? sunlightLeafSprite : shadeLeafSprite;
            
            // 타이머 리셋
            sunlightTimer = 0f;
            shadeTimer = 0f;
            
            if (isTimerActive)
            {
                StopAllCoroutines();
                isTimerActive = false;
            }
        }
        
        // 햇빛 상태 타이머
        if (isInSunlight)
        {
            sunlightTimer += Time.deltaTime;
            if (sunlightTimer >= 2f && !isTimerActive)
            {
                StartCoroutine(IncreasePlantGrowth());
                isTimerActive = true;
            }
        }
        // 그늘 상태 타이머
        else
        {
            shadeTimer += Time.deltaTime;
            if (shadeTimer >= 4f && !isTimerActive)
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