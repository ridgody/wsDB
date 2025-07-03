using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using wsDB.OracleExecutionPlan.Core.Parsing;

namespace wsDB.OracleExecutionPlan.UI.Popups
{
    public class PredicatePopupManager
    {
        private Popup predicatePopup;
        private TextBlock predicateTextBlock;
        private DbmsXPlanParser planParser;

        public PredicatePopupManager(DbmsXPlanParser parser)
        {
            planParser = parser;
            InitializePredicatePopup();
        }

        private void InitializePredicatePopup()
        {
            predicatePopup = new Popup
            {
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                Placement = PlacementMode.Mouse
            };

            Border border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(240, 255, 255, 220)),
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8),
                MaxWidth = 600
            };

            predicateTextBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = predicateTextBlock;
            predicatePopup.Child = border;
        }

        // public void ShowPredicatePopup(string id, Point position)
        // {
        //     string predicateInfo = planParser.GetPredicateInfo(id);

        //     if (!string.IsNullOrEmpty(predicateInfo))
        //     {
        //         predicateTextBlock.Text = $"ID {id} Predicate:\n{predicateInfo}";
        //         predicatePopup.HorizontalOffset = position.X;
        //         predicatePopup.VerticalOffset = position.Y - 10;
        //         predicatePopup.IsOpen = true;
        //     }

        // }

        public void ShowPredicatePopup(string id, RichTextBox rtb, Point relativePosition)
        {
            string predicateInfo = planParser.GetPredicateInfo(id);

            if (!string.IsNullOrEmpty(predicateInfo))
            {
                predicateTextBlock.Text = $"ID {id} Predicate:\n{predicateInfo}";
                
                // Popup의 PlacementTarget과 Placement 속성 사용
                predicatePopup.PlacementTarget = rtb;
                predicatePopup.Placement = PlacementMode.MousePoint;
                predicatePopup.HorizontalOffset = 10;
                predicatePopup.VerticalOffset = -10;
                predicatePopup.IsOpen = true;
            }
        }

        public void HidePopup()
        {
            predicatePopup.IsOpen = false;
        }
    }
}