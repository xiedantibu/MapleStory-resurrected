﻿#define USE_NX

using System;
using System.Collections.Generic;
using MapleLib.WzLib;
using ms.Helper;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;

namespace ms
{
	public enum MobStance : byte
	{
		MOVE = 2,
		STAND = 4,
		JUMP = 6,
		HIT = 8,
		DIE = 10
	}

	public class Mob : MapObject
	{
		public const uint NUM_STANCES = 6;

		static readonly string[] stancenames = { "move", "stand", "jump", "hit1", "die1", "fly" };

		public static string nameof (MobStance stance)
		{
			int index = ((int)stance - 1) / 2;

			return stancenames[index];
		}

		public static byte value_of (MobStance stance, bool flip)
		{
			return (byte)(flip ? stance : stance + 1);
		}

		// Construct a mob by combining data from game files with data sent by the server
		public Mob (int oi, int mid, sbyte mode, sbyte st, ushort fh, bool newspawn, sbyte tm, Point_short position) : base (oi, position)
		{
			string strid = string_format.extend_id (mid, 7);
			var src = ms.wz.wzFile_mob[strid + ".img"];

			var info = src["info"];

			level = info["level"];
			watk = info["PADamage"];
			matk = info["MADamage"];
			wdef = info["PDDamage"];
			mdef = info["MDDamage"];
			accuracy = info["acc"];
			avoid = info["eva"];
			knockback = info["pushed"];
			speed = info["speed"];
			flyspeed = info["flySpeed"]; //might not have flySpeed
			touchdamage = info["bodyAttack"];
			undead = info["undead"];
			noflip = info["noFlip"];
			notattack = info["notAttack"];
			canjump = (src["jump"] as WzImageProperty)?.WzProperties.Count > 0;
			canfly = (src["fly"] as WzImageProperty)?.WzProperties.Count > 0;
			canmove = (src["move"] as WzImageProperty)?.WzProperties.Count > 0 || canfly;

			if (canfly)
			{
				animations[MobStance.STAND] = src["fly"];
				animations[MobStance.MOVE] = src["fly"];
			}
			else
			{
				animations[MobStance.STAND] = src["stand"];
				animations[MobStance.MOVE] = src["move"];
			}

			animations[MobStance.JUMP] = src["jump"];
			animations[MobStance.HIT] = src["hit1"];
			animations[MobStance.DIE] = src["die1"];

			name = ms.wz.wzFile_string["Mob.img"][Convert.ToString (mid)]["name"].ToString ();

			var sndsrc = ms.wz.wzFile_sound["Mob.img"][strid];

			hitsound = sndsrc?["Damage"];
			diesound = sndsrc?["Die"];

			speed += 100;
			speed *= 0.001f;

			flyspeed += 100;
			flyspeed *= 0.0005f;

			if (canfly)
			{
				phobj.type = PhysicsObject.Type.FLYING;
			}

			id = mid;
			team = tm;
			set_position (new Point_short (position));
			set_control (mode);
			phobj.fhid = fh;
			//phobj.set_flag (PhysicsObject.Flag.TURNATEDGES);

			hppercent = 0;
			dying = false;
			dead = false;
			fading = false;
			set_stance ((byte)st);
			flydirection = FlyDirection.STRAIGHT;
			counter = 0;

			namelabel = new Text (Text.Font.A13M, Text.Alignment.CENTER, Color.Name.WHITE, Text.Background.NAMETAG, name);

			if (newspawn)
			{
				fadein = true;
				opacity.set (0.0f);
			}
			else
			{
				fadein = false;
				opacity.set (1.0f);
			}

			if (control && stance == MobStance.STAND)
			{
				next_move ();
			}

			int i = 1;
			while (src[$"attack{i}"] != null)
			{
				var mobAttack = new MobAttack (src[$"attack{i}"], id, oid);
				mobAttacks.Add (mobAttack);
				i++;
			}
		}

		private int lastDraw_Stance = -1;

