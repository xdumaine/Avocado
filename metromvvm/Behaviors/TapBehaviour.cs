// See: http://www.olsonsoft.com/blogs/stefanolson/post/WinRT-Tapped-Behaviour.aspx
// Updated to work in Windows 8 Release Preview
namespace MetroMVVM.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    public enum TapBehaviourOptions
    {
        None,
        On
    }
    public class TapBehaviourExtension
    {
        #region StartPoint

        /// <summary>
        /// StartPoint Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.RegisterAttached("StartPoint", typeof(Point), typeof(TapBehaviourExtension),
                new PropertyMetadata((Point)new Point(0,0)));

        /// <summary>
        /// Gets the StartPoint property. This dependency property 
        /// indicates ....
        /// </summary>
        public static Point GetStartPoint(DependencyObject d)
        {
            return (Point)d.GetValue(StartPointProperty);
        }

        /// <summary>
        /// Sets the StartPoint property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetStartPoint(DependencyObject d, Point value)
        {
            d.SetValue(StartPointProperty, value);
        }

        #endregion

        #region IsDown

        /// <summary>
        /// IsDown Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsDownProperty =
            DependencyProperty.RegisterAttached("IsDown", typeof(bool), typeof(TapBehaviourExtension),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets the IsDown property. This dependency property 
        /// indicates ....
        /// </summary>
        public static bool GetIsDown(DependencyObject d)
        {
            return (bool)d.GetValue(IsDownProperty);
        }

        /// <summary>
        /// Sets the IsDown property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetIsDown(DependencyObject d, bool value)
        {
            d.SetValue(IsDownProperty, value);
        }

        #endregion

        #region TapBehavour

        /// <summary>
        /// TapBehavour Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty TapBehavourProperty =
            DependencyProperty.RegisterAttached("TapBehavour", typeof(TapBehaviourOptions), typeof(TapBehaviourExtension),
                new PropertyMetadata(TapBehaviourOptions.None,
                    OnTapBehavourChanged));

        /// <summary>
        /// Gets the TapBehavour property. This dependency property 
        /// indicates ....
        /// </summary>
        public static TapBehaviourOptions GetTapBehavour(DependencyObject d)
        {
            return (TapBehaviourOptions)d.GetValue(TapBehavourProperty);
        }

        /// <summary>
        /// Sets the TapBehavour property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetTapBehavour(DependencyObject d, TapBehaviourOptions value)
        {
            d.SetValue(TapBehavourProperty, value);
        }

        /// <summary>
        /// Handles changes to the TapBehavour property.
        /// </summary>
        private static void OnTapBehavourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TapBehaviourOptions oldTapBehavour = (TapBehaviourOptions)e.OldValue;
            TapBehaviourOptions newTapBehavour = (TapBehaviourOptions)d.GetValue(TapBehavourProperty);

            FrameworkElement fe = d as FrameworkElement;
            if (fe==null) return;
            if (oldTapBehavour != TapBehaviourOptions.None)
            {
                // remove event handlers
                fe.RemoveHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(PointerCaptureLost));
                fe.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(PointerPressed));
                fe.RemoveHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(PointerReleased));
            }
            if (newTapBehavour==TapBehaviourOptions.On)
            {
                // attach some event handlers
                fe.AddHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(PointerCaptureLost), true);
                fe.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(PointerPressed), true);
                fe.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(PointerReleased), true);
            }
        }

        private static void DisplayUpAnimation(DependencyObject sender)
        {
            if (sender!=null && GetIsDown(sender))
            {
                Storyboard sb = new Storyboard();
                PointerUpThemeAnimation animation = new PointerUpThemeAnimation();
                Storyboard.SetTarget(animation, sender as FrameworkElement);
                sb.Children.Add(animation);
                sb.Begin();
                SetIsDown(sender, false);
            }
        }

        private static void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            DisplayUpAnimation(sender as DependencyObject);
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;
            fe.ReleasePointerCapture(e.Pointer);
        }

        private static void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            DisplayDownAnimation(sender as DependencyObject);
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;
            fe.PointerMoved += PointerMoved;
            fe.CapturePointer(e.Pointer);
            var visual = fe.TransformToVisual(null);
            Point ptStart = visual.TransformPoint(new Point(0, 0));
            SetStartPoint(fe, ptStart);
        }

        static void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            FrameworkElement uiElement = sender as FrameworkElement;
            if (uiElement == null) return;
            Point ptStart = GetStartPoint(uiElement);
            Point pt = e.GetCurrentPoint(null).Position;
            
            Rect itemRect = new Rect(ptStart.X, ptStart.Y, uiElement.ActualWidth, uiElement.ActualHeight);

            Debug.WriteLine(itemRect.ToString());
            if (itemRect.Contains(pt)) {
                Debug.WriteLine("down:" + pt.ToString());
                DisplayDownAnimation(uiElement);
            }
            else
            {
                Debug.WriteLine("up:"+pt.ToString());
                DisplayUpAnimation(uiElement);
            }

            // DOESN'T work
            //var elems = VisualTreeHelper.FindElementsInHostCoordinates(pt, uiElement);
            //if (elems.Contains(uiElement))
            
        }

        private static void DisplayDownAnimation(DependencyObject sender)
        {
            if (sender != null && !GetIsDown(sender))
            {
                Storyboard sb = new Storyboard();
                PointerDownThemeAnimation animation = new PointerDownThemeAnimation();
                Storyboard.SetTarget(animation, sender as FrameworkElement);
                sb.Children.Add(animation);
                sb.Begin();
                SetIsDown(sender, true);
            }
        }

        private static void PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            fe.PointerMoved -= PointerMoved;
            DisplayUpAnimation(sender as DependencyObject);
        }

        #endregion
    }
}
