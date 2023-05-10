using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition
{
	public enum Predicate
	{
		EQUAL,
		LESS,
		GREATER,
		LESSEQUAL
	}

	public abstract bool IsTrue();
}
