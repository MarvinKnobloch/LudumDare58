using UnityEngine;
using UnityEditor;

public class MoveComponentTool
{
    private const string menuMoveTop = "CONTEXT/Component/Move To Top";
    private const string menuMoveBottom = "CONTEXT/Component/Move To Bottom";

    [MenuItem(menuMoveTop, priority = 501)]
    public static void MoveComponentToTop(MenuCommand menuCommand)
    {
        while (UnityEditorInternal.ComponentUtility.MoveComponentUp((Component)menuCommand.context));
    }

    [MenuItem(menuMoveTop, validate = true)]
    public static bool MoveComponentToTopValidate(MenuCommand menuCommand)
    {
        Component[] components = ((Component)menuCommand.context).gameObject.GetComponents<Component>();

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == ((Component)menuCommand.context))
            {
                if(i == 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    [MenuItem(menuMoveBottom, priority = 502)]
    public static void MoveComponentToBottom(MenuCommand menuCommand)
    {
        while (UnityEditorInternal.ComponentUtility.MoveComponentDown((Component)menuCommand.context)) ;
    }
    [MenuItem(menuMoveBottom, validate = true)]
    public static bool MoveComponentToBottomValidate(MenuCommand menuCommand)
    {
        Component[] components = ((Component)menuCommand.context).gameObject.GetComponents<Component>();

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == ((Component)menuCommand.context))
            {
                if (i == components.Length - 1)
                {
                    return false;
                }
            }
        }
        return true;
    }
}