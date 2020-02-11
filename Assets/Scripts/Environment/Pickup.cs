using Cawotte.Toolbox.Audio;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [SerializeField]
    private IntVariable toIncreaseOnPickup = null;

    [Header("Audio")]
    [SerializeField]
    private AudioManager audioManager = null;

    [SerializeField]
    private Sound toPlayOnPickup = null;

    private AudioSourcePlayer audioPlayer = null;

    private void Awake()
    {
        audioPlayer = AudioSourcePlayer.AddAsComponent(gameObject, audioManager);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {

            audioPlayer.PlaySound(toPlayOnPickup,
                onEndPlay: () => Destroy(gameObject)
            );

            toIncreaseOnPickup.Value++;

            //Hide the sprite once picked up
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            //Disable the collider once picked up
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
