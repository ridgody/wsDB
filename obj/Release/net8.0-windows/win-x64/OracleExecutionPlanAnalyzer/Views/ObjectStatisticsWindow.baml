   M S B A M L   `   `   `  ����   wsDBC    =wsDB.OracleExecutionPlanAnalyzer.Views.ObjectStatisticsWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  객체 통계 정보��$	��500��$	��700�� � WindowStartupLocation$ CenterOwner=�.���  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��
����ح� �� Padding$ 10q�$��0,0,0,5q�._��� .+��� -    ��Name$ ObjectTypeText��$
��Bold"�$��14(��� -   $ ObjectNameText��$��12(�$��0,2,0,0q��� -    ��Name$ StatisticsTabControl��$��1��$��5q�.@��� $��기본 정보��.���� .��&   System.Windows.Controls.DataGrid  -     Name$ BasicInfoDataGrid��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�	   GridLinesVisibility$	 
Horizontal=�
   AlternatingRowBackground

 �����   
IsReadOnly$
 True��   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 항목��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
Property+  $	 값��$ *  �� *Value+�� $��통계 정보��.���� .��  -   $ StatisticsDataGrid��   $ 	통계명��$	 200  �� *
StatName+  $	 값��$	 150  �� *	StatValue+  $ 설명��$ *  �� *Description+�� -    ��Name$ ColumnInfoTab��$��컬럼 정보��.���� .��  -   $ ColumnInfoDataGrid��   $ 	컬럼명��$	 150  �� *
ColumnName+  $ 데이터타입��$	 100  �� *
DataType+  $ 길이��$ 80  �� *
DataLength+  $ 
NULL허용��$ 80  �� *
Nullable+  $ 	기본값��$ *  �� *DefaultValue+�� -   $ IndexInfoTab��$��인덱스 정보��.���� .��  -	   $ IndexInfoDataGrid��   $ 	컬럼명��$	 150  �� *
ColumnName+  $ 위치��$ 80  �� *ColumnPosition+  $ 정렬��$ 80  �� *DescendFlag+�� $��2�� �� Orientation$ 
Horizontal=�$��Right=�$��5q�.+��� -
    ��Name$ RefreshButton��$��새로고침��$��80��$��30��$��0,0,10,0q��� -   $ CloseButton��$��닫기��$��80��$��30�� �� IsCancel$
 True��