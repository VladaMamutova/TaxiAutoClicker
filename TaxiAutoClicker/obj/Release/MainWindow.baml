   M S B A M L   `   `   `  ����   TaxiAutoClicker     TaxiAutoClicker.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  TaxiAutoClicker��$��14(� � SizeToContent$ Height=�$	��310�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � 	Resources  �� Color   
HoverColor  APIKeyPasteButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name �� Triggers P� Property P� Value �� 
TargetName �� ContentStringFormat��%%  &	        &	      '��     7   �� $ 	#FF1E78FF���� " ��.���  � ��$ 20����  � ��$ 20����  � �� �� "	 ��.R��� $
 border��
�������$��Center=�$��Center=�$��0q�._�(� $��18��$��18��.���� $��20��$��20��.+�F� $��IconPath������Р.c ��- ��� ��- P�� ��@  � ��[ р�O��[ р�O@T� р�@T� р� � ����  � ���� @T� � � @T� �P�� 0� ��  � �.c  � � 	=  � �� ��� �� �pr Ӏ� @KL  	= ��- �.c ��- Р.c ��[ �@KL �pr  `� `� נ.c @T�  `� `� � z �pr  `� `� נ.c ��[  `� `� � z @f��ا @f��ا ��� �@]� ��� �@]� @f� $� @f� $� ��!�@]� ��!�@]� ��O��ا ��O��ا ��!� z ��!� z @f	
����·� P�  �@J�IsMouseOver . . ���  �@�Cursor$
 Hand���� $ IconPath�� � ��
 ������ " ��.���  � �� �� "	 ��.R��� $
 ButtonBorder��$��35��
������$��0q�._��� #��z $��Center=�$��Center=�
������� P�  �@J�IsMouseOver . . ���  �@�Cursor$
 Hand���� $ ButtonBorder��8    � ��.��� .+��� .+�o� .@�m� $��Настройки��$��	15 0 15 0q�.@�m� -   $,��&Управление кликами...���� $��15 15 15 15q�.+�� $��0 0 5 0q�.+��� $*��$API-ключ для sms-activate.ru:��$��0 2 0 0q��� -   #��[  � ToolTip$# Вставить Api-ключ��$��Right=��� -   $��APIKeyTextBox��$��5q�$��0 5 0 7q��� $"��Почтовый адрес:���� -   $
��Mail��$��5q�$��0 5 0 7q��  � ColumnDefinitions �� $
��0.7* ��� $��5 ��� .+��� .+��� $��Имя:���� -   $��	FirstName��$��5q�$��0 5 0 7q��� $��2��.+��� $��Фамилия:���� -   $��LastName��$��5q�$��0 5 0 7q��� -   $��LaunchButton��$. (Запустить заказ такси��$��0 5 0 0q�$h bАвтокликер будет запущен в каждом открытом окне NoxPlayer���� $
��15 5q�$��Bottom=�$��Right=� �� Orientation$ 
Horizontal=�.+��� $	��X: ���� -	   $��
XTextBlock��$��30���� $��5���� $	��Y: ���� -
   $��
YTextBlock��$��30��