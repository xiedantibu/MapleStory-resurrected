﻿using System;
using Helper;
using MapleLib.WzLib;
using ms.Util;

namespace ms
{
    public class UINpcTalk : UIElement
    {
        public enum TalkType : sbyte
        {
            NONE = -1,
            SENDOK,
            SENDYESNO,

            // TODO: Unconfirmed
            SENDNEXT,
            SENDNEXTPREV,
            SENDACCEPTDECLINE,
            SENDGETTEXT,
            SENDGETNUMBER,
            SENDSIMPLE,
        }

        public const Type TYPE = UIElement.Type.NPCTALK;
        public const bool FOCUSED = true;
        public const bool TOGGLED = false;

        public UINpcTalk()
        {
            this.offset = 0;
            this.unitrows = 0;
            this.rowmax = 0;
            this.show_slider = false;
            this.draw_text = false;
            this.formatted_text = "";
            this.formatted_text_pos = 0;
            this.timestep = 0;
            WzObject UtilDlgEx = ms.wz.wzFile_ui["UIWindow2.img"]["UtilDlgEx"];

            top = UtilDlgEx["t"];
            fill = UtilDlgEx["c"];
            bottom = UtilDlgEx["s"];
            nametag = UtilDlgEx["bar"];

            min_height = (short)(8 * fill.height() + 14);

            buttons[(int)Buttons.ALLLEVEL] = new MapleButton(UtilDlgEx["BtAllLevel"]);
            buttons[(int)Buttons.CLOSE] = new MapleButton(UtilDlgEx["BtClose"]);
            buttons[(int)Buttons.MYLEVEL] = new MapleButton(UtilDlgEx["BtMyLevel"]);
            buttons[(int)Buttons.NEXT] = new MapleButton(UtilDlgEx["BtNext"]);
            buttons[(int)Buttons.NO] = new MapleButton(UtilDlgEx["BtNo"]);
            buttons[(int)Buttons.OK] = new MapleButton(UtilDlgEx["BtOK"]);
            buttons[(int)Buttons.PREV] = new MapleButton(UtilDlgEx["BtPrev"]);
            buttons[(int)Buttons.QAFTER] = new MapleButton(UtilDlgEx["BtQAfter"]);
            buttons[(int)Buttons.QCNO] = new MapleButton(UtilDlgEx["BtQCNo"]);
            buttons[(int)Buttons.QCYES] = new MapleButton(UtilDlgEx["BtQCYes"]);
            buttons[(int)Buttons.QGIVEUP] = new MapleButton(UtilDlgEx["BtQGiveup"]);
            buttons[(int)Buttons.QNO] = new MapleButton(UtilDlgEx["BtQNo"]);
            buttons[(int)Buttons.QSTART] = new MapleButton(UtilDlgEx["BtQStart"]);
            buttons[(int)Buttons.QYES] = new MapleButton(UtilDlgEx["BtQYes"]);
            buttons[(int)Buttons.YES] = new MapleButton(UtilDlgEx["BtYes"]);

            name = new Text(Text.Font.A11M, Text.Alignment.CENTER, Color.Name.WHITE);
            text = new Text(Text.Font.A12M, Text.Alignment.LEFT, Color.Name.DARKGREY, formatted_text, 320);

            onmoved = (bool upwards) =>
            {
                short shift = (short)(upwards ? -unitrows : unitrows);
                bool above = offset + shift >= 0;
                bool below = offset + shift <= rowmax - unitrows;

                if (above && below)
                {
                    offset += shift;
                }
            };

            UI.get().remove_textfield();
        }

