﻿using System;
using System.Collections.Generic;
using System.Linq;
using Beebyte.Obfuscator;
using MapleLib.WzLib;




namespace ms
{
    [Skip]
    // Race selection screen
    public class UIRaceSelect : UIElement
    {
        public const Type TYPE = UIElement.Type.RACESELECT;
        public const bool FOCUSED = false;
        public const bool TOGGLED = false;

        public UIRaceSelect(params object[] args) : this()
        {
        }

        public UIRaceSelect() : base(new Point_short(0, 0), new Point_short(800, 600))
        {
            string version_text = Configuration.get().get_version();
            version = new Text(Text.Font.A11B, Text.Alignment.LEFT, Color.Name.LEMONGRASS, "Ver. " + version_text);

            WzObject Login = ms.wz.wzFile_ui["Login.img"];
            WzObject Common = Login["Common"];
            WzObject RaceSelect = Login["RaceSelect_new"];

            WzObject frame = ms.wz.wzFile_mapLatest["Obj"]["login.img"]["Common"]["frame"]["2"]["0"];

            Point_short make_pos = RaceSelect["make"]["pos"];
            Point_short make_posZero = RaceSelect["make"]["posZero"];

            pos = new Point_short(Math.Abs(make_pos.x()), Math.Abs(make_pos.y()));
            posZero = new Point_short(Math.Abs(make_posZero.x()), Math.Abs(make_posZero.y()));

            order = (RaceSelect["order"][SELECTED_LIST.ToString()]);
            hotlist = (RaceSelect["hotList"][SELECTED_LIST.ToString()]);
            newlist = (RaceSelect["newList"][SELECTED_LIST.ToString()]);
            bgm = (RaceSelect["bgm"]);

            hotlabel = RaceSelect["hotLabel"];
            hotlabelZero = RaceSelect["hotLabel2"];
            newlabel = RaceSelect["newLabel"];
            hotbtn = RaceSelect["hot"];
            newbtn = RaceSelect["new"];

            class_index[0] = order[0.ToString()];
            class_index[1] = order[1.ToString()];
            class_index[2] = order[2.ToString()];
            class_index[3] = order[3.ToString()];
            class_index[4] = order[4.ToString()];

            mouseover[0] = true;
            mouseover[1] = false;
            mouseover[2] = false;
            mouseover[3] = false;
            mouseover[4] = false;

            WzObject button = RaceSelect["button"];
            WzObject buttonDisabled = RaceSelect["buttonDisabled"];

            class_count = (ushort)button.Count();
            class_isdisabled = new List<bool>(class_count);
            class_disabled = new List<BoolPairNew<Texture>>(class_count);
            class_normal = new List<BoolPairNew<Texture>>(class_count);
            class_background = new List<Texture>(class_count);
            class_details = new List<Texture>(class_count);
            class_title = new List<Texture>(class_count);
            class_map = new List<ushort>(class_count);

            class_isdisabled.TryAdd((int)Classes.EXPLORER, false);
            class_isdisabled.TryAdd((int)Classes.CYGNUSKNIGHTS, false);
            class_isdisabled.TryAdd((int)Classes.ARAN, false);

            sprites.Add(new Sprite(frame, new Point_short(400, 300)));
            sprites.Add(new Sprite(Common["frame"], new Point_short(400, 300)));

            back = RaceSelect["Back"]["1"]["0"];
            backZero = RaceSelect["Back"]["2"]["0"];
            back_ani = RaceSelect["BackAni"];
            class_details_background = RaceSelect["Back1"]["0"]["0"];
            class_details_backgroundZero = RaceSelect["Back1"]["1"]["0"];

            ushort node_index = 0;

            foreach (var node in button)
            {
                class_map.TryAdd(node_index++, (ushort)Convert.ToInt32(node.Name));
                //class_map[node_index++] = (ushort)Convert.ToInt32(node.Name);
            }
            class_map.Sort(0, class_count, default);
            //sort (class_map.GetEnumerator (), class_map.GetEnumerator () + class_count);

            for (ushort i = 0; i < class_count; i++)
            {
                string corrected_index = class_map[i].ToString();

                class_normal.TryAdd(i, new BoolPairNew<Texture>(button[corrected_index]["mouseOver"]["0"], button[corrected_index]["normal"]["0"]));
                //class_normal[i][false] = button[corrected_index]["normal"]["0"];
                //class_normal[i][true] = button[corrected_index]["mouseOver"]["0"];

                class_disabled.TryAdd(i, new BoolPairNew<Texture>(buttonDisabled[corrected_index]["mouseOver"]["0"], buttonDisabled[corrected_index]["normal"]["0"]));
                //class_disabled[i][false] = buttonDisabled[corrected_index]["normal"]["0"];
                //class_disabled[i][true] = buttonDisabled[corrected_index]["mouseOver"]["0"];

                class_background.TryAdd(i, RaceSelect["Back0"][corrected_index]["0"]);
                //class_background[i] = RaceSelect["Back0"][corrected_index]["0"];

                class_background.TryAdd(i, RaceSelect["Back2"][corrected_index]["0"]);
                //class_details[i] = RaceSelect["Back2"][corrected_index]["0"];

                class_background.TryAdd(i, RaceSelect["Back3"][corrected_index]["0"]);
                //class_title[i] = RaceSelect["Back3"][corrected_index]["0"];
            }

            buttons[(int)Buttons.BACK] = new MapleButton(Common["BtStart"], new Point_short(0, 515));
            buttons[(int)Buttons.MAKE] = new MapleButton(RaceSelect["make"]);
            buttons[(int)Buttons.LEFT] = new MapleButton(RaceSelect["leftArrow"], new Point_short(41, 458));
            buttons[(int)Buttons.RIGHT] = new MapleButton(RaceSelect["rightArrow"], new Point_short(718, 458));

            for (uint i = 0; i <= (ulong)Buttons.CLASS0; i++)
            {
                buttons[(int)Buttons.CLASS0 + i] = new AreaButton(get_class_pos(i), class_normal[0][true].get_dimensions());
            }

            index_shift = 0;
            selected_index = 0;
            selected_class = class_index[selected_index];

            buttons[(int)Buttons.LEFT].set_state(Button.State.DISABLED);

            new Sound(Sound.Name.RACESELECT).play();
        }

