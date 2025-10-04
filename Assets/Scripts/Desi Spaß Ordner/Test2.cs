using UnityEngine;
using UnityEngine.EventSystems;

public class NewMonoBehaviourScript : MonoBehaviour, IPointerEnterHandler

{
    [SerializeField] private Test ScriptObj;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Test");
        Debug.Log(ScriptObj.Dgm);
        Debug.Log(ScriptObj.Name);

        ScriptObj.Dgm = 100;
        //
    }

    

}




