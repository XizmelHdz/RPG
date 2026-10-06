using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost }

public class BattleManager : MonoBehaviour
{
    [Header("Unidades")]
    public Unit playerUnit;
    public Unit enemyUnit;

    [Header("Interfaz de Usuario (UI)")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public Button attackButton;
    public Button skillButton;
    public Button healButton;

    [Header("Estado de Batalla")]
    public BattleState state;

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // Inicializar HP
        playerUnit.currentHP = playerUnit.maxHP;
        enemyUnit.currentHP = enemyUnit.maxHP;

        UpdateUI();

        yield return new WaitForSeconds(1f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        Debug.Log("Turno del jugador: Elige una acción");
        SetButtonsInteractable(true);
    }

    // Acción de Ataque Básico
    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;

        SetButtonsInteractable(false);
        StartCoroutine(PlayerAttack());
    }

    // Acción de Habilidad Especial
    public void OnSkillButton()
    {
        if (state != BattleState.PlayerTurn) return;

        SetButtonsInteractable(false);
        StartCoroutine(PlayerSkill());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PlayerTurn) return;
        SetButtonsInteractable(false);
        StartCoroutine(PlayerHeal());
    }

    IEnumerator PlayerAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.attackSFX);

        // Ataque básico: Daño aleatorio (ejemplo: entre 8 y 12 de daño)
        int randomDamage = Random.Range(8, 13); // El límite superior (13) es exclusivo, por lo que da 8 a 12.

        bool isDead = enemyUnit.TakeDamage(randomDamage);
        UpdateUI();
        Debug.Log("¡Ataque Normal! Infligiste " + randomDamage + " de daño.");

        yield return new WaitForSeconds(1f);

        CheckEnemyStatus(isDead);
    }

    IEnumerator PlayerSkill()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.attackSFX);

        // Habilidad Especial: Rango de daño significativamente mayor (ejemplo: entre 18 y 25)
        int randomSkillDamage = Random.Range(20, 40);

        bool isDead = enemyUnit.TakeDamage(randomSkillDamage);
        UpdateUI();
        Debug.Log("¡Habilidad Especial! Infligiste " + randomSkillDamage + " de daño crítico.");

        yield return new WaitForSeconds(1f);

        CheckEnemyStatus(isDead);
    }

    IEnumerator PlayerHeal()
    {
        // Reproducir sonido de curación
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.healSFX);

        // Curación con RNG artificial (ejemplo: cura entre 15 y 25 puntos de HP)
        int healAmount = Random.Range(15, 26); // 26 es exclusivo

        playerUnit.currentHP += healAmount;
        if (playerUnit.currentHP > playerUnit.maxHP)
        {
            playerUnit.currentHP = playerUnit.maxHP;
        }

        UpdateUI();
        Debug.Log("¡Te has curado! Recuperaste " + healAmount + " HP.");

        yield return new WaitForSeconds(1f);

        state = BattleState.EnemyTurn;
        StartCoroutine(EnemyTurn());
    }

    void CheckEnemyStatus(bool isDead)
    {
        if (isDead)
        {
            state = BattleState.Won;
            EndBattle();
        }
        else
        {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Turno del enemigo: El enemigo contraataca");

        yield return new WaitForSeconds(1f);

        // Reproducir sonido de ataque
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.attackSFX);

        // Ataque del enemigo con RNG artificial (ejemplo: entre 6 y 14 de daño)
        int enemyDamage = Random.Range(6, 15); // 15 es exclusivo

        bool isDead = playerUnit.TakeDamage(enemyDamage);
        UpdateUI();
        Debug.Log("El enemigo atacó e infligió " + enemyDamage + " de daño.");

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.Lost;
            EndBattle();
        }
        else
        {
            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.Won)
        {
            Debug.Log("¡Ganaste la batalla!");
        }
        else if (state == BattleState.Lost)
        {
            Debug.Log("¡Has sido derrotado!");
        }
    }

    void UpdateUI()
    {
        playerHPText.text = "Jugador HP: " + playerUnit.currentHP + "/" + playerUnit.maxHP;
        enemyHPText.text = "Enemigo HP: " + enemyUnit.currentHP + "/" + enemyUnit.maxHP;
    }

    void SetButtonsInteractable(bool interactable)
    {
        if (attackButton != null) attackButton.interactable = interactable;
        if (skillButton != null) skillButton.interactable = interactable;
    }
}