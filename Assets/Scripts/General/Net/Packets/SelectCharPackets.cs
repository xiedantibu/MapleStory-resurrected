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
	// Tell the server which character was picked.
	// Opcode: SELECT_CHAR(19)
	public class SelectCharPacket : OutPacket
	{
		private static sbyte[] tempbyte =new sbyte[46] {1,0,0,0,17,0,48,48,45,70,70,45,50,55,45,65,67,45,57,67,45,68,54,21,0,50,69,70,68,66,57,56,55,57,57,68,68,95,67,66,52,70,52,70,56,56 };
		public SelectCharPacket(int cid) : base((short)OutPacket.Opcode.SELECT_CHAR)
		{
			write_int(cid);
			write_hardware_info();
			//write_Bytes (tempbyte);
		}
	}

	// Registers a pic and tells the server which character was picked.
	// Opcode: REGISTER_PIC(29)
	public class RegisterPicPacket : OutPacket
	{
		public RegisterPicPacket(int cid, string pic) : base((short)OutPacket.Opcode.REGISTER_PIC)
		{
			skip(1);

			write_int(cid);
			write_hardware_info();
			write_string(pic);
		}
	}

	// Requests using the specified character with the specified pic.
	// Opcode: SELECT_CHAR_PIC(30)
	public class SelectCharPicPacket : OutPacket
	{
		public SelectCharPicPacket(string pic, int cid) : base((short)OutPacket.Opcode.SELECT_CHAR_PIC)
		{
			write_string(pic);
			write_int(cid);
			write_hardware_info();
		}
	}

	// Requests deleting the specified character without a pic.
	// Opcode: DELETE_CHAR(23)
	public class DeleteCharPacket : OutPacket
	{
		public DeleteCharPacket(int cid) : base((short)OutPacket.Opcode.DELETE_CHAR)
		{
			write_string("");
			write_int(cid);
		}
	}

	// Requests deleting the specified character with the specified pic.
	// Opcode: DELETE_CHAR(23)
	public class DeleteCharPicPacket : OutPacket
	{
		public DeleteCharPicPacket(string pic, int cid) : base((short)OutPacket.Opcode.DELETE_CHAR)
		{
			write_string(pic);
			write_int(cid);
		}
	}
}