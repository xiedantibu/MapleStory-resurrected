/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ms_Unity
{
    public partial class FGUI_StatusBar : GComponent
    {
        public Controller _c1;
        public Controller _c2;
        public GProgressBar _ProgressBar_HP;
        public GProgressBar _ProgressBar_MP;
        public GTextField _Txt_Name;
        public GTextField _Txt_Level;
        public GGroup _Level;
        public GGroup _TopLeft;
        public GButton _Btn_OpenFunctionPanel;
        public GButton _Btn_OpenInventoryPanel;
        public GGroup _TopRight;
        public GGroup _Left;
        public GButton _BT_MENU;
        public GButton _BT_CASHSHOP;
        public GButton _BT_CHARACTER_SKILL;
        public GButton _BT_SETTING_CHANNEL;
        public GButton _BT_COMMUNITY_FRIENDS;
        public GButton _BT_MENU_QUEST;
        public GButton _BT_COMMUNITY_PARTY;
        public GButton _BT_BackToChooseChar;
        public GButton _BT_BackToLogin;
        public GGroup _mid;
        public GGroup _Right;
        public GGroup _FunctionPanel;
        public GTextField _Txt_EXP;
        public GTextField _Txt_Channel;
        public GTextField _Txt_Time;
        public GProgressBar _ProgressBar_EXP;
        public GTextField _Txt_Version;
        public GGroup _Bottom;
        public Transition _t_ShowFunctionPanel;
        public Transition _t_HideFunctionPanel;
        public const string URL = "ui://4916gthqq03inpi";

        public static FGUI_StatusBar CreateInstance()
        {
            return (FGUI_StatusBar)UIPackage.CreateObject("ms_Unity", "StatusBar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            _c1 = GetControllerAt(0);
            _c2 = GetControllerAt(1);
            _ProgressBar_HP = (GProgressBar)GetChildAt(3);
            _ProgressBar_MP = (GProgressBar)GetChildAt(4);
            _Txt_Name = (GTextField)GetChildAt(5);
            _Txt_Level = (GTextField)GetChildAt(7);
            _Level = (GGroup)GetChildAt(8);
            _TopLeft = (GGroup)GetChildAt(9);
            _Btn_OpenFunctionPanel = (GButton)GetChildAt(11);
            _Btn_OpenInventoryPanel = (GButton)GetChildAt(12);
            _TopRight = (GGroup)GetChildAt(13);
            _Left = (GGroup)GetChildAt(15);
            _BT_MENU = (GButton)GetChildAt(18);
            _BT_CASHSHOP = (GButton)GetChildAt(21);
            _BT_CHARACTER_SKILL = (GButton)GetChildAt(22);
            _BT_SETTING_CHANNEL = (GButton)GetChildAt(24);
            _BT_COMMUNITY_FRIENDS = (GButton)GetChildAt(25);
            _BT_MENU_QUEST = (GButton)GetChildAt(26);
            _BT_COMMUNITY_PARTY = (GButton)GetChildAt(27);
            _BT_BackToChooseChar = (GButton)GetChildAt(28);
            _BT_BackToLogin = (GButton)GetChildAt(29);
            _mid = (GGroup)GetChildAt(30);
            _Right = (GGroup)GetChildAt(31);
            _FunctionPanel = (GGroup)GetChildAt(32);
            _Txt_EXP = (GTextField)GetChildAt(34);
            _Txt_Channel = (GTextField)GetChildAt(35);
            _Txt_Time = (GTextField)GetChildAt(36);
            _ProgressBar_EXP = (GProgressBar)GetChildAt(37);
            _Txt_Version = (GTextField)GetChildAt(38);
            _Bottom = (GGroup)GetChildAt(39);
            _t_ShowFunctionPanel = GetTransitionAt(0);
            _t_HideFunctionPanel = GetTransitionAt(1);
            OnCreate();

        }
    }
}