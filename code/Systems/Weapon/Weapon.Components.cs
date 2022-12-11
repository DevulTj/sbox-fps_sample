using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Weapon
{
	public T GetComponent<T>() where T : WeaponComponent
	{
		return Components.Get<T>( false );
	}

	protected void SimulateComponents( IClient cl )
	{
		var player = Owner as Player;
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.Simulate( cl, player );
		}
	}

	protected void CreateComponents()
	{
		foreach ( var name in WeaponData.Components )
		{
			var component = TypeLibrary.Create<WeaponComponent>( name );
			if ( component == null )
				continue;

			component.Initialize( this );
			Components.Add( component );
			Log.Info( $"Adding component {component} to {this}" );
		}
	}
}
