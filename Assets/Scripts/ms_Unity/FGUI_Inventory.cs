/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ms_Unity
{
    public partial class FGUI_Inventory : GComponent
    {
        public Controller _c_PanelType;
        public GButton _Btn_Home;
        public FGUI_ItemInventory _ItemInventory;
        public const string URL = "ui://4916gthqfza1m3b";

        public static FGUI_Inventory CreateInstance()
        {
            return (FGUI_Inventory)UIPackage.CreateObject("ms_Unity", "Inventory");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            _c_PanelType = GetControllerAt(0);
            _Btn_Home = (GButton)GetChildAt(6);
            _ItemInventory = (FGUI_ItemInventory)GetChildAt(7);
            OnCreate();

        }
    }
}