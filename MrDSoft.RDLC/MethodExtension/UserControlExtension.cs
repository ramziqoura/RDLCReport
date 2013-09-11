using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace DSoft.MethodExtension
{
    public static class UserControlExtension
    {
        /// <summary>
        /// Déplace le focus sur le prochain control vers la droite dans l'ordre de tabulation
        /// </summary>        
        public static void MoveFocusNext(this UserControl ctrl)
        {
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            if (keyboardFocus != null)
                keyboardFocus.MoveFocus(tRequest);                        
        }

        /// <summary>
        /// Déplace le focus sur le prochain control vers la gauche dans l'ordre de tabulation
        /// </summary>        
        public static void MoveFocusPrevious(this UserControl ctrl)
        {
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Previous);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            if (keyboardFocus != null)
                keyboardFocus.MoveFocus(tRequest);
        }
    }
}