        public override void draw(float inter)
        {
            Point_short drawpos = new Point_short(position);
            top.draw(drawpos);
            drawpos.shift_y(top.height());
            fill.draw(new DrawArgument(new Point_short(drawpos), new Point_short(0, height)));
            drawpos.shift_y(height);
            bottom.draw(drawpos);
            drawpos.shift_y(bottom.height());

            base.draw(inter);

            short speaker_y = (short)((top.height() + height + bottom.height()) / 2);
            Point_short speaker_pos = position + new Point_short(22, (short)(11 + speaker_y));
            Point_short center_pos = speaker_pos + new Point_short((short)(nametag.width() / 2), 0);

            speaker.draw(new DrawArgument(new Point_short(center_pos), true));
            nametag.draw(speaker_pos);
            name.draw(center_pos + new Point_short(0, -4));

            if (show_slider)
            {
                short text_min_height = (short)(position.y() + top.height() - 1);
                text.draw(position + new Point_short(162, (short)(19 - offset * 400)), new Range_short(text_min_height, (short)(text_min_height + height - 18)));
                slider.draw(new Point_short(position));
            }
            else
            {
                short y_adj = (short)(height - min_height);
                //text.draw(position + new Point_short(166, (short)(48 - y_adj)));
                //text.draw(position + new Point_short(160, (short)(28 - y_adj)));

                /*var posX = position.x() + 160;
                var posY = position.y() + 28 - y_adj;*/

                var posX = position.x() + 150;
                var posY = position.y() + 28;

               // var transformedPos = ms.Window.get().Point_VirtualToPhysics(posX, posY);

                text.draw(new Point_short((short)posX, (short)posY));
                //text.draw(new Point_short((short)transformedPos.X, (short)transformedPos.Y));
                //AppDebug.Log($"npcTalk pos:{transformedPos}");

            }
        }
        public override void update()
        {
            base.update();

            /*if (draw_text)
            {
                if (timestep > 4)
                {
                    if (formatted_text_pos < formatted_text.Length)
                    {
                        string t = text.get_text();
                        sbyte c = (sbyte)formatted_text[formatted_text_pos];

                        text.change_text(t + (char)c);

                        formatted_text_pos++;
                        timestep = 0;
                    }
                    else
                    {
                        draw_text = false;
                    }
                }
                else
                {
                    timestep++;
                }
            }*/
        }

        public override Cursor.State send_cursor(bool clicked, Point_short cursorpos)
        {
            Point_short cursor_relative = cursorpos - position;

            if (show_slider && slider.isenabled())
            {
                Cursor.State sstate = slider.send_cursor(new Point_short(cursor_relative), clicked);
                if (sstate != Cursor.State.IDLE)
                {
                    return sstate;
                }
            }

            Cursor.State estate = base.send_cursor(clicked, new Point_short(cursorpos));

            if (estate == Cursor.State.CLICKING && clicked && draw_text)
            {
                draw_text = false;
                text.change_text(formatted_text);
            }

            return estate;
        }
        public override void send_key(int keycode, bool pressed, bool escape)
        {
            if (pressed && escape)
            {
                deactivate();

                new NpcTalkMorePacket((sbyte)type, 0).dispatch();
            }
        }

        public override UIElement.Type get_type()
        {
            return TYPE;
        }

