   M S B A M L   `   `   `  ����  wsDBE    ?wsDB.OracleExecutionPlan.DbmsXplan.Views.ObjectStatisticsWindow   5   
   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   
   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   
     � Title$  객체 통계 정보��5   
   $	��500��6   $	��700��6(    � WindowStartupLocation$ CenterOwner=�5   
   .��5      �  � RowDefinitions 5	      �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5      �� $��0��6   
����ح� �� Padding$ 10q�66   $��0,0,0,5q�6C   ._�5      �� .+�5      �� -   5         ��Name$ ObjectTypeText��5      $
��Bold"�65   $��14(�6G   �� 5      -   5        $ ObjectNameText��5      $��12(�65   $��0,2,0,0q�6C   5      5      �� 5      -   5         ��Name$ StatisticsTabControl��5      $��1��64   $��5q�6A   .@�5      �� $��기본 정보��6   .��5      �� .��5      &   System.Windows.Controls.DataGrid  -   5          Name$ BasicInfoDataGrid��5          AutoGenerateColumns$ False��5         HeadersVisibility$ Column=�5      	   GridLinesVisibility$	 
Horizontal=�5      
   AlternatingRowBackground

 �����   
IsReadOnly$
 True��5          Columns 5!      0  *System.Windows.Controls.DataGridTextColumn  5"      ,  &System.Windows.Controls.DataGridColumn   Header$ 항목��62      Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 6[   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6>   �� *
Property+6[     5#      $	 값��62   $ * 6W    6=   �� *Value+6W   5%      5&      5'      �� 5)      $��통계 정보��6   .��5*      �� .��5+        -   5        $ StatisticsDataGrid��5+       $ False��5,      $ Column=�5-       5.        5/      $ 	통계명��62   $	 200 6\    6?   �� *
StatName+6\     50      $	 값��62   $	 150 6[    6=   �� *	StatValue+6[     51      $ 설명��62   $ * 6^    6>   �� *Description+6^   53      54      55      �� 57      -   5         ��Name$ ColumnInfoTab��57   &   $��컬럼 정보��6   .��58      �� .��59        -   5        $ ColumnInfoDataGrid��59       $ False��5:      $ Column=�5;       5<        5=      $ 	컬럼명��62   $	 150 6^    6?   �� *
ColumnName+6^     5>      $ 데이터타입��62   $	 100 6^    6A   �� *
DataType+6^     5?      $ 길이��62   $ 80 6]    6>   �� *
DataLength+6]     5@      $ 
NULL허용��62   $ 80 6_    6B   �� *
Nullable+6_     5A      $ 	기본값��62   $ * 6`    6?   �� *DefaultValue+6`   5C      5D      5E      �� 5G      -   5        $ IndexInfoTab��5G   '   $��인덱스 정보��6   .��5H      �� .��5I        -	   5        $ IndexInfoDataGrid��5I       $ False��5J      $ Column=�5K       5L        5M      $ 	컬럼명��62   $	 150 6^    6?   �� *
ColumnName+6^     5N      $ 위치��62   $ 80 6a    6>   �� *ColumnPosition+6a     5O      $ 정렬��62   $ 80 6^    6>   �� *DescendFlag+6^   5Q      5R      5S      5U      �� 5X      $��2��6    �� Orientation$ 
Horizontal=�6#   $��Right=�6<   $��5q�6X   .+�5Y      �� -
   5         ��Name$ RefreshButton��5Y      $��새로고침��6-   $��80��6<   $��30��6G   $��0,0,10,0q�5Z      6(   �� 5[      -   5        $ CloseButton��5[      $��닫기��6+   $��80��68   $��30��6C    �� IsCancel$
 True��5\   0   5]      5^      5_      