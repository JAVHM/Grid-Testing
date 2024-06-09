using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject textDamage;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Method to take damage
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        GameObject textInst = Instantiate(textDamage, transform.position + new Vector3(0,1,0), Quaternion.identity);
        textInst.GetComponent<TextMeshPro>().text = amount.ToString();
        Destroy(textInst, 2f);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        Debug.Log("Health: " + currentHealth);
    }

    // Method to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Health: " + currentHealth);
    }

    // Method to check if the player is dead
    private void Die()
    {
        Debug.Log("Death");
        // Add death logic here (e.g., respawn, game over screen, etc.)
    }

    // Method to get the current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
