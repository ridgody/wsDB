   M S B A M L   `   `   `  ����  wsDB6    0wsDB.Common.DBObject.Views.ObjectSelectionWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  객체 선택��5   	   $	��400��6   $	��500��6$    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ 	CanResize=�5   	    � ShowInTaskbar$ False��5   	   .��5	      � $��10q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��14(�5      $
��Bold"�5      $��0,0,0,10q�5      ��5      �� *Title+5      �� 5   
   $��1��6   $'��!동일한 이름의 객체 목록��6    .��5      &   System.Windows.Controls.DataGrid  -   5        $��ObjectsDataGrid��5         AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5         SelectionMode$ Single=�5         GridLinesVisibility$ 
Horizontal=�5      	   AlternatingRowBackground
	 �������5      �� *	Objects+
 �� SelectedItem
 5      �� *SelectedObject+   Columns 5"      0  *System.Windows.Controls.DataGridTextColumn  5#      ,  &System.Windows.Controls.DataGridColumn   Header$ 	소유자��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$	 150 6P   1  +System.Windows.Controls.DataGridBoundColumn   Binding 66   �� *Owner+6P     5$      $ 	객체명��6)   $	 200 6U    66   �� *
ObjectName+6U     5%      $ 객체 타입��6)   $	 100 6W    68   �� *
ObjectType+6W   5'      5(      �� 5+   
   $��2��6    �� Orientation$ 
Horizontal=�5,      $��Right=�5-      $��0,10,0,0q�5.      .+�5/      �� -   5        $��OkButton��5/      $��확인��50      $��80��51      $��30��52      $��0,0,10,0q�53      {�54      �� *IsObjectSelected+55      �� 56      -   5        $��CancelButton��56      $��취소��57      $��80��58      $��30��59      5:      5;      5<      5=      