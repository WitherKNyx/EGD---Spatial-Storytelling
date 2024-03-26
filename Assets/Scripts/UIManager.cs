using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
	public HealthUI HealthUI;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		} else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		HealthUI = GetComponentInChildren<HealthUI>();
	}
}
