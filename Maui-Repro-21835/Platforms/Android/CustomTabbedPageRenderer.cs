using Android.Content;
using Android.Views;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using View = Microsoft.Maui.Controls.View;

namespace Maui_Repro_21835;

public class CustomTabbedPageRenderer :TabbedPageRenderer
{
	private IViewHandler _handler;

	protected BottomNavigationView NativeTabBar
	{
		get
		{
			for (var i = 0; i < ChildCount; i++)
			{
				if (GetChildAt(i) is Android.Widget.RelativeLayout relativeLayout)
					for (var j = 0; j < relativeLayout.ChildCount; j++)
					{
						if (relativeLayout.GetChildAt(j) is BottomNavigationView nativeTabBar)
							return nativeTabBar;
					}
			}
			return null;

		}
	}

	protected ViewPager Pager
	{
		get
		{

			for (var i = 0; i < ChildCount; i++)
			{
				if (GetChildAt(i) is Android.Widget.RelativeLayout relativeLayout)
					for (var j = 0; j < relativeLayout.ChildCount; j++)
					{
						if (relativeLayout.GetChildAt(j) is ViewPager viewPager)
							return viewPager;
					}
			}
			return null;
		}
	}

	protected IMauiContext MauiContext => FindMauiContext(Element);

	public CustomTabbedPageRenderer(Context context) : base(context)
	{
	}

	protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
	{
		base.OnElementChanged(e);

		if (e.NewElement is CustomTabbedPage page)
		{
			_handler = page.TabBar.ToHandler(MauiContext);
			AddView(_handler.PlatformView as Android.Views.View);
			if (NativeTabBar is { Visibility: not ViewStates.Gone } tabBar)
				tabBar.Visibility = ViewStates.Gone;
		}
	}

	protected override void OnLayout(bool changed, int l, int t, int r, int b)
	{
		if (NativeTabBar is { Visibility: not ViewStates.Gone } tabBar)
			tabBar.Visibility = ViewStates.Gone;

		base.OnLayout(changed, l, t, r, b);
		Pager?.Layout(l, t, r, b);
			
		var x = Context.FromPixels(l);
		var y = Context.FromPixels(b - t) - 70;
		var w = Context.FromPixels(r - l);
		var h = 70;
		_handler.PlatformArrange(new(x, y, w, h));
	}
	
	private static IMauiContext FindMauiContext(Element element, bool fallbackToAppMauiContext = false)
	{
		if (element is null)
			return null;
		
		if (element is IElement { Handler.MauiContext: { } elementContext })
			return elementContext;

		foreach (var parent in GetParentsPath(element))
		{
			if (parent is IElement { Handler.MauiContext: { } parentContext })
				return parentContext;
		}

		return fallbackToAppMauiContext
			? FindMauiContext(Application.Current)
			: default;
	}
	
	private static IEnumerable<Element> GetParentsPath(Element self)
	{
		Element current = self;
		while (current.RealParent is not null or IApplication or IWindow) // not application or null
		{
			current = current.RealParent;
			yield return current;
		}
	}
}