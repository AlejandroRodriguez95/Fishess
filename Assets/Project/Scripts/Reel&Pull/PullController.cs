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
    private float lerpSpeed = 50;


    private float sweetSpotXScale;
    private float bitterSpotXScale;
    private float indicatorXScale;

    private bool leftToRight = true;

    private Vector3 indicatorStartingPos;
    private Vector3 indicatorEndPos;

    public Vector2 sweetSpotRange;
    public Vector2 bitterSpotRange;



    private void Awake()
    {
        indicatorRectTransform = indicator.GetComponent<RectTransform>();
        sweetSpotRectTransform = sweetSpot.GetComponent<RectTransform>();
        backgroundRectTransform = background.GetComponent<RectTransform>();
        bitterSpotRectTransform = bitterSpot.GetComponent<RectTransform>();

        indicatorStartingPos = new Vector3(-maximumValueXAxis, 0, 0);
        indicatorEndPos = new Vector3(maximumValueXAxis, 0, 0);

        indicatorXScale = indicatorRectTransform.localScale.x;
        sweetSpotXScale = sweetSpotRectTransform.localScale.x;
        bitterSpotXScale = bitterSpotRectTransform.localScale.x;
    }

    private void Start()
    {
        UpdateSweetAndBitterSpotPosition();
    }

    private void Update()
    {
        if (leftToRight)
            indicatorRectTransform.localPosition = new Vector3(indicatorRectTransform.localPosition.x + lerpSpeed * Time.deltaTime , 0, 0);
        else
            indicatorRectTransform.localPosition = new Vector3(indicatorRectTransform.localPosition.x - lerpSpeed * Time.deltaTime, 0, 0);

        if(indicatorRectTransform.localPosition.x >= indicatorEndPos.x)
            leftToRight = false;

        if (indicatorRectTransform.localPosition.x <= indicatorStartingPos.x)
            leftToRight = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckIfIndicatorIsInContactWithSweetSpot();
        }

    }

    private void UpdateSweetAndBitterSpotPosition()
    {
        sweetSpotRectTransform.localPosition = new Vector3(Random.Range(-sweetSpotMaxPosition, sweetSpotMaxPosition), 0, 0);
        bitterSpotRectTransform.localPosition = sweetSpotRectTransform.localPosition;
        sweetSpotRange = new Vector2(
            (sweetSpotRectTransform.localPosition.x - (sweetSpotXScale * backgroundRectTransform.rect.width)/2), 
            (sweetSpotRectTransform.localPosition.x + (sweetSpotXScale * backgroundRectTransform.rect.width)/2)
            );

        bitterSpotRange = new Vector2(
            (bitterSpotRectTransform.localPosition.x - (bitterSpotXScale * backgroundRectTransform.rect.width)/2),
            (bitterSpotRectTransform.localPosition.x + (bitterSpotXScale * backgroundRectTransform.rect.width)/2)
            );
        
    }

    private void CheckIfIndicatorIsInContactWithSweetSpot()
    {
        if (indicatorRectTransform.localPosition.x >= sweetSpotRange.x && indicatorRectTransform.localPosition.x <= sweetSpotRange.y) // if sweet spot is hit
        {
            UpdateSweetAndBitterSpotPosition();
        }

        else if(indicatorRectTransform.localPosition.x >= bitterSpotRange.x && indicatorRectTransform.localPosition.x <= bitterSpotRange.y)
        {
            Debug.Log("Bitter!");
        }
        else
        {
            Debug.Log("Miss");
        }
    }

}
