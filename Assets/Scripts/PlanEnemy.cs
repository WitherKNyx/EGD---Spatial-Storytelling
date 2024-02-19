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
		throw new System.NotImplementedException();
	}
}
