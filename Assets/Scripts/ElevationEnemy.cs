using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationEnemy : EnemyAI
{
	protected override void InitEnemy()
	{
		_enemyType = ViewMode.elevation;
		_rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
	}

	protected override void UpdateMovement()
	{
		throw new System.NotImplementedException();
	}
}
