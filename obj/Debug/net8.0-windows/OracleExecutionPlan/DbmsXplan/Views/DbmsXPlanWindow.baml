   M S B A M L   `   `   `  ����  wsDB>    8wsDB.OracleExecutionPlan.DbmsXplan.Views.DbmsXPlanWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$   Oracle 실행계획 분석��5   	   $	��700��6    $
��1200��6-    � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   .+�5   
   `� $��0��6    `� 
Background
 �����.�5      c� .@�5      �� -   5         ��Name$ LoadFileButton��5      $��파일 로드��61    � ToolTip$  실행계획 파일 로드��5      �� 5      �� 5      -   5        $ PasteButton��5      $��붙여넣기��6.   $2 ,클립보드에서 실행계획 붙여넣기��5      �� 5      �� 5      -   5        $ FormatButton��5      $��	포맷팅��6/   $# 실행계획 포맷팅 적용��5      �� 5      �� 5      -   5        $ ClearButton��5      $��	지우기��6.   $! 현재 탭 내용 지우기��5      �� 5      �� 5      -   5        $ AnalyzePerformanceButton��5      $��성능 분석��6;   $  실행계획 성능 분석��5      5      5       �� 5#   
   -   5         ��Name$ PlanTabControl��5#      $��1��6-   $��5q�6:   .@�5$      �� -   5         ��Name$ FirstTab��5$   &   $��실행계획 1��6   .��5%      �  5&      �� 5'      $��* �6(   �� 5(      $
��Auto �6(   .+�51      �� -   5         ��Name$ PlanRichTextBox��51   "   $��0��52      $��15(�53      
�������
���   �$��Stretch=�56      $��Stretch=�57      $	 �� HorizontalScrollBarVisibility$
	 Auto=�58      "
 �� VerticalScrollBarVisibility$

 Auto=�59      $��Courier New)�5:       �� AcceptsReturn$
 True��5;       �� 
AcceptsTab$
 True��5<       �5@      /� 5A      -	   5         /�Name$ flowDocument��5A   +    /� 	PageWidth$ 20000��5B   !    /� PagePadding$ 10q�5C   !     /� IsOptimalParagraphEnabled$ False��5D   !    /� IsHyphenationEnabled$ False��5E   !   5G      5I      �� 5L      $��1��6   
�������
�������$��0,1,0,0q�5M      $��30��67   ._�5N      ��  �� Orientation$ 
Horizontal=�6%   $��Center=�6>   $	��5,0q�6Y   .+�5O      �� $��선택된 객체: ��6(   $
��Bold"�68   �� 5P      -
   5         ��Name$ SelectedObjectText��5P   (   $��없음��6D   
����  �6N   �� 5Q      $.��( | Ctrl+Click으로 객체 통계 보기��6(   $��20,0,0,0q�6H   
�������$��Italic$�5R   8   5S      5T      5U      5V      5W      5X      5Y      