        public override void draw(float inter)
        {
            ushort corrected_index = get_corrected_class_index(selected_class);

            if (selected_class == (int)Classes.ZERO)
            {
                backZero.draw(position);
            }
            else
            {
                back.draw(position);
            }

            base.draw_sprites(inter);

            version.draw(position + new Point_short(707, 4));

            if (selected_class == (int)Classes.KANNA || selected_class == (int)Classes.CHASE)
            {
                if (selected_class == (int)Classes.ZERO)
                {
                    class_details_backgroundZero.draw(position);
                }
                else
                {
                    class_details_background.draw(position);
                }

                class_background[corrected_index].draw(position);
            }
            else
            {
                class_background[corrected_index].draw(position);

                if (selected_class == (int)Classes.ZERO)
                {
                    class_details_backgroundZero.draw(position);
                }
                else
                {
                    class_details_background.draw(position);
                }
            }
            class_details.TryGet(corrected_index)?.draw(position);
            class_title.TryGet(corrected_index)?.draw(position);

            //class_details[corrected_index].draw(position);
            //class_title[corrected_index]?.draw(position);


            foreach (var node in hotlist ?? Enumerable.Empty<WzImageProperty>())
            {
                if (node == selected_class)
                {
                    if (selected_class == (int)Classes.ZERO)
                    {
                        hotlabelZero.draw(new Point_short(position), inter);
                    }
                    else
                    {
                        hotlabel.draw(new Point_short(position), inter);
                    }

                    break;
                }
            }

            foreach (var node in newlist ?? Enumerable.Empty<WzImageProperty>())
            {
                if (node == selected_class)
                {
                    newlabel.draw(new Point_short(position), inter);
                    break;
                }
            }

            for (ushort i = 0; i < INDEX_COUNT; i++)
            {
                Point_short button_pos = get_class_pos(i);

                ushort cur_class = get_corrected_class_index(class_index[i]);
                var found_class = class_isdisabled.TryGet(cur_class) ? class_disabled : class_normal;
                found_class[cur_class][mouseover[i]].draw(position + button_pos);

                foreach (var node in hotlist ?? Enumerable.Empty<WzImageProperty>())
                {
                    if (node == class_index[i])
                    {
                        hotbtn.draw(position + button_pos, inter);
                        break;
                    }
                }

                foreach (var node in newlist ?? Enumerable.Empty<WzImageProperty>())
                {
                    if (node == class_index[i])
                    {
                        newbtn.draw(position + button_pos, inter);
                        break;
                    }
                }
            }

            base.draw_buttons(inter);

            back_ani.draw(new Point_short(position), inter);
        }