        public void change_text(int npcid, sbyte msgtype, short style, sbyte speakerbyte, string tx)
        {
            AppDebug.Log($"npcid:{npcid}");
            type = get_by_value(msgtype);

            timestep = 0;
            draw_text = true;
            formatted_text_pos = 0;
            formatted_text = format_text(tx, npcid);

            //text = new Text(Text.Font.A12M, Text.Alignment.LEFT, Color.Name.DARKGREY, formatted_text, 320);
            text.change_text(formatted_text);

            short text_height = text.height();

            //text.change_text("");

            if (speakerbyte == 0)
            {
                string strid = Convert.ToString(npcid);
                strid = strid.insert(0, 7 - strid.Length, '0');
                strid = strid.append(".img");

                speaker = ms.wz.wzFile_npc[strid]["stand"]["0"];

                string namestr = ms.wz.wzFile_string["Npc.img"][Convert.ToString(npcid)]["name"].ToString();
                name.change_text(namestr);
            }
            else
            {
                speaker = new Texture();
                name.change_text("");
            }

            height = min_height;
            show_slider = false;

            if (text_height > height)
            {
                if (text_height > MAX_HEIGHT)
                {
                    height = MAX_HEIGHT;
                    show_slider = true;
                    rowmax = (short)(text_height / 400 + 1);
                    unitrows = 1;

                    short slider_y = (short)(top.height() - 7);
                    slider = new Slider((int)Slider.Type.DEFAULT_SILVER, new Range_short(slider_y, (short)(slider_y + height - 20)), (short)(top.width() - 26), unitrows, rowmax, onmoved);
                }
                else
                {
                    height = text_height;
                }
            }

            foreach (var button in buttons)
            {
                button.Value.set_active(false);
                button.Value.set_state(Button.State.NORMAL);
            }

            short y_cord = (short)(height + 48);

            buttons[(int)Buttons.CLOSE].set_position(new Point_short(9, y_cord));
            buttons[(int)Buttons.CLOSE].set_active(true);

            switch (type)
            {
                case TalkType.SENDOK:
                    {
                        buttons[(int)Buttons.OK].set_position(new Point_short(471, y_cord));
                        buttons[(int)Buttons.OK].set_active(true);
                        break;
                    }
                case TalkType.SENDYESNO:
                    {
                        Point_short yes_position = new Point_short(389, y_cord);

                        buttons[(int)Buttons.YES].set_position(yes_position);
                        buttons[(int)Buttons.YES].set_active(true);

                        buttons[(int)Buttons.NO].set_position(yes_position + new Point_short(65, 0));
                        buttons[(int)Buttons.NO].set_active(true);
                        break;
                    }
                case TalkType.SENDNEXT:
                case TalkType.SENDNEXTPREV:
                case TalkType.SENDACCEPTDECLINE:
                case TalkType.SENDGETTEXT:
                case TalkType.SENDGETNUMBER:
                case TalkType.SENDSIMPLE:
                default:
                    {
                        break;
                    }
            }

            position = new Point_short((short)(400 - top.width() / 2), (short)(240 - height / 2));
            dimension = new Point_short(top.width(), (short)(height + 120));

            /*text.TextWidth = 500;
            text.TextHeight = height;*/
        }

