using System;

using UnityEngine;

[CustomGVData("PlayerData")]
[Serializable]
public class CustomPlayerData
{
    [SerializeField]
   Vector3 position;
   [SerializeField]
   Vector3 rotation;
   public float health;
   public float damage;

   public float speed;
}
