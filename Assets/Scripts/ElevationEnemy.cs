using System.Collections;
using System.Collections.Generic;
using TMPro;
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
		Vector3 targetDir = (_target.transform.position - transform.position);
		targetDir.z = 0;
		Vector3 moveVec = _speed * Time.fixedDeltaTime * targetDir.normalized;
		transform.position += moveVec;
	}
}
