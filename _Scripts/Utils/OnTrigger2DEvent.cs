using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class OnTrigger2DEvent : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTriggerStay;
    [Header("Tags")]
    [SerializeField]
    private List<string> checkedTags= new();
    [Header("Params")]
    [SerializeField]
    private bool isOneTime = true;
    private bool performedEnter = false;
    private bool performedExit = false;
    [SerializeField]
    private bool DestroyOnEnter = false;
    [SerializeField]
    private bool DestroyOnExit = false;
    [SerializeField]
    private float delayEnter = 0.0f;
    private float orginalDelayEnter = 0.0f;
    [SerializeField]
    private float delayExit = 0.0f;
    private float orginalDelayExit = 0.0f;

    private bool isTriggeredEnter = false;
    private bool isTriggeredExit = false;
    private void Awake()
    {
        var temp = GetComponent<Collider2D>();
        if (temp == null)
        {
            Debug.LogError("No hay trigger para este ontrigger event");
        }
        else
        {
            temp.isTrigger = true;
        }
        orginalDelayEnter = delayEnter;
        orginalDelayExit = delayExit;

    }
    private void Update()
    {
        
        if (isTriggeredEnter && !performedEnter)
        {
            delayEnter -= Time.deltaTime;
            if (delayEnter <= 0)
            {
                onTriggerEnter.Invoke();
                performedEnter = isOneTime;
                
                isTriggeredEnter = false;
                if (DestroyOnEnter)
                {
                    Destroy(gameObject);
                }
                else
                {
                    delayEnter = orginalDelayEnter;
                }
               
            }
        }
        if (isTriggeredExit && !performedExit)
        {
            delayExit -= Time.deltaTime;
            if (delayExit <= 0)
            {
                onTriggerExit.Invoke();
                isTriggeredExit = false;
                performedExit = isOneTime;
                if (DestroyOnExit)
                {
                    Destroy(gameObject);
                }
                else
                {
                    delayExit = orginalDelayExit;
                }
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isOneTime && isTriggeredEnter)
        {
            return;
        }
        if (checkedTags.Contains(other.tag))
        {

            isTriggeredEnter = true;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
       
        if (checkedTags.Contains(other.tag))
        {
           onTriggerStay.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isOneTime && isTriggeredExit)
        {
            return;
        }
        if (checkedTags.Contains(other.tag))
        {
            isTriggeredExit = true;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(GetComponent<Collider>() != null)
        {
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
        }

    }
}

