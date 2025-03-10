using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private CombatCharacter[] characters;
    [SerializeField] private float nextTurnDelay = 1.0f;

    private int curCharacterIndex = -1;
    public CombatCharacter currentCharacter;

    public static TurnManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void OnBeginTurn()
    {
        //Increase the index of the current character
        curCharacterIndex++;

        //Reset the index if it goes out of bounds
        if (curCharacterIndex == characters.Length)
        {
            curCharacterIndex = 0;
        }

        //Set the current character
        currentCharacter = characters[curCharacterIndex];
        CombatEvents.instance.e_onBeginTurn.Invoke(currentCharacter);
    }

    //End the turn of the current character after a breif delay
    public void EndTurn()
    {
        CombatEvents.instance.e_onEndTurn.Invoke(currentCharacter);

        Invoke(nameof(OnBeginTurn), nextTurnDelay);
    }

    //Handle the character dying
    private void OnCharacterDie(CombatCharacter character)
    {
        Debug.Log("Character Died");
    }
}