        public override void update()
        {
            base.update();

            hotlabel.update();
            hotlabelZero.update();
            newlabel.update();
            hotbtn.update();
            newbtn.update();

            if (selected_class == (int)Classes.ZERO)
            {
                buttons[(int)Buttons.MAKE].set_position(position + posZero);
            }
            else
            {
                buttons[(int)Buttons.MAKE].set_position(position + pos);
            }

            back_ani.update();
        }

        public override Cursor.State send_cursor(bool clicked, Point_short cursorpos)
        {
            foreach (var btit in buttons)
            {
                if (btit.Value.is_active() && btit.Value.bounds(position).contains(cursorpos))
                {
                    if (btit.Value.get_state() == Button.State.NORMAL)
                    {
                        new Sound(Sound.Name.BUTTONOVER).play();

                        if (btit.Key >= (ulong)Buttons.CLASS0)
                        {
                            mouseover[btit.Key - (ulong)Buttons.CLASS0] = true;
                        }

                        btit.Value.set_state(Button.State.MOUSEOVER);
                    }
                    else if (btit.Value.get_state() == Button.State.MOUSEOVER)
                    {
                        if (clicked)
                        {
                            new Sound(Sound.Name.BUTTONCLICK).play();

                            btit.Value.set_state(button_pressed((ushort)btit.Key));
                        }
                        else
                        {
                            if (btit.Key >= (ulong)Buttons.CLASS0)
                            {
                                mouseover[btit.Key - (ulong)Buttons.CLASS0] = true;
                            }
                        }
                    }
                }
                else if (btit.Value.get_state() == Button.State.MOUSEOVER)
                {
                    if (btit.Key >= (ulong)Buttons.CLASS0)
                    {
                        mouseover[btit.Key - (ulong)Buttons.CLASS0] = false;
                    }

                    btit.Value.set_state(Button.State.NORMAL);
                }
            }

            return Cursor.State.LEAF;
        }

        public override void send_key(int keycode, bool pressed, bool escape)
        {
            if (pressed)
            {
                if (escape)
                {
                    show_charselect();
                }
                else if (keycode == (int)KeyAction.Id.LEFT || keycode == (int)KeyAction.Id.DOWN)
                {
                    if (buttons[(int)Buttons.LEFT].get_state() == Button.State.NORMAL)
                    {
                        button_pressed((ushort)Buttons.LEFT);
                    }
                }
                else if (keycode == (int)KeyAction.Id.RIGHT || keycode == (int)KeyAction.Id.UP)
                {
                    if (buttons[(int)Buttons.RIGHT].get_state() == Button.State.NORMAL)
                    {
                        button_pressed((ushort)Buttons.RIGHT);
                    }
                }
                else if (keycode == (int)KeyAction.Id.RETURN)
                {
                    button_pressed((ushort)Buttons.MAKE);
                }
            }
        }

        public override UIElement.Type get_type()
        {
            return TYPE;
        }

        public bool check_name(string name)
        {
            WzObject ForbiddenName = ms.wz.wzFile_etc["ForbiddenName.img"];

            foreach (var forbiddenName in ForbiddenName)
            {
                string lName = to_lower(name);
                string fName = to_lower(forbiddenName.ToString());

                if (lName.IndexOf(fName) != -1)
                {
                    return false;
                }
            }

            return true;
        }

        public void send_naming_result(bool nameused)
        {
            /*todo if (selected_class == (int)Classes.EXPLORER)
			{
				var explorercreation = UI.get ().get_element<UIExplorerCreation> ();
				if (explorercreation != null)
				{
					explorercreation.send_naming_result (nameused);
				}
			}
			else if (selected_class == (int)Classes.CYGNUSKNIGHTS)
			{
				var cygnuscreation = UI.get ().get_element<UICygnusCreation> ();
				if (cygnuscreation!=null)
				{
					cygnuscreation.send_naming_result (nameused);
				}
			}
			else if (selected_class == (int)Classes.ARAN)
			{
				var arancreation = UI.get ().get_element<UIAranCreation> ();
				if (arancreation!=null)
				{
					arancreation.send_naming_result (nameused);
				}
			}*/
        }

