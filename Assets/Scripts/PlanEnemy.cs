using UnityEngine;

public class PlanEnemy : EnemyAI
{
	protected override void InitEnemy()
	{
		_enemyType = ViewMode.plan;
		_rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
	}

	protected override void UpdateMovement()
	{
		Vector3 targetDir = GetMovementDir();
		//Debug.Log(targetDir);
		targetDir.y = 0;
		Vector3 moveVec = _speed * Time.fixedDeltaTime * targetDir.normalized;
		_rb.velocity = moveVec;
	}
}
