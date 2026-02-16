using Character.Controls.Damage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Character.Controls.Damage.PlayerDamage;

public class PlayerStats : MonoBehaviour
{
    //TODO: Tener algún stats manager del que leer los valores de stats en el caso de que haya alguún sistema de progresion
    //claseHija que lea del loader???
    // Start is called before the first frame update
    [SerializeField]
    int maxHealth = 3;
    int health;
    public int Health { get { return health; } set { health = value; } }
    [SerializeField]
    float invincibilityDuration = 0.5f;
    public float InvincibilityDuration { get { return invincibilityDuration; } set { invincibilityDuration = value; } }
    public UnityEvent isDead;
    void Start()
    {
        //seria leer de algun loader de stats de personaje, de momento es 3
        health = maxHealth;
        PlayerDamage playerDamage = FindAnyObjectByType<PlayerDamage>();
        playerDamage.onDamage.AddListener(onTakeDamage);
        playerDamage.onHeal.AddListener(onHealDamage);
        playerDamage.invincibilityDuration = InvincibilityDuration;

    }

   public void onTakeDamage(DamageLevel damage)
    {
        Debug.Log("Tomado " + damage + "da�o");
        health -=(int) damage;
        health=Mathf.Clamp(health, 0, maxHealth);
        if (health < 1)
        {
            Debug.Log("Fin de la partida");
            isDead.Invoke();
        }
    }
    public void onHealDamage(int heal)
    {
        health +=heal;
        health=Mathf.Clamp(health, 0, maxHealth);
    }
}
