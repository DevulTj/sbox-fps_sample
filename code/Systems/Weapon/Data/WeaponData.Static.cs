namespace GameTemplate.Weapons;

public partial class WeaponData
{
	/// <summary>
	/// A list of all weapon data.
	/// </summary>
	public static List<WeaponData> All = new();

	/// <summary>
	/// Find weapon data from its full name.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static WeaponData FindName( string name )
	{
		return All.FirstOrDefault( x => x.Name.ToLower() == name.ToLower() );
	}

	/// <summary>
	/// Find weapon data from its resource name.
	/// </summary>
	/// <param name="resource"></param>
	/// <returns></returns>
	public static WeaponData FindResource( string resource )
	{
		return All.FirstOrDefault( x => x.ResourceName.ToLower() == resource.ToLower() );
	}

	/// <summary>
	/// Create an instance of a weapon based on a data asset.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static Weapon CreateInstance( WeaponData data )
	{
		var wpn = new Weapon();
		wpn.WeaponData = data;
		return wpn;
	}

	/// <summary>
	/// Create an instance of a weapon based on an identifier.
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	public static Weapon CreateInstance( string identifier )
	{
		var data = FindResource( identifier );

		// If we can't match a resource, try finding it through name.
		if ( data == null ) data = FindName( identifier );

		// If we're still null, fail.
		if ( data == null ) return null;

		return CreateInstance( data );
	}
}
