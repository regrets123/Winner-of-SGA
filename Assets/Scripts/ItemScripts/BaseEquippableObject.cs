using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseEquippableObject : MonoBehaviour {

    [SerializeField]
    protected Sprite inventoryIcon;

    [SerializeField]
    protected string objectName;

    protected PlayerControls player;

    public Sprite InventoryIcon
    {
        get { return this.inventoryIcon; }
    }

    public virtual void Equip()
    {

    }

    protected void Start()
    {
        this.player = FindObjectOfType<PlayerControls>();
    }

}
