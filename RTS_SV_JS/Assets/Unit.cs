using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class Unit : ScriptableObject
{
   
   public float gatherSpeed;
   public int gatherTick;
   public int capacity;
   public int damage;
   public float speed;
}
