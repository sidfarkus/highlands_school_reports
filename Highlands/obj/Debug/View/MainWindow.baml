   M S B A M L   `   `   `  ����D  @Highlands, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Highlands	Highlands  5   	       Highlands.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   %hclr-namespace:Highlands     5   	     � Title$  Highlands School��5   	    � Icon$* $/Highlands;component/Images/main.ico��6"    � WindowState$ 	Maximized=�6N   .��5      � .+�5   
   �  � RowDefinitions 5	      �� 5
      $
��Auto �6    �� 5      .+�5      c�  `� IsLocked . .@�5      �� -   5        $��	btnImport��5      $��80��6B   $��3q�6M   .��6X   Import Students6i   �� 5      -   5        $��	btnExport��5      $��80��6B   $��3q�6M   .��6X   Export Students6i   �� 5      -   5        $��80��5   1   $��3q�6<   .��6G   Logout6O   5      �� 5      $��1��6   .@�5      �� -   5        $��tabAttendance��5      ��5      �� 5      $��16(�6$   $
��Bold"�62   
Attendance6P   .��5          Highlands.AttendeesControl  5      �� 5      -   5        $��
tabClasses��5      ��5      �� 5      $��16(�6$   $
��Bold"�62   Current Classes6U   .��5         Highlands.ClassesControl  -   5          Name$ ctrlClasses��5   '   5      �� 5      -   5        $��tabStudents��5      ��5       �� 5!      $��16(�6$   $
��Bold"�62   
Students6N   .��5#         Highlands.StudentsControl  -   5          Name$ studentsControl��5#   (   5$      5%      5&      �� 5'   
   -	   5         ��Name$ loginOverlay��5'      ��5(      �� 5)      .��5*      �  � Color$ 
LightGreen��6#   	 � Offset$	 0Q�66   � 5+      $ White��6#   $		 0.6Q�61   � 5,      $ 
LightGreen��6#   $	 1Q�66   5-      ._�5/         Highlands.LoginWindow  -
   5        
  Name$
 loginWindow��5/         Highlands.LoginWindow   LoggedIn$ OnLogin��61   $��Center=�6D   $��Center=�6a   $	��400��6|   $	��300��6�   50      51      52      