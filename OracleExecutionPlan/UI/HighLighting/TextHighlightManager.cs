using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace wsDB.OracleExecutionPlan.UI.Highlighting
{
    public class TextHighlightManager
    {
        private TextPointer lastHoveredStart;
        private TextPointer lastHoveredEnd;
        private Run currentHighlightedRun = null;
        private Brush originalBrush = null;

        public void HighlightWord(TextPointer start, TextPointer end)
        {
            if (start != null && end != null)
            {
                try
                {
                    TextRange range = new TextRange(start, end);
                    range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Color.FromArgb(100, 135, 206, 250)));
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkBlue);
                    
                    lastHoveredStart = start;
                    lastHoveredEnd = end;
                }
                catch (Exception)
                {
                    // TextPointer 관련 예외 무시
                }
            }
        }

        public void RemoveHighlight()
        {
            if (lastHoveredStart != null && lastHoveredEnd != null)
            {
                try
                {
                    TextRange range = new TextRange(lastHoveredStart, lastHoveredEnd);
                    range.ClearAllProperties();
                }
                catch (Exception)
                {
                    // TextPointer가 유효하지 않을 경우 무시
                }
            }
        }

        public (TextPointer start, TextPointer end) GetLastHoveredBounds()
        {
            return (lastHoveredStart, lastHoveredEnd);
        }
    }
}
