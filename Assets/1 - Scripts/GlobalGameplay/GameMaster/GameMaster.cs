using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private AISystem aiSystem;
    private InputSystem inputSystem;

    //for Testing
    [SerializeField] private bool isAIEnable = false;

    private void Start()
    {
        aiSystem = GlobalStorage.instance.aiSystem;
        inputSystem = GlobalStorage.instance.inputSystem;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1) == true)
        {
            isAIEnable = !isAIEnable;
        }    
    }

    public void EnemyTurn()
    {
        if(isAIEnable == true)
        {
            inputSystem.ActivateInput(false);
            aiSystem.StartMoves();
        }
    }

    public void EndEnemyMoves()
    {
        inputSystem.ActivateInput(true);
    }

}
