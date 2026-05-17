using UnityEngine;


public class ShadyGroundedState : EnemyState
{

    protected Transform player;
    protected Enemy_Shady enemy;
    public ShadyGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerInAgroRange())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
