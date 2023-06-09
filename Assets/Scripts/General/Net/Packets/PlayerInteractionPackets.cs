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
	// Packet to request character info
	// Opcode: CHAR_INFO_REQUEST(97)
	public class CharInfoRequestPacket : OutPacket
	{
		public CharInfoRequestPacket(int character_id) : base((short)OutPacket.Opcode.CHAR_INFO_REQUEST)
		{
			write_random();
			write_int(character_id);
		}
	}

	// Packet to CHANGE_MAP_SPECIAL
	// Opcode: CHANGE_MAP_SPECIAL(100)
	public class ChangeMapSpecialPacket : OutPacket
	{
		public ChangeMapSpecialPacket () : base ((short)OutPacket.Opcode.CHANGE_MAP_SPECIAL)
		{
			write_byte (0);
			write_string (Stage.get ().just_Entered_portalName);
			write_short (0);
		}
	}
}