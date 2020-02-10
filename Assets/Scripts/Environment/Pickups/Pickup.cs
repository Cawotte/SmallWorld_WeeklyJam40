using Cawotte.Toolbox.Audio;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    protected AudioManager audioManager = null;

    [SerializeField]
    protected Sound toPlayOnPickup = null;

    private AudioSourcePlayer audioPlayer = null;

    private void Awake()
    {
        audioPlayer = AudioSourcePlayer.AddAsComponent(gameObject, audioManager);
    }

    protected abstract void onPickup(Player player);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {

            audioPlayer.PlaySound(toPlayOnPickup,
                onEndPlay: () => Destroy(gameObject)
            );

            onPickup(player);

            //Hide the sprite once picked up
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            //Disable the collider once picked up
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
