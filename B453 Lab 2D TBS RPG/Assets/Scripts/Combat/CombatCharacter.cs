using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCharacter : MonoBehaviour
{
    public bool isPlayer;                       //Handles if the character is player controlled or not
    public List<CombatActions> combatActions;   //List of combat actions the character can perform (Damaging, Healing, etc...)

    public int curHp;
    public int maxHp;

    [SerializeField] private CombatCharacter opponent;  //Target of the attack
    private Vector3 startPos;                           //Starting position of the character for the attack animation

    //Sets the starting position of the character
    private void Start()
    {
        startPos = transform.position;
    }

    /// <summary>
    /// Takes damage and checks if the character is dead
    /// </summary>
    /// <param name="damageToTake"> desired amount of damage dealt to the character</param>
    public void TakeDamage(int damageToTake)
    {

        Debug.Log("Damage to take: " + damageToTake);
        curHp -= damageToTake;

        CombatEvents.instance.e_onHealthChange.Invoke();

        if (curHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Invokes the onCharacterDie event and then destroy the character
    /// </summary>
    private void Die()
    {
        CombatEvents.instance.e_onCharacterDie.Invoke(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Heals the character and checks if the heal amount is greater than the max hp
    /// </summary>
    /// <param name="healAmount">desired healing amount</param>
    public void Heal(int healAmount)
    {
        curHp += healAmount;

        CombatEvents.instance.e_onHealthChange.Invoke();

        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
    }

    /// <summary>
    /// Casts the combat action and checks if the action is an attack, a projectile or a heal
    /// </summary>
    /// <param name="combatAction">CombatActions SO</param>
    public void CastCombatAction(CombatActions combatAction)
    {
        if (combatAction.Damage > 0)
        {
            StartCoroutine(AttackOpponent(combatAction));
        }
        else if (combatAction.ProjectilePrefab != null)
        {
            GameObject proj = Instantiate(combatAction.ProjectilePrefab, transform.position, Quaternion.identity);
            //proj.GetComponent<Projectile>().Initialize(opponent, TurnManager.instance.EndTurn);
        }
        else if (combatAction.HealAmount > 0)
        {
            Heal(combatAction.HealAmount);
            TurnManager.instance.EndTurn();
        }
        else
        {
            TurnManager.instance.EndTurn();
        }
    }

    /// <summary>
    /// Moves the character towards the opponent to attack, 
    /// and move them back to the starting position once they finish the attack
    /// </summary>
    /// <param name="combatAction"> CombatActions SO that stores the amount of damage</param>
    /// <returns></returns>
    IEnumerator AttackOpponent(CombatActions combatAction)
    {
        while (transform.position != opponent.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, opponent.transform.position, 50 * Time.deltaTime);
            yield return null;
        }

        opponent.TakeDamage(combatAction.Damage);

        while (transform.position != startPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, 20 * Time.deltaTime);
            yield return null;
        }

        TurnManager.instance.EndTurn();
    }

}
