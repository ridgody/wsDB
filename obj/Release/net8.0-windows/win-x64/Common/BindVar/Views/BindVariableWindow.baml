   M S B A M L   `   `   `  ����   wsDB2    ,wsDB.Common.BindVar.Views.BindVariableWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  바인드 변수 입력��$	��450��$	��500�� � WindowStartupLocation$ CenterOwner=� � 
ResizeMode$ 	CanResize=� � ShowInTaskbar$ False��.��� $��10q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$B��<쿼리에 사용할 바인드 변수 값을 입력하세요.��$��14(�$��0,0,0,10q�� $��1�� � ColumnDefinitions �� $	��200 ��� $��10 ��� $��* �.+��� $��0��$��변수 목록��.���� -   $��VariableListBox�� �� DisplayMemberPath$
 Name������ *BindVariables+ �� SelectedItem �� *SelectedVariable+�� $��2��$��변수 정보��.��� $��10q� �� $
��Auto ��� $��10 ��� $
��Auto ��� $
��Auto ��� $��10 ��� $
��Auto ��� $
��Auto ��� $��* �.+��� $��0��$��
변수명:���� $��1�� �� 
IsReadOnly$
 True��
����������� *SelectedVariable.Name+	 �� Mode$	 OneWay=��� $��2��$��타입:���� -   $��TypeComboBox��$��3������ *AvailableTypes+ �� *SelectedVariable.Type+���� .R��� ���� �� $��5��$
��값:���� -   $��ValueTextBox��$��6��$��25������ *SelectedVariable.Value+
 �� UpdateSourceTrigger$
 PropertyChanged=��� $��7��$����도움말:
• VARCHAR2: 문자열 값
• NUMBER: 숫자 값 (예: 123, 123.45)
• DATE: 날짜 값 (예: 2024-01-01, 2024-01-01 14:30:00)��$��10(�
�������$
��Wrap=�$��Bottom=�$��0,10,0,0q��� $��2�� �� Orientation$ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� -   $��OkButton��$��확인��$��80��$��30��$��0,0,10,0q��� -   $��CancelButton��$��취소��$��80��$��30��