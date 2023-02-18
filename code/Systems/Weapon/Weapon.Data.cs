namespace GameTemplate.Weapons;

public partial class Weapon
{
	[Net, Change( nameof( OnWeaponDataChanged ) )] private WeaponData weaponData { get; set; }

	/// <summary>
	/// The weapon data resource. This drives all weapon stats and information.
	/// </summary>
	public WeaponData WeaponData
	{
		get => weaponData;
		set
		{
			weaponData = value;
			SetupData( value );
		}
	}

	protected void OnWeaponDataChanged( WeaponData _, WeaponData data )
	{
		SetupData( data );
	}

	protected void SetupData( WeaponData data )
	{
		Model = data.CachedModel;

		CreateComponents();
	}
}
