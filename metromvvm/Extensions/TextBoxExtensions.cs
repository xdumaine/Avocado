namespace MetroMVVM.Extensions
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public class TextBoxExtensions
    {
        /// <summary>
        /// AutoSelectText dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSelectTextProperty = DependencyProperty.RegisterAttached("AutoSelectText", typeof (Boolean), typeof (TextBoxExtensions), new PropertyMetadata(DependencyProperty.UnsetValue, OnAutoSelectTextChanged));

        /// <summary>
        /// PreventAutoSelectText dependency property.
        /// </summary>
        public static readonly DependencyProperty PreventAutoSelectTextProperty = DependencyProperty.RegisterAttached("PreventAutoSelectText", typeof (Boolean), typeof (TextBoxExtensions), new PropertyMetadata(DependencyProperty.UnsetValue));

        /// <summary>
        /// This method is called when the value of the AutoSelectText property
        /// is set from the xaml
        /// </summary>
        /// <param name="d">The content control on which the property is set.</param>
        /// <param name="e"></param>
        private static void OnAutoSelectTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //This works because of event bubbling. 
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                if ((bool)e.NewValue)
                {
                    frameworkElement.GotFocus += OnGotFocus;
                }
                else
                {
                    frameworkElement.GotFocus -= OnGotFocus;
                }
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            //Since we are using routed events, the sender parameter will not be the textbox that currently has focus. 
            //It will the root level content control (Grid) which has the AutoSelectText attached property.
            //The FocusManager class is used to get a reference to the control that has the focus.
            var textBox = FocusManager.GetFocusedElement() as TextBox;
            if (textBox != null && !(bool)textBox.GetValue(PreventAutoSelectTextProperty))
            {
                textBox.Select(0, textBox.Text.Length);
            }
        }

        #region Dependency property Get/Set
        public static Boolean GetAutoSelectText(DependencyObject target)
        {
            return (Boolean)target.GetValue(AutoSelectTextProperty);
        }

        public static void SetAutoSelectText(DependencyObject target, Boolean value)
        {
            target.SetValue(AutoSelectTextProperty, value);
        }

        public static Boolean GetPreventAutoSelectText(DependencyObject target)
        {
            return (Boolean)target.GetValue(PreventAutoSelectTextProperty);
        }

        public static void SetPreventAutoSelectText(DependencyObject target, Boolean value)
        {
            target.SetValue(PreventAutoSelectTextProperty, value);
        }
        #endregion
    }
}