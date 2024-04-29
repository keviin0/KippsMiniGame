using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public GameObject cursorObj;
    private Animator[] hearts = new Animator[3];
    private SpriteRenderer[] heartRenderers = new SpriteRenderer[3];
    private int health = 3;

    void Start()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = transform.GetChild(i).GetComponent<Animator>();
            heartRenderers[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0.0f;
        worldPosition.y += 0.6f;
        transform.position = worldPosition;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Update heart states based on the new health value
        for (int i = 0; i < hearts.Length; i++)
        {
            heartRenderers[i].enabled = true;
            StopCoroutine(FadeHeart(i));
            StartCoroutine(FadeHeart(i));
            if (i < health)
            {
                hearts[i].SetBool("IsFull", true);
            }
            else
            {
                hearts[i].SetBool("IsFull", false);
            }
        }
    }

    IEnumerator FadeHeart(int index)
    {
        yield return new WaitForSeconds(1.5f);
        heartRenderers[index].enabled = false;
    }

    public int GetHealth()
    {
        return health;
    }
}
