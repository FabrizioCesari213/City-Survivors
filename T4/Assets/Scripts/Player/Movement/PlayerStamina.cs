using UnityEngine;

public class PlayerStamina : MonoBehaviour
{

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float sprintCostPerSecond = 20f;

    [Header("Regeneration")]
    [SerializeField] private float regenerationRate = 15f;
    [SerializeField] private float regenerationDelay = 1.5f;
    [SerializeField] private float exhaustionRecovery = 20f;


    private float currentStamina;
    private float regenerationTimer;
    private bool isExhausted;

    
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool CanSprint => !isExhausted && currentStamina > 0f;


    private void Awake()
    {
        currentStamina = maxStamina;
    }

    public void ConsumeSprint(float deltaTime)
    {
        if(!CanSprint) return;

        currentStamina -= sprintCostPerSecond * deltaTime;
        currentStamina = Mathf.Max(0f, currentStamina);
        regenerationTimer = regenerationDelay;

        if (currentStamina <= 0f)
        {
            isExhausted = true;
        }
        
    }
    private void OnGUI()
{
    GUI.Label(
        new Rect(20, 20, 300, 30),
        $"Stamina: {currentStamina:F1}/{maxStamina}"
    );
}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (regenerationTimer > 0f)
        {
            regenerationTimer -= Time.deltaTime;
            return;
        }
        if (currentStamina < maxStamina)
        {
            currentStamina += regenerationRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
        if (isExhausted && currentStamina >= exhaustionRecovery)
        {
            isExhausted = false;
        }
    }
}
