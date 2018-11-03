using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour {

	public enum BonusType
	{
		SpeedUp,
		GravityDown
	}
	
	public interface IHaveBonus
	{
		void AddBonus(BonusType bonusType);
	}

}
