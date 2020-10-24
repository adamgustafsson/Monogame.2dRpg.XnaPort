using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class Friend : Unit
    {
        //Typ ID
        public const int OLD_MAN = 0;
        public const int CITY_GUARD = 1;
        public const int FEMALE_CITIZEN = 2;

        private bool m_canInterract;

        public Friend(MapObject a_thisUnit, int a_friendId, bool a_canInterract)
        {
            this.UnitId = a_friendId;
            this.ThisUnit = a_thisUnit;

            this.TotalHp = 10;
            this.CurrentHp = this.TotalHp;

            m_canInterract = a_canInterract;

            this.ThisUnit.Bounds.Width = 64;
            this.ThisUnit.Bounds.Height = 64;

            if (a_thisUnit.Properties["Type"].AsInt32 == OLD_MAN)
            {
                this.Type = OLD_MAN;
                this.UnitState = Unit.FACING_CAMERA;
            }
            else if (a_thisUnit.Properties["Type"].AsInt32 == CITY_GUARD)
            {
                this.Type = CITY_GUARD;
                this.UnitState = Unit.FACING_LEFT;
            }
            else if (a_thisUnit.Properties["Type"].AsInt32 == FEMALE_CITIZEN)
                this.Type = FEMALE_CITIZEN;
        }

        public bool CanInterract
        {
            get { return m_canInterract; }
            set { m_canInterract = value; }
        }

    }
}
