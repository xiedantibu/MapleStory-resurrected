﻿using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace ms.SkillTrees
{

    ///<summary> Base class for fsm nodes that are actually states</summary>
    [Color("ff6d53")]
    abstract public class STState : STNode, IState
    {

        public enum TransitionEvaluationMode
        {
            CheckContinuously,
            CheckAfterStateFinished,
            CheckManually
        }

        [SerializeField]
        private TransitionEvaluationMode _transitionEvaluation;

        private bool _hasInit;

        public override bool allowAsPrime => true;
        public override bool canSelfConnect => true;
        public override int maxInConnections => -1;
        public override int maxOutConnections => -1;

        public TransitionEvaluationMode transitionEvaluation {
            get { return _transitionEvaluation; }
            set { _transitionEvaluation = value; }
        }

        ///<summary>Returns all transitions of the state</summary>
        public STConnection[] GetTransitions() {
            var result = new STConnection[outConnections.Count];
            for ( var i = 0; i < outConnections.Count; i++ ) {
                result[i] = (STConnection)outConnections[i];
            }
            return result;
        }

        ///<summary>Declares that the state has finished</summary>
        public void Finish() { Finish(Status.Success); }
        public void Finish(bool inSuccess) { Finish(inSuccess ? Status.Success : Status.Failure); }
        public void Finish(Status status) { this.status = status; }

        ///----------------------------------------------------------------------------------------------
        public override void OnGraphPaused() { if ( status == Status.Running ) { OnPause(); } }

        ///----------------------------------------------------------------------------------------------

        //avoid connecting from same source
        protected override bool CanConnectFromSource(Node sourceNode) {
            if ( this.IsChildOf(sourceNode) ) {
                Logger.LogWarning("States are already connected together. Consider using multiple conditions on an existing transition instead", LogTag.EDITOR, this);
                return false;
            }
            return true;
        }

        //avoid connecting to same target
        protected override bool CanConnectToTarget(Node targetNode) {
            if ( this.IsParentOf(targetNode) ) {
                Logger.LogWarning("States are already connected together. Consider using multiple conditions on an existing transition instead", LogTag.EDITOR, this);
                return false;
            }
            return true;
        }

        //OnEnter...
        sealed protected override Status OnExecute(Component agent, IBlackboard bb) {

            if ( !_hasInit ) {
                _hasInit = true;
                OnInit();
            }

            if ( status == Status.Resting ) {
                status = Status.Running;

                for ( int i = 0; i < outConnections.Count; i++ ) {
                    ( (STConnection)outConnections[i] ).EnableCondition(agent, bb);
                }

                OnEnter();

            } else {

                var case1 = transitionEvaluation == TransitionEvaluationMode.CheckContinuously;
                var case2 = transitionEvaluation == TransitionEvaluationMode.CheckAfterStateFinished && status != Status.Running;
                if ( case1 || case2 ) {
                    CheckTransitions();
                }

                if ( status == Status.Running ) {
                    OnUpdate();
                }
            }

            return status;
        }

        ///<summary>Checks and performs possible transition. Returns true if a transition was performed.</summary>
        public bool CheckTransitions() {

            for ( var i = 0; i < outConnections.Count; i++ ) {

                var connection = (STConnection)outConnections[i];
                var condition = connection.condition;

                if ( !connection.isActive ) {
                    continue;
                }

                if ( ( condition != null && condition.Check(graphAgent, graphBlackboard) ) || ( condition == null && status != Status.Running ) ) {
                    FSM.EnterState((STState)connection.targetNode, connection.transitionCallMode);
                    connection.status = Status.Success; //editor vis
                    return true;
                }

                connection.status = Status.Failure; //editor vis
            }

            return false;
        }
        public bool ISEligible()
        {
            for (var i = 0; i < inConnections.Count; i++)
            {

                var connection = (STConnection)inConnections[i];
                FSM.previousState = (STState)connection.sourceNode;
                FSM.currentState = (STState)connection.targetNode;

                var condition = connection.condition;
                if (!connection.isActive)
                {
                    continue;
                }

                if (condition == null)
                {
                    continue;
                }
                condition.EnableRuntime();
                if ((condition != null && condition.Check(graphAgent, graphBlackboard)) || (condition == null && status != Status.Running))
                {
                    //FSM.EnterState((STState)connection.targetNode, connection.transitionCallMode);
                    //connection.status = Status.Success; //editor vis
                    continue;
                }
                connection.status = Status.Failure; //editor vis

                return false;
            }
            return true;
        }
        //OnExit...
        sealed protected override void OnReset() {
            for ( int i = 0; i < outConnections.Count; i++ ) {
                ( (STConnection)outConnections[i] ).DisableCondition();
            }

#if UNITY_EDITOR
            //Done for visualizing in editor
            for ( var i = 0; i < inConnections.Count; i++ ) {
                inConnections[i].status = Status.Resting;
            }
#endif

            OnExit();
        }


        //Converted
        virtual protected void OnInit() { }
        virtual protected void OnEnter() { }
        virtual protected void OnUpdate() { }
        virtual protected void OnExit() { }
        virtual protected void OnPause() { }
        //


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        //...
        protected override void OnNodeInspectorGUI() {
            ShowTransitionsInspector();
            DrawDefaultInspector();
        }

        protected override void OnNodeExternalGUI() {
            var peek = FSM.PeekStack();
            if ( peek != null && FSM.currentState == this ) {
                UnityEditor.Handles.color = UnityEngine.Color.grey;
                UnityEditor.Handles.DrawAAPolyLine(rect.center, peek.rect.center);
                UnityEditor.Handles.color = UnityEngine.Color.white;
            }
        }

        //...
        protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu) {
            if ( Application.isPlaying ) {
                menu.AddItem(new GUIContent("Enter State"), false, () => { FSM.EnterState(this, SkillTree.TransitionCallMode.Normal); });
            } else { menu.AddDisabledItem(new GUIContent("Enter State")); }
            menu.AddItem(new GUIContent("Breakpoint"), isBreakpoint, () => { isBreakpoint = !isBreakpoint; });
            menu.AddItem(new GUIContent("Evaluate"), false, () => { FSM.Evaluate(); });
            return menu;
        }

        //...
        protected void ShowTransitionsInspector() {

            EditorUtils.CoolLabel("Transitions");

            if ( outConnections.Count == 0 ) {
                UnityEditor.EditorGUILayout.HelpBox("No Transition", UnityEditor.MessageType.None);
            }

            var onFinishExists = false;
            EditorUtils.ReorderableList(outConnections, (i, picked) =>
            {
                var connection = (STConnection)outConnections[i];
                GUILayout.BeginHorizontal("box");
                if ( connection.condition != null ) {
                    GUILayout.Label(connection.condition.summaryInfo, GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
                } else {
                    GUILayout.Label("OnFinish" + ( onFinishExists ? " (exists)" : string.Empty ), GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
                    onFinishExists = true;
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("► '" + connection.targetNode.name + "'");
                GUILayout.EndHorizontal();
            });

            transitionEvaluation = (TransitionEvaluationMode)UnityEditor.EditorGUILayout.EnumPopup(transitionEvaluation);
            EditorUtils.BoldSeparator();
        }

#endif

        ///----------------------------------------------------------------------------------------------



    }
}