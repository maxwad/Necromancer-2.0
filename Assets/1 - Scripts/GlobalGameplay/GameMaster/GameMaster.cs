using UnityEngine;
using Zenject;

public class GameMaster : MonoBehaviour
{
    private AISystem aiSystem;
    private InputSystem inputSystem;

    //for Testing
    [SerializeField] private bool isAIEnable = false;

    [Inject]
    public void Construct(
        AISystem aiSystem,
        InputSystem inputSystem
        )
    {
        this.inputSystem = inputSystem;
        this.aiSystem = aiSystem;

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
