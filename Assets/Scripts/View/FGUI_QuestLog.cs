/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using System;
using FairyGUI;
using FairyGUI.Utils;
using ms;

namespace ms_Unity
{
	public partial class FGUI_QuestLog
	{
		UIQuestLog uIQuestLog;
		void OnCreate ()
		{
			_GList_QuestInfo_Available.itemRenderer = ItemRenderer_started;
			_GList_QuestInfo_in_progress.itemRenderer = ItemRenderer_in_progress;
			_GList_QuestInfo_completed.itemRenderer = ItemRenderer_completed;

			_GList_QuestInfo_Available.onClickItem.Add (OnClick_QuestInfo_started);
			_GList_QuestInfo_in_progress.onClickItem.Add (OnClick_QuestInfo_in_progress);
			_GList_QuestInfo_completed.onClickItem.Add (QuestInfo_QuestInfo_completed);

			_Btn_ForfeitQuest.onClick.Add (onClick_Btn_ForfeitQuest);

			_c_QuestState.onChanged.Add (onChanged_c_QuestState);
		}

		private void onChanged_c_QuestState (EventContext context)
		{
			_Txt_Desc.text = "";
			_GList_QuestInfo_Available.selectedIndex = -1;
			_GList_QuestInfo_in_progress.selectedIndex = -1;
			_GList_QuestInfo_completed.selectedIndex = -1;
		}

		private void onClick_Btn_ForfeitQuest (EventContext context)
		{
			if (_GList_QuestInfo_in_progress.selectedIndex != -1)
			{
				new ForfeitQuestPacket(currentQuestId).dispatch ();
			}
		}

		private void ItemRenderer_started (int index, GObject item)
		{
			var listPool = UnityEngine.Rendering.ListPool<short>.Get ();
			listPool.AddRange (quest.AvailableQuests.Keys);
			var questId = listPool[index];
			var questInfo = questLog.GetQuestInfo (questId);
			var ListItem_QuestLog = item as FGUI_ListItem_QuestLog;
			ListItem_QuestLog._Txt_Name.text = $"{questInfo.Id} {questInfo.DisplayName}";
			ListItem_QuestLog.data = questId;
			UnityEngine.Rendering.ListPool<short>.Release (listPool);
		}
		private void ItemRenderer_in_progress (int index, GObject item)
		{
			var listPool = UnityEngine.Rendering.ListPool<short>.Get ();
			listPool.AddRange (questLog.In_progress.Keys);
			var questId = listPool[index];
			var questInfo = questLog.GetQuestInfo (questId);
			var ListItem_QuestLog = item as FGUI_ListItem_QuestLog;
			ListItem_QuestLog._Txt_Name.text = $"{questInfo.Id} {questInfo.Name}";
			ListItem_QuestLog.data = questId;
			UnityEngine.Rendering.ListPool<short>.Release (listPool);

			_Btn_ForfeitQuest.visible = true;
		}
		private void ItemRenderer_completed (int index, GObject item)
		{
			var listPool = UnityEngine.Rendering.ListPool<short>.Get ();
			listPool.AddRange (questLog.Completed.Keys);
			var questId = listPool[index];
			var questInfo = questLog.GetQuestInfo (questId);
			var ListItem_QuestLog = item as FGUI_ListItem_QuestLog;
			ListItem_QuestLog._Txt_Name.text = $"{questInfo.Id} {questInfo.Name}";
			ListItem_QuestLog.data = questId;
			UnityEngine.Rendering.ListPool<short>.Release (listPool);
		}

		short currentQuestId = 0;
		private void OnClick_QuestInfo_started (EventContext context)
		{
			var ListItem_QuestLog = context.data as FGUI_ListItem_QuestLog;
			var questId = (short)ListItem_QuestLog.data;
			currentQuestId = questId;
			var questInfo = questLog.GetQuestInfo (questId);
			_Txt_Desc.text = questInfo.Info_started ?? $"quest {questId} Info_started not exist";

		}

		private void OnClick_QuestInfo_in_progress (EventContext context)
		{
			var ListItem_QuestLog = context.data as FGUI_ListItem_QuestLog;
			var questId = (short)ListItem_QuestLog.data;
			currentQuestId = questId;
			var questInfo = questLog.GetQuestInfo (questId);
			_Txt_Desc.text = questInfo.Info_in_progress;

		}

		private void QuestInfo_QuestInfo_completed (EventContext context)
		{
			var ListItem_QuestLog = context.data as FGUI_ListItem_QuestLog;
			var questId = (short)ListItem_QuestLog.data;
			currentQuestId = questId;
			var questInfo = questLog.GetQuestInfo (questId);
			_Txt_Desc.text = questInfo.Info_completed;
		}

		public void OnVisiblityChanged (bool isVisible)
		{
			if (isVisible)
			{
				UpdateQuest ();
			}
		}

		Quest quest => ms.Stage.Instance.get_player ().get_quest ();
		QuestLog questLog => ms.Stage.Instance.get_player ().get_questlog ();
		public void UpdateQuest ()
		{
			_Btn_ForfeitQuest.visible = false;
			_Txt_Desc.text = "";

			quest.GetAvailable_Quest (true);

			_GList_QuestInfo_Available.numItems = quest.AvailableQuests.Count;
			_GList_QuestInfo_in_progress.numItems = questLog.In_progress.Count;
			_GList_QuestInfo_completed.numItems = questLog.Completed.Count;

		}
	}
}