        public override Button.State button_pressed(ushort buttonid)
        {
            if (buttonid == (int)Buttons.BACK)
            {
                show_charselect();

                return Button.State.NORMAL;
            }
            else if (buttonid == (int)Buttons.MAKE)
            {
                ushort corrected_index = get_corrected_class_index(selected_class);

                System.Action okhandler = () =>
                {
                    if (!class_isdisabled[corrected_index])
                    {
                        new Sound(Sound.Name.SCROLLUP).play();

                        deactivate();

                        if (selected_class == (int)Classes.EXPLORER)
                        {
                            UI.get().emplace<UIExplorerCreation>();
                        }
                        else if (selected_class == (int)Classes.CYGNUSKNIGHTS)
                        {
                            UI.get().emplace<UICygnusCreation>();
                        }
                        else if (selected_class == (int)Classes.ARAN)
                        {
                            UI.get().emplace<UIAranCreation>();
                        }
                    }
                };

                UI.get().emplace<UIClassConfirm>(selected_class, class_isdisabled[corrected_index], okhandler);

                return Button.State.NORMAL;
            }
            else if (buttonid == (int)Buttons.LEFT)
            {
                ushort new_index = (ushort)(selected_index - 1);

                if (selected_index - index_shift == 0)
                {
                    index_shift--;

                    class_index[0] = order[(new_index + 4 - (int)Buttons.CLASS0).ToString()];
                    class_index[1] = order[(new_index + 5 - (int)Buttons.CLASS0).ToString()];
                    class_index[2] = order[(new_index + 6 - (int)Buttons.CLASS0).ToString()];
                    class_index[3] = order[(new_index + 7 - (int)Buttons.CLASS0).ToString()];
                    class_index[4] = order[(new_index + 8 - (int)Buttons.CLASS0).ToString()];
                }

                select_class(new_index);

                return Button.State.IDENTITY;
            }
            else if (buttonid == (int)Buttons.RIGHT)
            {
                ushort new_index = (ushort)(selected_index + 1);
                ushort selected = class_index[selected_index - index_shift];

                if (selected == class_index[4])
                {
                    index_shift++;

                    class_index[0] = order[(new_index + 0 - (int)Buttons.CLASS0).ToString()];
                    class_index[1] = order[(new_index + 1 - (int)Buttons.CLASS0).ToString()];
                    class_index[2] = order[(new_index + 2 - (int)Buttons.CLASS0).ToString()];
                    class_index[3] = order[(new_index + 3 - (int)Buttons.CLASS0).ToString()];
                    class_index[4] = order[(new_index + 4 - (int)Buttons.CLASS0).ToString()];
                }

                select_class(new_index);

                return Button.State.IDENTITY;
            }
            else if (buttonid >= (int)Buttons.CLASS0)
            {
                ushort index = (ushort)(buttonid - Buttons.CLASS0 + index_shift);

                select_class(index);

                return Button.State.IDENTITY;
            }
            else
            {
                return Button.State.DISABLED;
            }
        }

