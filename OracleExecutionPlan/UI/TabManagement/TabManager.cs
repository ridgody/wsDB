using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace wsDB.OracleExecutionPlan.UI.TabManagement
{
    public class TabManager
    {
        private TabControl tabControl;
        private int tabCounter = 1;

        public TabManager(TabControl tabControl)
        {
            this.tabControl = tabControl;
        }

        public RichTextBox GetCurrentRichTextBox()
        {
            // TabItem selectedTab = tabControl.SelectedItem as TabItem;
            // if (selectedTab?.Name == "FirstTab") // FirstTab이라고 가정
            // {
            //     // 첫 번째 탭의 RichTextBox 반환 로직
            //     return selectedTab.Content as RichTextBox ??
            //            FindRichTextBoxInContent(selectedTab.Content);
            // }
            // else
            // {
            //     Grid grid = selectedTab?.Content as Grid;
            //     ScrollViewer scrollViewer = grid?.Children.OfType<ScrollViewer>().FirstOrDefault();
            //     return scrollViewer?.Content as RichTextBox;
            // }

            TabItem selectedTab = tabControl.SelectedItem as TabItem;
            RichTextBox richTextBox = null;
            
            if (selectedTab?.Name == "FirstTab") // FirstTab이라고 가정
            {
                // 첫 번째 탭의 RichTextBox 반환 로직
                richTextBox = selectedTab.Content as RichTextBox ?? 
                            FindRichTextBoxInContent(selectedTab.Content);
            }
            else if (selectedTab != null)
            {
                Grid grid = selectedTab.Content as Grid;
                ScrollViewer scrollViewer = grid?.Children.OfType<ScrollViewer>().FirstOrDefault();
                richTextBox = scrollViewer?.Content as RichTextBox;
            }
            
            // null이면 첫 번째 탭의 RichTextBox를 반환
            if (richTextBox == null)
            {
                TabItem firstTab = tabControl.Items.OfType<TabItem>().FirstOrDefault();
                if (firstTab != null)
                {
                    if (firstTab.Name == "FirstTab")
                    {
                        richTextBox = firstTab.Content as RichTextBox ?? 
                                    FindRichTextBoxInContent(firstTab.Content);
                    }
                    else
                    {
                        Grid grid = firstTab.Content as Grid;
                        ScrollViewer scrollViewer = grid?.Children.OfType<ScrollViewer>().FirstOrDefault();
                        richTextBox = scrollViewer?.Content as RichTextBox;
                    }
                }
            }
            
            return richTextBox;
            
        }

        private RichTextBox FindRichTextBoxInContent(object content)
        {
            // 복잡한 컨텐츠 구조에서 RichTextBox 찾기
            if (content is RichTextBox rtb)
                return rtb;
            
            if (content is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    var result = FindRichTextBoxInContent(child);
                    if (result != null) return result;
                }
            }
            
            if (content is ScrollViewer sv)
                return FindRichTextBoxInContent(sv.Content);
                
            return null;
        }

        public void AddNewTab(Action<RichTextBox> setupRichTextBoxAction)
        {
            tabCounter++;
            TabItem newTab = new TabItem
            {
                Header = $"실행계획 {tabCounter}"
            };

            // 탭 컨텐츠 생성
            var content = CreateTabContent(setupRichTextBoxAction);
            newTab.Content = content;

            tabControl.Items.Add(newTab);
            tabControl.SelectedItem = newTab;
        }

        private Grid CreateTabContent(Action<RichTextBox> setupRichTextBoxAction)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            ScrollViewer scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            Grid.SetRow(scrollViewer, 0);

            RichTextBox newRichTextBox = new RichTextBox
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                IsReadOnly = true,
                Background = Brushes.White,
                Foreground = Brushes.Black
            };

            setupRichTextBoxAction?.Invoke(newRichTextBox);
            scrollViewer.Content = newRichTextBox;

            // 하단 상태바 생성
            Border border = CreateStatusBar();
            Grid.SetRow(border, 1);

            grid.Children.Add(scrollViewer);
            grid.Children.Add(border);

            return grid;
        }

        private Border CreateStatusBar()
        {
            Border border = new Border
            {
                Background = Brushes.LightGray,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 1, 0, 0),
                Height = 30
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            };

            stackPanel.Children.Add(new TextBlock { Text = "선택된 객체: ", FontWeight = FontWeights.Bold });
            stackPanel.Children.Add(new TextBlock { Text = "없음", Foreground = Brushes.Blue, Name = "SelectedObjectText" });
            stackPanel.Children.Add(new TextBlock
            {
                Text = " | Ctrl+Click으로 객체 통계 보기",
                Margin = new Thickness(20, 0, 0, 0),
                Foreground = Brushes.Gray,
                FontStyle = FontStyles.Italic
            });

            border.Child = stackPanel;
            return border;
        }
    }
}