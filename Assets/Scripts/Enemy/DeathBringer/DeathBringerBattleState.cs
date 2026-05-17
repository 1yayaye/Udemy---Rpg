using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerBattleState : EnemyState
{
    private Enemy_DeathBringer enemy;
    private Transform player;
    private int moveDir;

    public DeathBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        //if (player.GetComponent<PlayerStats>().isDead)
            //stateMachine.ChangeState(enemy.moveState);


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
            if (CanAttack())
            {
                stateMachine.ChangeState(enemy.attackState);
                return;
            }
        }

        moveDir = enemy.GetPlayerDirection();

        if (enemy.DistanceToPlayer() < enemy.attackDistance - .1f)
        {
            enemy.SetZeroVelocity();
            return;
        }

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
