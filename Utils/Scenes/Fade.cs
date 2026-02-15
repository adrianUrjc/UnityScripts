using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour
{
    enum FadeType
    {
        FadeIn,
        FadeOut
    }
    [SerializeField]
    FadeType fadeType;
    [SerializeField, Range(0, 3)]
    public float timeToFade;
    [ReadOnly]
    public float timeElapsed;
    public bool destroyOnFinished;
    private Image fadeImage;
    void Awake()
    {

        fadeImage = GetComponent<Image>();
    }
    void Start()
    {
        if (fadeType == FadeType.FadeIn)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (fadeType == FadeType.FadeIn)
        {
            fadeImage.color = new Color(0, 0, 0, (1 - timeElapsed / timeToFade));
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, timeElapsed / timeToFade);
        }

        if (timeElapsed >= timeToFade)
        {
            if (fadeType == FadeType.FadeIn)
            {
                fadeImage.color = new Color(0, 0, 0, 0);
            }
            else
            {
                fadeImage.color = new Color(0, 0, 0, 1);
            }
            if (destroyOnFinished)
            {
                Destroy(gameObject);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
