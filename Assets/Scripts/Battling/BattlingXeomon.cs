using UnityEngine;
using UnityEngine.UI;

public class BattlingXeomon : MonoBehaviour
{
    [SerializeField] bool isPlayerXeomon;
    [SerializeField] BattleHUD hud;

    public bool IsPlayerXeomon
    {
        get { return isPlayerXeomon; }
    }

    public BattleHUD Hud
    {
        get {return hud; }
    }

    public Xeomon Xeomon { get; set; }

    private Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Xeomon xeomon)
    {
        Xeomon = xeomon;
        if (isPlayerXeomon)
        {
            image.sprite = Xeomon.BaseInformation.BackSprite;
        }
        else{
            image.sprite = Xeomon.BaseInformation.FrontSprite;
        }

        hud.gameObject.SetActive(true);
        hud.SetData(xeomon);

        transform.localScale = new Vector3(1, 1, 1);
        image.color = originalColor;
    }
}
