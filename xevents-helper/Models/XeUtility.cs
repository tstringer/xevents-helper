﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xevents_helper.Models
{
    public class XeUtility
    {
        public string GetCreateDdl(Session session)
        {
            string sessionDefinition = GetCreateEventSessionClause(session);

            sessionDefinition += ";\r\nGO";

            return sessionDefinition;
        }

        private string GetCreateEventSessionClause(Session session)
        {
            string createEventSessionDdl;

            createEventSessionDdl = string.Format("CREATE EVENT SESSION {0}\r\nON SERVER\r\n", QuoteName(session.Name));

            if (session.Events != null)
                foreach (Event xeEvent in session.Events)
                    createEventSessionDdl += GetAddEventClause(xeEvent);
            if (session.Targets != null)
                foreach (Target target in session.Targets)
                    createEventSessionDdl += GetAddTargetClause(target);

            return createEventSessionDdl;
        }
        private string GetStartEventSessionClause(Session session)
        {
            return
                string.Format("ALTER EVENT SESSION {0}\r\nON SERVER\r\nSTATE = START;\r\nGO", QuoteName(session.Name));
        }
        private string GetStopEventSessionClause(Session session)
        {
            return
                string.Format("ALTER EVENT SESSION {0}\r\nON SERVER\r\nSTATE = STOP;\r\nGO", QuoteName(session.Name));
        }
        private string GetDropEventSessionClause(Session session)
        {
            return
                string.Format("DROP EVENT SESSION {0}\r\nON SERVER;\r\nGO", QuoteName(session.Name));
        }

        private string GetAddEventClause(Event xeEvent)
        {
            string addEventClause;

            addEventClause = string.Format("ADD EVENT {0}.{1}", xeEvent.PackageName, xeEvent.Name);

            // parenthesis for the event would only be added if there are 
            // either actions and/or predicates to specify for the event 
            //
            if (xeEvent.Actions != null || xeEvent.Predicates != null)
            { 
                addEventClause += "\r\n(\r\n";
                addEventClause += GetEventActionClause(xeEvent.Actions);
                addEventClause += GetEventWhereClause(xeEvent.Predicates);
                addEventClause += "\r\n)";
            }
            
            return addEventClause;
        }
        private string GetEventActionClause(IEnumerable<Action> actions)
        {
            if (actions == null || actions.Count() == 0)
                return "";

            string actionClause = "ACTION\r\n(";
            string newAction;

            bool isFirst = true;
            foreach (Action action in actions)
            {
                newAction = string.Format("\r\n{0}.{1}", action.PackageName, action.Name);

                if (isFirst)
                    isFirst = false;
                else
                    newAction = "," + newAction;

                actionClause += newAction;
            }

            actionClause += "\r\n)";

            return actionClause;
        }
        private string GetEventWhereClause(IEnumerable<Predicate> predicates)
        {
            if (predicates == null || predicates.Count() == 0)
                return "";

            string whereClause = "WHERE\r\n(";
            string newWhere;

            bool isFirst = true;
            foreach (Predicate predicate in predicates)
            {
                newWhere = string.Format(
                    "\r\n{0} = {1}",
                    GetEventDataFullName(predicate.EventData),
                    FormatEventDataComparisonData(predicate));

                if (isFirst)
                    isFirst = false;
                else
                {
                    // append the conditional operator, as as this point 
                    // we are no longer on the first element and we need 
                    // to handle conditional AND or OR
                    //
                    string conditionalOperatorString;
                    if (predicate.ConditionalOperator == ConditionalOperator.And)
                        conditionalOperatorString = "AND";
                    else if (predicate.ConditionalOperator == ConditionalOperator.Or)
                        conditionalOperatorString = "OR";
                    else
                        throw new NotImplementedException("Only AND and OR have been implemented for predicates");

                    newWhere = string.Format("{0} {1}", conditionalOperatorString, newWhere);
                }

                whereClause += newWhere;
            }

            return whereClause;
        }
        private string GetEventDataFullName(IEventData eventData)
        {
            // if the event data is an action then we need to show this in 
            // the form of package_name.action_name, otherwise it must be 
            // an event field in which case the field name is sufficient
            //
            if (eventData is IAction)
                return string.Format("{0}.{1}", ((IAction)eventData).PackageName, eventData.Name);
            else
                return eventData.Name;
        }
        private string FormatEventDataComparisonData(Predicate predicate)
        {
            if (predicate == null)
                return "";

            return 
                predicate.EventData.DataType == XeDataType.Character ? 
                string.Format("N'{0}'", predicate.ComparisonValue) :
                predicate.ComparisonValue.ToString();
        }

        private string GetAddTargetClause(Target target)
        {
            if (target == null)
                return "";

            string addTargetClause;

            addTargetClause = string.Format("ADD TARGET {0}.{1}", QuoteName(target.PackageName), QuoteName(target.Name));
            addTargetClause += "\r\n(\r\n";
            addTargetClause += GetTargetOptionsClause(target.Settings);
            addTargetClause += "\r\n)";

            return addTargetClause;
        }
        private string GetTargetOptionsClause(IEnumerable<TargetSetting> targetSettings)
        {
            if (targetSettings == null)
                return "";

            string targetSettingsClause;

            targetSettingsClause = "SET";

            bool isFirst = true;
            string newSettingStatement;
            foreach (TargetSetting setting in targetSettings)
            {
                newSettingStatement = string.Format("\r\n{0} = {1}", setting.Parameter.Name, FormatSettingData(setting));

                if (isFirst)
                    isFirst = false;
                else
                    newSettingStatement = "," + newSettingStatement;

                targetSettingsClause += newSettingStatement;
            }

            return targetSettingsClause;
        }
        private string FormatSettingData(TargetSetting targetSetting)
        {
            if (targetSetting == null)
                return "";

            return
                targetSetting.Parameter.DataType == XeDataType.Character ?
                string.Format("N'{0}'", targetSetting.Setting.ToString()) :
                targetSetting.Setting.ToString();
        }

        private string QuoteName(string name)
        {
            return string.Format("[{0}]", name);
        }
    }
}