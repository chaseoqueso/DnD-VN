using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum ActionButtonType{
    basicAttack,
    basicGuard,

    actionPanelToggle,
    action1,
    action2,
    action3,

    specialPanelToggle,
    special1,
    special2,
    special3,

    back,

    enumSize
}

public class ActionButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ActionButtonType actionType;
    [SerializeField] private Button button;

    [SerializeField] private TMP_Text actionName;

    public string actionDescription {get; private set;}

    private DialogueBox dialogueBox;

    void Start()
    {
        if(actionType == ActionButtonType.actionPanelToggle){
            actionName.text = "ACTION >>";
            actionDescription = "...";
        }
        else if(actionType == ActionButtonType.specialPanelToggle){
            actionName.text = "SPECIAL >>";
            actionDescription = "...";            
        }
        else if(actionType == ActionButtonType.back){
            actionName.text = "<< BACK";
            actionDescription = "...";            
        }
        dialogueBox = UIManager.instance.combatUI.GetDialogueBox();
    }

    public ActionButtonType ActionType()
    {
        return actionType;
    }

    public Button Button()
    {
        return button;
    }

    public void OnActionButtonClicked()
    {
        UIManager.instance.combatUI.ActionButtonClicked(actionType);
    }

    public void SetActionValues(CharacterActionData actionData)
    {
        actionName.text = actionData.SkillName;
        actionDescription = actionData.SkillDescription;
    }

    public void SetActionValues(string _actionName, string _description)
    {
        actionName.text = _actionName;
        actionDescription = _description;
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
            dialogueBox.SetDialogueBoxText(actionDescription, false);
        }

        private void ExitAction()
        {
            dialogueBox.SetDialogueBoxToCurrentDefault();
        }
    #endregion
}