        private void select_class(ushort index)
        {
            ushort previous_index = selected_index;
            selected_index = index;

            if (previous_index != selected_index)
            {
                new Sound(Sound.Name.RACESELECT).play();

                ushort previous = (ushort)(previous_index - index_shift);

                mouseover[previous] = false;
                buttons[(uint)(previous + Buttons.CLASS0)].set_state(Button.State.NORMAL);

                ushort selected = (ushort)(selected_index - index_shift);

                selected_class = class_index[selected];
                mouseover[selected] = true;

                if (selected_class == (int)Classes.KINESIS)
                {
                    WzObject node = bgm[selected_class.ToString()];
                    string found_bgm = node["bgm"].ToString();
                    uint found_img = (uint)found_bgm.IndexOf(".img");

                    if (found_img == -1)
                    {
                        uint found_slash = (uint)found_bgm.IndexOf('/');

                        if (found_slash != -1)
                        {
                            found_bgm = found_bgm.Insert((int)found_slash, ".img");

                            Music.get().play(found_bgm);
                        }
                    }
                }
                else if (class_index[previous] == (int)Classes.KINESIS)
                {
                    string LoginMusicNewtro = Configuration.get().get_login_music_newtro();

                    Music.get().play(LoginMusicNewtro);
                }
            }
            else
            {
                button_pressed((ushort)Buttons.MAKE);
            }

            if (selected_index > 0)
            {
                buttons[(int)Buttons.LEFT].set_state(Button.State.NORMAL);
            }
            else
            {
                buttons[(int)Buttons.LEFT].set_state(Button.State.DISABLED);
            }

            if (selected_index < order.Count() - 1)
            {
                buttons[(int)Buttons.RIGHT].set_state(Button.State.NORMAL);
            }
            else
            {
                buttons[(int)Buttons.RIGHT].set_state(Button.State.DISABLED);
            }
        }

        private void show_charselect()
        {
            new Sound(Sound.Name.SCROLLUP).play();

            deactivate();

            var charselect = UI.get().get_element<UICharSelect>();
            if (charselect)
            {
                charselect.get().makeactive();
            }
        }

        private Point_short get_class_pos(uint index)
        {
            ushort x_adj = (ushort)(index * 126);

            return new Point_short((short)(95 + x_adj), 430);
        }

        private string to_lower(string value)
        {
            return value.ToLower();
            //transform (value.GetEnumerator (), value.end (), value.GetEnumerator (), global::tolower);

            //return value;
        }

        private ushort get_corrected_class_index(ushort index)
        {
            for (ushort i = 0; i < class_count; i++)
            {
                if (index == class_map[i])
                {
                    return i;
                }
            }

            Console.Write("Failed to find corrected class index");
            Console.Write("\n");

            return index;
        }

        private const ushort INDEX_COUNT = 5;
        private const ushort SELECTED_LIST = 33;

        private enum Buttons : ushort
        {
            BACK,
            MAKE,
            LEFT,
            RIGHT,
            CLASS0,
            CLASS1,
            CLASS2,
            CLASS3,
            CLASS4
        }

        private enum Classes : ushort
        {
            RESISTANCE,
            EXPLORER,
            CYGNUSKNIGHTS,
            ARAN,
            EVAN,
            MERCEDES,
            DEMON,
            PHANTOM,
            DUALBLADE,
            MIHILE,
            LUMINOUS,
            KAISER,
            ANGELICBUSTER,
            CANNONEER,
            XENON,
            ZERO,
            SHADE,
            PINKBEAN,
            KINESIS,
            CADENA,
            ILLIUM,
            ARK,
            PATHFINDER,
            HOYOUNG,
            JETT,
            HAYATO,
            KANNA,
            CHASE
        }

        private Text version = new Text();
        private Point_short pos = new Point_short();
        private Point_short posZero = new Point_short();
        private WzObject order;
        private WzObject hotlist;
        private WzObject newlist;
        private WzObject bgm;
        private Sprite hotlabel = new Sprite();
        private Sprite hotlabelZero = new Sprite();
        private Sprite newlabel = new Sprite();
        private Sprite hotbtn = new Sprite();
        private Sprite newbtn = new Sprite();
        private ushort[] class_index = new ushort[INDEX_COUNT];
        private bool[] mouseover = new bool[INDEX_COUNT];
        private ushort selected_class;
        private ushort index_shift;
        private ushort selected_index;
        private ushort class_count;
        private List<bool> class_isdisabled = new List<bool>();
        private List<BoolPairNew<Texture>> class_disabled = new List<BoolPairNew<Texture>>();
        private List<BoolPairNew<Texture>> class_normal = new List<BoolPairNew<Texture>>();
        private List<Texture> class_background = new List<Texture>();
        private List<Texture> class_details = new List<Texture>();
        private List<Texture> class_title = new List<Texture>();
        private List<ushort> class_map = new List<ushort>();
        private Texture back = new Texture();
        private Texture backZero = new Texture();
        private Sprite back_ani = new Sprite();
        private Texture class_details_background = new Texture();
        private Texture class_details_backgroundZero = new Texture();
    }
}


#if USE_NX
#endif