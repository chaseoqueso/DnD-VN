using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIPanel : MonoBehaviour
{
    [HideInInspector] public int enemyIndex;

    public void SetEnemyIndex(int i)
    {
        enemyIndex = i;
    }

    public void OnEnemySelected()
    {
        UIManager.instance.combatUI.EnemyTargeted(enemyIndex);
    }
}
