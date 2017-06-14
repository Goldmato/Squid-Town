using System.Collections;
using System.Collections.Generic;

using UnityEngine;

///<summary>
/// Base squid enemy behaviour
///</summary>
public class SquidEnemy : SimpleEnemy 
{
	// TODO: Implement squid-only behaviours

	void Start()
	{
        SetRunState(false);
    }
}
