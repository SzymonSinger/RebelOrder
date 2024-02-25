using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem instance;

    public int maxHealth = 100;
    private int currentHealth;

    public Slider slider;

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
        slider.maxValue = maxHealth;
        SetHealthBar(currentHealth);
    }

    // testowy void, do usuniêcia
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Heal(25);
        }
    }

    public void SetHealthBar(int health)
    {
        StartCoroutine(UpdateHealthSmoothly(health));
    }

    IEnumerator UpdateHealthSmoothly(int targetHealth)
    {
        float elapsedTime = 0f;
        float updateDuration = 0.25f;
        int startHealth = (int)slider.value;

        while (elapsedTime < updateDuration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startHealth, targetHealth, elapsedTime / updateDuration);
            yield return null;
        }

        slider.value = targetHealth; 
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
}
