   M S B A M L   `   `   `  ����  wsDBV    PwsDB.OracleExecutionPlan.DbmsXplanAnalyzer.Views.PerformanceAnalysisResultWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$'  !실행계획 성능 분석 결과��5   	   $	��600��6   $
��1000��6,    � WindowStartupLocation$ CenterOwner=�5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   .+�5   
   �� $��0��6   $��성능 분석 요약��6    $��5q�62   .��5      � $��10q�6    � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'    5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� $��0��6   $��0��6)   $
��Bold"�6l   ��69   �� *#!Summary.TotalExecutionTimeDisplay+6l   �� 5      $��0��6   $��1��6)   $
��Bold"�6j   ��69   �� *!Summary.TotalBufferReadsDisplay+6j   �� 5      $��0��6   $��2��6)   $
��Bold"�6l   ��69   �� *#!Summary.TotalPhysicalReadsDisplay+6l   �� 5      $��1��6   $��0��6)   
���  ����69   �� *Summary.CriticalIssuesDisplay+6h   �� 5       $��1��6   $��1��6)   
��� �����69   �� *Summary.HighIssuesDisplay+6d   �� 5!      $��1��6   $��2��6)   ��69   �� *Summary.MostExpensiveDisplay+5"      5#      �� 5&   
   $��1��6   $��5q�6"   .@�5(      �� $��성능 이슈��6   .��5)      &   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��5*         
IsReadOnly$
 True��5+         GridLinesVisibility$ 
Horizontal=�5,         AlternatingRowBackground
 �������5)      �� *Issues+   Columns 5.      0  *System.Windows.Controls.DataGridTextColumn  5/      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Step ID��6-   
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 60 6Y   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6>   �� *StepId+6Y   4  .System.Windows.Controls.DataGridTemplateColumn  50      $	 	심각도��61   $
 80 6>      CellTemplate 51      �� 52   "   .R�53   &   ��  �� CornerRadius$ 3��6R    �� Padding$	 5,2q�6c   ��6-   �� *SeverityColor+._�54   *   �� 
�������$��Center=�55   2   $
��Bold"�6O   ��54   4   �� *
Severity+55   O   56   '   57   #   59        5:      $	 유형��6-   $	
 120 6Y    69   �� *TypeDisplay+6Y     5;      $	 설명��6-   $
 * 6Y    69   �� *Description+6Y     5<      $	 권장사항��6-   $
 * 6^    6;   �� *Recommendation+6^   5>      5?      �� 5B      $��단계별 분석��6   .��5C        ��6   �� *Steps+ 5D        5E      $	 ID��6-   $
 40 6P    69   �� *Id+6P     5F      $	 	Operation��6-   $	
 200 6^    6@   �� *	Operation+6^     5G      $
	 Name��6-   $	
 150 6T    6;   �� *Name+6T     5H      $	 실행시간(초)��6-   $
 80 6u    6>   �� *ActualTimeSeconds+ �� StringFormat$ F2��6u     5I      $	 Buffer��6-   $
 80 6j    6=   �� *	Buffers+$ N0��6j     5J      $	 Reads��6-   $
 80 6g    6<   �� *Reads+$ N0��6g     5K      $	 실제행수��6-   $
 80 6k    6;   �� *
ActualRows+$ N0��6k     5L      $	 예상행수��6-   $
 80 6n    6;   �� *EstimatedRows+$ N0��6n     5M      $	 
Buffer/Row��6-   $
 80 6w    6A   �� *BufferToRowRatio+$ F1��6w     5N      $	 	심각도��61   $
 60 6>    5O      �� 5P   "   .R�5Q   &   A� $��12��6.   $��12��69   ��6E   �� *SeverityColor+5R   #   5T      5V      5W      �� 5Z      $��권장사항��6   .��5[      �� $��10q�6   .��5\      �� ��6#   �� *Recommendations+��5]      �� 5^      .R�5_   "   �� $	��0,5q�6=   $��12(�6J   $
��Wrap=�6X   ��6,   �� 6X   5`      5b      5c      5d      5e      5f      5g      