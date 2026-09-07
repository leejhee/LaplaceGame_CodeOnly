using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Client
{
    public abstract class UIParameter
    {
        
    }

    public class SynergyUIParameter : UIParameter
    {
        public Sprite SynergySprite;
    }

    public class ItemUIParameter : UIParameter
    {
        public Item Item;

        public ItemUIParameter(Item item)
        {
            this.Item = item;
        }
    }

}