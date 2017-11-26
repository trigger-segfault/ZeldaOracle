using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class RewardPropertyEditor : ComboBoxEditor {

		protected EditorControl EditorControl {
			get { return PropertyDescriptor.EditorControl; }
		}
		protected CustomPropertyDescriptor PropertyDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomPropertyDescriptor; }
		}

		protected override ComboBox CreateEditor() {
			var editor = new PropertyGridEditorComboBox();
			editor.IsTextSearchEnabled = true;
			editor.IsTextSearchCaseSensitive = true;
			return editor;
		}

		protected override IValueConverter CreateValueConverter() {
			return new RewardValueConverter(EditorControl.RewardManager, Editor);
		}

		protected override IEnumerable CreateItemsSource(PropertyItem item) {
			RewardManager rewardManager = EditorControl.RewardManager;
			FrameworkElement[] rewardItems = new FrameworkElement[rewardManager.RewardDictionary.Count + 1];

			int index = 0;
			rewardItems[index] = CreateListItem(null, "");
			index++;
			foreach (var rewardPair in rewardManager.RewardDictionary) {
				Canvas canvasSprite = null;
				Reward reward = rewardPair.Value;

				if (reward != null && reward.Animation != null) {
					Sprite sprite = reward.Animation.Frames[0].Sprite;
					canvasSprite = EditorResources.GetSprite(sprite);
				}

				rewardItems[index] = CreateListItem(canvasSprite, reward.ID);
				index++;
			}
			
			return rewardItems;
		}

		private FrameworkElement CreateListItem(Canvas canvasSprite, string name) {
			Grid grid = new Grid();
			grid.Tag = name;
			grid.Height = 20;
			TextSearch.SetText(grid, name);
			
			if (canvasSprite != null) {
				canvasSprite.Margin = new Thickness(2, 2, 0, 0);
				grid.Children.Add(canvasSprite);
			}

			TextBlock textBlock = new TextBlock();
			textBlock.Text = (name == "" ? "(none)" : name);
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			textBlock.Margin = new Thickness(22, 0, 0, 0);
			grid.Children.Add(textBlock);

			return grid;
		}
	}

	public class RewardValueConverter : IValueConverter {
		RewardManager rewardManager;
		ComboBox comboBox;
		public RewardValueConverter(RewardManager rewardManager, ComboBox comboBox) {
			this.rewardManager = rewardManager;
			this.comboBox = comboBox;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				foreach (object item in comboBox.Items) {
					string tag = (item as FrameworkElement).Tag as string;
					if (tag == (string)value)
						return item;
				}
				return comboBox.Items[0];
			}
			else if (value is FrameworkElement) {
				return (value as FrameworkElement).Tag as string;
			}
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				foreach (object item in comboBox.Items) {
					string tag = (item as FrameworkElement).Tag as string;
					if (tag == (string)value)
						return item;
				}
				return comboBox.Items[0];
			}
			else if (value is FrameworkElement) {
				return (value as FrameworkElement).Tag as string;
			}
			return null;
		}
	}
}
