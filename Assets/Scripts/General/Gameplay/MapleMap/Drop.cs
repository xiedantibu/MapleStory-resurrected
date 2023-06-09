﻿using System;




namespace ms
{
	public abstract class Drop : MapObject
	{
		const float SPINSTEP = 0.2f;

		public override sbyte update (Physics physics)
		{
			physics.move_object (phobj);

			if (state == Drop.State.DROPPED)
			{
				if (phobj.onground)
				{
					phobj.hspeed = 0.0;
					phobj.type = PhysicsObject.Type.FIXATED;
					state = Drop.State.FLOATING;
					angle.set (0.0f);
					set_position (dest.x (), (short)(dest.y () - 4));
				}
				else
				{
					angle += SPINSTEP;
				}
			}

			if (state == Drop.State.FLOATING)
			{
				phobj.y = basey + 5.0f + (Math.Cos (moved) - 1.0f) * 2.5f;
				moved = (moved < 360.0f) ? moved + 0.025f : 0.0f;
			}

			if (state == Drop.State.PICKEDUP)
			{
				const ushort PICKUPTIME = 48;
				float OPCSTEP = 1.0f / PICKUPTIME;

				if (looter != null)
				{
					double hdelta = looter.x - phobj.x;
					phobj.hspeed = looter.hspeed / 2.0 + (hdelta - 16.0) / PICKUPTIME;
				}

				opacity -= OPCSTEP;

				if (opacity.last () <= OPCSTEP)
				{
					opacity.set (1.0f);

					base.deactivate ();
					return -1;
				}
			}

			return phobj.fhlayer;
		}

		public void expire (sbyte type, PhysicsObject lt)
		{
			switch (type)
			{
				case 0:
					state = Drop.State.PICKEDUP;
					break;
				case 1:
					deactivate ();
					break;
				case 2:
					angle.set (0.0f);
					state = Drop.State.PICKEDUP;
					looter = lt;
					phobj.vspeed = -4.5f;
					phobj.type = PhysicsObject.Type.NORMAL;
					break;
			}
		}

		public Rectangle_short bounds ()
		{
			var lt = get_position ();
			var rb = lt + new Point_short (32, 32);

			return new Rectangle_short (lt, rb);
		}

		protected Drop (int id, int own, Point_short start, Point_short dst, sbyte type, sbyte mode, bool pldrp) : base (id, start)
		{
			owner = own;
			set_position (start.x (), (short)(start.y () - 4));
			dest = new Point_short (dst);
			pickuptype = type;
			playerdrop = pldrp;

			angle.set (0.0f);
			opacity.set (1.0f);
			moved = 0.0f;
			looter = null;

			switch (mode)
			{
				case 0:
				case 1:
					state = Drop.State.DROPPED;
					basey = (double)(dest.y () - 4);
					phobj.vspeed = -5.0f;
					phobj.hspeed = (double)(dest.x () - start.x ()) / 48;
					break;
				case 2:
					state = Drop.State.FLOATING;
					basey = phobj.crnt_y ();
					phobj.type = PhysicsObject.Type.FIXATED;
					break;
				case 3:
					state = Drop.State.PICKEDUP;
					phobj.vspeed = -5.0f;
					break;
			}
		}

		protected Linear_float opacity = new Linear_float ();
		protected Linear_float angle = new Linear_float ();

		
		
		private enum State
		{
			DROPPED,
			FLOATING,
			PICKEDUP
		}

		private int owner;
		private sbyte pickuptype;
		private bool playerdrop;

		private PhysicsObject looter;
		private State state;

		private Point_short dest = new Point_short ();
		private double basey;
		private double moved;
	}
}