   M S B A M L   `   `   `  ����   wsDBB    <wsDB.OracleExecutionPlanAnalyzer.Views.ExecutionPlanAnalyzer   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$   Oracle 실행계획 분석��$	��700��$
��1200�� � WindowStartupLocation$ CenterScreen=�.���  � RowDefinitions �� $
��Auto ��� $��* �.+�`� $��0�� `� 
Background
 �����.�c� .@��� -    ��Name$ LoadFileButton��$��파일 로드�� � ToolTip$  실행계획 파일 로드���� �� -   $ PasteButton��$��붙여넣기��$2 ,클립보드에서 실행계획 붙여넣기���� �� -   $ FormatButton��$��	포맷팅��$# 실행계획 포맷팅 적용���� �� -   $ ClearButton��$��	지우기��$! 현재 탭 내용 지우기���� -    ��Name$ PlanTabControl��$��1��$��5q�.@��� -    ��Name$ FirstTab��$��실행계획 1��.���  �� $��* ��� $
��Auto �.+��� -    ��Name$ PlanRichTextBox��$��0��$��15(�
�������
���   �$��Stretch=�$��Stretch=�$	 �� HorizontalScrollBarVisibility$
	 Auto=�"
 �� VerticalScrollBarVisibility$

 Auto=�$��Courier New)� �� AcceptsReturn$
 True�� �� 
AcceptsTab$
 True�� �/� -    /�Name$ flowDocument�� /� 	PageWidth$ 20000�� /� PagePadding$ 10q�  /� IsOptimalParagraphEnabled$ False�� /� IsHyphenationEnabled$ False���� $��1��
�������
�������$��0,1,0,0q�$��30��._���  �� Orientation$ 
Horizontal=�$��Center=�$	��5,0q�.+��� $��선택된 객체: ��$
��Bold"��� -	    ��Name$ SelectedObjectText��$��없음��
����  ��� $.��( | Ctrl+Click으로 객체 통계 보기��$��20,0,0,0q�
�������$��Italic$