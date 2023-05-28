using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullController : MonoBehaviour
{
    [SerializeField]
    GameObject indicator;
    RectTransform indicatorRectTransform;

    [SerializeField]
    GameObject sweetSpot;
    RectTransform sweetSpotRectTransform;

    [SerializeField]
    GameObject bitterSpot;
    RectTransform bitterSpotRectTransform;


    [SerializeField]
    GameObject background;
    RectTransform backgroundRectTransform;

    [SerializeField]
    private float maximumValueXAxis; // maximum and minimum position for the indicator, should be lower than the width of the background

    [SerializeField]
    private float sweetSpotMaxPosition;

    [SerializeField]
    private float lerpSpeed = 250;
    public float LerpSpeed
    {
        get { return lerpSpeed; }
        set { lerpSpeed = value; }
    }

    private bool leftToRight = true;

    private Vector3 indicatorStartingPos;
    private Vector3 indicatorEndPos;

    public Vector2 sweetSpotRange;
    public Vector2 bitterSpotRange;

    private Vector2 sweetSpotMinAndMaxSize;

    public Vector2 SweetSpotMinAndMaxSize
    {
        get { return sweetSpotMinAndMaxSize; }
        set { sweetSpotMinAndMaxSize = value; }
    }

    private void Awake()
    {
        indicatorRectTransform = indicator.GetComponent<RectTransform>();
        sweetSpotRectTransform = sweetSpot.GetComponent<RectTransform>();
        backgroundRectTransform = background.GetComponent<RectTransform>();
        bitterSpotRectTransform = bitterSpot.GetComponent<RectTransform>();

        indicatorStartingPos = new Vector3(-maximumValueXAxis, 0, 0);
        indicatorEndPos = new Vector3(maximumValueXAxis, 0, 0);
    }

    private void Start()
    {
        UpdateSweetAndBitterSpotPositionAndSize();
    }

    private void Update()
    {
        if (leftToRight)
            indicatorRectTransform.localPosition = new Vector3(indicatorRectTransform.localPosition.x + lerpSpeed * Time.deltaTime, 0, 0);
        else
            indicatorRectTransform.localPosition = new Vector3(indicatorRectTransform.localPosition.x - lerpSpeed * Time.deltaTime, 0, 0);

        if (indicatorRectTransform.localPosition.x >= indicatorEndPos.x)
            leftToRight = false;

        if (indicatorRectTransform.localPosition.x <= indicatorStartingPos.x)
            leftToRight = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckIfIndicatorIsInContactWithSweetSpot();
        }

    }

    public void UpdateSweetAndBitterSpotPositionAndSize()
    {
        sweetSpotRectTransform.sizeDelta = new Vector2((int)Random.Range(sweetSpotMinAndMaxSize.x, sweetSpotMinAndMaxSize.y), sweetSpotRectTransform.sizeDelta.y);
        sweetSpotRectTransform.localPosition = new Vector3(Random.Range(-sweetSpotMaxPosition, sweetSpotMaxPosition), 0, 0);
        bitterSpotRectTransform.sizeDelta = new Vector2(sweetSpotRectTransform.sizeDelta.x * 2f, sweetSpotRectTransform.sizeDelta.y);

        sweetSpotMaxPosition = backgroundRectTransform.sizeDelta.x / 2 - bitterSpotRectTransform.sizeDelta.x / 2 - 25;

        bitterSpotRectTransform.localPosition = sweetSpotRectTransform.localPosition;


        sweetSpotRange = new Vector2(
            (sweetSpotRectTransform.localPosition.x - sweetSpotRectTransform.rect.width / 2),
            (sweetSpotRectTransform.localPosition.x + sweetSpotRectTransform.rect.width / 2)
            );

        bitterSpotRange = new Vector2(
            (bitterSpotRectTransform.localPosition.x - bitterSpotRectTransform.rect.width / 2),
            (bitterSpotRectTransform.localPosition.x + bitterSpotRectTransform.rect.width / 2)
            );

    }

    public PullResult CheckIfIndicatorIsInContactWithSweetSpot()
    {
        if (indicatorRectTransform.localPosition.x >= sweetSpotRange.x && indicatorRectTransform.localPosition.x <= sweetSpotRange.y) // if sweet spot is hit
        {
            UpdateSweetAndBitterSpotPositionAndSize();
            return PullResult.Hit; // sweet spot
        }

        else if (indicatorRectTransform.localPosition.x >= bitterSpotRange.x && indicatorRectTransform.localPosition.x <= bitterSpotRange.y)
            return PullResult.Bitter; // bitter spot
        else
            return PullResult.Miss; // miss
    }

}
