namespace GameTemplate.Weapons;

/// <summary>
/// Describes the holdtype of a weapon, which tells our animgraph which animations to use.
/// </summary>
public enum WeaponHoldType
{
	None,
	Pistol,
	Rifle,
	Shotgun,
	Item,
	Fists,
	Swing
}

/// <summary>
/// Describes the handedness of a weapon, which hand (or both) we hold the weapon in.
/// </summary>
public enum WeaponHandedness
{
	Both,
	Right,
	Left
}
