using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalStatsHandler : MonoBehaviour
{
    public static VitalStatsHandler instance;

    // Health
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBar;

    // Stamina
    public float maxStamina = 100f; 
    private float currentStamina; 
    public float regenerationRate = 5f;
    public bool isUsingStamina = false;
    private bool isRegenerationDelayActive = false;
    private bool canStartRegenerationCoroutine = true;
    public Slider staminaBar;

    public float GetCurrentStamina() => currentStamina;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        SetHealthBar(currentHealth);

        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
    }

    private void Update()
    {
        HandleStaminaRegeneration();
        SetStaminaBar(currentStamina);
    }

    #region Health
    public void SetHealthBar(int health)
    {
        StartCoroutine(UpdateHealthSmoothly(health));
    }

    IEnumerator UpdateHealthSmoothly(int targetHealth)
    {
        float elapsedTime = 0f;
        float updateDuration = 0.25f;
        int startHealth = (int)healthBar.value;

        while (elapsedTime < updateDuration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startHealth, targetHealth, elapsedTime / updateDuration);
            yield return null;
        }

        healthBar.value = targetHealth; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        SetHealthBar(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        SetHealthBar(currentHealth);
    }
    #endregion

    #region Stamina
    private void HandleStaminaRegeneration()
    {
        if (!isUsingStamina && !isRegenerationDelayActive)
        {
            if (currentStamina == 0f && canStartRegenerationCoroutine)
            {
                canStartRegenerationCoroutine = false;
                StartCoroutine(StaminaRegenerationDelay());
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += regenerationRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                canStartRegenerationCoroutine = true;
            }
        }
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public void SetStaminaBar(float stamina)
    {
        StartCoroutine(UpdateStaminaSmoothly(stamina));
    }

    IEnumerator UpdateStaminaSmoothly(float targetStamina)
    {
        float elapsedTime = 0f;
        float updateDuration = 0.1f;
        int startHealth = (int)staminaBar.value;

        while (elapsedTime < updateDuration)
        {
            elapsedTime += Time.deltaTime;
            staminaBar.value = Mathf.Lerp(startHealth, targetStamina, elapsedTime / updateDuration);
            yield return null;
        }

        staminaBar.value = targetStamina;
    }
    IEnumerator StaminaRegenerationDelay()
    {
        isRegenerationDelayActive = true;

        yield return new WaitForSeconds(5f);

        isRegenerationDelayActive = false;
    }

    #endregion
}
