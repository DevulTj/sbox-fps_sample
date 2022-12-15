using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Aim : WeaponComponent, ISingletonComponent
{
	protected override bool CanStart( Player player )
	{
		if ( !Input.Down( InputButton.SecondaryAttack ) ) return false;

		return true;
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
