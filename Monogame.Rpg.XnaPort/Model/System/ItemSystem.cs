using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class ItemSystem
    {
        public List<Item> m_items;
        
        public ItemSystem(ObjectLayer a_itemLayer)
        {
            m_items = new List<Item>();
            LoadArmors(a_itemLayer);
        }

        //Lägger till alla armor objekt på kartan i en lista.
        public void LoadArmors(ObjectLayer a_itemLayer)
        {
            int itemID = 0;
            foreach (MapObject item in a_itemLayer.MapObjects)
            {
                //TODO: Fördela roller via Objektnamn tex W, M
                itemID++;
                m_items.Add(new Armor(item, itemID));
            }
        }

        internal void UpdateItemSystem(float a_elapsedTime, Model.Player a_player)
        {
            if (a_player.IsAlive())
            {
                //Kan göra 1 item target om man får någonannan lösning på att kolla vilken metod som ska köras i itemsystem.
                //Kollar att spelaren har ett itemtarget, isf läggs det till i backpacken.
                if (a_player.ItemTarget != null && a_player.ItemTarget.WasLooted)
                {
                    AddToBackPack(a_player, a_player.ItemTarget);
                }
                //Kollar om spealren har ett backpack target, isf använder / equippar spelaren det.
                if (a_player.BackpackTarget != null)
                {
                    EquipOrUse(a_player);
                }
                //Kollar om spelaren har ett charpanel Target, isf läggs det till i backpacken igen.
                if (a_player.CharPanelTarget != null)
                {
                    UnEquip(a_player, a_player.CharPanelTarget);
                } 
            }
        }

        #region Backpack ADD / REMOVE

        //Metod som lägger till ett item i spelarens backpack lista och tar bort spelarens item target.
        public void AddToBackPack(Player a_player, Item a_item)
        {
            if(a_item.GetType() == Model.GameModel.ARMOR)
            {
                //Skapar och lägger till nytt armor objekt i backpacken.
                a_player.BackPack.BackpackItems.Add(a_item as Model.Armor);
                m_items.Remove(a_item);
            }
            else if (a_item.GetType() == Model.GameModel.QUEST_ITEM)
            {
                //Skapar och lägger till nytt armor objekt i backpacken.
                a_player.BackPack.BackpackItems.Add(a_item as Model.QuestItem);
            }
            
            a_player.ItemTarget = null;
        }

        //Metod som tar bort ett item från spelarens backpack.
        public void RemoveFromBackpack(Player a_player, Item a_item)
        {
            a_player.BackPack.BackpackItems.Remove(a_item);
        }

        #endregion

        public void EquipOrUse(Player a_player)
        { 
            if(a_player.BackpackTarget.GetType() == Model.GameModel.ARMOR)
            {
                EquipArmor(a_player, a_player.BackpackTarget as Model.Armor);
            }
        }

        //Sätter spelarens armor efter att han hittat en bit på kartan eller lootat från monster.
        public void EquipArmor(Player a_player, Armor a_armor)
        {
            //Kontrollerar att spelaren inte redan har ett item för slotten som han försöker plocka upp.
            if (a_player.BackpackTarget.GetType() == Model.GameModel.ARMOR)
            {
                if (!PlayerHasArmor(a_player, a_player.BackpackTarget as Armor))
                {
                    //Ökar spelarens armor med itemets armor, tar bort itemet från backpacken och sätter backpack target till null.
                    a_player.Armor += a_armor.ArmorValue;
                    RemoveFromBackpack(a_player, a_armor);
                    a_player.BackpackTarget = null;
                    a_player.CharPanel.EquipedItems.Add(a_armor as Armor);
                } 
            }
        }

        public void UnEquip(Player a_player, Item a_charPanelTarget)
        {
            a_player.CharPanel.EquipedItems.Remove(a_charPanelTarget);
            a_player.BackPack.BackpackItems.Add(a_charPanelTarget);

            if(a_charPanelTarget.GetType() == Model.GameModel.ARMOR)
            {
                Armor armor = a_charPanelTarget as Armor;
                a_player.Armor -= armor.ArmorValue;

                if(armor.Type == Armor.HEAD_ARMOR)
                {
                    a_player.HasHelm = false;
                }
            }

            a_player.CharPanelTarget = null;
        }

        //Kontrollerar så att spelaren inte redan har en armordel equippad.
        public bool PlayerHasArmor(Player a_player, Armor a_armor)
        {
            int armorType = a_armor.Type;

            switch(armorType)
            {
                case Model.Armor.HEAD_ARMOR:
                    //Om spelaren inte hade en hjälm.
                    if (!a_player.HasHelm)
                    {
                        //Så har han det nu...
                        a_player.HasHelm = true;
                        return false;
                    }
                    return true;

                default:
                    return true;
            }
        }
    }
}
