using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int damage;
    public int maxHP;
    public int currentHP;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;

        // Iniciar el efecto de parpadeo si hay un SpriteRenderer
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        return currentHP <= 0;
    }

    IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;

        // Cambiar a color rojo
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);

        // Volver al color original
        spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.15f);

        // Segundo parpadeo
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = originalColor;
    }
}