		private Animation mobAttackAni;
		public void Set_MobAttackAni(Animation ani)
		{
			mobAttackAni = ani;
		}
		// Draw the mob
		public override void draw (double viewx, double viewy, float alpha)
		{
			if (lastDraw_Stance != -1)
			{
				animations[(MobStance)lastDraw_Stance].eraseAllFrame ();
			}

#if BackgroundStatic
			Point_short absp = phobj.get_position ();
#else
			Point_short absp = phobj.get_absolute (viewx, viewy, alpha);
#endif
			Point_short headpos = get_head_position (new Point_short (absp));
			//AppDebug.Log ($"Mob draw absp:{absp}");
			effects.drawbelow (new Point_short (absp), alpha);
			mobAbsolutePos = absp;
			if (!dead)
			{
				float interopc = opacity.get (alpha);

				if (mobAttackAni != null)
				{
					mobAttackAni.draw (new DrawArgument (absp, flip && !noflip, interopc).SetParent (MapGameObject), alpha);
				}
				else
				{
					animations[stance].draw (new DrawArgument (absp, flip && !noflip, interopc).SetParent (MapGameObject), alpha);
				}

				if (showhp)
				{
					namelabel.draw (absp);

					if (!dying && hppercent > 0)
					{
						hpbar.draw (new Point_short (headpos), hppercent);
					}
				}
			}
			else
			{
				animations[MobStance.DIE].eraseAllFrame ();
			}

			effects.drawabove (new Point_short (absp), alpha);

			lastDraw_Stance = (int)stance;


		}

		// Update movement and animations
		public override sbyte update (Physics physics)
		{
			if (!active)
			{
				return phobj.fhlayer;
			}

			bool aniend = false;
			if (mobAttackAni != null)
			{
				//aniend = mobAttackAni.update ();
				 mobAttackAni.update ();
			}
			else
			{
				aniend = animations[stance].update ();
			}

			if (aniend && stance == MobStance.DIE)
			{
				dead = true;
			}

			if (fading)
			{
				opacity -= 0.025f;

				if (opacity.last () < 0.025f)
				{
					opacity.set (0.0f);
					fading = false;
					dead = true;
				}
			}
			else if (fadein)
			{
				opacity += 0.025f;

				if (opacity.last () > 0.975f)
				{
					opacity.set (1.0f);
					fadein = false;
				}
			}

			if (dead)
			{
				deactivate ();

				return -1;
			}

			effects.update ();
			showhp.update ();

			if (!dying)
			{
				if (!canfly)
				{
					if (phobj.is_flag_not_set (PhysicsObject.Flag.TURNATEDGES))
					{
						flip = !flip;
						phobj.set_flag (PhysicsObject.Flag.TURNATEDGES);

						if (stance == MobStance.HIT)
						{
							set_stance (MobStance.STAND);
						}
					}
				}

				switch (stance)
				{
					case MobStance.MOVE:
						if (canfly)
						{
							phobj.hforce = flip ? flyspeed : -flyspeed;

							switch (flydirection)
							{
								case FlyDirection.UPWARDS:
									phobj.vforce = -flyspeed;
									break;
								case FlyDirection.DOWNWARDS:
									phobj.vforce = flyspeed;
									break;
							}
						}
						else
						{
							phobj.hforce = flip ? speed : -speed;
						}

						break;
					case MobStance.HIT:
						if (canmove)
						{
							double KBFORCE = phobj.onground ? 0.2 : 0.1;
							phobj.hforce = flip ? -KBFORCE : KBFORCE;
						}

						break;
					case MobStance.JUMP:
						phobj.vforce = -5.0;
						break;
				}

				physics.move_object (phobj);

				if (control)
				{
					counter++;

					if (BTree != null)
					{
						Agent.Tick ();
					}
					else
					{
						//是否可以切换到下个动作
						bool next;

						switch (stance)
						{
							case MobStance.HIT:
								next = counter > 200;
								break;
							case MobStance.JUMP:
								next = phobj.onground;
								break;
							default:
								next = aniend && counter > 200;
								break;
						}

						if (next)
						{
							//如果能切换，要切换到什么动作
							next_move ();
							update_movement ();
							counter = 0;
						}
					}
				}
			}
			else
			{
				phobj.normalize ();
				physics.get_fht ().update_fh (phobj);
			}

			//AppDebug.Log ($"mob id:{id} name:{name} stance:{stance}");
			return phobj.fhlayer;
		}

		// Change this mob's control mode:
		// 0 - no control, 1 - control, 2 - aggro
		public void set_control (sbyte mode)
		{
			control = mode > 0;
			aggro = mode == 2;
		}

		// Send movement to the mob
		public void send_movement (Point_short start, List<Movement> in_movements)
		{
			if (control)
			{
				return;
			}

			set_position (new Point_short (start));

			movements = in_movements;

			if (movements.Count == 0)
			{
				return;
			}

			Movement lastmove = movements[0];

			byte laststance = lastmove.newstate;
			set_stance (laststance);

			phobj.fhid = lastmove.fh;
		}

