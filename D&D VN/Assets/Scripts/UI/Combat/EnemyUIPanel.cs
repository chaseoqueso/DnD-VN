using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EnemyUIPanel : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int enemyIndex;
    private string enemyDescription = "[enemy description]";

    [SerializeField] private Image enemyPortrait;
    [SerializeField] private TMP_Text enemyHealth;

    [SerializeField] private Slider healthBar;
    private int maxHP;

    private DialogueBox dialogueBox;

    public void SetEnemyPanelValues(int index, Sprite portrait, string description, float _maxHP)
    {
        SetEnemyPortrait(portrait);
        SetEnemyIndex(index);
        SetEnemyDescription(description);

        maxHP = Mathf.CeilToInt(_maxHP);
        healthBar.maxValue = maxHP;
        UpdateHealthUI(maxHP);

        dialogueBox =  UIManager.instance.combatUI.GetDialogueBox();
    }

    public void SetEnemyPortrait(Sprite portrait)
    {
        enemyPortrait.sprite = portrait;
    }

    public void SetEnemyIndex(int i)
    {
        enemyIndex = i;
    }

    public void UpdateHealthUI(float health)
    {
        healthBar.value = health;
        enemyHealth.text = "<b>HP:</b> " + Mathf.CeilToInt(health) + " / " + maxHP;
    }

    public void SetEnemyDescription(string description)
    {
        enemyDescription = description;
    }

    public void OnEnemySelected()
    {
        UIManager.instance.combatUI.EnemyTargeted(enemyIndex);
    }

    #region Hover Text
        public void OnPointerEnter(PointerEventData eventData)
        {
            HoverAction();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExitAction();
        }

        public void OnSelect(BaseEventData eventData)
        {
            HoverAction();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            ExitAction();
        }

        private void HoverAction()
        {
            if(!CombatUI.enemySelectIsActive){
                return;
            }
            dialogueBox.SetDialogueBoxText(enemyDescription, false);
        }

        private void ExitAction()
        {
            if(!CombatUI.enemySelectIsActive){
                return;
            }
            dialogueBox.SetDialogueBoxToCurrentDefault();
        }
    #endregion
}
