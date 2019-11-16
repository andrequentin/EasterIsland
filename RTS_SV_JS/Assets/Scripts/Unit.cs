using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class Unit : ScriptableObject
{
   public UnitTypes unitType;
   public int health;
   public float gatherSpeed;
   public float attackSpeed;
   public int gatherTick;
   public int capacity;
   public int damage;
   public float speed;
    public int buildPower;
}
