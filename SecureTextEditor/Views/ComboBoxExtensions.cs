using System;
using System.Collections.Generic;
using System.Linq;
using Medja.Controls;

namespace SecureTextEditor.Views
{
    static class ComboBoxExtensions
    {
        public static void AddRange(this ComboBox comboBox, IEnumerable<string> items)
        {
            foreach (var title in items)
                comboBox.Add(title);
        }

        public static void AddRange(this ComboBox comboBox, params string[] items)
        {
            AddRange(comboBox, (IEnumerable<string>)items);
        }

        public static void AddRange<T>(this ComboBox comboBox, params T[] items)
        where T : Enum
        {
            AddRange(comboBox, (IEnumerable<T>)items);
        }

        public static void AddRange<T>(this ComboBox comboBox, IEnumerable<T> items)
        where T : Enum
        {
            AddRange(comboBox, items.Select(p => Enum.GetName(typeof(T), p)));
        }

        public static void Add<T>(this ComboBox comboBox, T item)
        where T : Enum
        {
            comboBox.Add(Enum.GetName(typeof(T), item));
        }

        public static void SelectFirstItem(this ComboBox comboBox)
        {
            if (comboBox.ItemsPanel.Children.Count == 0)
                return;

            comboBox.SelectedItem = comboBox.ItemsPanel.Children[0];
        }
    }
}