		// Kill the mob with the appropriate type:
		// 0 - make inactive 1 - death animation 2 - fade out
		public void kill (sbyte animation)
		{
			switch (animation)
			{
				case 0:
					deactivate ();
					break;
				case 1:
					dying = true;

					apply_death ();
					break;
				case 2:
					fading = true;
					dying = true;
					break;
			}
		}

		// Display the hp percentage above the mob
		// Use the playerlevel to determine color of NameTag
		public void show_hp (sbyte percent, ushort playerlevel)
		{
			if (hppercent == 0)
			{
				short delta = (short)(playerlevel - level);

				if (delta > 9)
				{
					namelabel.change_color (Color.Name.YELLOW);
				}
				else if (delta < -9)
				{
					namelabel.change_color (Color.Name.RED);
				}
			}

			if (percent > 100)
			{
				percent = 100;
			}
			else if (percent < 0)
			{
				percent = 0;
			}

			hppercent = percent;
			showhp.set_for (2000);
		}

		// Show an effect at the mob's position
		public void show_effect (Animation animation, sbyte pos, sbyte z, bool f)
		{
			if (!active)
			{
				return;
			}

			Point_short shift = new Point_short ();

			switch (pos)
			{
				case 0:
					shift = new Point_short (get_head_position (new Point_short ()));
					break;
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
			}

			effects.add (animation, new DrawArgument (new Point_short (shift), f), z);
		}

		List<System.Tuple<int, bool>> result = new List<System.Tuple<int, bool>> ();

		// Calculate the damage to this mob with the specified attack
		public List<System.Tuple<int, bool>> calculate_damage (Attack attack)
		{
			double mindamage = 0;
			double maxdamage = 0;
			float hitchance = 1;
			float critical = 0;
			short leveldelta = (short)(level - attack.playerlevel);

			if (leveldelta < 0)
			{
				leveldelta = 0;
			}

			Attack.DamageType damagetype = attack.damagetype;

			switch (damagetype)
			{
				case Attack.DamageType.DMG_WEAPON:
				case Attack.DamageType.DMG_MAGIC:
					mindamage = calculate_mindamage (leveldelta, attack.mindamage, damagetype == Attack.DamageType.DMG_MAGIC);
					maxdamage = calculate_maxdamage (leveldelta, attack.maxdamage, damagetype == Attack.DamageType.DMG_MAGIC);
					hitchance = calculate_hitchance (leveldelta, attack.accuracy);
					critical = attack.critical;
					break;
				case Attack.DamageType.DMG_FIXED:
					mindamage = attack.fixdamage;
					maxdamage = attack.fixdamage;
					hitchance = 1.0f;
					critical = 0.0f;
					break;
			}

			result.Clear ();
			for (int i = 0; i < attack.hitcount; i++)
			{
				result.Add (next_damage (mindamage, maxdamage, hitchance, critical));
			}
			/*generate(result.GetEnumerator(), result.end(), () =>
			{
					return next_damage(mindamage, maxdamage, hitchance, critical);
			});*/

			update_movement ();

			return result;
		}

		// Apply damage to the mob
		public void apply_damage (int damage, bool toleft, float hforce = 0, float vforce = 0)
		{
			hitsound?.play ();

			if (dying && stance != MobStance.DIE)
			{
				apply_death ();
			}
			else if (control && is_alive () && damage >= knockback || (hforce != 0 || vforce != 0))
			{
				flip = toleft;
				counter = 170;
				set_stance (MobStance.HIT);

				update_movement ();
			}
		}

		//- is upwards 
		public void add_Force (float hforce, float vforce)
		{
			phobj.hforce += hforce;
			phobj.vforce += vforce;
		}
		// Create a touch damage attack to the player
		public MobAttack create_touch_attack ()
		{
			if (!touchdamage)
			{
				return new MobAttack ();
			}

			int minattack = (int)(watk * 0.8f);
			int maxattack = watk;
			int attack = randomizer.next_int (minattack, maxattack);

			return new MobAttack (attack, get_position (), id, oid);
		}

		// Check if this mob collides with the specified rectangle
		public bool is_in_range (Rectangle_short range, bool debug = false)
		{
			if (!active)
			{
				return false;
			}

			Rectangle_short bounds = animations[stance].get_bounds (debug);
			var tempCacheBounds = new Rectangle_short (bounds);
			bounds.shift (get_position ());
			if (debug)
			{
				//AppDebug.Log ($"range:{range}\t get_bounds:{tempCacheBounds}\t get_position():{get_position ()}\t bounds.shift(get_position()):{bounds}\t {range.overlaps (bounds)}");
			}

			return range.overlaps (bounds);
		}

