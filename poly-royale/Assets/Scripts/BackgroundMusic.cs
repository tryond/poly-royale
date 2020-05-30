using UnityEngine;
 
public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic current;
    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            current = this;
        }
        DontDestroyOnLoad(transform.gameObject);
    }
}
