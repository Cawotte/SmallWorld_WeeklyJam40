
namespace Cawotte.Toolbox
{
   
    using UnityEngine;

    /// <summary>
    /// Turns a gameobject into a ruler. Attach it to a GameObject, give it a target's transform, 
    /// and it will draw a gizmos line and show the distance between them. 
    /// <para/>
    /// It's useless in the game, it's purely a dev tool. 
    /// </summary>
    public class RulerGameObject : MonoBehaviour
    {
        [SerializeField]
        Transform target;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.position);
                UnityEditor.Handles.Label((transform.position + target.position) / 2,
                    Vector3.Distance(transform.position, target.position).ToString()
                );
            }
        }
#endif
    } 

}
