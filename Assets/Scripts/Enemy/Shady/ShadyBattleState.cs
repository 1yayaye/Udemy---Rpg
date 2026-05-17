using UnityEngine;


public class ShadyBattleState : EnemyState
{

    private Transform player;
    private Enemy_Shady enemy;
    private int moveDir;

    private float defaultSpeed;

    public ShadyBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {

        this.enemy = _enemy;
    }


    public override void Enter()
    {
        base.Enter();

        defaultSpeed = enemy.moveSpeed;

        enemy.moveSpeed = enemy.battleStateMoveSpeed;

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);


    }

    public override void Update()
    {
        base.Update();

        if (enemy.ShouldLoseTarget())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        enemy.FacePlayer();

        RaycastHit2D playerDetected = enemy.IsPlayerDetected();

        if (playerDetected && playerDetected.distance < enemy.attackDistance)
        {
            enemy.stats.KillEntity(); // this enteres dead state which triggers explosion + drop items and souls
            return;
        }

        moveDir = enemy.GetPlayerDirection();

        if (!enemy.CanMoveSafelyInDirection(moveDir))
        {
            enemy.SetZeroVelocity();
            return;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = defaultSpeed;
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
}
