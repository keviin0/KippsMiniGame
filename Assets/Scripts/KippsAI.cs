using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const float DECAY_RATE = 10.0f;
    public const float INITIAL_INTEREST = 100.0f;
    public const float GROWTH_RATE = 5.0f;
    public const float CLOSE_UPPER_POUNCE_COOLDOWN = 0.15f;
    public const float FAR_UPPER_POUNCE_COOLDOWN = 1.0f;
    public const float LOWER_POUNCE_COOLDOWN = 0.05f;
    public const float UPPER_POUNCE_BRAKING_FACTOR = 0.0025f;
    public const float LOWER_POUNCE_BRAKING_FACTOR = 0.0005f;
    public const float POUNCE_SPEED = 70.0f;
    public const float LOST_BRAKING_FACTOR = 0.001f;
    public const float BRAKING_FACTOR = 0.10f;
    public const float INNER_RADIUS = 4.0f;
    public const float OUTER_RADIUS = 6.5f;
    public const float INVUL_TIME = 1.5f;

}

public class KippsAI : MonoBehaviour
{
    private BoxCollider2D mBoxCollider;
    public float mInterest = 100.0f;
    public float mPounceCooldown = Constants.FAR_UPPER_POUNCE_COOLDOWN;
    private Texture2D LaserCursor;
    private GameObject mCursorObj;
    private Vector3 mVelocity = Vector3.zero;
    private bool mLost = false;
    public HealthBar healthBar;
    private float mBrakingFactor = 0.85f;
    private float mInvulTimer = 0.0f;
    public List<GameObject> mScorePrefabs = new List<GameObject>();
    private List<int> mScoreValues = new List<int>() { 100, 200, 300 };
    public ScoreManager mScoreManager;
    public Collider2D collider1;
    public Collider2D collider2;

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
        mCursorObj.SetActive(true);
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
        Vector3 laserPosition = mCursorObj.transform.position;
        Vector3 direction = laserPosition - transform.position;
        direction.z = 0;
        float distance = direction.magnitude;
        direction.Normalize();

        // Adjust the cooldown based on distance, closer is faster
        float initialCooldown = Mathf.Lerp(Constants.CLOSE_UPPER_POUNCE_COOLDOWN, Constants.FAR_UPPER_POUNCE_COOLDOWN, Mathf.Clamp(distance / 15.0f, 0.0f, 1.0f)); // example scaling
        mPounceCooldown = Random.Range(0.05f, Mathf.Clamp(distance / 15.0f, 0.0f, 1.0f) * 0.15f) + initialCooldown;

        // Adjust velocity based on windup time (mPounceCooldown). The longer the windup, the faster the pounce
        mVelocity = direction * Constants.POUNCE_SPEED * (initialCooldown / Constants.FAR_UPPER_POUNCE_COOLDOWN);

        // Adjust braking factor based on mVelocity. The faster, the less braking
        mBrakingFactor = Mathf.Lerp(Constants.LOWER_POUNCE_BRAKING_FACTOR, Constants.UPPER_POUNCE_BRAKING_FACTOR, Mathf.Clamp(mVelocity.magnitude / Constants.POUNCE_SPEED, 0.0f, 1.0f));
    
        /*
            0 => 100
            1 => 200
            3 => 300
        */
        int scoreIndex = 0;
        if (distance < Constants.INNER_RADIUS)
        {
            scoreIndex = 2;
        }
        else if (distance < Constants.OUTER_RADIUS)
        {
            scoreIndex = 1;
        }
        mScoreManager.AddScore(mScoreValues[scoreIndex]);
        GameObject scorePrefab = Instantiate(mScorePrefabs[scoreIndex], transform.position, Quaternion.identity);
        scorePrefab.SetActive(true);
        Destroy(scorePrefab, 1.0f);
    }

    void Lose()
    {
        Debug.Log("You Lose!");
        mLost = true;
    }

    Vector3 ClampToNearestCollider(Vector3 position)
    {
        // Find the closest points on each collider
        Vector3 closestPoint1 = collider1.ClosestPoint(position);
        Vector3 closestPoint2 = collider2.ClosestPoint(position);

        // Determine which point is closer to the position
        float dist1 = (closestPoint1 - position).sqrMagnitude;
        float dist2 = (closestPoint2 - position).sqrMagnitude;

        return dist1 < dist2 ? closestPoint1 : closestPoint2;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(mVelocity.magnitude);
        mPounceCooldown -= Time.deltaTime;
        mInvulTimer -= Time.deltaTime;
        transform.position += mVelocity * Time.deltaTime;

        if (mLost)
        {
            mVelocity *= Mathf.Pow(Constants.LOST_BRAKING_FACTOR, Time.deltaTime);
            return;
        }
        else
        {
            mVelocity *= Mathf.Pow(mBrakingFactor, Time.deltaTime);
        }
        
        SetSprite();
        
        // If Kipps catches the laser
        if (CheckForOverlap() && mInvulTimer < 0.0f)
        {
            healthBar.TakeDamage(1);
            mInvulTimer = Constants.INVUL_TIME;
        }

        // Mouse checks
        if (IsMouseInWindow())
        {
            // Move cursor to mouse position
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Valid Cursor Boundary"));
            if (hitCollider == null)
            {
                mouseWorldPos = ClampToNearestCollider(mouseWorldPos);         
                Debug.Log("After" + mouseWorldPos);   
            }

            mouseWorldPos.z = 0;  

            mCursorObj.transform.position = mouseWorldPos;
        }
        else
        {
            if (mInvulTimer < 0.0f)
            {
                healthBar.TakeDamage(1);
                mInvulTimer = Constants.INVUL_TIME;
            }
            Cursor.SetCursor(null, new Vector2(16f, 16f), CursorMode.Auto);
        }

        // Kipps pounce
        if (mPounceCooldown < 0.0f)
        {
            Pounce();
        }

        // Check conditions for a loss
        if (healthBar.GetHealth() == 0)
        {
            Lose();
        }
    }
}
