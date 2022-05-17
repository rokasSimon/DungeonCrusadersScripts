using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Constants
{
    [JsonObject]
    public class Inventory : INotifyPropertyChanged
    {
        private int _coins;
        private List<Item> _items;
        
        public int Coins
        {
            get => _coins;
            set
            {
                _coins = value;
                OnPropertyChanged(nameof(Coins));
            }
        }

        public List<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public void AddItem(Item itemClone)
        {
            itemClone.Quantity = 1;
            var itemInInv = Items.FirstOrDefault(item => item.Name.Equals(itemClone.Name));

            if (itemInInv is null)
            {
                Items.Add(itemClone);
                return;
            }

            itemInInv.Quantity++;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}