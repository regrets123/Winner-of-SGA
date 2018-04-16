using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FamineBossAI : BaseEnemyScript {

    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Text bossNameText;

    protected override void Start()
    {
        base.Start();
        bossNameText.text = this.name;
    }

    public override void TakeDamage(int incomingDamage, DamageType dmgType)
    {
        base.TakeDamage(incomingDamage, dmgType);
        healthSlider.value = health;
    }

    void Enrage()
    {
        
    }
}
