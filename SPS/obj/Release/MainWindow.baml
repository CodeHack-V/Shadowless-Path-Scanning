   M S B A M L   `   `   `  ���� >  :SPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SPSSPS      SPS.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:SPS       � Title$  SPS-Powerd by V��$	��750��$
��1100�� � WindowStartupLocation$ CenterScreen=� � 	Resources  �� 
TargetType �� Property �� Value��%v   '��       �� " ��.���  � ��$ Center=���  � ��$ Center=���  � ��$ 20(���  � ��$ 5q�.���  � RowDefinitions �� $��1* ��� $��9* � � ColumnDefinitions �� $��8* ��� $��2* �.+�� $��0��$��2�� �� $��0.75* ��� $
��2.5* ��� $��0.75* ��� $��0.75* ��� $��0.75* ��� $��0.75* ��� $��0.75* ��� $��0.75* ��� $��0.75* ��� $
��1.5* �.+��� $��Center=�$��0��$��Center=�.��	目标:�� -   $��Target��$��1�����#169fe6���#fff���#169fe6$��Center=�$��Center=�$��10,25,10,25q� �� IsInputMethodEnabled .  .��https://www.Example.com�� $��2��$��Center=�$��Center=�.��超时(秒):�� -   $��TimeOut��$��3�����#169fe6���#fff���#169fe6$��Center=�$��Center=�$��10,25,10,25q� .  .��5�� $��4��$��Center=�$��Center=�.��	协议:�� -   $��Protocol��$��5��$��10,25,10,25q�$��Center=�$��Center=�.@��� .��HEAD�� .��GET�� .��POST�� $��6��$��Center=�$��Center=�.��
线程数:�� -   $��Threads��$��7�����#169fe6���#fff���#169fe6$��Center=�$��Center=� .  .��20�� -   $��Up��$��7�����#169fe6���#fff$��Right=�$	��Top=�.��∧�� -   $
��Down��$��7�����#169fe6���#fff$��Right=�$��Bottom=�.��∨�� -   $��Reset�����#169fe6���#fff$��8��$��20,10,20,10q�{�.  .��重置�� -   $��
StartPause�����#169fe6���#fff$��9��$��20,10,20,10q�	 �� 	IsDefault	 . .��启动� $��1�� �� $
��9.5* ��� $
��0.5* �.+��� $��0��.@��� $	��200�����#e8edf0.���� -	   $��
listbox200�����#e8edf0���#169fe6$��3q����#169fe6�� $	��403�����#e8edf0.���� -
   $��
listbox403�����#e8edf0
���<��$��3q�
���<���� $	��3XX�����#e8edf0.���� -   $��
listbox3XX�����#e8edf0���	#FFF75676$��3q�
���<��� -   $��ProgressBar1��$��1��$��5q�� $��1��$��1�� �� $��1* ��� $��9* �.+�� $��0�� �� $��* ��� $��* ��� $��* �.+��� -   $��Checkbox_200��$��0��
 d� 	IsChecked$

 TrueY�.��200�� -   $��Checkbox_403��$��1��.��403�� -   $��Checkbox_3XX��$��2��.��3XX�� $��1�����#e8edf0���#169fe6$��3q�$��10q�._��� -   $��FileGrid��