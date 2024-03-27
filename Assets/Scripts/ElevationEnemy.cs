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
		Vector3 targetDir = GetMovementDir();
		if(targetDir.x > 0)
		{
			_sprite.flipX = true;
		}
        else if (targetDir.x < 0)
        {
            _sprite.flipX = false;
        }
        targetDir.z = 0;
		Vector3 moveVec = _speed * Time.fixedDeltaTime * targetDir.normalized;
		_rb.velocity = moveVec;
	}
}
