using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class QuestItem : Item
    {
        public const int ENEMY_HEAD = 1;

        public QuestItem(int a_type)
        {
            this.Type = a_type;
            this.WasLooted = false;
            this.CanAddToQuest = true;
            this.ThisItem = new MapObject();
            this.ThisItem.Bounds.Width = 48;
            this.ThisItem.Bounds.Height = 48;
        }
    }
}
