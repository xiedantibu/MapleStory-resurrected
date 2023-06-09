﻿using System.Collections.Generic;
using System.Linq;
using MapleLib.WzLib;




namespace ms
{
	public class Charset
	{
		public enum Alignment
		{
			LEFT,
			CENTER,
			RIGHT
		}

		public Charset (WzObject src, Alignment alignment)
		{
			this.alignment = alignment;
			foreach (var sub in src)
			{
				string name = sub.Name;

				if (!sub.IsTexture () || string.IsNullOrEmpty (name))
				{
					continue;
				}

				var c = name.FirstOrDefault ();

				//sbyte c = name.GetEnumerator();

				if (c == '\\')
				{
					c = '/';
				}

				//AppDebug.Log ($"{src.FullPath}\t name:{name}\t c:{c}");

				chars.TryAdd ((sbyte)c, new Texture (sub), true); //todo 2 refresh?
			}
		}

		public Charset ()
		{
			this.alignment = Charset.Alignment.LEFT;
		}

		public void draw (sbyte c, DrawArgument args)
		{
			if (chars.TryGetValue (c, out var texture))
			{
				texture.draw (args);
			}
		}

		public short draw (string text, DrawArgument args)
		{
			if (string.IsNullOrEmpty (text)) return 0;
			short shift = 0;
			short total = 0;

			switch (alignment)
			{
				case Charset.Alignment.CENTER:
				{
					foreach (sbyte c in text)
					{
						short width = getw (c);

						draw (c, args + new Point_short (shift, 0));
						shift += (short)(width + 2);
						total += (short)width;
					}

					shift -= (short)(total / 2);
					break;
				}
				case Charset.Alignment.LEFT:
				{
					foreach (sbyte c in text)
					{
						draw (c, args + new Point_short (shift, 0));
						shift += (short)(getw (c) + 1);
					}

					break;
				}
				case Charset.Alignment.RIGHT:
				{
					for (int i = text.Length - 1; i >= 0; i--)
					{
						var c = text[i];
						shift += getw ((sbyte)c);
						draw ((sbyte)c, args - new Point_short (shift, 0));
					}

					break;
				}
			}

			return shift;
		}

		public short draw (string text, short hspace, DrawArgument args)
		{
			uint length = (uint)text.Length;
			short shift = 0;

			switch (alignment)
			{
				case Charset.Alignment.CENTER:
				{
					shift -= (short)(hspace * (short)length / 2);
					break;
				}
				case Charset.Alignment.LEFT:
				{
					foreach (sbyte c in text)
					{
						draw (c, args + new Point_short (shift, 0));

						shift += hspace;
					}

					break;
				}
				case Charset.Alignment.RIGHT:
				{
					foreach (sbyte c in text)
					{
						shift += hspace;

						draw (c, args - new Point_short (shift, 0));
					}

					break;
				}
			}

			return shift;
		}

		public short getw (sbyte c)
		{
			if (chars.TryGetValue (c, out var texture))
			{
				return texture.width ();
			}

			return 0;
		}

		private Dictionary<sbyte, Texture> chars = new Dictionary<sbyte, Texture> ();
		private Alignment alignment;
	}
}