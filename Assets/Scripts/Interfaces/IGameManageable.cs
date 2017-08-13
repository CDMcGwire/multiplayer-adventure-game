using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManageable {
	void Reset();
	void Go();
	void Halt();
}
