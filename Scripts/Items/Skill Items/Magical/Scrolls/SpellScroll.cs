using System.Collections.Generic;
using Server.Spells;
using Server.ContextMenus;

namespace Server.Items
{
	public class SpellScroll : Item, ICommodity
	{
		private int m_SpellID;

		public int SpellID => m_SpellID;

		int ICommodity.DescriptionNumber => LabelNumber;
		bool ICommodity.IsDeedable => (Core.ML);

		public SpellScroll( Serial serial ) : base( serial )
		{
		}

		[Constructible]
		public SpellScroll( int spellID, int itemID ) : this( spellID, itemID, 1 )
		{
		}

		[Constructible]
		public SpellScroll( int spellID, int itemID, int amount ) : base( itemID )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;

			m_SpellID = spellID;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_SpellID );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_SpellID = reader.ReadInt();

					break;
				}
			}
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive && Movable )
				list.Add( new AddToSpellbookEntry() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Multis.DesignContext.Check( from ) )
				return; // They are customizing

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			Spell spell = SpellRegistry.NewSpell( m_SpellID, from, this );

			if ( spell != null )
				spell.Cast();
			else
				from.SendLocalizedMessage( 502345 ); // This spell has been temporarily disabled.
		}
	}
}