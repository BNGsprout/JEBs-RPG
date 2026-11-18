using UnityEngine;

public class SaveKickstarter : MonoBehaviour
{
    [SerializeField] private int slot = 0;

    void Start()
    {
        var (world, player) = SaveManager.Instance.Load(slot);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
