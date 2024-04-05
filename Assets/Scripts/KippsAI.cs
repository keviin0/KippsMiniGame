using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KippsAI : MonoBehaviour
{
    public float mDecayRate = 10.0f;
    public float mInterestVal = 100.0f;
    public float mGrowthRate = 5.0f; 
    public float mJumpCooldown = 1.0f;
    private Texture2D LaserCursor;
    private GameObject mCursorObj;

    private bool IsMouseInWindow()
    {
        return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
    }

    void Jump()
    {
        Vector3 mousePosition = Input.mousePosition;
    }
    
    void Awake()
    {
        mCursorObj = GameObject.Find("Cursor");
        LaserCursor = Resources.Load<Texture2D>("laser_cursor");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMouseInWindow())
        {
            mCursorObj.SetActive(true);
            Cursor.SetCursor(LaserCursor, new Vector2(16f, 16f), CursorMode.Auto);
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0; 
            mCursorObj.transform.position = mousePos;
        }
        else
        {
            mCursorObj.SetActive(false);
            Cursor.SetCursor(null, new Vector2(16f, 16f), CursorMode.Auto);
        }
    }
}
