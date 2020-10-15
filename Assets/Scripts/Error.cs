﻿/*//////////////////////////////////////////////////////////////////////////////////
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
	// Error codes to be checked after initialization
	public class Error
	{
		public enum Code
		{
			NONE,
			CONNECTION,
			NLNX,
			WZ,
			MISSING_FILE,
			WRONG_UI_FILE,
			GLFW,
			GLEW,
			FREETYPE,
			VERTEX_SHADER,
			FRAGMENT_SHADER,
			SHADER_PROGRAM_LINK,
			SHADER_PROGRAM_VALID,
			SHADER_VARS,
			WINDOW,
			AUDIO,
			MISSING_ICON,
			FONT_PATH,
			LENGTH
		}

		public Error(Code c) : this(c, "")
		{
		}
		public Error(Code c, string args)
		{
			this.code = new ms.Error.Code(c);
			this.args = args;
		}

//C++ TO C# CONVERTER CRACKED BY X-CRACKER 2017 WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: constexpr operator bool() const
		public static implicit operator bool(Error ImpliedObject)
		{
			return ImpliedObject.code != Code.NONE;
		}

//C++ TO C# CONVERTER CRACKED BY X-CRACKER 2017 WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: constexpr bool can_retry() const
		public bool can_retry()
		{
			return code == Code.CONNECTION || code == Code.MISSING_FILE || code == Code.WRONG_UI_FILE || code == Code.MISSING_ICON || code == Code.FONT_PATH;
		}

//C++ TO C# CONVERTER CRACKED BY X-CRACKER 2017 WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: constexpr const sbyte* get_message() const
		public string get_message()
		{
			return messages[(int)code];
		}

//C++ TO C# CONVERTER CRACKED BY X-CRACKER 2017 WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: constexpr const sbyte* get_args() const
		public string get_args()
		{
			return args;
		}

		private Code code;
		private readonly string args;

		private const string[] messages = {"", "Cannot connect to server.", "Could not initialize NLNX.", "Could not initialize WZ.", "Missing a game file: ", "UI.nx has wrong version.", "Could not initialize GLFW.", "Could not initialize GLEW.", "Could not initialize FreeType.", "Failed to compile vertex shader.", "Failed to compile fragment shader.", "Failed to link shader program.", "Failed to validate shader program.", "Failed to locate shader variables.", "Failed to create window.", "Failed to initialize audio.", "Could not load icon.", "Could not load fonts."};
	}
}*/