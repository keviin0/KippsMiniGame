using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const float DECAY_RATE = 10.0f;
    public const float INITIAL_INTEREST = 100.0f;
    public const float GROWTH_RATE = 5.0f;
    public const float UPPER_POUNCE_COOLDOWN = 0.75f;
    public const float LOWER_POUNCE_COOLDOWN = 0.3f;
    public const float POUNCE_SPEED = 30.0f;
    public const float BRAKING_FACTOR = 0.95f;
    public const float LOST_BRAKING_FACTOR = 0.85f;
}

public class KippsAI : MonoBehaviour
{
    private BoxCollider2D mBoxCollider;
    public float mInterest = 100.0f;
    public float mPounceCooldown = Constants.UPPER_POUNCE_COOLDOWN;
    private Texture2D LaserCursor;
    private GameObject mCursorObj;
    private Vector3 mVelocity = Vector3.zero;
    private bool mLost = false;

    public enum State
    {
        IDLE,
        INTERESTED,
        POUNCING
    }

    private State mCurrState = State.IDLE;

    private bool IsMouseInWindow()
    {
        return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
    }

    void SetSprite()
    {
        if (Vector3.Dot(transform.right, mCursorObj.transform.position - transform.position) > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void Jump()
    {
        Vector3 mousePosition = Input.mousePosition;
    }
    
    void Awake()
    {
        mCursorObj = GameObject.Find("Cursor");
        LaserCursor = Resources.Load<Texture2D>("laser_cursor");
        mBoxCollider = GetComponent<BoxCollider2D>();
    }

    bool CheckForOverlap()
    {
        Vector2 boxSize = mBoxCollider.size;
        boxSize.x *= transform.localScale.x;
        boxSize.y *= transform.localScale.y;
        
        BoxCollider2D mouseCollider = mCursorObj.GetComponent<BoxCollider2D>();
        if (mouseCollider != null)
        {
            Vector2 mouseSize = mouseCollider.size;
            mouseSize.x *= mCursorObj.transform.localScale.x;
            mouseSize.y *= mCursorObj.transform.localScale.y;

            Collider2D overlapResult = Physics2D.OverlapBox(transform.position, boxSize, 0, LayerMask.GetMask("Mouse"));
            if (overlapResult != null)
            {
                return true;
            }
        }
        return false;
    }

    void Pounce()
    {
        mVelocity = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = worldPosition - transform.position;
        direction.z = 0;
        direction.Normalize();
        mVelocity += direction * Constants.POUNCE_SPEED;
        mPounceCooldown = Random.Range(Constants.LOWER_POUNCE_COOLDOWN, Constants.UPPER_POUNCE_COOLDOWN);
    }

    // Update is called once per frame
    void Update()
    {
        mPounceCooldown -= Time.deltaTime;
        transform.position += mVelocity * Time.deltaTime;
        if (mLost)
        {
            mVelocity *= Constants.LOST_BRAKING_FACTOR;
            return;
        }
        else
        {
            mVelocity *= Constants.BRAKING_FACTOR;
        }
        

        SetSprite();
        
        if (CheckForOverlap())
        {
            Debug.Log("You Lose!");
            mLost = true;
        }

        if (IsMouseInWindow())
        {
            // Enable cursor
            mCursorObj.SetActive(true);
            Cursor.SetCursor(LaserCursor, new Vector2(16f, 16f), CursorMode.Auto);

            // Move cursor to mouse position
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = 0; 
            mCursorObj.transform.position = mouseWorldPos;

            if (mPounceCooldown < 0.0f)
            {
                Pounce();
            }
        }
        else
        {
            mCursorObj.SetActive(false);
            Cursor.SetCursor(null, new Vector2(16f, 16f), CursorMode.Auto);
        }
    }
}
