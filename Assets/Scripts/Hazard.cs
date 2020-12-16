using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public GameObject playerDeathPrefab;
    public AudioClip deathClip;
    public Sprite hitSprite;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // 1. First you check to ensure the colliding GameObject has the "Player" tag.
        if (coll.transform.tag == "Player")
        {
            // 2. Next, determine if an AudioClip has been assigned to the script and that a valid
            // Audio Source component exists. If so, play the deathClip audio effect.
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && deathClip != null)
            {
                audioSource.PlayOneShot(deathClip);
            }
            // 3. Instantiate the playerDeathPrefab at the collision point and swap the sprite of the
            // saw blade with the hitSprite version.
            Instantiate(playerDeathPrefab, coll.contacts[0].point,
            Quaternion.identity);
            spriteRenderer.sprite = hitSprite;
            // 4. Lastly, destroy the colliding object (the player).
            Destroy(coll.gameObject);
        }
    }
}
