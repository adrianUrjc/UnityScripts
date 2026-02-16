using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Character.Controls.Damage
{
    //TO DO: Quitar lo hardcodeado, poner interfaz IHealth
    public class PlayerDamage : MonoBehaviour, IDamageable
    {

        [HideInInspector]
        public float invincibilityDuration;
        private float inviciblityTimer;
        [SerializeField]
        float mediumPushForce = 10f;
        [SerializeField]
        [Tooltip("Minimun speed at which medium damage is applied")]
        float mediumSpeedThresHold = 2f;
        [SerializeField]
        float heabyPushForce = 30f;
        [SerializeField]
        [Tooltip("Minimun speed at which heavy damage is applied")]
        float heayySpeedThresHold = 5f;
        [SerializeField]
        public
        UnityEvent<DamageLevel> onDamage;
        public
        UnityEvent<int> onHeal;
        [SerializeField]
        // public static readonly CameraShakeSettings LightHit = new CameraShakeSettings(1f, 1.5f, 0.2f);
        // [SerializeField]
        // public static readonly CameraShakeSettings MediumHit = new CameraShakeSettings(2.5f, 2f, 0.4f);
        // [SerializeField]
        // public static readonly CameraShakeSettings HeavyHit = new CameraShakeSettings(4f, 3f, 0.7f);


        void Start()
        {
            onDamage.AddListener(OnDamage);
        }
        private void Update()
        {
            if (!(inviciblityTimer < invincibilityDuration)) return;

            inviciblityTimer += Time.deltaTime;

        }

        [ContextMenu("TakeDamage")]
        void OnDamage(DamageLevel i)
        {
            switch (i)
            {
                case DamageLevel.Light:
                    StartCoroutine(HitStop(0.05f));
                    // FindAnyObjectByType<CameraShakeController>().Shake(LightHit);
                    break;
                case DamageLevel.Medium:
                    StartCoroutine(HitStop(0.1f));
                    //FindAnyObjectByType<CameraShakeController>().Shake(MediumHit);
                    break;
                case DamageLevel.Heavy:
                    StartCoroutine(HitStop(0.2f));
                    // FindAnyObjectByType<CameraShakeController>().Shake(HeavyHit);
                    break;
            }

        }
        IEnumerator HitStop(float duration)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }

        private void OnCollisionEnter(Collision collision)
        {

            DetectDamageHeal(collision);
        }
        private void OnCollisionStay(Collision collision)
        {

            DetectDamageHeal(collision);
        }

        void DetectDamageHeal(Collision collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                if (inviciblityTimer >= invincibilityDuration)
                {

                    onDamage.Invoke(DetermineDamageLevel(collision) + 1);
                    inviciblityTimer = 0;
                }
            }
            else if (collision.gameObject.tag.Equals("Health"))
            {
                onHeal.Invoke(1);//este 1 esta hardcodeado buscar interfaz como IHealth y pedir valor a curar, de momento se queda asi
                inviciblityTimer = invincibilityDuration / 2;//dar tiempo de invencibilidad si te curas un poco de tregua conio
            }
        }

        public enum DamageLevel { Light, Medium, Heavy }

        DamageLevel DetermineDamageLevel(Collision enemy)
        {
            Rigidbody rb = enemy.gameObject.GetComponent<Rigidbody>();
            // Si no tiene Rigidbody, asumimos da�o m�nimo
            if (rb == null)
            {
                //HeavyDamageFeedBack(enemy);
                return DamageLevel.Light;
                //return DamageLevel.Heavy;
            }

            float speed = rb.velocity.magnitude;

            if (speed < mediumSpeedThresHold)
            {

                return DamageLevel.Light;
            }
            else if (speed < heayySpeedThresHold)
            {
                MediumDamageFeedBack(enemy);

                return DamageLevel.Medium;
            }
            else
            {
                HeavyDamageFeedBack(enemy);
                return DamageLevel.Heavy;
            }
        }
        void MediumDamageFeedBack(Collision enemy)
        {
            Vector3 pushDirection = enemy.contacts[0].normal;

            // Empujar al objeto da�ado (este objeto)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(pushDirection * mediumPushForce, ForceMode.Impulse);
            }
        }
        void HeavyDamageFeedBack(Collision enemy)
        {
            Vector3 pushDirection = enemy.contacts[0].normal;

            // Empujar al objeto da�ado (este objeto)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(pushDirection * heabyPushForce, ForceMode.Impulse);
            }
        }

        public void OnTakeDamage()
        {
            throw new System.NotImplementedException();
        }
    }
}
