using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Aim : WeaponComponent, ISingletonComponent
{
	protected override bool CanActivate( Player player )
	{
		if ( !Input.Down( InputButton.SecondaryAttack ) ) return false;

		return true;
	}

	protected override void OnActivated( Player player )
	{
		base.OnActivated( player );
	}

	/// <summary>
	/// Data asset information.
	/// </summary>
	public struct ComponentData
	{
		public float AimTime { get; set; }
	}
}
