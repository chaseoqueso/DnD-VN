using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

public class ActionButton : MonoBehaviour
{
    [SerializeField] private ActionButtonType actionType;
    [SerializeField] private Button button;

    [SerializeField] private TMP_Text actionName;

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

    public void SetActionName(string _actionName)
    {
        actionName.text = _actionName;
    }
}
