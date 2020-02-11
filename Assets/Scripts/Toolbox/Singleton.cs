
namespace Cawotte.Toolbox
{
    using UnityEngine;

    /// <summary>
    /// Singleton class. 
    /// 
    /// Removed the global access because Singletons are BAD.
    /// Used here to make sure there's always one and a single instance of a same MonoBehaviour
    /// in the scene.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region  Fields
        private static Singleton<T> _instance = null;

        [SerializeField]
        protected bool _persistent = true;
        #endregion

        #region  Methods
        private void Awake()
        {
            if (_persistent)
            {
                //If there's no instance yet
                if (_instance == null)
                {
                    //Declare this one as the one and only, and make it sure it survives through scenes.
                    _instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    //Destroy the fake god
                    Destroy(gameObject);
                }
            }
            OnAwake();
        }

        protected virtual void OnAwake() { }
        #endregion
    }


}
