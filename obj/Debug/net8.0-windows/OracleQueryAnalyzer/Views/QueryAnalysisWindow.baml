   M S B A M L   `   `   `  ����  wsDB8    2wsDB.OracleQueryAnalyzer.Views.QueryAnalysisWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  쿼리 분석��5   	   $	��600��6   $	��900��6$    � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $��* �6   �� 5
      $
��Auto �6   .+�5   
   c� $��0��6   $��5q�6   .@�5      �� -   5        $��LoadQueryButton��5      $��쿼리 불러오기��6,   6>   �� 5      �� 5      $��상태:��6   $��Center=�6#   $��10,0,5,0q�6>   �� 5      -   5        $��StatusTextBlock��5      $��쿼리를 입력하세요��6/   $��Center=�6@   5      �� 5   
   $��1��6   $��
SQL 쿼리��6    $��5q�60   .��5      �� -   5        $��QueryTextBox��5       �� AcceptsReturn$
 True��5       �� 
AcceptsTab$
 True��5      " �� VerticalScrollBarVisibility$
 Auto=�5      $ �� HorizontalScrollBarVisibility$
 Auto=�5      $��Consolas)�5      $��12(�5      5      5       �� 5#   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��5q�6W   .+�5$      �� -   5        $��AnalyzeButton��5$      $��실행계획 분석��5%      ${�False��5'      $	��120��5(      $��30��5)      $��5q�5*      �� 5+      -   5        $��CloseButton��5+      $��닫기��5,      $��80��5.      $��30��5/      $��5q�50      51      52      53      