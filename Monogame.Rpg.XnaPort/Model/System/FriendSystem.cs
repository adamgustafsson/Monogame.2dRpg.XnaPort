using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    class FriendSystem
    {
        public List<Model.Friend> m_friends;

        public FriendSystem(ObjectLayer a_friendLayer)
        {
            m_friends = new List<Model.Friend>();
            LoadFriends(a_friendLayer);
        }
        private void LoadFriends(ObjectLayer a_friendLayer)
        {
            foreach (MapObject friend in a_friendLayer.MapObjects)
            {
                m_friends.Add(new Friend(friend, Convert.ToInt32(friend.Properties["ID"].AsInt32), Convert.ToBoolean(friend.Properties["CanInterract"].AsBoolean)));
            }
        }
    }
}
