using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Aim : WeaponComponent, ISingletonComponent
{
	protected override bool CanStart( Player player )
	{
		if ( !Input.Down( InputButton.SecondaryAttack ) ) return false;

		return true;
	}

	protected override void OnStart( Player player )
	{
		base.OnStart( player );

		Weapon.Tags.Set( "aiming", true );
	}

	protected override void OnStop( Player player )
	{
		base.OnStop( player );

		Weapon.Tags.Set( "aiming", false );
	}

	public override void BuildInput()
	{
		if ( IsActive )
		{
			Input.AnalogLook *= 0.5f;
		}
	}

	/// <summary>
	/// Data asset information.
	/// </summary>
	public struct ComponentData
	{
		public float AimTime { get; set; }
	}
}
