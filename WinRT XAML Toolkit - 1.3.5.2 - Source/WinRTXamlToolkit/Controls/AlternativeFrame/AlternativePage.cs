﻿using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Controls
{
    /// <summary>
    /// Alternative to the standard Page control, with a similar API,
    /// for use with the AlternativeFrame control.
    /// </summary>
    public class AlternativePage : UserControl
    {
        #region Frame
        /// <summary>
        /// Frame Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register(
                "Frame",
                typeof(AlternativeFrame),
                typeof(AlternativePage),
                new PropertyMetadata(null, OnFrameChanged));

        /// <summary>
        /// Gets or sets the Frame property. This dependency property 
        /// indicates the frame the page is hosted in.
        /// </summary>
        public AlternativeFrame Frame
        {
            get { return (AlternativeFrame)GetValue(FrameProperty); }
            internal set { SetValue(FrameProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Frame property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnFrameChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (AlternativePage)d;
            AlternativeFrame oldFrame = (AlternativeFrame)e.OldValue;
            AlternativeFrame newFrame = target.Frame;
            target.OnFrameChanged(oldFrame, newFrame);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the Frame property.
        /// </summary>
        /// <param name="oldFrame">The old Frame value</param>
        /// <param name="newFrame">The new Frame value</param>
        protected virtual void OnFrameChanged(
            AlternativeFrame oldFrame, AlternativeFrame newFrame)
        {
        }
        #endregion

        #region ShouldWaitForImagesToLoad
        /// <summary>
        /// ShouldWaitForImagesToLoad Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShouldWaitForImagesToLoadProperty =
            DependencyProperty.Register(
                "ShouldWaitForImagesToLoad",
                typeof(bool?),
                typeof(AlternativePage),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ShouldWaitForImagesToLoad property. This dependency property 
        /// indicates whether the frame should wait for all images in a page to load
        /// before transitioning to the next page.
        /// </summary>
        public bool? ShouldWaitForImagesToLoad
        {
            get { return (bool?)GetValue(ShouldWaitForImagesToLoadProperty); }
            set { SetValue(ShouldWaitForImagesToLoadProperty, value); }
        }
        #endregion

        #region NavigationState
        /// <summary>
        /// NavigationState Dependency Property
        /// </summary>
        public static readonly DependencyProperty NavigationStateProperty =
            DependencyProperty.Register(
                "NavigationState",
                typeof(NavigationState),
                typeof(AlternativePage),
                new PropertyMetadata(NavigationState.Initializing));

        /// <summary>
        /// Gets or sets the NavigationState property. This dependency property 
        /// indicates the state of the page in the navigation sequence.
        /// </summary>
        public NavigationState NavigationState
        {
            get { return (NavigationState)GetValue(NavigationStateProperty); }
            private set { SetValue(NavigationStateProperty, value); }
        }
        #endregion

        #region PageTransition
        /// <summary>
        /// PageTransition Dependency Property
        /// </summary>
        public static readonly DependencyProperty PageTransitionProperty =
            DependencyProperty.Register(
                "PageTransition",
                typeof(PageTransition),
                typeof(AlternativePage),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the PageTransition property. This dependency property 
        /// indicates the PageTransition to use to transition to this page.
        /// </summary>
        public PageTransition PageTransition
        {
            get { return (PageTransition)GetValue(PageTransitionProperty); }
            set { SetValue(PageTransitionProperty, value); }
        }
        #endregion

        // Summary:
        //     Invoked immediately after the Page is unloaded and is no longer the current
        //     source of a parent Frame.
        //
        // Parameters:
        //   e:
        //     Event data that can be examined by overriding code. The event data is representative
        //     of the navigation that has unloaded the current Page.
        protected virtual async Task OnNavigatedFrom(AlternativeNavigationEventArgs e)
        {
        }

        //
        // Summary:
        //     Invoked when the Page is loaded and becomes the current source of a parent
        //     Frame.
        //
        // Parameters:
        //   e:
        //     Event data that can be examined by overriding code. The event data is representative
        //     of the pending navigation that will load the current Page. Usually the most
        //     relevant property to examine is Parameter.
        protected virtual async Task OnNavigatedTo(AlternativeNavigationEventArgs e)
        {
        }

        //
        // Summary:
        //     Invoked immediately before the Page is unloaded and is no longer the current
        //     source of a parent Frame.
        //
        // Parameters:
        //   e:
        //     Event data that can be examined by overriding code. The event data is representative
        //     of the navigation that will unload the current Page unless canceled. The
        //     navigation can potentially be canceled by setting Cancel.
        protected virtual async Task OnNavigatingFrom(AlternativeNavigatingCancelEventArgs e)
        {
            this.NavigationState = NavigationState.NavigatingFrom;
        }

        protected virtual async Task OnNavigatingTo(AlternativeNavigationEventArgs e)
        {
            this.NavigationState = NavigationState.NavigatingTo;
        }

        protected virtual async Task OnTransitioningTo()
        {
        }

        protected virtual async Task OnTransitionedTo()
        {
        }

        protected virtual async Task OnTransitioningFrom()
        {
        }

        protected virtual async Task OnTransitionedFrom()
        {
        }

        internal async Task OnTransitioningToInternal()
        {
            this.NavigationState = NavigationState.TransitioningTo;
            await OnTransitioningTo();
        }

        internal async Task OnTransitionedToInternal()
        {
            this.NavigationState = NavigationState.TransitionedTo;
            await OnTransitionedTo();
        }

        internal async Task OnTransitioningFromInternal()
        {
            this.NavigationState = NavigationState.TransitioningFrom;
            await OnTransitioningFrom();
        }

        internal async Task OnTransitionedFromInternal()
        {
            this.NavigationState = NavigationState.TransitionedFrom;
            await OnTransitionedFrom();
        }

        internal async Task OnNavigatingFromInternal(AlternativeNavigatingCancelEventArgs e)
        {
            this.NavigationState = NavigationState.NavigatingFrom;
            await OnNavigatingFrom(e);
        }

        /// <summary>
        /// The last call before page transition occurs, but after the page has been added to visual tree.
        /// An opportunity to wait for some limited loading to complete before the transition animation is played.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal async Task OnNavigatingToInternal(AlternativeNavigationEventArgs e)
        {
            this.NavigationState = NavigationState.NavigatingTo;
            await OnNavigatingTo(e);
        }

        internal async Task OnNavigatedFromInternal(AlternativeNavigationEventArgs e)
        {
            this.NavigationState = NavigationState.NavigatedFrom;
            await OnNavigatedFrom(e);
        }

        internal async Task OnNavigatedToInternal(AlternativeNavigationEventArgs e)
        {
            this.NavigationState = NavigationState.NavigatedTo;
            await OnNavigatedTo(e);
        }

        protected virtual async Task Preload(object parameter)
        {
        }

        protected virtual async Task UnloadPreloaded()
        {
        }

        internal async Task PreloadInternal(object parameter)
        {
            this.NavigationState = NavigationState.Preloading;
            await Preload(parameter);
            this.NavigationState = NavigationState.Preloaded;
        }

        internal async Task UnloadPreloadedInternal()
        {
            this.NavigationState = NavigationState.UnloadingPreloaded;
            await UnloadPreloaded();
            this.NavigationState = NavigationState.UnloadedPreloaded;
        }
    }
}
