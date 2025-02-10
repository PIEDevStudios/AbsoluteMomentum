using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class NewScriptableObjectScript : ScriptableObject
{
    [SerializeField] private ItemPoolSO itemPool;

    Update() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectItem()
        }

    }
}



private void SelectItem()
{
    Random rnd = new Random();

    int randomItemNum = rnd.Next(ItemPoolSO.items.Length);

    Instantiate(ItemPoolSO[randomItemNum])
}