		public Point_short mobAbsolutePos = new Point_short ();
		public Rectangle_short get_Range ()
		{
			return animations[stance].get_bounds (false).shift (mobAbsolutePos);
		}
		// Check if this mob is still alive
		public bool is_alive ()
		{
			return active && !dying;
		}
		public bool is_Controlled ()
		{
			return control;
		}
		// Return the head position
		public Point_short get_head_position ()
		{
			Point_short position = get_position ();

			return get_head_position (new Point_short (position));
		}
		public PhysicsObject get_phobj()
		{
			return phobj;
		}
		public static string get_name (int mobId)
		{
			return ms.wz.wzFile_string["Mob.img"][Convert.ToString (mobId)]["name"].ToString ();
		}

		private enum FlyDirection
		{
			STRAIGHT,
			UPWARDS,
			DOWNWARDS,
			NUM_DIRECTIONS
		}

		public void set_flip(bool flip)
		{
			this.flip = flip;
		}
		// Set the stance by byte value
		private void set_stance (byte stancebyte)
		{
			flip = (stancebyte % 2) == 0;

			if (!flip)
			{
				stancebyte -= 1;
			}

			if (stancebyte < (int)MobStance.MOVE)
			{
				stancebyte = (int)MobStance.MOVE;
			}

			set_stance ((MobStance)stancebyte);
		}

		// Set the stance by enumeration value
		public void set_stance (MobStance newstance)
		{
			if (stance != newstance)
			{
				stance = newstance;

				animations[stance].reset ();
			}
		}

		// Start the death animation
		private void apply_death ()
		{
			set_stance (MobStance.DIE);
			diesound?.play ();
			dying = true;
		}

		// Decide on the next state
		private void next_move ()
		{
			if (canmove)
			{
				switch (stance)
				{
					case MobStance.HIT:
					case MobStance.STAND:
						set_stance (MobStance.MOVE);
						flip = randomizer.next_bool ();
						break;
					case MobStance.MOVE:
					case MobStance.JUMP:
						if (canjump && phobj.onground && randomizer.below (0.25f))
						{
							set_stance (MobStance.JUMP);
						}
						else
						{
							switch (randomizer.next_int (3))
							{
								case 0:
									set_stance (MobStance.STAND);
									break;
								case 1:
									set_stance (MobStance.MOVE);
									flip = false;
									break;
								case 2:
									set_stance (MobStance.MOVE);
									flip = true;
									break;
							}
						}

						break;
				}

				if (stance == MobStance.MOVE && canfly)
				{
					flydirection = randomizer.next_enum (FlyDirection.NUM_DIRECTIONS);
				}
			}
			else
			{
				set_stance (MobStance.STAND);
			}
		}

		// Send the current position and state to the server
		private void update_movement ()
		{
			new MoveMobPacket (oid, 1, 0, 0, 0, 0, 0, 0, get_position (), new Movement (phobj, value_of (stance, flip))).dispatch ();
		}

		// Calculate the hit chance
		private float calculate_hitchance (short leveldelta, int player_accuracy)
		{
			float faccuracy = (float)player_accuracy;
			float hitchance = faccuracy / (((1.84f + 0.07f * leveldelta) * avoid) + 1.0f);

			if (hitchance < 0.01f)
			{
				hitchance = 0.01f;
			}

			return hitchance;
		}

		// Calculate the minimum damage
		private double calculate_mindamage (short leveldelta, double damage, bool magic)
		{
			double mindamage = magic ? damage - (1 + 0.01 * leveldelta) * mdef * 0.6 : damage * (1 - 0.01 * leveldelta) - wdef * 0.6;

			return mindamage < 1.0 ? 1.0 : mindamage;
		}

		// Calculate the maximum damage
		private double calculate_maxdamage (short leveldelta, double damage, bool magic)
		{
			double maxdamage = magic ? damage - (1 + 0.01 * leveldelta) * mdef * 0.5 : damage * (1 - 0.01 * leveldelta) - wdef * 0.5;

			return maxdamage < 1.0 ? 1.0 : maxdamage;
		}

		// Calculate a random damage line based on the specified values
		private System.Tuple<int, bool> next_damage (double mindamage, double maxdamage, float hitchance, float critical)
		{
			bool hit = randomizer.below (hitchance);

			if (!hit)
			{
				return new System.Tuple<int, bool> (0, false);
			}

			const double DAMAGECAP = 999999.0;

			double damage = randomizer.next_real (mindamage, maxdamage);
			bool iscritical = randomizer.below (critical);

			if (iscritical)
			{
				damage *= 1.5;
			}

			if (damage < 1)
			{
				damage = 1;
			}
			else if (damage > DAMAGECAP)
			{
				damage = DAMAGECAP;
			}

			var intdamage = (int)damage;

			return new System.Tuple<int, bool> (intdamage, iscritical);
		}