        public override Button.State button_pressed(ushort buttonid)
        {
            deactivate();

            switch (type)
            {
                case TalkType.SENDNEXT:
                case TalkType.SENDOK:
                    {
                        // Type = 0
                        switch ((Buttons)buttonid)
                        {
                            case Buttons.CLOSE:
                                new NpcTalkMorePacket((sbyte)type, -1).dispatch();
                                break;
                            case Buttons.NEXT:
                            case Buttons.OK:
                                new NpcTalkMorePacket((sbyte)type, 1).dispatch();
                                break;
                        }

                        break;
                    }
                case TalkType.SENDNEXTPREV:
                    {
                        // Type = 0
                        switch ((Buttons)buttonid)
                        {
                            case Buttons.CLOSE:
                                new NpcTalkMorePacket((sbyte)type, -1).dispatch();
                                break;
                            case Buttons.NEXT:
                                new NpcTalkMorePacket((sbyte)type, 1).dispatch();
                                break;
                            case Buttons.PREV:
                                new NpcTalkMorePacket((sbyte)type, 0).dispatch();
                                break;
                        }

                        break;
                    }
                case TalkType.SENDYESNO:
                    {
                        // Type = 1
                        switch ((Buttons)buttonid)
                        {
                            case Buttons.CLOSE:
                                new NpcTalkMorePacket((sbyte)type, -1).dispatch();
                                break;
                            case Buttons.NO:
                                new NpcTalkMorePacket((sbyte)type, 0).dispatch();
                                break;
                            case Buttons.YES:
                                new NpcTalkMorePacket((sbyte)type, 1).dispatch();
                                break;
                        }

                        break;
                    }
                case TalkType.SENDACCEPTDECLINE:
                    {
                        // Type = 1
                        switch ((Buttons)buttonid)
                        {
                            case Buttons.CLOSE:
                                new NpcTalkMorePacket((sbyte)type, -1).dispatch();
                                break;
                            case Buttons.QNO:
                                new NpcTalkMorePacket((sbyte)type, 0).dispatch();
                                break;
                            case Buttons.QYES:
                                new NpcTalkMorePacket((sbyte)type, 1).dispatch();
                                break;
                        }

                        break;
                    }
                case TalkType.SENDGETTEXT:
                    {
                        // TODO: What is this?
                        break;
                    }
                case TalkType.SENDGETNUMBER:
                    {
                        // Type = 3
                        switch ((Buttons)buttonid)
                        {
                            case Buttons.CLOSE:
                                new NpcTalkMorePacket((sbyte)type, 0).dispatch();
                                break;
                            case Buttons.OK:
                                new NpcTalkMorePacket((sbyte)type, 1).dispatch();
                                break;
                        }

                        break;
                    }
                case TalkType.SENDSIMPLE:
                    {
                        // Type = 4
                        switch ((Buttons)buttonid)
                        {
                            case Buttons.CLOSE:
                                new NpcTalkMorePacket((sbyte)type, 0).dispatch();
                                break;
                            default:
                                new NpcTalkMorePacket(0).dispatch(); // TODO: Selection
                                break;
                        }

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return Button.State.NORMAL;
        }

        private UINpcTalk.TalkType get_by_value(sbyte value)
        {
            if (value > (int)TalkType.NONE && value < EnumUtil.GetEnumLength<TalkType>())
            {
                return (TalkType)value;
            }

            return TalkType.NONE;
        }

        // TODO: Move this to GraphicsGL?11
        private string format_text(string tx, int npcid)
        {
            return NpcTextParser.inst.Parse(tx);
            /* var lines = tx.Split("\r\n");

			string formatted_text = tx;
			int begin = (int)formatted_text.IndexOf("#p");

			if (begin != -1)
			{
				int end = formatted_text.IndexOf('#', (int)(begin + 1));

				if (end != -1)
				{
					string namestr = nl.nx.wzFile_string["Npc.img"][Convert.ToString(npcid)]["name"].ToString ();
					formatted_text = formatted_text.Remove((int)begin, (int)(end - begin)).Insert((int)begin, namestr);
				}
			}

			begin = formatted_text.IndexOf("#h");

			if (begin != -1)
			{
				int end = formatted_text.IndexOf('#', (int)begin + 1);

				if (end != -1)
				{
					string charstr = Stage.get().get_player().get_name();
					formatted_text = formatted_text.Remove(begin, end - begin).Insert(begin, charstr);
				}
			}

			begin = formatted_text.IndexOf("#t");

			if (begin != -1)
			{
				int end = formatted_text.IndexOf('#', begin + 1);

				if (end != -1)
				{
					int b = begin + 2;
					int itemid = Convert.ToInt32(formatted_text.Substring(b, end - b));
					string itemname = nl.nx.wzFile_string["Consume.img"][itemid.ToString ()]["name"].ToString ();

					formatted_text = formatted_text.Remove(begin, end - begin).Insert(begin, itemname);
				}
			}

			return formatted_text; */
        }

        private const short MAX_HEIGHT = 248;

        public override void OnAdd()
        {
            text?.AddGRichTextToGRoot();

            if (text != null) text.onClickLinkHandler = OnClickLink;
        }

        public override void OnRemove()
        {
            if (text != null) text.onClickLinkHandler = null;
            text?.Dispose();
        }

        public override void OnActivityChange(bool isActive)
        {
            text?.OnActivityChange(isActive);
        }

        private void OnClickLink(int selection)
        {
            new NpcTalkMorePacket(selection).dispatch();
        }
        private enum Buttons
        {
            ALLLEVEL,
            CLOSE,
            MYLEVEL,
            NEXT,
            NO,
            OK,
            PREV,
            QAFTER,
            QCNO,
            QCYES,
            QGIVEUP,
            QNO,
            QSTART,
            QYES,
            YES
        }

        private readonly Texture top;
        private readonly Texture fill;
        private readonly Texture bottom;
        private readonly Texture nametag;
        private Texture speaker = new Texture();

        private Text text;
        private readonly Text name;

        private short height;
        private short offset;
        private short unitrows;
        private short rowmax;
        private readonly short min_height;

        private bool show_slider;
        private bool draw_text;
        private Slider slider = new Slider();
        private TalkType type;
        private string formatted_text;
        private int formatted_text_pos;
        private ushort timestep;

        private readonly System.Action<bool> onmoved;
    }
}





#if USE_NX
#endif
