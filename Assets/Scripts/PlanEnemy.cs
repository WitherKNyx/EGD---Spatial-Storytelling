using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanEnemy : EnemyAI
{
	protected override void InitEnemy()
	{
		_enemyType = ViewMode.plan;
		_rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	protected override void UpdateMovement()
	{
		Vector3 targetDir = (_target.transform.position - transform.position);
		targetDir.y = 0;
		Vector3 moveVec = _speed * Time.fixedDeltaTime * targetDir.normalized;
		transform.position += moveVec;
	}
}