		// Return the current 'head' position
		private Point_short get_head_position (Point_short position)
		{
			Point_short head = animations[stance].get_head ();

			position.shift_x ((short)((flip && !noflip) ? -head.x () : head.x ()));
			position.shift_y (head.y ());

			return position;
		}

		public override void Dispose ()
		{
			base.Dispose ();
			foreach (var pair in animations)
			{
				pair.Value.Dispose ();
			}

			animations.Clear ();
		}

		private SortedDictionary<MobStance, Animation> animations = new SortedDictionary<MobStance, Animation> ();

		private string name;

		private Sound hitsound;
		private Sound diesound;
		private ushort level;
		private float speed;
		private float flyspeed;
		private ushort watk;
		private ushort matk;
		private ushort wdef;
		private ushort mdef;
		private ushort accuracy;
		private ushort avoid;
		private ushort knockback;
		private bool undead;
		private bool touchdamage;
		private bool noflip;
		private bool notattack;
		private bool canmove;
		private bool canjump;
		private bool canfly;

		private EffectLayer effects = new EffectLayer ();

		private Text namelabel;
		private MobHpBar hpbar = new MobHpBar ();
		private Randomizer randomizer = new Randomizer ();

		private TimedBool showhp = new TimedBool ();

		private List<Movement> movements = new List<Movement> ();
		private ushort counter;

		private int id;
		private sbyte effect;
		private sbyte team;
		private bool dying;
		private bool dead;
		private bool control;
		private bool aggro;
		private MobStance stance;
		private bool flip;
		private FlyDirection flydirection;
		private float walkforce;
		private sbyte hppercent;
		private bool fading;
		private bool fadein;
		private Linear_float opacity = new Linear_float ();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="attackNumber"></param>
		/// <returns></returns>
		public MobAttack get_MobAttack (int attackNumber)
		{
			return mobAttacks.TryGet (attackNumber - 1);
		}

		private List<MobAttack> mobAttacks = new List<MobAttack> ();

		protected BehaviourTree btree;
		public BehaviourTree BTree => btree ??= ResourcesManager.Instance.GetMobBTree (id.ToString ());

		BehaviourTreeOwner agent;
		public BehaviourTreeOwner Agent
		{
			get
			{
				if (agent == null)
				{
					agent = MapGameObject.AddComponent<BehaviourTreeOwner> ();
					agent.blackboard = MapGameObject.AddComponent<Blackboard> ();
					agent.repeat = false;
					PlayMobBTree (BTree, this);
				}

				return agent;
			}
			set => agent = value;
		}


		public bool PlayMobBTree (BehaviourTree BTree, Mob mob)
		{
			bool result = false;
			if (BTree != null)
			{
				var StopBehaviourWhenEnter = BTree.blackboard.GetVariable<bool> ("StopBehaviourWhenEnter")?.value ?? false;
				if (StopBehaviourWhenEnter)
				{
					Agent.StopBehaviour ();
					//AppDebug.LogError ($"StopBehaviour");

				}

				//behaviorTreeOwner.blackboard.SetVariableValue(GetType().Name,this);
				Agent.repeat = false;
				//AppDebug.LogError ($"after1 graph: {behaviorTreeOwner.graph?.GetHashCode ()}");

				//behaviorTreeOwner.graph = BTree;
				//AppDebug.LogError ($"after2 graph: {behaviorTreeOwner.graph?.GetHashCode ()}");

				Agent.StartBehaviour (BTree, Graph.UpdateMode.Manual, null);//todo 会把传入的graph产生一个新实例，所以设置变量要后设置

				Agent.graph.blackboard.SetVariableValue (typeof (Mob).Name, mob);
				//BehaviorTreeOwner.graph.blackboard.SetVariableValue ("InputSD", move_id);
				//AppDebug.LogError ($"after3 graph: {behaviorTreeOwner.graph?.GetHashCode ()}");
				//AppDebug.LogError ($"PlaySkillGraph blackboard:{graph.blackboard.GetHashCode ()}\tbehaviorTreeOwner.blackboard:{behaviorTreeOwner.blackboard.GetHashCode ()}\tbehaviorTreeOwner.graph.blackboard:{behaviorTreeOwner.graph.blackboard.GetHashCode()}\t InputSD:{behaviorTreeOwner.graph.blackboard.GetVariable<int> ("InputSD").value}");
				result = true;
			}
			return result;
		}
	}
}