using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using UIKit;

namespace Maui_Repro_21835;

public class CustomTabbedPageRenderer : TabbedRenderer
{
	private UIView _nativeTabBar;

	protected IMauiContext MauiContext => Element?.Handler?.MauiContext;
	
	
	protected override void OnElementChanged(VisualElementChangedEventArgs e)
	{
		base.OnElementChanged(e);

		if (e.OldElement is CustomTabbedPage oldPage)
		{
			oldPage.PropertyChanged -= HandlePropertyChanged;
		}

		if (e.NewElement is CustomTabbedPage newPage)
		{
			newPage.PropertyChanged += HandlePropertyChanged;
			InitTabBarIfNecessary();
		}
	}

	public override void ViewDidLayoutSubviews()
	{
		if (Element is not CustomTabbedPage page)
		{
			base.ViewDidLayoutSubviews();
			return;
		}
		
		TabBar.Hidden = true;
		TabBar.Frame = new RectangleF(0, 0, 0, 0);
		MoreNavigationController.SetNavigationBarHidden(true, false);

		if (NavigationController is { View : { } navView })
			navView.BackgroundColor = Element.BackgroundColor.ToPlatform();

		base.ViewDidLayoutSubviews();

		var bottomInset = View?.SafeAreaInsets.Bottom ?? 0;
		var frameWidth = View?.Frame.Width ?? 0;
		var frameHeight = View?.Frame.Height ?? 0;

		page.ContainerArea = new(0, 0, frameWidth, frameHeight);
		page.Padding = new(0, 0, 0, bottomInset);

		var fx = Element.X;
		var fy = Element.Height - 70 - bottomInset;
		var fw = Element.Width;
		var fh = 70;
		if (_nativeTabBar is not null)
			_nativeTabBar.Frame = new(fx, fy, fw, fh);
	}



	private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (Element is not CustomTabbedPage page)
			return;

		if (e.PropertyName == NavigationPage.CurrentPageProperty.PropertyName)
			ScheduleLayout();
	}

	private void HandleTabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		InitTabBarIfNecessary();
		ScheduleLayout();
	}

	private void InitTabBarIfNecessary()
	{
		if (_nativeTabBar is not null)
			return;

		if (Element is not CustomTabbedPage { TabBar: { } tabBar } page)
			return;

		InvokeOnMainThread(() =>
		{
			var handler = tabBar.ToHandler(MauiContext);
			_nativeTabBar = handler.PlatformView;
			View?.AddSubview(_nativeTabBar);
		});
	}

	private void ScheduleLayout()
	{
		InvokeOnMainThread(() => View?.SetNeedsLayout());
	}
}