using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIPanel : MonoBehaviour
{
    [HideInInspector] public int enemyIndex;

    [SerializeField] private Image enemyPortrait;

    public void SetEnemyPortrait(Sprite portrait)
    {
        enemyPortrait.sprite = portrait;
    }

    public void SetEnemyIndex(int i)
    {
        enemyIndex = i;
    }

    public void OnEnemySelected()
    {
        UIManager.instance.combatUI.EnemyTargeted(enemyIndex);
    }
}
