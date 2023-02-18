using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusStarHandler : MonoBehaviour
{
    public int numStarsCollected = 0;

    [SerializeField] Sprite bonusSpriteComplete;
    [SerializeField] Sprite bonusSpriteIncomplete;
    [SerializeField] Image[] bonusImages;

    void Start()
    {
        // Set all bonus images to the incomplete sprite at the beginning of the game
        foreach (Image bonusImage in bonusImages)
        {
            bonusImage.sprite = bonusSpriteIncomplete;
        }
    }

    void Update()
    {
        // Update bonus images based on the number of stars collected
        for (int i = 0; i < bonusImages.Length; i++)
        {
            if (i < numStarsCollected)
            {
                bonusImages[i].sprite = bonusSpriteComplete;
            }
            else
            {
                bonusImages[i].sprite = bonusSpriteIncomplete;
            }
        }
    }

    // This method can be called when the player collects a star
    public void CollectStar()
    {
        numStarsCollected++;
    }
}
