using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace ms.SkillTrees
{

    [Description("Execute a number of Actions repeatedly and in parallel to any other FSM state while the FSM is running. Conditions are optional. This is not a state.")]
    [Color("ff64cb")]
    [ParadoxNotion.Design.Icon("Repeat")]
    [Name("On FSM Update")]
    public class OnSTUpdate : STNode, IUpdatable
    {

        [SerializeField] private ConditionList _conditionList;
        [SerializeField] private ActionList _actionList;

        public override string name => base.name.ToUpper();
        public override int maxInConnections => 0;
        public override int maxOutConnections => 0;
        public override bool allowAsPrime => false;

        ///----------------------------------------------------------------------------------------------

        public override void OnValidate(Graph assignedGraph) {
            if ( _conditionList == null ) {
                _conditionList = (ConditionList)Task.Create(typeof(ConditionList), assignedGraph);
                _conditionList.checkMode = ConditionList.ConditionsCheckMode.AllTrueRequired;
            }
            if ( _actionList == null ) {
                _actionList = (ActionList)Task.Create(typeof(ActionList), assignedGraph);
                _actionList.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
            }
        }

        public override void OnGraphStarted() {
            _conditionList.Enable(graphAgent, graphBlackboard);
        }

        public override void OnGraphStoped() {
            _conditionList.Disable();
            _actionList.EndAction(null);
        }

        void IUpdatable.Update() {
            if ( _conditionList.Check(graphAgent, graphBlackboard) ) {
                status = _actionList.Execute(graphAgent, graphBlackboard);
            } else {
                _actionList.EndAction(null);
                status = Status.Failure;
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            if ( _conditionList.conditions.Count > 0 ) {
                GUILayout.BeginVertical(Styles.roundedBox);
                GUILayout.Label(_conditionList.summaryInfo);
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label(_actionList.summaryInfo);
            GUILayout.EndVertical();

            base.OnNodeGUI();
        }

        protected override void OnNodeInspectorGUI() {
            EditorUtils.CoolLabel("Conditions (optional)");
            _conditionList.ShowListGUI();
            _conditionList.ShowNestedConditionsGUI();
            EditorUtils.BoldSeparator();
            EditorUtils.CoolLabel("Actions");
            _actionList.ShowListGUI();
            _actionList.ShowNestedActionsGUI();
        }

#endif
    }
}