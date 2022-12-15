using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Aim : WeaponComponent, ISingletonComponent
{
	protected override bool CanStart( Player player )
	{
		if ( !Input.Down( InputButton.SecondaryAttack ) ) return false;

		return true;
	}

	/// <summary>
	/// Data asset information.
	/// </summary>
	public struct ComponentData
	{
		public float AimTime { get; set; }
	}
}
