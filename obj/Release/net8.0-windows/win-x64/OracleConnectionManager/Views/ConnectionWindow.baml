   M S B A M L   `   `   `  ����   wsDB9    3wsDB.OracleConnectionManager.Views.ConnectionWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Oracle 연결��$	��500��$	��450�� � WindowStartupLocation$ CenterOwner=� � 
ResizeMode$ NoResize=�.��� $��10q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��연결 프로파일��$��0,0,0,10q�.���  �� $
��Auto ��� $
��Auto �.+��� $��0��$��프로파일:���� -    ��Name$ ProfileComboBox��$��1�� �� DisplayMemberPath$ ProfileName������ *
Profiles+ �� SelectedItem �� *SelectedProfile+�� -    ��Name$ ConnectionTabControl��$��1��.@��� -    ��Name$ TnsTab��$��
TNS 연결��.��� $��10q� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �	 � ColumnDefinitions	 �� $
��Auto ��� $��* �.+��� $��0��$��0��$��프로파일명:���� -   
 ��Name$
 TnsProfileNameTextBox��$��0��$��1���� $��1��$��0��$��TNS 별칭:���� -   $ TnsAliasComboBox��$��1��$��1�� �� 
IsEditable$
 True������ *
TnsAliases+ �� *SelectedTnsAlias+�� $��2��$��0��$��사용자명:���� -   $
 TnsUsernameTextBox��$��2��$��1���� $��3��$��0��$��비밀번호:��G� -    G�Name$ TnsPasswordBox��$��3��$��1���� -	   $ 	DirectTab��$��Direct 연결��.��� $��10q� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �	 �� $
��Auto ��� $��* �.+��� $��0��$��0��$��프로파일명:���� -
   $
 DirectProfileNameTextBox��$��0��$��1���� $��1��$��0��$��
호스트:���� -   $
 DirectHostTextBox��$��1��$��1���� $��2��$��0��$��포트:���� -   $
 DirectPortTextBox��$��2��$��1��$
��1521���� $��3��$��0��$��서비스명:���� -   $
 DirectServiceNameTextBox��$��3��$��1���� $��4��$��0��$��사용자명:���� -   $
 DirectUsernameTextBox��$��4��$��1���� $��5��$��0��$��비밀번호:��G� -   $ DirectPasswordBox��$��5��$��1���� $��2�� �� Orientation$ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� -    ��Name$ 
TestButton��$��연결 테스트���� -   $ ConnectButton��$��연결�� �� 	IsDefault$
 True���� -   $ CancelButton��$��취소�� �� IsCancel$
 True��