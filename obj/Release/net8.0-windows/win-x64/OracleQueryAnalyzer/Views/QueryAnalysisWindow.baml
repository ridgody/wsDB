   M S B A M L   `   `   `  ����   wsDB8    2wsDB.OracleQueryAnalyzer.Views.QueryAnalysisWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  쿼리 분석��$	��600��$	��900�� � WindowStartupLocation$ CenterScreen=�.���  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+�c� $��0��$��5q�.@��� -   $��LoadQueryButton��$��쿼리 불러오기���� �� $��상태:��$��Center=�$��10,0,5,0q��� -   $��StatusTextBlock��$��쿼리를 입력하세요��$��Center=��� $��1��$��
SQL 쿼리��$��5q�.���� -   $��QueryTextBox�� �� AcceptsReturn$
 True�� �� 
AcceptsTab$
 True��" �� VerticalScrollBarVisibility$
 Auto=�$ �� HorizontalScrollBarVisibility$
 Auto=�$��Consolas)�$��12(��� $��2�� �� Orientation$ 
Horizontal=�$��Right=�$��5q�.+��� -   $��AnalyzeButton��$��실행계획 분석��${�False��$	��120��$��30��$��5q��� -   $��CloseButton��$��닫기��$��80��$��30��$��5q