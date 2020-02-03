
using UnityEngine;

public class Wood : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            if (AudioManager.getInstance() != null)
                AudioManager.getInstance().Find("woodpickup").source.Play();

            player.WoodCount++; 

            Destroy(gameObject);
        }
    }
}
