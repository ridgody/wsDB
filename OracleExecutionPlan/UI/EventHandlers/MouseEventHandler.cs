using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using wsDB.OracleExecutionPlan.Core.Text;
using wsDB.OracleExecutionPlan.UI.Highlighting;
using wsDB.OracleExecutionPlan.UI.Popups;
using wsDB.OracleExecutionPlan.Core.Extensions;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace wsDB.OracleExecutionPlan.UI.EventHandlers
{
    public abstract class TextSelectEventHandler
    {
        protected TextHighlightManager highlightManager{ get; init; }
        protected TextProcessor textProcessor{ get; init; }

        protected TextSelectEventHandler(TextHighlightManager highlightManager, TextProcessor textProcessor)
        {
            this.highlightManager = highlightManager;
            this.textProcessor = textProcessor;
        }

        protected Run FindRunAtPosition(TextPointer position)
        {
            if (position.Parent is Run run)
                return run;

            TextPointer forward = position.GetNextContextPosition(LogicalDirection.Forward);
            if (forward?.Parent is Run forwardRun)
                return forwardRun;

            TextPointer backward = position.GetNextContextPosition(LogicalDirection.Backward);
            if (backward?.Parent is Run backwardRun)
                return backwardRun;

            return null;
        }
    }


    public class MouseEventHandler : TextSelectEventHandler
    {
        // private TextHighlightManager highlightManager;
        private PredicatePopupManager popupManager;
        // private TextProcessor textProcessor;
        private bool isCtrlPressed = false;

        public MouseEventHandler(TextHighlightManager highlightManager,
                               PredicatePopupManager popupManager,
                               TextProcessor textProcessor) : base(highlightManager, textProcessor) 
        {  
            this.popupManager = popupManager; 
        }

        public void HandleMouseMove(object sender, MouseEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            TextPointer pointer = rtb.GetPositionFromPoint(e.GetPosition(rtb), true);

            if (pointer != null)
            {
                // Run 요소 찾기
                Run run = FindRunAtPosition(pointer);
                if (run?.Tag != null)
                {
                    string tag = run.Tag.ToString();

                    // ID 태그인 경우 Predicate 정보 표시
                    if (tag.StartsWith("ID:"))
                    {
                        string id = tag.Substring(3);


                        // 마우스 위치를 화면 좌표로 변환
                        Point mousePosition = e.GetPosition(rtb);
                        Point screenPosition = rtb.PointToScreen(mousePosition);

                        popupManager.ShowPredicatePopup(id, rtb, screenPosition);
                        rtb.Cursor = Cursors.Help;
                        return;
                    }
                }
            }

            // Predicate 팝업 숨기기
            popupManager.HidePopup();

            if (isCtrlPressed)
            {
                HandleCtrlMouseMove(rtb, pointer);
            }
            else
            {
                highlightManager.RemoveHighlight();
                rtb.Cursor = Cursors.IBeam;
            }
        }

        private void HandleCtrlMouseMove(RichTextBox rtb, TextPointer pointer)
        {
            if (pointer != null)
            {
                var wordBounds = TextPointerExtensions.GetWordBounds(pointer);
                var lastBounds = highlightManager.GetLastHoveredBounds();

                if (wordBounds.start != null && wordBounds.end != null)
                {
                    if (!textProcessor.AreTextPointersEqual(wordBounds.start, lastBounds.start) ||
                        !textProcessor.AreTextPointersEqual(wordBounds.end, lastBounds.end))
                    {
                        highlightManager.RemoveHighlight();
                        highlightManager.HighlightWord(wordBounds.start, wordBounds.end);
                    }
                }
                else
                {
                    highlightManager.RemoveHighlight();
                }
            }
        }

        // private Run FindRunAtPosition(TextPointer position)
        // {
        //     if (position.Parent is Run run)
        //         return run;

        //     TextPointer forward = position.GetNextContextPosition(LogicalDirection.Forward);
        //     if (forward?.Parent is Run forwardRun)
        //         return forwardRun;

        //     TextPointer backward = position.GetNextContextPosition(LogicalDirection.Backward);
        //     if (backward?.Parent is Run backwardRun)
        //         return backwardRun;

        //     return null;
        // }

        public void HandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isCtrlPressed)
            {
                RichTextBox rtb = sender as RichTextBox;
                // TextPointer pointer = rtb.GetPositionFromPoint(e.GetPosition(rtb), true);
                
                WordSelectionHandler.HandleMouseWordSelection(rtb, e.GetPosition(rtb));

                // if (pointer != null)
                // {
                //     var wordBounds = TextPointerExtensions.GetWordBounds(pointer);

                //     if (wordBounds.start != null && wordBounds.end != null)
                //     {
                //         rtb.Selection.Select(wordBounds.start, wordBounds.end);
                //         string selectedWord = rtb.Selection.Text.Trim();

                //         if (!string.IsNullOrEmpty(selectedWord))
                //         {
                //             Clipboard.SetText(selectedWord);
                //         }
                //     }
                // }
            }
        }

        public void SetCtrlPressed(bool pressed)
        {
            isCtrlPressed = pressed;
            if (!pressed)
            {
                highlightManager.RemoveHighlight();
            }
        }
    }
}