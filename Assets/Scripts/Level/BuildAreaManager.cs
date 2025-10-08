using UnityEngine;

public class BuildAreaManager : MonoBehaviour
{
    public static BuildAreaManager Instance { get; private set; }

    public Collider2D AreaCollider { get; private set; }



    private void Awake()
    {

        Instance = this;
        AreaCollider = GetComponent<Collider2D>();

    }


}
