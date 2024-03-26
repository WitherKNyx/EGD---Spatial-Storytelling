using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    private Image[] healthIcons;

	private void Start()
	{
		for (int i = 0; i < healthIcons.Length; i++)
		{
			healthIcons[i].enabled = true;
		}
	}

	public void SetHealth(int health)
	{
		for(int i = 0; i < healthIcons.Length; i++)
		{
			healthIcons[i].enabled = i < health;
		}
	}
}
