
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {

            if (AudioManager.getInstance() != null)
                AudioManager.getInstance().Find("keypickup").source.Play();

            player.keyCount++; 
            player.updateKeyText();

            Destroy(gameObject);
        }
    }
}
