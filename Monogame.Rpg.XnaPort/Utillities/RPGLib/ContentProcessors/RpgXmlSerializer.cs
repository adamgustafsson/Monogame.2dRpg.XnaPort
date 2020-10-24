using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LudosProduction.RpgLib
{
    public static class RpgXmlSerializer
    {
        public static List<Quest> LoadQuestsFromXml(string fullPath)
        {
            var quests = new List<Quest>();

            var doc = XDocument.Load(fullPath);
            IEnumerable<XElement> questElements = doc.Root.Elements();

            foreach (var e in questElements)
            {
                var quest = new Quest();
                quest.Id = Int32.Parse(e.Element("Id").Value);
                quest.QuestPickup = Int32.Parse(e.Element("QuestPickup").Value);
                quest.QuestTurnIn = Int32.Parse(e.Element("QuestTurnIn").Value);
                quest.PreMessage = e.Element("PreMessage").Value;
                quest.MidMessage = e.Element("MidMessage").Value;
                quest.EndMessage = e.Element("EndMessage").Value;

                IEnumerable<XElement> objectiveElements = e.Element("Objectives").Elements();
                var objectives = new List<Objective>();

                foreach (var oe in objectiveElements)
                {
                    var objective = new Objective();
                    objective.Obj = Int32.Parse(oe.Element("Obj").Value);
                    objective.ObjType = Int32.Parse(oe.Element("ObjType").Value);
                    objective.Amount = Int32.Parse(oe.Element("Amount").Value);
                    objective.Name = oe.Element("Name").Value;

                    objectives.Add(objective);
                }

                quest.Objectives = objectives;

                quests.Add(quest);
            }

            return quests;

        }

        public static List<Dialogue> LoadDialogsFromXml(string fullPath)
        {
            var dialogues = new List<Dialogue>();

            var doc = XDocument.Load(fullPath);
            IEnumerable<XElement> dialogElements = doc.Root.Elements();

            foreach (var d in dialogElements)
            {
                var dialogue = new Dialogue();
                dialogue.id = Int32.Parse(d.Element("id").Value);

                IEnumerable<XElement> messagesElements = d.Element("message").Elements();
                var messages = new List<Message>();

                foreach (var m in messagesElements)
                {
                    var message = new Message();
                    message.msg = m.Element("msg").Value;

                    IEnumerable<XElement> choiceElements = m.Element("choices").Elements();
                    message.choices = new List<string>();
                    foreach (var c in choiceElements)
                        message.choices.Add(c.Value);

                    messages.Add(message);
                }

                dialogue.message = messages;
                dialogues.Add(dialogue);
            }

            return dialogues;

        }

    }
}

