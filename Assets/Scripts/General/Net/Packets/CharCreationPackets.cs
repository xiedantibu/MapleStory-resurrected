﻿//////////////////////////////////////////////////////////////////////////////////
//	This file is part of the continued Journey MMORPG client					//
//	Copyright (C) 2015-2019  Daniel Allendorf, Ryan Payton						//
//																				//
//	This program is free software: you can redistribute it and/or modify		//
//	it under the terms of the GNU Affero General Public License as published by	//
//	the Free Software Foundation, either version 3 of the License, or			//
//	(at your option) any later version.											//
//																				//
//	This program is distributed in the hope that it will be useful,				//
//	but WITHOUT ANY WARRANTY; without even the implied warranty of				//
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the				//
//	GNU Affero General Public License for more details.							//
//																				//
//	You should have received a copy of the GNU Affero General Public License	//
//	along with this program.  If not, see <https://www.gnu.org/licenses/>.		//
//////////////////////////////////////////////////////////////////////////////////


namespace ms
{
	// Reserve a name for the character to be created.
	// Opcode: NAME_CHAR(21)
	public class NameCharPacket : OutPacket
	{
		public NameCharPacket (string name) : base ((short)OutPacket.Opcode.NAME_CHAR)
		{
			write_string (name);
		}
	}

	// Requests creation of a character with the specified stats.
	// Opcode: CREATE_CHAR(22)
	public class CreateCharPacket : OutPacket
	{
		public CreateCharPacket (string name, ushort job, int face, int hair, byte hairc, byte skin, int top, int bot, int shoes, int weapon, bool female) : base ((short)OutPacket.Opcode.CREATE_CHAR)
		{
			write_string (name);
			write_int (job);
			write_int (face);
			write_int (hair);
			write_int (hairc);
			write_int (skin);
			write_int (top);
			write_int (bot);
			write_int (shoes);
			write_int (weapon);
			write_byte ((sbyte)(female ? 1 : 0));
		}
	}
}